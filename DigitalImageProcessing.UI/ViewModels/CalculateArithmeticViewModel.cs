using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class CalculateArithmeticViewModel : TwoImagesInputViewModel
{
    [ObservableProperty] private string _calcMode = "加法";
    
    [RelayCommand]
    private void Calculate(string mode)
    {
        if (Src1 == null || Src2 == null)
        {
            return;
        }

        CalcMode = mode;

        Res = CalcMode switch
        {
            "加法" => API.Calculate.Add(Src1, Src2),
            "减法" => API.Calculate.Subtract(Src1, Src2),
            "乘法" => API.Calculate.Multiply(Src1, Src2),
            _ => throw new InvalidOperationException("CalcMode not found")
        };
    }

    protected override void Src1Changed(Mat? value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(CalcMode);
        }
    }

    protected override void Src2Changed(Mat? value)
    {
        if (CanExecute())
        {
            CalculateCommand.Execute(CalcMode);
        }
    }
}