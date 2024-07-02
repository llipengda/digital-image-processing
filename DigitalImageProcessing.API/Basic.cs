using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Basic
{
    public static Mat ReadImage(Uri uri) => Cv2.ImRead(uri.LocalPath);

    public static Mat ReadImage(string path) => Cv2.ImRead(path);

    public static void SaveImage(Mat image, string path) => Cv2.ImWrite(path, image);

    public static void ShowImage(Mat image, int width = 800, int height = 600, string name = "Image",
        bool autoSize = true, bool wait = true)
    {
        Cv2.NamedWindow(name, WindowFlags.AutoSize);
        
        image = Calculate.ResizeIf(i => i.Width > width || i.Height > height, image, (width, height));

        if (!autoSize)
        {
            Cv2.ImShow(name, image);

            if (!wait)
            {
                return;
            }
            
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            
            return;
        }

        Cv2.ResizeWindow(name, width, height);

        var dx = (width - image.Width) / 2;
        var dy = (height - image.Height) / 2;
        
        Cv2.CopyMakeBorder(image, image, 1, 1, 1, 1, 
            BorderTypes.Constant, Scalar.White);

        Cv2.CopyMakeBorder(image, image, dy, dy, dx, dx, 
            BorderTypes.Constant, Scalar.Black);

        Cv2.ImShow(name, image);

        if (!wait)
        {
            return;
        }
        
        Cv2.WaitKey();
        Cv2.DestroyAllWindows();
    }
    
    public static void ShowImages(params (Mat image, string name)[] images)
    {
        foreach (var (image, name) in images)
        {
            ShowImage(image, name: name, autoSize: false, wait: false);
        }
        Cv2.WaitKey();
        Cv2.DestroyAllWindows();
    }

    public static (Mat b, Mat g, Mat r) SplitChannels(Mat image)
    {
        if (image.Channels() == 1)
        {
            Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2BGR);
        }
        var channels = Cv2.Split(image);
        return (channels[0], channels[1], channels[2]);
    }

    public static Mat ToHsv(Mat image)
    {
        Mat hsv = new();
        Cv2.CvtColor(image, hsv, ColorConversionCodes.BGR2HSV);
        return hsv;
    }
    
    public static Mat ToGray(Mat image)
    {
        if (image.Channels() == 1)
        {
            return image;
        }
        Mat gray = new();
        Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
        return gray;
    }
    
    public static Mat ToBinary(Mat image, int threshold = 128)
    {
        image = ToGray(image);
        Mat binary = new();
        Cv2.Threshold(image, binary, threshold, 255, ThresholdTypes.Binary);
        return binary;
    }
}