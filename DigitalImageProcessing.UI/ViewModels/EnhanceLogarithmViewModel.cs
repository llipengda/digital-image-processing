using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class EnhanceLogarithmViewModel : SingleImageInputViewModel
{
    [RelayCommand]
    private void Enhance()
    {
        if (Src is null)
        {
            return;
        }

        Res = API.Enhancement.LogTransform(Src);
    }

    protected override void SrcChanged(Mat? value)
    {
        EnhanceCommand.Execute(null);
    }
}
