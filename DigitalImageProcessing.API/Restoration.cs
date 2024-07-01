using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Restoration
{
    public static Mat GaussianNoise(Mat image, double mean, double sigma)
    {
        var noise = new Mat(image.Size(), image.Type());
        Cv2.Randn(noise, mean, sigma);
        var result = new Mat();
        Cv2.Add(image, noise, result);
        Cv2.Normalize(result, result, 0, 255, NormTypes.MinMax);
        return result;
    }
    
    public static Mat SaltAndPepperNoise(Mat image, double saltProbability, double pepperProbability)
    {
        var result = image.Clone();
        var random = new Random();
        for (var i = 0; i < image.Rows; i++)
        {
            for (var j = 0; j < image.Cols; j++)
            {
                var p = random.NextDouble();
                if (p < saltProbability)
                {
                    result.Set(i, j, 255);
                }
                else if (p > pepperProbability)
                {
                    result.Set(i, j, 0);
                }
            }
        }

        return result;
    }
    
    public static Mat MeanFilter(Mat image, int kernelSize, MeanFilterType type)
    {
        if (kernelSize % 2 == 0)
        {
            throw new ArgumentException("Kernel size must be odd.");
        }
        var r = kernelSize / 2;
        image = Basic.ToGray(image);
        var result = new Mat(image.Size(), MatType.CV_8U);
        for (var i = 0; i < image.Height; i++)
        {
            for (var j = 0; j < image.Width; j++)
            {
                switch (type)
                {
                    case MeanFilterType.Arithmetic:
                        var sum = 0;
                        for (var m = -r; m <= r; m++)
                        {
                            for (var n = -r; n <= r; n++)
                            {
                                if (i + m >= 0 && i + m < image.Height && j + n >= 0 && j + n < image.Width)
                                {
                                    sum += image.At<byte>(i + m, j + n);
                                }
                            }
                        }
                        result.Set(i, j, sum / (kernelSize * kernelSize));
                        break;
                    case MeanFilterType.Geometric:
                        double product = 1;
                        for (var m = -r; m <= r; m++)
                        {
                            for (var n = -r; n <= r; n++)
                            {
                                if (i + m >= 0 && i + m < image.Height && j + n >= 0 && j + n < image.Width)
                                {
                                    product *= image.At<byte>(i + m, j + n);
                                }
                            }
                        }
                        result.Set(i, j, (byte)Math.Pow(product, 1.0 / (kernelSize * kernelSize)));
                        break;
                    case MeanFilterType.Harmonic:
                        double sumInv = 0;
                        for (var m = -r; m <= r; m++)
                        {
                            for (var n = -r; n <= r; n++)
                            {
                                if (i + m >= 0 && i + m < image.Height && j + n >= 0 && j + n < image.Width)
                                {
                                    sumInv += 1.0 / image.At<byte>(i + m, j + n);
                                }
                            }
                        }
                        result.Set(i, j, (byte)(kernelSize * kernelSize / sumInv));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }
        
        return result;
    }

    public static Mat SortFilter(Mat image, int kernelSize, SortFilterType type)
    {
        var arr = new List<int>(kernelSize * kernelSize);
        var r = kernelSize / 2;
        image = Basic.ToGray(image);
        var result = new Mat(image.Size(), MatType.CV_8U);
        for (var i = 0; i < image.Height; i++)
        {
            for (var j = 0; j < image.Width; j++)
            {
                arr.Clear();
                for (var m = -r; m <= r; m++)
                {
                    for (var n = -r; n <= r; n++)
                    {
                        if (i + m >= 0 && i + m < image.Height && j + n >= 0 && j + n < image.Width)
                        {
                            arr.Add(image.At<byte>(i + m, j + n));
                        }
                    }
                }

                arr.Sort();
                switch (type)
                {
                    case SortFilterType.Median:
                        result.Set(i, j, (byte)arr[arr.Count / 2]);
                        break;
                    case SortFilterType.Max:
                        result.Set(i, j, (byte)arr[^1]);
                        break;
                    case SortFilterType.Min:
                        result.Set(i, j, (byte)arr[0]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }
        
        return result;
    }

    public static Mat SelectFilter(Mat image, Func<int, bool> filter, int orSetValue)
    {
        image = Basic.ToGray(image);
        var result = new Mat(image.Size(), MatType.CV_8U);
        for (var i = 0; i < image.Height; i++)
        {
            for (var j = 0; j < image.Width; j++)
            {
                result.Set(i, j, filter(image.At<byte>(i, j)) ? image.At<byte>(i, j) : orSetValue);
            }
        }

        return result;
    }
}

public enum MeanFilterType
{
    Arithmetic,
    Geometric,
    Harmonic
}

public enum SortFilterType
{
    Median,
    Min,
    Max
}