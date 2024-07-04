using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class StyleTransferViewModel : TwoImagesInputViewModel
{
    [ObservableProperty] private bool _success = true;

    [ObservableProperty] private bool _loading;
    
    [RelayCommand]
    private void Calculate()
    {
        if (Src1 == null || Src2 == null)
        {
            return;
        }

        Success = true;

        var dir = $"{Directory.GetCurrentDirectory()}/vgg19";
        
        API.Basic.SaveImage(Src1, $"{dir}/src1.png");
        
        API.Basic.SaveImage(Src2, $"{dir}/src2.png");
        
        Loading = true;
        
        Success = ML.Vgg19.Execute($"{dir}/src1.png", $"{dir}/src2.png");
        
        Loading = false;
        
        if (!Success)
        {
            Res = null;
            return;
        }
        
        Res = API.Basic.ReadImage($"{dir}/result.png");
    }

    protected override void Src1Changed(Mat? value)
    {
        if (CanExecute())
        {
            Task.Run(() =>
            {
                CalculateCommand.Execute(null);
            });
        }
    }

    protected override void Src2Changed(Mat? value)
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