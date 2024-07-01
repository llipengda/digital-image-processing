using DigitalImageProcessing.API;

var image111 = Basic.ReadImage(@"C:\Users\lipen\Desktop\111.png");
var image111Hist = Calculate.HistImage(image111);
var image222 = Basic.ReadImage(@"C:\Users\lipen\Desktop\222.png");
var image222Hist = Calculate.HistImage(image222);
var normalizedImage = Enhancement.NormalizeHist(image111, image222);
var normalizedImageHist = Calculate.HistImage(normalizedImage);

Basic.ShowImages(
    (image111, "image111"), (image111Hist, "image111Hist"),
    (image222, "image222"), (image222Hist, "image222Hist"),
    (normalizedImage, "normalizedImage"), (normalizedImageHist, "normalizedImageHist")
);