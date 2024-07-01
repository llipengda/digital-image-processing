using DigitalImageProcessing.API;

var image = Basic.ReadImage(@"C:\Users\lipen\Desktop\1.png");
var smoothedIdeal = Smoothing.Frequency(image, FrequencySmoothingType.IdealLowPass);
var smoothedButterworth = Smoothing.Frequency(image, FrequencySmoothingType.ButterworthLowPass);
var smoothedGaussian = Smoothing.Frequency(image, FrequencySmoothingType.GaussianLowPass);

Basic.ShowImages(
    (Basic.ToGray(image), "Original"),
    (smoothedIdeal, "Ideal Low Pass"),
    (smoothedButterworth, "Butterworth Low Pass"),
    (smoothedGaussian, "Gaussian Low Pass")
);
