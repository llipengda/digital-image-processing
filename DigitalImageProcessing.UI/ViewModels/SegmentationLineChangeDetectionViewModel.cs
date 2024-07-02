using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class SegmentationLineChangeDetectionViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private string _algorithm = "HoughLines";

    [ObservableProperty] private int _threshold = 80;

    [ObservableProperty] private int _rho = 1;

    [ObservableProperty] private int _theta = 60;

    [ObservableProperty] private int _minLineLength = 200;

    [ObservableProperty] private int _maxLineGap = 20;

    [RelayCommand]
    private void DoLineChangeDetection(string algorithm)
    {
        if (Src is null)
        {
            return;
        }

        Algorithm = algorithm;

        var al = algorithm switch
        {
            "HoughLines" => API.LineChangeDetectionAlgorithm.HoughLines,
            "HoughLinesP" => API.LineChangeDetectionAlgorithm.HoughLinesP,
            _ => throw new InvalidOperationException("Algorithm not found"),
        };
        
        var theta = Theta * Math.PI / 180;

        Res = Segmentation.LineChangeDetection(Src, al, Rho, theta, Threshold, MinLineLength, MaxLineGap);
    }
    
    private readonly Action _debouncedDoLineChangeDetection;
    
    public SegmentationLineChangeDetectionViewModel()
    {
        _debouncedDoLineChangeDetection = new DebounceHelper()
            .Debounce(() => DoLineChangeDetectionCommand.Execute(Algorithm), TimeSpan.FromMilliseconds(200));
    }
    
    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoLineChangeDetectionCommand.Execute(Algorithm);
        }
    }
    
    partial void OnThresholdChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDoLineChangeDetection();
        }
    }
    
    partial void OnRhoChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDoLineChangeDetection();
        }
    }
    
    partial void OnThetaChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDoLineChangeDetection();
        }
    }
    
    partial void OnMinLineLengthChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDoLineChangeDetection();
        }
    }
    
    partial void OnMaxLineGapChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDoLineChangeDetection();
        }
    }
    
    partial void OnAlgorithmChanged(string value)
    {
        if (CanExecute())
        {
            _debouncedDoLineChangeDetection();
        }
    }
}