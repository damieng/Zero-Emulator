using System;
using System.Diagnostics;
using System.IO;

namespace ZeroWin
{
    public class Logger: IDisposable
    {
        StreamWriter sw;

        public void Log(string s, bool finalise = false)
        {
            if (sw == null)
               sw = new StreamWriter(@"ZeroLog.txt");

            sw.WriteLine(s);
            sw.Flush();

            if (finalise)
                sw.Close();
        }

        public void DebugLog(string s)
        {
            Debug.WriteLine(s);
        }

        public void Dispose()
        {
            sw?.Close();
        }
    }
}
