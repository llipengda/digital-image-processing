using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class RestoreNoiseViewModel : SingleImageInputViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsGaussian))]
    private string _type = "高斯噪声";

    [ObservableProperty] private double _mean;

    [ObservableProperty] private double _sigma = 10;

    [ObservableProperty] private double _salt = 0.01;

    [ObservableProperty] private double _pepper = 0.99;

    public bool IsGaussian => Type == "高斯噪声";

    [RelayCommand]
    private void Do(string type)
    {
        if (Src is null)
        {
            return;
        }

        Type = type;

        Res = Type switch
        {
            "高斯噪声" => Restoration.GaussianNoise(Src, Mean, Sigma),
            "椒盐噪声" => Restoration.SaltAndPepperNoise(Src, Salt, Pepper),
            _ => throw new InvalidOperationException()
        };
    }

    private readonly Action _debouncedDo;

    public RestoreNoiseViewModel()
    {
        _debouncedDo =
            new DebounceHelper().Debounce(() => DoCommand.Execute(Type), TimeSpan.FromMilliseconds(200));
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoCommand.Execute(Type);
        }
    }

    partial void OnTypeChanged(string value)
    {
        if (CanExecute())
        {
            _debouncedDo();
        }
    }

    partial void OnMeanChanged(double value)
    {
        if (CanExecute())
        {
            _debouncedDo();
        }
    }

    partial void OnSigmaChanged(double value)
    {
        if (CanExecute())
        {
            _debouncedDo();
        }
    }

    partial void OnSaltChanged(double value)
    {
        if (CanExecute())
        {
            _debouncedDo();
        }
    }

    partial void OnPepperChanged(double value)
    {
        if (CanExecute())
        {
            _debouncedDo();
        }
    }
}