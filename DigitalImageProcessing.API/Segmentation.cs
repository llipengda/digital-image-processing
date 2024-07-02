using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Segmentation
{
    public static Mat Threshold(Mat image, ThresholdType type, int threshold)
    {
        image = Basic.ToGray(image);
        Mat result = new();
        Cv2.Threshold(image, result, threshold, 255, (ThresholdTypes)type);
        return result;
    }

    public static Mat LineChangeDetection(
        Mat image, LineChangeDetectionAlgorithm algorithm, double rho, double theta,
        int threshold, int minLineLength = 200, int maxLineGap = 15)
    {
        Mat edges = new();
        Cv2.GaussianBlur(image, image, new Size(1, 1), 0);
        Cv2.Canny(image, edges, 50, 150);
        if (algorithm == LineChangeDetectionAlgorithm.HoughLines)
        {
            var lines = Cv2.HoughLines(edges, rho, theta, threshold);
            var result = image.Clone();
            foreach (var line in lines)
            {
                var lRho = line.Rho;
                var lTheta = line.Theta;
                if (lTheta < Math.PI / 4 || lTheta > 3 * Math.PI / 4)
                {
                    var pt1 = new Point((int)(lRho / Math.Cos(lTheta)), 0);
                    var pt2 = new Point((int)((lRho - result.Height * Math.Sin(lTheta)) / Math.Cos(lTheta)),
                        result.Height);
                    Cv2.Line(result, pt1, pt2, Scalar.Red);
                }
                else
                {
                    var pt1 = new Point(0, (int)(lRho / Math.Sin(lTheta)));
                    var pt2 = new Point(result.Width,
                        (int)((lRho - result.Width * Math.Cos(lTheta)) / Math.Sin(lTheta)));
                    Cv2.Line(result, pt1, pt2, Scalar.Red);
                }
            }

            return result;
        }

        if (algorithm == LineChangeDetectionAlgorithm.HoughLinesP)
        {
            var linesP = Cv2.HoughLinesP(edges, rho, theta, threshold, minLineLength, maxLineGap);
            var result = image.Clone();
            foreach (var line in linesP)
            {
                Cv2.Line(result, line.P1, line.P2, new Scalar(0, 255, 0));
            }

            return result;
        }

        throw new InvalidOperationException();
    }

    public static Mat EdgeDetection(Mat image, EdgeDetectionAlgorithm algorithm)
    {
        image = Basic.ToGray(image);

        if (algorithm == EdgeDetectionAlgorithm.Roberts)
        {
            var kernelX = Mat.FromArray(new short[,]
            {
                { 1, 0 },
                { 0, -1 }
            });
            var kernelY = Mat.FromArray(new short[,]
            {
                { 0, -1 },
                { 1, 0 }
            });

            var x = new Mat();
            var y = new Mat();

            Cv2.Filter2D(image, x, MatType.CV_16S, kernelX);
            Cv2.Filter2D(image, y, MatType.CV_16S, kernelY);

            var absX = new Mat();
            var absY = new Mat();

            Cv2.ConvertScaleAbs(x, absX);
            Cv2.ConvertScaleAbs(y, absY);

            var result = new Mat();

            Cv2.AddWeighted(absX, 0.5, absY, 0.5, 0, result);

            return result;
        }

        if (algorithm == EdgeDetectionAlgorithm.Prewitt)
        {
            var kernelX = Mat.FromArray(new short[,]
            {
                { 1, 0, -1 },
                { 1, 0, -1 },
                { 1, 0, -1 }
            });
            var kernelY = Mat.FromArray(new short[,]
            {
                { -1, -1, -1 },
                { 0, 0, 0 },
                { 1, 1, 1 }
            });

            var x = new Mat();
            var y = new Mat();

            Cv2.Filter2D(image, x, MatType.CV_16S, kernelX);
            Cv2.Filter2D(image, y, MatType.CV_16S, kernelY);

            var absX = new Mat();
            var absY = new Mat();

            Cv2.ConvertScaleAbs(x, absX);
            Cv2.ConvertScaleAbs(y, absY);

            var result = new Mat();

            Cv2.AddWeighted(absX, 0.5, absY, 0.5, 0, result);

            return result;
        }

        if (algorithm == EdgeDetectionAlgorithm.Sobel)
        {
            var x = new Mat();
            var y = new Mat();

            Cv2.Sobel(image, x, MatType.CV_16S, 1, 0);
            Cv2.Sobel(image, y, MatType.CV_16S, 0, 1);

            var absX = new Mat();
            var absY = new Mat();

            Cv2.ConvertScaleAbs(x, absX);
            Cv2.ConvertScaleAbs(y, absY);

            var result = new Mat();

            Cv2.AddWeighted(absX, 0.5, absY, 0.5, 0, result);

            return result;
        }

        if (algorithm == EdgeDetectionAlgorithm.Laplacian)
        {
            Cv2.GaussianBlur(image, image, new Size(3, 3), 0);

            var result = new Mat();

            Cv2.Laplacian(image, result, MatType.CV_16S, 3);

            Cv2.ConvertScaleAbs(result, result);

            return result;
        }

        if (algorithm == EdgeDetectionAlgorithm.LoG)
        {
            Cv2.CopyMakeBorder(image, image, 2, 2, 2, 2, BorderTypes.Replicate);
            Cv2.GaussianBlur(image, image, new Size(3, 3), 0);
            var loG = new short[,]
            {
                { 0, 0, -1, 0, 0 },
                { 0, -1, -2, -1, 0 },
                { -1, -2, 16, -2, -1 },
                { 0, -1, -2, -1, 0 },
                { 0, 0, -1, 0, 0 }
            };

            var kernel = Mat.FromArray(loG);

            var result = new Mat(image.Size(), MatType.CV_32S);

            for (var i = 2; i < image.Rows - 2; i++)
            {
                for (var j = 2; j < image.Cols - 2; j++)
                {
                    var sum = 0;
                    for (var k = 0; k < 5; k++)
                    {
                        for (var l = 0; l < 5; l++)
                        {
                            sum += image.At<byte>(i + k - 2, j + l - 2) * kernel.At<short>(k, l);
                        }
                    }

                    result.Set(i, j, sum);
                }
            }
            
            Cv2.ConvertScaleAbs(result, result);
            
            return result;
        }
        
        if (algorithm == EdgeDetectionAlgorithm.Canny)
        {
            Cv2.GaussianBlur(image, image, new Size(3, 3), 0);
            
            var x = new Mat();
            var y = new Mat();
            
            Cv2.Sobel(image, x, MatType.CV_16S, 1, 0);
            Cv2.Sobel(image, y, MatType.CV_16S, 0, 1);
            
            var result = new Mat();
            
            Cv2.Canny(x, y, result, 50, 150);
            
            return result;
        }

        throw new InvalidOperationException();
    }
}

[Flags]
public enum ThresholdType
{
    Binary = 0,
    Trunc = 1 << 1,
    Otsu = 1 << 3,
    Triangle = 1 << 4,
    ToZero = 3
}

public enum LineChangeDetectionAlgorithm
{
    HoughLines,
    HoughLinesP
}

public enum EdgeDetectionAlgorithm
{
    Roberts,
    Prewitt,
    Sobel,
    Laplacian,
    LoG,
    Canny
}