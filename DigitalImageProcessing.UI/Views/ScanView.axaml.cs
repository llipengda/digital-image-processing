using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using DigitalImageProcessing.UI.ViewModels;

namespace DigitalImageProcessing.UI.Views;

public partial class ScanView : UserControl
{
    public ScanView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(this);
        (DataContext as ScanViewModel)?.StartSelectionCommand.Execute(point);
    }

    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(this);
            (DataContext as ScanViewModel)?.UpdateSelectionCommand.Execute(point);
        }
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        var point = e.GetPosition(this);
        (DataContext as ScanViewModel)?.EndSelectionCommand.Execute(point);
    }
}