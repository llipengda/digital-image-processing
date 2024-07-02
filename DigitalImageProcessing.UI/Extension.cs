using Avalonia.Media.Imaging;
using OpenCvSharp;

namespace DigitalImageProcessing.UI;

public static class Extension
{
    public static Bitmap ToBitmap(this Mat image)
    {
        using var memory = image.ToMemoryStream();
        return new Bitmap(memory);
    }
}