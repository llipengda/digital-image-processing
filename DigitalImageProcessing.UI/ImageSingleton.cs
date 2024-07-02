using System;
using Avalonia.Media.Imaging;
using OpenCvSharp;

namespace DigitalImageProcessing.UI;

public class ImageSingleton
{
    private static readonly Lazy<ImageSingleton> Lazy = new(() => new ImageSingleton());

    private ImageSingleton()
    {
    }
    
    public static ImageSingleton Instance => Lazy.Value;

    public Mat? Mat { get; set; }
    
    public Bitmap? Bmp { get; set; }
    
    public Mat? Mat2 { get; set; }
    
    public Bitmap? Bmp2 { get; set; }
}