namespace DigitalImageProcessing.ML;

using System.Diagnostics;

public static class Cartoon
{
    private static Process? _process;
    
    public static bool Execute(string path)
    {
        var cur = Directory.GetCurrentDirectory();
        
        var result = @$"{cur}\cartoon\result.png";
        
        if (File.Exists(result))
        {
            File.Delete(result);
        }

        _process?.Kill();

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = @$"{cur}\cartoon\",
                FileName = @$"{cur}\cartoon\cartoon.exe",
                Arguments = $"--photo_path {path} --save_path ./result.png",
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