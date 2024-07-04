using System.Diagnostics;

namespace DigitalImageProcessing.ML;

public static class Vgg19
{
    private static Process? _process;
    
    public static bool Execute(string path1, string path2)
    {
        var cur = Directory.GetCurrentDirectory();

        var result = @$"{cur}\vgg19\result.png";

        if (File.Exists(result))
        {
            File.Delete(result);
        }
        
        _process?.Kill();

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = @$"{cur}\vgg19\",
                FileName = @$"{cur}\vgg19\vgg19.exe",
                Arguments = $"{path1} {path2}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        _process.Start();

        _process.WaitForExit();

        var error = _process.StandardError.ReadToEnd();
        var output = _process.StandardOutput.ReadToEnd();

        Console.WriteLine(error);
        Console.WriteLine(output);

        _process.Close();
        
        _process.Dispose();
        
        _process = null;

        return File.Exists(result);
    }
}