using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class MorphologyCloseViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private int _size = 3;

    [RelayCommand]
    private void DoClose()
    {
        if (Src is null)
        {
            return;
        }

        Res = Morphology.Close(Src, Size);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoCloseCommand.Execute(null);
        }
    }

    private readonly Action _debouncedDoClose;

    public MorphologyCloseViewModel()
    {
        _debouncedDoClose =
            new DebounceHelper().Debounce(() => DoCloseCommand.Execute(null), TimeSpan.FromMilliseconds(200));
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
            _debouncedDoClose();
        }
    }
}
