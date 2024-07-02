using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class CalculateAffineViewModel : SingleImageInputViewModel
{
    [ObservableProperty] private double _sx1;

    [ObservableProperty] private double _sy1;

    [ObservableProperty] private double _sx2;

    [ObservableProperty] private double _sy2;

    [ObservableProperty] private double _sx3;

    [ObservableProperty] private double _sy3;

    [ObservableProperty] private double _dx1;

    [ObservableProperty] private double _dy1;

    [ObservableProperty] private double _dx2;

    [ObservableProperty] private double _dy2;

    [ObservableProperty] private double _dx3;

    [ObservableProperty] private double _dy3;

    protected override void SrcChanged(Mat? value)
    {
        Sx2 = Dx2 = Sx3 = Dx3 = Src!.Width;

        Sy3 = Dy3 = Src!.Height;

        CalculateCommand.Execute(null);
    }

    [RelayCommand]
    private void Calculate()
    {
        if (Src is null)
        {
            return;
        }

        int[][] srcPoints = [[(int)Sx1, (int)Sy1], [(int)Sx2, (int)Sy2], [(int)Sx3, (int)Sy3]];
        int[][] dstPoints = [[(int)Dx1, (int)Dy1], [(int)Dx2, (int)Dy2], [(int)Dx3, (int)Dy3]];

        try
        {
            Res = API.Calculate.AffineTransform(Src, srcPoints, dstPoints);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    partial void OnSx1Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnSy1Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnSx2Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnSy2Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnSx3Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnSy3Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnDx1Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnDy1Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnDx2Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnDy2Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnDx3Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }

    partial void OnDy3Changed(double value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(null);
        }
    }
}