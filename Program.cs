using LayoutParserLib;

using System;
using System.IO;
using System.Text;

namespace LayoutParserDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Environment.Exit(1);
                return;
            }

            try
            {
                string inputFile = args[0];
                string outputFile = args[1];
                string correlationId = args.Length >= 3 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : Guid.NewGuid().ToString();
                string logDir = args.Length >= 4 ? args[3] : (Environment.GetEnvironmentVariable("LAYOUTPARSER_LOG_DIR") ?? "");

                RollingFileLogger.Log(logDir, "layoutparserdecrypt.log", correlationId, "INF", $"START decrypt input='{inputFile}' output='{outputFile}'");

                if (!File.Exists(inputFile))
                {
                    RollingFileLogger.Log(logDir, "layoutparserdecrypt.log", correlationId, "ERR", "InputFile não existe");
                    Environment.Exit(1);
                    return;
                }

                string encryptedContent = File.ReadAllText(inputFile, Encoding.UTF8);

                string contentToDecrypt = encryptedContent;
                if (encryptedContent.Length > 3)
                    contentToDecrypt = encryptedContent.Substring(3);

                RollingFileLogger.Log(logDir, "layoutparserdecrypt.log", correlationId, "INF", $"Decrypting {contentToDecrypt.Length} chars");
                string decrypted = CryptographySysMiddle.Decrypt(contentToDecrypt);

                File.WriteAllText(outputFile, decrypted, Encoding.UTF8);

                RollingFileLogger.Log(logDir, "layoutparserdecrypt.log", correlationId, "INF", $"END success outputChars={decrypted?.Length ?? 0}");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                try
                {
                    var correlationId = args.Length >= 3 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : Guid.NewGuid().ToString();
                    var logDir = args.Length >= 4 ? args[3] : (Environment.GetEnvironmentVariable("LAYOUTPARSER_LOG_DIR") ?? "");
                    RollingFileLogger.Log(logDir, "layoutparserdecrypt.log", correlationId, "ERR", "FATAL decrypt", ex);
                }
                catch { }
                Environment.Exit(1);
            }
        }
    }
}