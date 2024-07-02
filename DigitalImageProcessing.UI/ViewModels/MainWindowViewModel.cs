using System;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentViewModel;

    public MainWindowViewModel()
    {
        CurrentViewModel = new CalculateViewModel();
    }

    [RelayCommand]
    public void NavigateTo(string tag)
    {
        var page = $"{Assembly.GetExecutingAssembly().GetName().Name}.ViewModels.{tag}ViewModel";
        var viewModel = Activator.CreateInstance(Type.GetType(page) ??
                                                 throw new InvalidOperationException("ViewModel not found"));
        CurrentViewModel = (ViewModelBase)(viewModel ?? throw new InvalidOperationException("ViewModel not found"));
    }
}