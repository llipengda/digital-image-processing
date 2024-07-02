using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class TwoImagesInputViewModel : ViewModelBase
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(Src1Bmp))]
    [NotifyCanExecuteChangedFor(nameof(SwapCommand))]
    private Mat? _src1;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(Src2Bmp))]
    [NotifyCanExecuteChangedFor(nameof(SwapCommand))]
    private Mat? _src2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResBmp))]
    [NotifyCanExecuteChangedFor(nameof(SwapCommand))]
    private Mat? _res;

    public Bitmap? Src1Bmp => Src1?.ToBitmap();
    
    public Bitmap? Src2Bmp => Src2?.ToBitmap();
    
    public Bitmap? ResBmp => Res?.ToBitmap();
    
    [ObservableProperty] private bool _src1Visible;
    
    [ObservableProperty] private bool _src2Visible;
    
    [ObservableProperty] private bool _resVisible;
    
    public bool CanExecute() => Src1 != null && Src2 != null;

    [RelayCommand]
    private async Task UploadSrc1()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)
            (Application.Current?.ApplicationLifetime ??
             throw new InvalidOperationException("ApplicationLifetime not found"));
        var topLevel = TopLevel.GetTopLevel(lifetime.MainWindow) ??
                       throw new InvalidOperationException("TopLevel not found");

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "选择图片",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("图片")
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp"]
                    }
                ]
            }
        );

        if (files.Count == 0)
        {
            return;
        }

        var file = files[0];

        var image = API.Basic.ReadImage(file.Path);

        Src1 = image;
        Src1Visible = true;
    }
    
    [RelayCommand]
    private async Task UploadSrc2()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)
            (Application.Current?.ApplicationLifetime ??
             throw new InvalidOperationException("ApplicationLifetime not found"));
        var topLevel = TopLevel.GetTopLevel(lifetime.MainWindow) ??
                       throw new InvalidOperationException("TopLevel not found");

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "选择图片",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("图片")
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp"]
                    }
                ]
            }
        );

        if (files.Count == 0)
        {
            return;
        }

        var file = files[0];

        var image = API.Basic.ReadImage(file.Path);

        Src2 = image;
        Src2Visible = true;
    }

    [RelayCommand]
    private async Task SaveRes()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)
            (Application.Current?.ApplicationLifetime ??
             throw new InvalidOperationException("ApplicationLifetime not found"));
        var topLevel = TopLevel.GetTopLevel(lifetime.MainWindow) ??
                       throw new InvalidOperationException("TopLevel not found");
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                SuggestedFileName = "result.png",
                FileTypeChoices =
                [
                    new FilePickerFileType("PNG")
                    {
                        Patterns = ["*.png"]
                    }
                ]
            });
        
        if (file == null)
        {
            return;
        }
        
        API.Basic.SaveImage(Res!, file.Path.AbsolutePath);
    }
    
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private void Swap()
    {
        (Src1, Src2) = (Src2, Src1);
        
        (Src1Visible, Src2Visible) = (Src2Visible, Src1Visible);
    }

    protected virtual void Src1Changed(Mat? value)
    {
        
    }

    protected virtual void Src2Changed(Mat? value)
    {
    }

    partial void OnSrc1Changed(Mat? value)
    {
        Src1Changed(value);
    }
    
    partial void OnSrc2Changed(Mat? value)
    {
        Src2Changed(value);
    }
}