using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SegmentationEdgeDetectionViewModel : SingleImageInputViewModel
{
    [ObservableProperty]
    private string _algorithm = "Sobel";
    
    [RelayCommand]
    private void DoEdgeDetection(string algorithm)
    {
        if (Src is null)
        {
            return;
        }

        Algorithm = algorithm;
        
        var al = algorithm switch
        {
            "Roberts" => EdgeDetectionAlgorithm.Roberts,
            "Prewitt" => EdgeDetectionAlgorithm.Prewitt,
            "Sobel" => EdgeDetectionAlgorithm.Sobel,
            "Laplacian" => EdgeDetectionAlgorithm.Laplacian,
            "LoG" => EdgeDetectionAlgorithm.LoG,
            "Canny" => EdgeDetectionAlgorithm.Canny,
            _ => throw new InvalidOperationException("Algorithm not found"),
        };

        Res = Segmentation.EdgeDetection(Src, al);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoEdgeDetectionCommand.Execute(Algorithm);
        }
    }
    
    partial void OnAlgorithmChanged(string value)
    {
        if (CanExecute())
        {
            DoEdgeDetectionCommand.Execute(value);
        }
    }
}
