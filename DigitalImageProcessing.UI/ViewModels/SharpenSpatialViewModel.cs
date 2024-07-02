using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SharpenSpatialViewModel : SingleImageInputViewModel
{
    [ObservableProperty]
    private string _algorithm = "Roberts";
    
    [RelayCommand]
    private void DoSmoothSpatial(string algorithm)
    {
        if (Src is null)
        {
            return;
        }

        Algorithm = algorithm;

        var al = algorithm switch
        {
            "Roberts" => SpatialSharpeningType.Roberts,
            "Sobel" => SpatialSharpeningType.Sobel,
            "Prewitt" => SpatialSharpeningType.Prewitt,
            "Laplacian" => SpatialSharpeningType.Laplacian,
            _ => throw new InvalidOperationException("Algorithm not found"),
        };

        Res = Sharpening.Spatial(Src, al);
    }
    
    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoSmoothSpatialCommand.Execute(Algorithm);
        }
    }
    
    partial void OnAlgorithmChanged(string value)
    {
        if (CanExecute())
        {
            DoSmoothSpatialCommand.Execute(value);
        }
    }
}