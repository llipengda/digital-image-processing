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
    [NotifyPropertyChangedFor(nameof(Src1Visible))]
    private Mat? _src1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Src2Bmp))]
    [NotifyCanExecuteChangedFor(nameof(SwapCommand))]
    [NotifyPropertyChangedFor(nameof(Src2Visible))]
    private Mat? _src2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResBmp))]
    [NotifyCanExecuteChangedFor(nameof(SwapCommand))]
    [NotifyPropertyChangedFor(nameof(ResVisible))]
    private Mat? _res;

    [ObservableProperty] private Bitmap? _src1Bmp;

    [ObservableProperty] private Bitmap? _src2Bmp;

    [ObservableProperty] private Bitmap? _resBmp;

    public bool Src1Visible => Src1 is not null;

    public bool Src2Visible => Src2 is not null;

    public bool ResVisible => Res is not null;

    public bool CanExecute() => Src1 != null && Src2 != null;

    protected TwoImagesInputViewModel()
    {
        Src1Bmp = ImageSingleton.Instance.Bmp;
        Src2Bmp = ImageSingleton.Instance.Bmp2;

        Task.WhenAll([Func1(), Func2()]);
        return;

        async Task<Task> Func2()
        {
            await Task.Delay(10);
            Src2 = ImageSingleton.Instance.Mat2;
            return Task.CompletedTask;
        }

        async Task<Task> Func1()
        {
            await Task.Delay(10);
            Src1 = ImageSingleton.Instance.Mat;
            return Task.CompletedTask;
        }
    }

    [RelayCommand]
    private void SaveToSrc1()
    {
        if (Res is null)
        {
            return;
        }

        Src1 = Res;

        ImageSingleton.Instance.Mat = Src1;
        ImageSingleton.Instance.Bmp = Src1Bmp;
    }
    
    [RelayCommand]
    private void SaveToSrc2()
    {
        if (Res is null)
        {
            return;
        }

        Src2 = Res;

        ImageSingleton.Instance.Mat2 = Src2;
        ImageSingleton.Instance.Bmp2 = Src2Bmp;
    }

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

        ImageSingleton.Instance.Mat = image;
        ImageSingleton.Instance.Bmp = image.ToBitmap();
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
        
        ImageSingleton.Instance.Mat2 = image;
        ImageSingleton.Instance.Bmp2 = image.ToBitmap();
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
        (ImageSingleton.Instance.Mat, ImageSingleton.Instance.Mat2) = (Src1, Src2);
        (ImageSingleton.Instance.Bmp, ImageSingleton.Instance.Bmp2) = (Src1Bmp, Src2Bmp);
    }

    protected virtual void Src1Changed(Mat? value)
    {
    }

    protected virtual void Src2Changed(Mat? value)
    {
    }
    
    protected virtual void ResChanged(Mat? value)
    {
    }

    partial void OnSrc1Changed(Mat? value)
    {
        Src1Bmp = value?.ToBitmap();
        Src1Changed(value);
    }

    partial void OnSrc2Changed(Mat? value)
    {
        Src2Bmp = value?.ToBitmap();
        Src2Changed(value);
    }

    partial void OnResChanged(Mat? value)
    {
        ResBmp = value?.ToBitmap();
        ResChanged(value);
    }
}