using System;
using System.IO;
using System.Text;

namespace LayoutParserDecrypt
{
    internal static class RollingFileLogger
    {
        private const long MaxBytes = 2049L * 1024L;
        private const int MaxFiles = 10;

        public static void Log(string logDir, string baseFileName, string correlationId, string level, string message, Exception ex = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(logDir))
                    logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

                Directory.CreateDirectory(logDir);

                var basePath = Path.Combine(logDir, baseFileName);
                RollIfNeeded(logDir, baseFileName, basePath);

                var line = $"{DateTime.UtcNow:O} [{level}] [Corr:{correlationId}] {message}";
                if (ex != null) line += " | " + ex;
                File.AppendAllText(basePath, line + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // nunca lançar log failure
            }
        }

        private static void RollIfNeeded(string logDir, string baseFileName, string basePath)
        {
            try
            {
                var fi = new FileInfo(basePath);
                if (!fi.Exists) return;
                if (fi.Length < MaxBytes) return;

                var stamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                var rolled = Path.Combine(logDir, Path.GetFileNameWithoutExtension(baseFileName) + "-" + stamp + Path.GetExtension(baseFileName));
                File.Move(basePath, rolled);

                // manter só os 10 mais recentes
                var pattern = Path.GetFileNameWithoutExtension(baseFileName) + "-*.log";
                var files = new DirectoryInfo(logDir).GetFiles(pattern);
                Array.Sort(files, (a, b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
                for (int i = MaxFiles - 1; i < files.Length; i++)
                {
                    try { files[i].Delete(); } catch { }
                }
            }
            catch
            {
            }
        }
    }
}


