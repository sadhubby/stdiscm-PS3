using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace ConsumerGUI
{
    public static class VideoThumbnailer
    {
        public static Image GetThumbnail(string videoPath)
        {
            //string ffmpeg = @"C:\ffmpeg\bin\ffmpeg.exe";   // <-- UPDATE PATH
            string ffmpeg = @"D:\Users\user\Downloads\ffmpeg-8.0.1-essentials_build\ffmpeg-8.0.1-essentials_build\bin\ffmpeg.exe";   // <-- UPDATE PATH
            string tempFile = Path.GetTempFileName() + ".jpg";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpeg,
                Arguments = $"-i \"{videoPath}\" -ss 00:00:01 -vframes 1 \"{tempFile}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            Image thumbnail = Image.FromFile(tempFile);
            return thumbnail;
        }
    }
}
