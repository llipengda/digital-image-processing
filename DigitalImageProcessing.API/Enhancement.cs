using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Enhancement
{
    /// <summary>
    /// 对数变换
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Mat LogTransform(Mat image)
    {
        image = Basic.ToGray(image);
        Mat result = new();
        Mat newImage = new();
        var c = 255 / Math.Log(1 + 255);
        image += Scalar.All(1);
        image.ConvertTo(newImage, MatType.CV_32F);
        Cv2.Log(newImage, result);
        result *= c;
        result.ConvertTo(result, MatType.CV_8U);
        return result;
    }

    /// <summary>
    /// 线性变换
    /// </summary>
    /// <param name="image"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static Mat LinearTransform(Mat image, (int a, int b) from, (int c, int d) to)
    {
        image = Basic.ToGray(image);
        var result = image.Clone();
        result.ConvertTo(result, MatType.CV_32F);
        var (a, b) = from;
        var (c, d) = to;
        var k = 1.0 * (d - c) / (b - a);
        result *= k;
        result += Scalar.All((b * c * 1.0 - a * d) / (b - a));
        result.ConvertTo(result, MatType.CV_8U);
        return result;
    }

    /// <summary>
    /// 直方图均衡化
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Mat EqualizeHist(Mat image)
    {
        image = Basic.ToGray(image);
        Mat result = new();
        Cv2.EqualizeHist(image, result);
        return result;
    }

    public static Mat NormalizeHist(Mat image, Mat template)
    {
        var mHist1 = new List<int>();
        var mNum1 = new List<double>();
        var inHist1 = new List<int>();

        var mHist2 = new List<int>();
        var mNum2 = new List<double>();
        var inHist2 = new List<int>();

        for (var i = 0; i < 256; i++)
        {
            mHist1.Add(0);
        }

        for (var i = 0; i < image.Rows; i++)
        {
            for (var j = 0; j < image.Cols; j++)
            {
                mHist1[image.At<byte>(i, j)]++;
            }
        }

        mNum1.Add(mHist1[0] / (double)image.Total());

        for (var i = 0; i < 255; i++)
        {
            mNum1.Add(mNum1[i] + mHist1[i + 1] / (double)image.Total());
        }

        for (var i = 0; i < 256; i++)
        {
            inHist1.Add((int)Math.Round(255 * mNum1[i]));
        }

        for (var i = 0; i < 256; i++)
        {
            mHist2.Add(0);
        }

        for (var i = 0; i < template.Rows; i++)
        {
            for (var j = 0; j < template.Cols; j++)
            {
                mHist2[template.At<byte>(i, j)]++;
            }
        }

        mNum2.Add(mHist2[0] / (double)template.Total());

        for (var i = 0; i < 255; i++)
        {
            mNum2.Add(mNum2[i] + mHist2[i + 1] / (double)template.Total());
        }

        for (var i = 0; i < 256; i++)
        {
            inHist2.Add((int)Math.Round(255 * mNum2[i]));
        }

        var g = new List<int>();
        for (var i = 0; i < 256; i++)
        {
            var a = inHist1[i];
            var flag = true;
            for (var j = 0; j < 256; j++)
            {
                if (inHist2[j] != a)
                {
                    continue;
                }

                g.Add(j);
                flag = false;
                break;
            }

            if (!flag)
            {
                continue;
            }

            var minP = 255;
            var jMin = 0;
            
            for (var j = 0; j < 256; j++)
            {
                var b = Math.Abs(inHist2[j] - a);
                
                if (b >= minP)
                {
                    continue;
                }
                
                minP = b;
                jMin = j;
            }

            g.Add(jMin);
        }
        
        Mat result = new(image.Size(), MatType.CV_8U);
        for (var i = 0; i < image.Rows; i++)
        {
            for (var j = 0; j < image.Cols; j++)
            {
                result.Set(i, j, (byte)g[image.At<byte>(i, j)]);
            }
        }

        return result;
    }
}