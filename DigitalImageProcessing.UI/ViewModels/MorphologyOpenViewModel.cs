using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class MorphologyOpenViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private int _size = 3;

    [RelayCommand]
    private void DoOpen()
    {
        if (Src is null)
        {
            return;
        }

        Res = Morphology.Open(Src, Size);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoOpenCommand.Execute(null);
        }
    }

    private readonly Action _debouncedDoOpen;

    public MorphologyOpenViewModel()
    {
        _debouncedDoOpen =
            new DebounceHelper().Debounce(() => DoOpenCommand.Execute(null), TimeSpan.FromMilliseconds(200));
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
            _debouncedDoOpen();
        }
    }
    
}
