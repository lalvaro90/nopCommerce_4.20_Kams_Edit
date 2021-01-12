using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataLibrary
{
    public class LogHelper
    {
        static private string MAINPATH = @"C:\store_sync";


        static public void WriteToLog(string content) {

            try
            {
                FileStream fs = null;
                var sdate = DateTime.Now.ToShortDateString();
                var path = Path.Combine(MAINPATH, $"{sdate}.log");
                if (!Directory.Exists(MAINPATH)) {
                    Directory.CreateDirectory(MAINPATH);
                }

                if (!File.Exists(path)) {
                    fs = File.Create(path);
                    fs.Close();
                    fs.Dispose();
                }

                var emptyLine = new string[] { "", content };
                File.AppendAllLines(path, emptyLine);               

            }
            catch (Exception ex)
            {

            }

        }


    }
}
