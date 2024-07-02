using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class CalculateTransformViewModel : SingleImageInputViewModel
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(IsResize))]
    [NotifyPropertyChangedFor(nameof(IsTranslate))]
    [NotifyPropertyChangedFor(nameof(IsRotate))]
    [NotifyPropertyChangedFor(nameof(IsFlip))]
    private string _mode = "缩放";

    [ObservableProperty] private double _scale = 1.0;

    [ObservableProperty] private int _translateX;

    [ObservableProperty] private int _translateY;
    
    [ObservableProperty] private int _rotateAngle;

    [ObservableProperty] private string _flipMode = "X";

    public bool IsResize => Mode == "缩放";
    
    public bool IsTranslate => Mode == "平移";
    
    public bool IsRotate => Mode == "旋转";

    public bool IsFlip => Mode == "翻转";

    [RelayCommand]
    private void Resize()
    {
        if (Src is null)
        {
            return;
        }

        Mode = "缩放";

        Res = API.Calculate.ResizeAndCrop(Src, Scale);
    }

    private readonly Action _debouncedResize;
    
    private readonly Action _debouncedTranslate;
    
    private readonly Action _debouncedRotate;
    
    public CalculateTransformViewModel()
    {
        _debouncedResize = new DebounceHelper().Debounce(Resize, TimeSpan.FromMilliseconds(200));
        _debouncedTranslate = new DebounceHelper().Debounce(Translate, TimeSpan.FromMilliseconds(200));
        _debouncedRotate = new DebounceHelper().Debounce(Rotate, TimeSpan.FromMilliseconds(200));
    }
    
    [RelayCommand]
    private void Translate()
    {
        if (Src is null)
        {
            return;
        }

        Mode = "平移";

        Res = API.Calculate.Translate(Src, TranslateX, TranslateY);
    }
    
    [RelayCommand]
    private void Rotate()
    {
        if (Src is null)
        {
            return;
        }

        Mode = "旋转";

        Res = API.Calculate.Rotate(Src, RotateAngle);
    }
    
    [RelayCommand]
    private void Flip(string mode)
    {
        if (Src is null)
        {
            return;
        }

        Mode = "翻转";
        
        FlipMode = mode;
        
        var flipMode = mode switch
        {
            "X" => API.FlipMode.X,
            "Y" => API.FlipMode.Y,
            "XY" => API.FlipMode.Both,
            _ => throw new InvalidOperationException("FlipMode not found")
        };

        Res = API.Calculate.Flip(Src, flipMode);
    }

    protected override void SrcChanged(Mat? value)
    {
        if (!CanExecute())
        {
            return;
        }
        
        switch (Mode)
        {
            case "缩放":
                ResizeCommand.Execute(null);
                break;
            case "平移":
                TranslateCommand.Execute(null);
                break;
            case "旋转":
                RotateCommand.Execute(null);
                break;
            case "翻转":
                FlipCommand.Execute(FlipMode);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    partial void OnScaleChanged(double value)
    {
        if (CanExecute())
        {
            _debouncedResize();
        }
    }
    
    partial void OnTranslateXChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedTranslate();
        }
    }
    
    partial void OnTranslateYChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedTranslate();
        }
    }
    
    partial void OnRotateAngleChanged(int value)
    {
        if (CanExecute())
        {
            _debouncedRotate();
        }
    }
}