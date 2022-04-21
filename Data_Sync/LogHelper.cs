using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Sync
{
    public class LogHelper
    {
        private static string MAINPATH = "C:\\store_sync";

        public static void WriteToLog(string content)
        {
            try
            {
                string shortDateString = DateTime.Now.ToShortDateString();
                string path = Path.Combine(LogHelper.MAINPATH, shortDateString + ".log");
                if (!Directory.Exists(LogHelper.MAINPATH))
                    Directory.CreateDirectory(LogHelper.MAINPATH);
                if (!File.Exists(path))
                {
                    FileStream fileStream = File.Create(path);
                    fileStream.Close();
                    fileStream.Dispose();
                }
                string[] contents = new string[2] { "", content };
                File.AppendAllLines(path, (IEnumerable<string>)contents);
            }
            catch (Exception ex)
            {
            }
        }

        public static void WriteToErrorLog(string content)
        {
            try
            {
                string shortDateString = DateTime.Now.ToShortDateString();
                string path = Path.Combine(LogHelper.MAINPATH, shortDateString + "-Error.log");
                if (!Directory.Exists(LogHelper.MAINPATH))
                    Directory.CreateDirectory(LogHelper.MAINPATH);
                if (!File.Exists(path))
                {
                    FileStream fileStream = File.Create(path);
                    fileStream.Close();
                    fileStream.Dispose();
                }
                string[] contents = new string[2] { "", content };
                File.AppendAllLines(path, (IEnumerable<string>)contents);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
