using System;
using Avalonia.Controls;
using DigitalImageProcessing.UI.ViewModels;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;

namespace DigitalImageProcessing.UI.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
        
        DataContext = new MainWindowViewModel();
        
        NavigationView.SelectionChanged += OnSelectionChanged;
    }
    
    private void OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem nvi)
        {
            var view = DataContext as MainWindowViewModel;
            view?.NavigateTo(nvi.Tag?.ToString() ?? throw new InvalidOperationException("Tag not found"));
        }
    }
}