using Microsoft.Extensions.Hosting.Internal;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace KardiaAPI
{
    public class Common
    {
        //private readonly IHostingEnvironment _environment;
        //public Common(IHostingEnvironment environment)
        //{
        //    _environment = environment;
        //}

        //public void LogError(string value)
        //{
        //    string logFilePath = @$"{DateTime.Now.ToString("dd - MMM - yyyy_HH - mm - ss").ToUpper()}.txt";
        //    string logMessage;
        //    string filePath = HostingEnvironment.WebRootPath;
        //    if (!filePath.EndsWith(@"\")) { filePath += @"\"; }
        //    filePath += @"logs";
        //    if (!System.IO.Directory.Exists(filePath))
        //    {
        //        System.IO.Directory.CreateDirectory(filePath);
        //        //}

        //        // Write a log message to the log file
        //        using (StreamWriter sw = File.AppendText(logFilePath))
        //        {
        //            logMessage = $"{DateTime.Now}: {value}";
        //            sw.WriteLine(logMessage);
        //        }

        //        // Read all the log messages from the log file
        //        string[] logMessages = File.ReadAllLines(logFilePath);
        //        foreach (string logMessag in logMessages)
        //        {
        //            Console.WriteLine(logMessag);
        //        }
        //    }

        public void logsData(string value)
        {
            string path = $"logs/log_{DateTime.Now.ToString("dd - MMM - yyyy_HH - mm - ss").ToUpper()}.txt";

            using (var writer = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                writer.WriteLine($"[{DateTime.Now}] : {value}");
            }
        }
    }
}  


