using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class RestoreOrderViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private string _type = "中值滤波";
    
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
            "中值滤波" => SortFilterType.Median,
            "最大值滤波" => SortFilterType.Max,
            "最小值滤波" => SortFilterType.Min,
            _ => throw new InvalidOperationException()
        };

        Res = Restoration.SortFilter(Src, Size, t);
    }
    
    private readonly Action _debouncedDo;
    
    public RestoreOrderViewModel()
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
