using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SingleImageInputWithHistViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private Bitmap? _srcHist;
    
    [ObservableProperty] private Bitmap? _resHist;

    protected override void SrcChanged(Mat? value)
    {
        if (value is null)
        {
            return;
        }

        SrcHist = API.Calculate.HistImage(value).ToBitmap();
    }
    
    protected override void ResChanged(Mat? value)
    {
        if (value is null)
        {
            return;
        }

        ResHist = API.Calculate.HistImage(value).ToBitmap();
    }
}