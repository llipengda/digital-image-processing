using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class EnhanceLinearViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private double _a = 120;

    [ObservableProperty] private double _b = 130;

    [ObservableProperty] private double _c = 200;

    [ObservableProperty] private double _d = 180;

    [RelayCommand]
    private void Enhance()
    {
        if (Src is null)
        {
            return;
        }

        Res = API.Enhancement.LinearTransform(Src, ((int)A, (int)B), ((int)C, (int)D));
    }

    private Action _debounceEnhance;

    public EnhanceLinearViewModel()
    {
        _debounceEnhance =
            new DebounceHelper().Debounce(() => { EnhanceCommand.Execute(null); }, TimeSpan.FromMilliseconds(200));
    }

    partial void OnAChanged(double value)
    {
        if (CanExecute())
        {
            _debounceEnhance();
        }
    }

    partial void OnBChanged(double value)
    {
        if (CanExecute())
        {
            _debounceEnhance();
        }
    }

    partial void OnCChanged(double value)
    {
        if (CanExecute())
        {
            _debounceEnhance();
        }
    }

    partial void OnDChanged(double value)
    {
        if (CanExecute())
        {
            _debounceEnhance();
        }
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            _debounceEnhance();
        }
    }
}