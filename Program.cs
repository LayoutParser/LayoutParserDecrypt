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

                if (!File.Exists(inputFile))
                {
                    Environment.Exit(1);
                    return;
                }

                string encryptedContent = File.ReadAllText(inputFile, Encoding.UTF8);

                string contentToDecrypt = encryptedContent;
                if (encryptedContent.Length > 3)
                    contentToDecrypt = encryptedContent.Substring(3);

                string decrypted = CryptographySysMiddle.Decrypt(contentToDecrypt);

                File.WriteAllText(outputFile, decrypted, Encoding.UTF8);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Environment.Exit(1);
            }
        }
    }
}