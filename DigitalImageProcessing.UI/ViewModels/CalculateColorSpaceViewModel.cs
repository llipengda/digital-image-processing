using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class CalculateColorSpaceViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private string _colorSpace = "R";
    
    [RelayCommand]
    public void Calculate(string colorSpace)
    {
        if (Src is null)
        {
            return;
        }

        ColorSpace = colorSpace;

        var channels = API.Basic.SplitChannels(Src);

        Res = ColorSpace switch
        {
            "R" => channels.r,
            "G" => channels.g,
            "B" => channels.b,
            _ => throw new InvalidOperationException("ColorSpace not found")
        };
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(ColorSpace);
        }
    }
}