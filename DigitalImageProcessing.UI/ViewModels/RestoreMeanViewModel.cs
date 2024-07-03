using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class RestoreMeanViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private string _type = "代数均值滤波";

    [ObservableProperty] private int _size = 3;
    
    [RelayCommand]
    private void Do(string type)
    {
        if (Src is null)
        {
            return;
        }

        Type = type;

        var t = type switch
        {
            "代数均值滤波" => MeanFilterType.Arithmetic,
            "几何均值滤波" => MeanFilterType.Geometric,
            "谐波均值滤波" => MeanFilterType.Harmonic,
            _ => throw new InvalidOperationException()
        };

        Res = Restoration.MeanFilter(Src, Size, t);
    }
    
    private readonly Action _debouncedDo;
    
    public RestoreMeanViewModel()
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
    
    partial void OnSizeChanged(int value)
    {
        if (value % 2 == 0)
        {
            value++;
            Size = value;
        }
        
        if (CanExecute())
        {
            _debouncedDo();
        }
    }
}
