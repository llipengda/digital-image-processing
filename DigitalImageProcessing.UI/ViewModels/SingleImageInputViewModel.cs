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

public partial class SingleImageInputViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SrcBmp))] [NotifyPropertyChangedFor(nameof(SrcVisible))]
    private Mat? _src;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ResBmp))] [NotifyPropertyChangedFor(nameof(ResVisible))]
    private Mat? _res;

    protected SingleImageInputViewModel()
    {
        SrcBmp = ImageSingleton.Instance.Bmp;
        Task.Run(async () =>
        {
            await Task.Delay(10);
            Src = ImageSingleton.Instance.Mat;
        });
    }

    [ObservableProperty] private Bitmap? _srcBmp;
    
    [ObservableProperty] private Bitmap? _resBmp;

    public bool SrcVisible => Src is not null;

    public bool ResVisible => Res is not null;

    protected bool CanExecute() => Src != null;

    [RelayCommand]
    private void SaveToSrc()
    {
        if (Res is null)
        {
            return;
        }

        Src = Res;

        ImageSingleton.Instance.Mat = Src;
        ImageSingleton.Instance.Bmp = SrcBmp;
    }

    [RelayCommand]
    private async Task UploadSrc()
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

        Src = image;

        ImageSingleton.Instance.Mat = image;
        ImageSingleton.Instance.Bmp = image.ToBitmap();
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


    protected virtual void SrcChanged(Mat? value)
    {
    }


    partial void OnSrcChanged(Mat? value)
    {
        SrcBmp = value?.ToBitmap();
        SrcChanged(value);
    }

    partial void OnResChanged(Mat? value)
    {
        ResBmp = value?.ToBitmap();
    }
}