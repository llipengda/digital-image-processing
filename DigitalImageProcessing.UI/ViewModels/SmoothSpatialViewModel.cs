using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SmoothSpatialViewModel : SingleImageInputViewModel
{
    [ObservableProperty]
    private string _algorithm = "均值滤波";
    
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
            "均值滤波" => SpatialSmoothingType.Mean,
            "3 * 3 中值滤波" => SpatialSmoothingType.Median3,
            "5 * 5 中值滤波" => SpatialSmoothingType.Median5,
            _ => throw new InvalidOperationException("Algorithm not found"),
        };

        Res = Smoothing.Spatial(Src, al);
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
