using OpenCvSharp;

namespace DigitalImageProcessing.API;

public static class Morphology
{
    public static Mat Erode(Mat image, int kernelSize)
    {
        image = Basic.ToBinary(image);
        var kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(kernelSize, kernelSize));
        var result = new Mat();
        Cv2.Erode(image, result, kernel);
        return result;
    }
    
    public static Mat Dilate(Mat image, int kernelSize)
    {
        image = Basic.ToBinary(image);
        var kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(kernelSize, kernelSize));
        var result = new Mat();
        Cv2.Dilate(image, result, kernel);
        return result;
    }
    
    public static Mat Open(Mat image, int kernelSize)
    {
        image = Basic.ToBinary(image);
        var kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(kernelSize, kernelSize));
        var result = new Mat();
        Cv2.MorphologyEx(image, result, MorphTypes.Open, kernel);
        return result;
    }
    
    public static Mat Close(Mat image, int kernelSize)
    {
        image = Basic.ToBinary(image);
        var kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(kernelSize, kernelSize));
        var result = new Mat();
        Cv2.MorphologyEx(image, result, MorphTypes.Close, kernel);
        return result;
    }
}