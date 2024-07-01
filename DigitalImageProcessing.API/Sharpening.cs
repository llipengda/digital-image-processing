using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Sharpening
{
    public static Mat Spatial(Mat image, SpatialSharpeningType type)
    {
        image = Basic.ToGray(image);
        
        var result = new Mat(image.Size(), MatType.CV_8U);
        
        for (var i = 1; i < image.Height - 1; i++)
        {
            for (var j = 1; j < image.Width - 1; j++)
            {
                int g, gx, gy;
                switch (type)
                {
                    case SpatialSharpeningType.Roberts:
                        g = Math.Abs(image.At<byte>(i, j) - image.At<byte>(i + 1, j + 1))
                            + Math.Abs(image.At<byte>(i, j + 1) - image.At<byte>(i + 1, j));
                        g = Math.Min(g, 255);
                        break;
                    case SpatialSharpeningType.Sobel:
                        gx = image.At<byte>(i - 1, j - 1) + 2 * image.At<byte>(i - 1, j) +
                                 image.At<byte>(i - 1, j + 1) - image.At<byte>(i + 1, j - 1) -
                                 2 * image.At<byte>(i + 1, j) - image.At<byte>(i + 1, j + 1);
                        gy = image.At<byte>(i - 1, j - 1) + 2 * image.At<byte>(i, j - 1) +
                                 image.At<byte>(i + 1, j - 1) - image.At<byte>(i - 1, j + 1) -
                                 2 * image.At<byte>(i, j + 1) - image.At<byte>(i + 1, j + 1);
                        g = Math.Abs(gx) + Math.Abs(gy);
                        g = Math.Min(g, 255);
                        break;
                    case SpatialSharpeningType.Prewitt:
                        gx = image.At<byte>(i - 1, j - 1) + image.At<byte>(i - 1, j) +
                                 image.At<byte>(i - 1, j + 1) - image.At<byte>(i + 1, j - 1) -
                                 image.At<byte>(i + 1, j) - image.At<byte>(i + 1, j + 1);
                        gy = image.At<byte>(i - 1, j - 1) + image.At<byte>(i, j - 1) +
                                 image.At<byte>(i + 1, j - 1) - image.At<byte>(i - 1, j + 1) -
                                 image.At<byte>(i, j + 1) - image.At<byte>(i + 1, j + 1);
                        g = Math.Abs(gx) + Math.Abs(gy);
                        g = Math.Min(g, 255);
                        break;
                    case SpatialSharpeningType.Laplacian:
                        g = 4 * image.At<byte>(i, j) - image.At<byte>(i - 1, j) - image.At<byte>(i + 1, j) -
                            image.At<byte>(i, j - 1) - image.At<byte>(i, j + 1);
                        g = Math.Min(g, 255);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                result.Set(i, j, g);
            }
        }
        
        return result;
    }

    public static Mat Frequency(Mat image, FrequencySharpeningType type, int d0 = 20)
    {
        image = Basic.ToGray(image);

        image = Calculate.Dft(image);

        image = Calculate.DftShift(image);

        var result = image.Clone();

        var mask = new float[image.Height, image.Width];

        for (var i = 0; i < image.Height; i++)
        {
            for (var j = 0; j < image.Width; j++)
            {
                var x = i - image.Height / 2.0;
                var y = j - image.Width / 2.0;
                var d = Math.Sqrt(x * x + y * y);
                double h;
                switch (type)
                {
                    case FrequencySharpeningType.IdealHighPass:
                        mask[i, j] = d >= d0 ? 1 : 0;
                        break;
                    case FrequencySharpeningType.ButterworthHighPass:
                        h = 1 / (1 + Math.Pow(d0 / d, 2 * 2));
                        mask[i, j] = (float)h;
                        break;
                    case FrequencySharpeningType.GaussianHighPass:
                        h = 1 - Math.Exp(-d * d / (2 * d0 * d0));
                        mask[i, j] = (float)h;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        var maskMat = Mat.FromArray(mask);
        
        var planes = result.Split();
        for (var i = 0; i < 2; i++)
        {
            Cv2.Multiply(planes[i], maskMat, planes[i]);
        }

        Cv2.Merge(planes, result);

        result = Calculate.DftShift(result);

        return Calculate.Idft(result);
    }
}

public enum SpatialSharpeningType
{
    Roberts,
    Sobel,
    Prewitt,
    Laplacian
}

public enum FrequencySharpeningType
{
    IdealHighPass,
    ButterworthHighPass,
    GaussianHighPass
}