using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class EnhanceHistogramNormalizationViewModel : TwoImagesInputWithHistViewModel
{
    [RelayCommand]
    private void Enhance()
    {
        if (Src1 is null || Src2 is null)
        {
            return;
        }

        Res = API.Enhancement.NormalizeHist(Src1, Src2);
    }
    
    protected override void Src1Changed(Mat? value)
    {
        if (CanExecute())
        {
            EnhanceCommand.Execute(null);
        }
        
        base.Src1Changed(value);
    }
    
    protected override void Src2Changed(Mat? value)
    {
        if (CanExecute())
        {
            EnhanceCommand.Execute(null);
        }
        
        base.Src2Changed(value);
    }
}
