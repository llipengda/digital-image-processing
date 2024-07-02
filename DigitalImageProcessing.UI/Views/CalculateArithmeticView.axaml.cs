using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using DigitalImageProcessing.UI.ViewModels;

namespace DigitalImageProcessing.UI.Views;

public partial class CalculateArithmeticView : UserControl
{
    public CalculateArithmeticView()
    {
        InitializeComponent();
        DataContext = new CalculateArithmeticViewModel();
    }
}