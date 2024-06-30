using DigitalImageProcessing.API;

var image = Basic.ReadImage(@"C:\Users\lipen\OneDrive\图片\屏幕快照\2022-10 -16.png");
image = Basic.ToHsv(image);
Basic.ShowImage(image, 800, 600, "HSV Image");