using MESInstaller.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MESInstaller.Helpers
{
    public class LogHelper : IDisposable
    {
        public static string LogFilePath = $"Log\\{DateTime.Now:yyyy-MM-dd}\\MesInstall_Log.txt";
        public static string ErrorLogFilePath = $"Log\\{DateTime.Now:yyyy-MM-dd}\\MesInstall_ErrorLog.txt";

        private readonly Queue<LogData> LogDataQueue = new Queue<LogData>();
        private bool LogHelpDisposed = false;

        private int numberOfError;

        public LogHelper()
        {
            Thread task = new Thread(() => {
                while(!LogHelpDisposed || LogDataQueue.Count != 0)
                {
                    WriteLog();
                    Thread.Sleep(100);
                }
            });

            task.Start();
        }

        public void AddLog(string ProgressName, string LogMessage, bool IsError = false)
        {
            LogData logData = new LogData { ProgressName = ProgressName, LogMessage = LogMessage, IsError = IsError };

            AddLog(logData);
        }

        public void AddLog(LogData logData)
        {
            if (logData.IsError)
            {
                numberOfError++;
            }

            LogDataQueue.Enqueue(logData);
        }

        public void Dispose()
        {
            LogHelpDisposed = true;
        }

        private void WriteLog()
        {
            if (LogDataQueue.Count <= 0)
            {
                return;
            }
            LogData logData = LogDataQueue.Dequeue();

            if (!logData.IsError)
            {
                string content = $"[{logData.LogTime:hh:mm:ss.fff}],{logData.ProgressName},{logData.LogMessage}\n";

                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                File.AppendAllText(LogFilePath, content);
            }
            else
            {
                string content = $"[{logData.LogTime:hh:mm:ss.fff}],{logData.ProgressName},ERROR_{logData.LogMessage}\n";

                Directory.CreateDirectory(Path.GetDirectoryName(ErrorLogFilePath));
                File.AppendAllText(ErrorLogFilePath, content);
            }
        }

        public int ErrorCount()
        {
            return numberOfError;
        }

        public void ResetErrorCount()
        {
            numberOfError = 0;
        }
    }
}

