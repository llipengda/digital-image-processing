using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class TwoImagesInputWithHistViewModel : TwoImagesInputViewModel
{
    [ObservableProperty] private Bitmap? _src1Hist;

    [ObservableProperty] private Bitmap? _src2Hist;

    [ObservableProperty] private Bitmap? _resHist;

    protected override void Src1Changed(Mat? value)
    {
        if (value is not null)
        {
            Src1Hist = API.Calculate.HistImage(value).ToBitmap();
        }
    }
    
    protected override void Src2Changed(Mat? value)
    {
        if (value is not null)
        {
            Src2Hist = API.Calculate.HistImage(value).ToBitmap();
        }
    }
    
    protected override void ResChanged(Mat? value)
    {
        if (value is not null)
        {
            ResHist = API.Calculate.HistImage(value).ToBitmap();
        }
    }
}