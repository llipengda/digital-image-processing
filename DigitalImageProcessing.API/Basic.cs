using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Basic
{
    public static Mat ReadImage(Uri uri) => Cv2.ImRead(uri.LocalPath, ImreadModes.Color);

    public static Mat ReadImage(string path) => Cv2.ImRead(path, ImreadModes.Color);

    public static void SaveImage(Mat image, string path) => Cv2.ImWrite(path, image);
    
    public static void ShowImage(Mat image, int width = 800, int height = 600, string name = "Image")
    {
        Cv2.NamedWindow(name, WindowFlags.AutoSize);
        image = ResizeIf(i => i.Width > width || i.Height > height, image, (width, height));
        Cv2.ImShow(name, image);
        Cv2.WaitKey(0);
    }

    public static Mat Resize(Mat image, (int width, int height) size)
    {
        Mat newImage = new();
        var scale = Math.Min(size.width * 1.0 / image.Width, size.height * 1.0 / image.Height);
        Cv2.Resize(image, newImage, new Size(), scale, scale);
        return newImage;
    }

    public static Mat ResizeIf(Func<Mat, bool> gard, Mat image, (int width, int height) size)
    {
        return gard(image) ? Resize(image, size) : image;
    }

    public static (Mat b, Mat g, Mat r) SplitChannels(Mat image)
    {
        var channels = Cv2.Split(image);
        return (channels[0], channels[1], channels[2]);
    }
    
    public static Mat ToHsv(Mat image)
    {
        Mat hsv = new();
        Cv2.CvtColor(image, hsv, ColorConversionCodes.BGR2HSV);
        return hsv;
    }
}