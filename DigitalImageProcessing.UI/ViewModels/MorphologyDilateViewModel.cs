using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class MorphologyDilateViewModel : SingleImageInputViewModel
{
    [ObservableProperty]
    private int _size = 3;
    
    [RelayCommand]
    private void DoDilate()
    {
        if (Src is null)
        {
            return;
        }

        Res = Morphology.Dilate(Src, Size);
    }
    
    protected override void SrcChanged(Mat? value)
    {
        if (CanExecute())
        {
            DoDilateCommand.Execute(null);
        }
    }
    
    private Action _debouncedDoDilate;
    
    public MorphologyDilateViewModel()
    {
        _debouncedDoDilate =
            new DebounceHelper().Debounce(() => DoDilateCommand.Execute(null), TimeSpan.FromMilliseconds(200));
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
            _debouncedDoDilate();
        }
    }
}
