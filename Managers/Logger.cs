using System.IO;

namespace Synthesis.Managers
{
    public class Logger
    {
        private const string LoggerDirectoryPath = "\\log.txt";
        public static void WriteToLog(string log)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + LoggerDirectoryPath))
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + LoggerDirectoryPath, log);
            }
            else
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + LoggerDirectoryPath, log);
            }
        }
    }
}
