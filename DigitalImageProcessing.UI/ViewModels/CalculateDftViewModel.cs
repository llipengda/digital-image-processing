using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class CalculateDftViewModel : SingleImageInputViewModel
{
    [RelayCommand]
    private void Calculate()
    {
        if (Src is null)
        {
            return;
        }

        Res = API.Calculate.DftImage(Src);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }
}
