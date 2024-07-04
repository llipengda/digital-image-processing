using OpenCvSharp;

namespace DigitalImageProcessing.API;

public class Scan
{
    public Mat OriginalImage { get; }

    public List<Point> Points { get; private set; } = new();

    private decimal Scale { get; set; }

    public Mat Image { get; private set; }

    private Point[][]? Corners { get; set; }

    public Scan(Mat image)
    {
        OriginalImage = Image = image;
        
        Resize();

        Close();

        GrabCut();

        EdgeDetection();

        CalcContours();

        GetCorners();
    }

    private void Resize()
    {
        var max = 1080;

        var res = new Mat();

        if (Math.Max(Image.Height, Image.Width) > max)
        {
            Scale = (decimal)max / Math.Max(Image.Height, Image.Width);

            Cv2.Resize(Image, res, new Size(), (double)Scale, (double)Scale);
        }
        else
        {
            res = Image;
            Scale = 1;
        }

        Image = res;
    }

    private void Close(int iter = 8)
    {
        var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));

        var res = new Mat();

        Cv2.MorphologyEx(Image, res, MorphTypes.Close, kernel, null, iter);

        Image = res;
    }

    private void GrabCut(int iter = 10)
    {
        if (Image.Channels() == 1)
        {
            Cv2.CvtColor(Image, Image, ColorConversionCodes.GRAY2BGR);
        }

        var mask = new Mat(Image.Size(), MatType.CV_8UC1, Scalar.All(0));

        var bgdModel = new Mat(new Size(65, 1), MatType.CV_64FC1, Scalar.All(0));

        var fgdModel = new Mat(new Size(65, 1), MatType.CV_64FC1, Scalar.All(0));

        var rect = new Rect(20, 20, Image.Width - 20, Image.Height - 20);

        Cv2.GrabCut(Image, mask, rect, bgdModel, fgdModel, iter, GrabCutModes.InitWithRect);

        var mask2 = new Mat(mask.Size(), MatType.CV_8U);

        for (var i = 0; i < mask.Rows; i++)
        {
            for (var j = 0; j < mask.Cols; j++)
            {
                var value = mask.At<byte>(i, j);
                mask2.Set(i, j, value is 2 or 0 ? (byte)0 : (byte)1);
            }
        }

        Cv2.CvtColor(mask2, mask2, ColorConversionCodes.GRAY2BGR);

        Mat res = Image.Mul(mask2);

        Image = res;
    }

    private void EdgeDetection()
    {
        var gray = Basic.ToGray(Image);
        Cv2.GaussianBlur(gray, gray, new Size(11, 11), 0);

        var canny = new Mat();

        Cv2.Canny(gray, canny, 80, 200);
        Cv2.Dilate(canny, canny,
            Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5)));

        Image = canny;
    }

    private void CalcContours()
    {
        Cv2.FindContours(Image, out var contours, out _,
            RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        Cv2.FindContours(Image, out var contours2, out _,
            RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

        contours = contours.Where(c => Cv2.ContourArea(c) > 500)
            .OrderByDescending(c => Cv2.ContourArea(c)).Take(5).ToArray();

        contours2 = contours2.Where(c => Cv2.ContourArea(c) > 500)
            .OrderByDescending(c => Cv2.ContourArea(c)).Take(5).ToArray();

        var res = new Mat(Image.Size(), MatType.CV_8UC3, Scalar.Black);

        Cv2.DrawContours(res, [..contours, ..contours2], -1, new Scalar(0, 255, 255), 3);

        Image = res;

        Corners = [..contours, ..contours2];
    }

    private void GetCorners()
    {
        var corners = new List<List<Point>>();

        var res = OriginalImage.Clone();

        foreach (var contour in Corners!)
        {
            var approx = Cv2.ApproxPolyDP(contour, Cv2.ArcLength(contour, true) * 0.02, true);

            if (approx.Length is 4 && Cv2.ContourArea(approx) > Image.Width / 4.0 * Image.Height / 4.0)
            {
                var center = new Point(approx.Sum(p => p.X) / approx.Length, approx.Sum(p => p.Y) / approx.Length);

                approx = approx.OrderBy(p => Math.Atan2(p.Y - center.Y / 2.0, p.X - center.X / 2.0)).ToArray();

                corners.Add(approx.ToList());
            }
        }
        
        if (corners.Count == 0)
        {
            foreach (var contour in Corners)
            {
                var approx = Cv2.ApproxPolyDP(contour, Cv2.ArcLength(contour, true) * 0.02, true);

                var points = new List<Point>();

                for (var i = 0; i < approx.Length; i++)
                {
                    var p1 = approx[i];
                    var p2 = approx[(i + 1) % approx.Length];
                    var p3 = approx[(i + 2) % approx.Length];

                    var angle = Math.Abs(Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) - Math.Atan2(p3.Y - p2.Y, p3.X - p2.X));

                    if (Math.Abs(angle * 180 / Math.PI - 90) < 20)
                    {
                        if (points.Any(p => Math.Abs(p.X - p2.X) < 10 && Math.Abs(p.Y - p2.Y) < 10))
                        {
                            continue;
                        }

                        points.Add(p2);
                    }
                }

                if (points.Count >= 2)
                {
                    var o0 = new Point(Image.Width, Image.Height);
                    var o1 = new Point(0, Image.Height);
                    var o2 = new Point(0, 0);
                    var o3 = new Point(Image.Width, 0);

                    var oPoints = new List<Point> { o0, o1, o2, o3 };

                    oPoints = oPoints
                        .OrderByDescending(p => points.Sum(p1 => Math.Pow(p1.X - p.X, 2) + Math.Pow(p1.Y - p.Y, 2)))
                        .ToList();

                    if (points.Count == 2)
                    {
                        points.Insert(0, oPoints[0]);
                        points.Insert(0, oPoints[1]);
                    }
                    else
                    {
                        points.Insert(0, oPoints[0]);
                    }

                    var center = new Point(points.Sum(p => p.X) / points.Count, points.Sum(p => p.Y) / points.Count);

                    points = points.OrderBy(p => Math.Atan2(p.Y - center.Y / 2.0, p.X - center.X / 2.0)).ToList();

                    corners.Add(points);
                }
            }
        }

        if (corners.Count == 0)
        {
            corners.Add([
                new Point(0, Image.Height),
                new Point(0, 0),
                new Point(Image.Width, 0),
                new Point(Image.Width, Image.Height)
            ]);
        }

        var ps = corners.MaxBy(c => Cv2.ContourArea(c))!
            .Select(p => new Point((int)(p.X / Scale), (int)(p.Y / Scale)))
            .ToList();

        var higher = ps.OrderBy(p => p.Y).Take(2).ToList();
        var lower = ps.OrderByDescending(p => p.Y).Take(2).ToList();
        var hr = higher.MaxBy(p => p.X);
        var hl = higher.MinBy(p => p.X);
        var lr = lower.MaxBy(p => p.X);
        var ll = lower.MinBy(p => p.X);

        Points = [hl, hr, lr, ll];
        
        Cv2.DrawContours(res, [Points], -1, Scalar.White, 3);

        Image = res;
    }

    public void Calc(Point[] points)
    {
        var (tl, tr, br, bl) = (points[0], points[1], points[2], points[3]);
        var width = (float)Math.Max(Point.Distance(tl, tr), Point.Distance(bl, br));
        var height = (float)Math.Max(Point.Distance(tl, bl), Point.Distance(tr, br));
        var m = Cv2.GetPerspectiveTransform(points.Select(p => new Point2f(p.X, p.Y)).ToArray(),
            new[] { new Point2f(0, 0), new Point2f(width, 0), new Point2f(width, height), new Point2f(0, height) });
        var dst = new Mat();
        Cv2.WarpPerspective(OriginalImage, dst, m, new Size(width, height));
        Cv2.CvtColor(dst, dst, ColorConversionCodes.BGR2GRAY);

        var binary = new Mat();
        Cv2.AdaptiveThreshold(dst, binary, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, 10);


        float[,] laplacianKernel =
        {
            { 0, -1, 0 },
            { -1, 5, -1 },
            { 0, -1, 0 }
        };

        Mat kernel = Mat.FromArray(laplacianKernel);

        var sharpened = new Mat();
        Cv2.Filter2D(binary, sharpened, -1, kernel);

        Cv2.Threshold(sharpened, sharpened, 0, 255, ThresholdTypes.Tozero);

        Image = sharpened;
    }
}