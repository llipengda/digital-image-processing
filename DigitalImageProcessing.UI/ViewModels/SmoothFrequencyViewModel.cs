using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SmoothFrequencyViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private string _algorithm = "理想低通滤波";

    [ObservableProperty] private int _d0 = 20;

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
            "理想低通滤波" => FrequencySmoothingType.IdealLowPass,
            "巴特沃斯低通滤波" => FrequencySmoothingType.ButterworthLowPass,
            "高斯低通滤波" => FrequencySmoothingType.GaussianLowPass,
            _ => throw new InvalidOperationException("Algorithm not found"),
        };

        Res = Smoothing.Frequency(Src, al, D0);
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

    private readonly Action _debouncedDoSmoothSpatial;

    public SmoothFrequencyViewModel()
    {
        _debouncedDoSmoothSpatial = new DebounceHelper().Debounce(() => DoSmoothSpatialCommand.Execute(Algorithm),
            TimeSpan.FromMilliseconds(200));
    }
    
    partial void OnD0Changed(int value)
    {
        if (CanExecute())
        {
            _debouncedDoSmoothSpatial();
        }
    }
}