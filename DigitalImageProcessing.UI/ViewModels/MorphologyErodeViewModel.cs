using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class MorphologyErodeViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private int _size = 3;

    [RelayCommand]
    private void DoErode()
    {
        if (Src is null)
        {
            return;
        }

        Res = Morphology.Erode(Src, Size);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoErodeCommand.Execute(null);
        }
    }

    private Action _debouncedDoErode;

    public MorphologyErodeViewModel()
    {
        _debouncedDoErode =
            new DebounceHelper().Debounce(() => DoErodeCommand.Execute(null), TimeSpan.FromMilliseconds(200));
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
            _debouncedDoErode();
        }
    }
}