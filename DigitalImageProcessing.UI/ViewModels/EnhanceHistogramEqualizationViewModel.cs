using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class EnhanceHistogramEqualizationViewModel : SingleImageInputWithHistViewModel
{
    [RelayCommand]
    private void Enhance()
    {
        if (Src is null)
        {
            return;
        }

        Res = API.Enhancement.EqualizeHist(Src);
    }

    protected override void SrcChanged(Mat? value)
    {
        EnhanceCommand.Execute(null);
        base.SrcChanged(value);
    }
}
