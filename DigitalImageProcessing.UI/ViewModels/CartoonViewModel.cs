using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class CartoonViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private bool _success = true;
    
    [ObservableProperty] private bool _loading;
    
    [RelayCommand]
    private void Calculate()
    {
        if (Src is null)
        {
            return;
        }

        Success = true;

        var dir = $"{Directory.GetCurrentDirectory()}/cartoon";
        
        API.Basic.SaveImage(Src, $"{dir}/src.png");
        
        Loading = true;

        Success = ML.Cartoon.Execute($"{dir}/src.png");
        
        Loading = false;

        if (!Success)
        {
            Res = null;
            return;
        }

        Res = API.Basic.ReadImage($"{dir}/result.png");
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            Task.Run(() =>
            {
                CalculateCommand.Execute(null);
            });
        }
    }
}
