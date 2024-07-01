using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Smoothing
{
    public static Mat Spatial(Mat image, SpatialSmoothingType type)
    {
        image = Basic.ToGray(image);
        var result = new Mat(image.Size(), MatType.CV_8U);

        if (type != SpatialSmoothingType.Median5)
        {
            var values = new int[9];
            for (var i = 1; i < image.Height - 1; i++)
            {
                for (var j = 1; j < image.Width - 1; j++)
                {
                    int res;
                    switch (type)
                    {
                        case SpatialSmoothingType.Mean:
                            res = (image.At<byte>(i - 1, j - 1) + image.At<byte>(i - 1, j) +
                                   image.At<byte>(i - 1, j + 1) + image.At<byte>(i, j - 1) +
                                   image.At<byte>(i, j) + image.At<byte>(i, j + 1) +
                                   image.At<byte>(i + 1, j - 1) + image.At<byte>(i + 1, j) +
                                   image.At<byte>(i + 1, j + 1)) / 9;
                            break;
                        case SpatialSmoothingType.Median3:
                            values[0] = image.At<byte>(i - 1, j - 1);
                            values[1] = image.At<byte>(i - 1, j);
                            values[2] = image.At<byte>(i - 1, j + 1);
                            values[3] = image.At<byte>(i, j - 1);
                            values[4] = image.At<byte>(i, j);
                            values[5] = image.At<byte>(i, j + 1);
                            values[6] = image.At<byte>(i + 1, j - 1);
                            values[7] = image.At<byte>(i + 1, j);
                            values[8] = image.At<byte>(i + 1, j + 1);
                            Array.Sort(values);
                            res = values[4];
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    result.Set(i, j, res);
                }
            }
        }
        else
        {
            var values = new int[25];
            for (var i = 2; i < image.Height - 2; i++)
            {
                for (var j = 2; j < image.Width - 2; j++)
                {
                    for (var k = 0; k < 5; k++)
                    {
                        for (var l = 0; l < 5; l++)
                        {
                            values[k * 5 + l] = image.At<byte>(i + k - 2, j + l - 2);
                        }
                    }

                    Array.Sort(values);
                    result.Set(i, j, values[12]);
                }
            }
        }

        return result;
    }

    public static Mat Frequency(Mat image, FrequencySmoothingType type, int d0 = 20)
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
                    case FrequencySmoothingType.IdealLowPass:
                        mask[i, j] = d <= d0 ? 1 : 0;
                        break;
                    case FrequencySmoothingType.ButterworthLowPass:
                        h = 1 / (1 + Math.Pow(d / d0, 2));
                        mask[i, j] = (float)h;
                        break;
                    case FrequencySmoothingType.GaussianLowPass:
                        h = Math.Exp(-d * d / (2 * d0 * d0));
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

public enum SpatialSmoothingType
{
    Mean,
    Median3,
    Median5
}

public enum FrequencySmoothingType
{
    IdealLowPass,
    ButterworthLowPass,
    GaussianLowPass,
}