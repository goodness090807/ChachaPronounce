using System.Diagnostics;

namespace AudioProcessConsumer.Services
{
    public static class FileService
    {
        public static string SaveFileByStream(this MemoryStream memoryStream, string name, string extension)
        {
            var fileName = $"{name}.{extension}";

            var fileStream = File.Create(fileName);
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(fileStream);
            fileStream.Close();

            return fileName;
        }
        
        public static string ConvertAudio(string fileName, string convertExtension)
        {
            var convertedFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.{convertExtension}";

            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = $"-i {fileName} {convertedFileName}";
            process.Start();
            process.WaitForExit();
            process.Close();

            return convertedFileName;
        }
    }
}
