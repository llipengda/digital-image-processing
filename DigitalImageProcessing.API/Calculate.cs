using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Calculate
{
    public static Mat Add(Mat a, Mat b)
    {
        b = b.Resize(a.Size());
        if (a.Channels() == 1)
        {
            Cv2.CvtColor(a, a, ColorConversionCodes.GRAY2BGR);
        }
        if (b.Channels() == 1)
        {
            Cv2.CvtColor(b, b, ColorConversionCodes.GRAY2BGR);
        }
        Mat result = new();
        Cv2.Add(a, b, result);
        return result;
    }

    public static Mat Subtract(Mat a, Mat b)
    {
        b = b.Resize(a.Size());
        if (a.Channels() == 1)
        {
            Cv2.CvtColor(a, a, ColorConversionCodes.GRAY2BGR);
        }
        if (b.Channels() == 1)
        {
            Cv2.CvtColor(b, b, ColorConversionCodes.GRAY2BGR);
        }
        Mat result = new();
        Cv2.Subtract(a, b, result);
        return result;
    }

    public static Mat Multiply(Mat a, Mat b)
    {
        b = b.Resize(a.Size());
        if (a.Channels() == 1)
        {
            Cv2.CvtColor(a, a, ColorConversionCodes.GRAY2BGR);
        }
        if (b.Channels() == 1)
        {
            Cv2.CvtColor(b, b, ColorConversionCodes.GRAY2BGR);
        }
        Mat result = new();
        Cv2.Multiply(a, b, result);
        return result;
    }

    public static Mat Resize(Mat image, (int width, int height) size)
    {
        Mat newImage = new();
        var scale = Math.Min(size.width * 1.0 / image.Width, size.height * 1.0 / image.Height);
        Cv2.Resize(image, newImage, new Size(), scale, scale);
        return newImage;
    }

    public static Mat Resize(Mat image, double scale)
    {
        Mat newImage = new();
        Cv2.Resize(image, newImage, new Size(), scale, scale);
        return newImage;
    }

    public static Mat ResizeAndCrop(Mat image, double scale)
    {
        var resized = Resize(image, scale);
        Mat res;
        res = new Mat(image.Size(), image.Type(), Scalar.Black);
        var x = Math.Abs(res.Width - resized.Width) / 2;
        var y = Math.Abs(res.Height - resized.Height) / 2;
        if (resized.Width < image.Width)
        {
            resized.CopyTo(res[new Rect(x, y, resized.Width, resized.Height)]);
        }
        else
        {
            resized[new Rect(x, y, image.Width, image.Height)].CopyTo(res);
        }
        return res;
    }

    public static Mat ResizeIf(Func<Mat, bool> gard, Mat image, (int width, int height) size)
    {
        return gard(image) ? Resize(image, size) : image;
    }

    public static Mat ResizeIf(Func<Mat, bool> gard, Mat image, double scale)
    {
        return gard(image) ? Resize(image, scale) : image;
    }

    /// <summary>
    /// 平移图像
    /// </summary>
    /// <param name="image"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    public static Mat Translate(Mat image, int dx, int dy)
    {
        Mat newImage = new();
        var translationMatrix = Mat.FromArray(new float[,]
        {
            { 1, 0, dx },
            { 0, 1, dy }
        });
        Cv2.WarpAffine(image, newImage, translationMatrix, image.Size());
        return newImage;
    }

    /// <summary>
    /// 旋转图像
    /// </summary>
    /// <param name="image"></param>
    /// <param name="angle"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Mat Rotate(Mat image, double angle, double scale = 1.0)
    {
        Mat newImage = new();
        var rotationMatrix =
            Cv2.GetRotationMatrix2D(new Point2f(image.Width / 2.0f, image.Height / 2.0f), angle, scale);
        Cv2.WarpAffine(image, newImage, rotationMatrix, image.Size());
        return newImage;
    }

    /// <summary>
    /// 翻转图像
    /// </summary>
    /// <param name="image"></param>
    /// <param name="mode">翻转轴</param>
    /// <returns></returns>
    public static Mat Flip(Mat image, FlipMode mode)
    {
        Mat newImage = new();
        Cv2.Flip(image, newImage, (OpenCvSharp.FlipMode)mode);
        return newImage;
    }

    /// <summary>
    /// 仿射变换
    /// srcPoints 和 dstPoints 必须有 3 个点，且不能重复 <br/>
    /// 至少有一个点的 x 坐标和 y 坐标不相等
    /// </summary>
    /// <param name="image"></param>
    /// <param name="srcPoints"></param>
    /// <param name="dstPoints"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">参数不满足需求</exception>
    public static Mat AffineTransform(Mat image, int[][] srcPoints, int[][] dstPoints)
    {
        if (srcPoints.Length != 3 || dstPoints.Length != 3)
        {
            throw new ArgumentException("srcPoints and dstPoints must have 3 points");
        }

        if (srcPoints[0].Length != 2 || srcPoints[1].Length != 2 || srcPoints[2].Length != 2)
        {
            throw new ArgumentException("srcPoints must have 2 coordinates");
        }

        if (dstPoints[0].Length != 2 || dstPoints[1].Length != 2 || dstPoints[2].Length != 2)
        {
            throw new ArgumentException("dstPoints must have 2 coordinates");
        }

        if (srcPoints.All(p => p[0] == p[1]))
        {
            throw new ArgumentException("must have a point that x != y in srcPoints");
        }

        if (dstPoints.All(p => p[0] == p[1]))
        {
            throw new ArgumentException("must have a point that x != y in dstPoints");
        }

        if (srcPoints.DistinctBy(p => $"${p[0]}-{p[1]}").Count() != 3 ||
            dstPoints.DistinctBy(p => $"${p[0]}-{p[1]}").Count() != 3)
        {
            throw new ArgumentException("srcPoints and dstPoints must have 3 different points");
        }

        var src = new Point2f[]
        {
            new(srcPoints[0][0], srcPoints[0][1]),
            new(srcPoints[1][0], srcPoints[1][1]),
            new(srcPoints[2][0], srcPoints[2][1])
        };
        var dst = new Point2f[]
        {
            new(dstPoints[0][0], dstPoints[0][1]),
            new(dstPoints[1][0], dstPoints[1][1]),
            new(dstPoints[2][0], dstPoints[2][1])
        };
        Mat newImage = new();
        var affineMatrix = Cv2.GetAffineTransform(src, dst);
        Cv2.WarpAffine(image, newImage, affineMatrix, image.Size());
        return newImage;
    }

    /// <summary>
    /// 离散傅里叶变换
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Mat Dft(Mat image)
    {
        var gray = Basic.ToGray(image);

        var m = Cv2.GetOptimalDFTSize(gray.Rows);
        var n = Cv2.GetOptimalDFTSize(gray.Cols);

        Mat padded = new();
        Cv2.CopyMakeBorder(gray, padded, 0, m - gray.Rows, 0,
            n - gray.Cols, BorderTypes.Constant, Scalar.All(0));

        Mat floatPadded = new();
        padded.ConvertTo(floatPadded, MatType.CV_32F);

        Mat[] planes = [floatPadded, Mat.Zeros(padded.Size(), MatType.CV_32F)];

        Mat complexI = new();

        Cv2.Merge(planes, complexI);

        Cv2.Dft(complexI, complexI);
        
        return complexI;
    }

    /// <summary>
    /// 离散傅里叶变换 - 绘制图片的频谱
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Mat DftImage(Mat image)
    {
        var complexI = Dft(image);
        
        Cv2.Split(complexI, out var planes);

        Cv2.Magnitude(planes[0], planes[1], planes[0]);

        var magI = planes[0];

        magI += Scalar.All(1);
        Cv2.Log(magI, magI);

        magI = DftShift(magI);

        Cv2.Normalize(magI, magI, 0, 1, NormTypes.MinMax);
        
        Cv2.Normalize(magI, magI, 0, 255, NormTypes.MinMax);

        magI.ConvertTo(magI, MatType.CV_8U);

        return magI;
    }

    public static Mat DftShift(Mat image)
    {
        var planes = image.Split();

        for (var i = 0; i < planes.Length; i++)
        {
            image = planes[i];
            
            image = new Mat(image, new Rect(0, 0, image.Cols & -2, image.Rows & -2));

            var cx = image.Cols / 2;
            var cy = image.Rows / 2;

            var q0 = new Mat(image, new Rect(0, 0, cx, cy));
            var q1 = new Mat(image, new Rect(cx, 0, cx, cy));
            var q2 = new Mat(image, new Rect(0, cy, cx, cy));
            var q3 = new Mat(image, new Rect(cx, cy, cx, cy));

            var tmp = new Mat();

            q0.CopyTo(tmp);
            q3.CopyTo(q0);
            tmp.CopyTo(q3);

            q1.CopyTo(tmp);
            q2.CopyTo(q1);
            tmp.CopyTo(q2);
            
            planes[i] = image;
        }
        
        Cv2.Merge(planes, image);
        
        return image;
    }

    public static Mat Idft(Mat image)
    {
        Mat result = new(image.Size(), MatType.CV_8U);
        
        Cv2.Idft(image, result, DftFlags.RealOutput | DftFlags.Scale);
        
        Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax);
        
        Cv2.Normalize(result, result, 0, 255, NormTypes.MinMax);
        
        Cv2.CvtColor(result, result, ColorConversionCodes.GRAY2BGR);
        
        return result;
    }
    
    public static Mat Hist(Mat image)
    {
        var gray = Basic.ToGray(image);
        var hist = new Mat();
        Cv2.CalcHist([gray], [0], null, hist, 1, [256], [[0, 256]]);
        return hist;
    }

    public static Mat HistImage(Mat image)
    {
        var hist = Hist(image);
        
        var histImage = new Mat(new Size(256, 100), MatType.CV_8UC3, Scalar.All(255));
        Cv2.Normalize(hist, hist, 0, histImage.Rows, NormTypes.MinMax);
        
        for (var i = 0; i < 256; i++)
        {
            Cv2.Line(histImage, new Point(i, histImage.Rows), 
                new Point(i, histImage.Rows - hist.At<float>(i)), Scalar.All(0));
        }
        
        return histImage;
    }
}

public enum FlipMode
{
    X = 0,
    Y = 1,
    Both = -1
}