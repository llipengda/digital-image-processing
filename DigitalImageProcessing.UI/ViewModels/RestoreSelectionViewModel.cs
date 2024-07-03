using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class RestoreSelectionViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private string _type = "大于";
    
    [ObservableProperty] private int _value = 3;
    
    [RelayCommand]
    private void Do(string type)
    {
        if (Src is null)
        {
            return;
        }

        Type = type;

        Func<int, bool> func = type switch
        {
            "大于" => v => v > Value,
            "小于" => v => v < Value,
            _ => throw new InvalidOperationException()
        };

        Res = Restoration.SelectFilter(Src, func, 0);
    }
    
    private readonly Action _debouncedDo;
    
    public RestoreSelectionViewModel()
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
    
    partial void OnValueChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedDo();
        }
    }
}