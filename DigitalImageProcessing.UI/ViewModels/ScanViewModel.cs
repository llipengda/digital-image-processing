﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalImageProcessing.API;
using OpenCvSharp;
using Point = Avalonia.Point;

namespace DigitalImageProcessing.UI.ViewModels;

public partial class ScanViewModel : SingleImageInputViewModel
{
    public ScanViewModel()
    {
        _points = new();
        PolygonPoints = new(Points.Select(p => new Point(p.X + 5, p.Y + 5)).ToList());
    }

    private Scan? _scan;

    [ObservableProperty] private ObservableCollection<Point> _points;

    private int _selectedPointIndex = -1;

    [ObservableProperty] private ObservableCollection<Point> _polygonPoints;

    [RelayCommand]
    private void StartSelection(Point point)
    {
        var points = Points.Where(p => Point.Distance(p, point) < 10).ToList();

        if (points.Count == 0)
        {
            return;
        }

        _selectedPointIndex = Points.IndexOf(points.MinBy(p => Point.Distance(p, point)));
    }

    [RelayCommand]
    private void UpdateSelection(Point point)
    {
        if (_selectedPointIndex != -1)
        {
            Points[_selectedPointIndex] = point;
            PolygonPoints[_selectedPointIndex] = new Point(point.X + 5, point.Y + 5);
        }
    }

    [RelayCommand]
    private void EndSelection(Point point)
    {
        UpdateSelection(point);
        _selectedPointIndex = -1;
    }

    private double _scale;

    private double _dx;

    private double _dy;

    [ObservableProperty] private bool _isPointsVisible;

    protected override void SrcChanged(Mat? value)
    {
        if (_scan is null)
        {
            IsPointsVisible = false;
            _scan = new(Src!);
            var width = _scan.OriginalImage.Width;
            var height = _scan.OriginalImage.Height;
            var scaleW = 600.0 / width;
            var scaleH = 400.0 / height;
            var scale = Math.Min(scaleW, scaleH);
            _scale = scale;
            var dx = 160 + 300 - width * scale / 2 - 5;
            var dy = 100 + 200 - height * scale / 2 - 5;
            _dx = dx;
            _dy = dy;
            Console.WriteLine(scale);
            Points = new(_scan.Points.Select(p => new Point(p.X * scale + dx, p.Y * scale + dy)).ToList());
            PolygonPoints = new(Points.Select(p => new Point(p.X + 5, p.Y + 5)).ToList());
            Src = _scan.Image;
            IsPointsVisible = true;
        }

        base.SrcChanged(value);
    }

    [RelayCommand]
    private void Calc()
    {
        if (_scan is null)
        {
            return;
        }

        _scan.Calc(Points.Select(p => new OpenCvSharp.Point((int)((p.X - _dx) / _scale), (int)((p.Y - _dy) / _scale)))
            .ToArray());
        Src = _scan.Image;
        Res = _scan.Image;
        
        IsPointsVisible = false;
    }

    [RelayCommand]
    private void Upload()
    {
        _scan = null;
        
        UploadSrcCommand.Execute(null);
    }
}