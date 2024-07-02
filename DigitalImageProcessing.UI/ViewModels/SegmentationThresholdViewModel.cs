using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SegmentationThresholdViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private int _threshold = 127;

    [ObservableProperty] private string _thresholdType = "Binary";

    [RelayCommand]
    private void DoThreshold(string tType)
    {
        if (Src is null)
        {
            return;
        }
        
        ThresholdType = tType;

        var type = tType switch
        {
            "Binary" => API.ThresholdType.Binary,
            "OTSU" => API.ThresholdType.Otsu,
            "Triangle" => API.ThresholdType.Triangle,
            "Trunc" => API.ThresholdType.Trunc,
            "ToZero" => API.ThresholdType.ToZero,
            _ => throw new InvalidOperationException("ThresholdType not found"),
        };

        Res = Segmentation.Threshold(Src, type, Threshold);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoThresholdCommand.Execute(ThresholdType);
        }
    }

    private readonly Action _debouncedDoThreshold;

    public SegmentationThresholdViewModel()
    {
        _debouncedDoThreshold = new DebounceHelper()
            .Debounce(() => DoThresholdCommand.Execute(ThresholdType), TimeSpan.FromMilliseconds(200));
    }

    partial void OnThresholdChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDoThreshold();
        }
    }
}