using MESInstaller.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows;

namespace MESInstaller.Helpers
{
    public class BasicContentHelper : Helper
    {
        // D:\MESContent
        //              |---TMAX.zip
        //              |---AGENT.zip
        //              |---MES.zip
        public static string TMAX_TargetDirectory { get; set; } = @"C:\TMAX";
        public static string AGENT_TargetDirectory { get; set; } = @"D:\Agent";
        public static string MES_TargetDirectory { get; set; } = @"D:\MES";

        private string basicContentPath = "";
        public BasicContentHelper(string basicContentPath, int percentageOfTotal)
        {
            this.basicContentPath = basicContentPath;
            PercentageOfTotal = percentageOfTotal;
        }

        protected override void Execute()
        {
            if (HelperStatus.UseBasicContentHelper == false) return;

            ContentCopy(basicContentPath);
            System.Diagnostics.Process.Start(@"D:\MES\AGENT_POWER\setup.exe");
        }

        public void ContentCopy(string Content_SourceDirectory)
        {
            CopyAGENT(Content_SourceDirectory);
            CompletedPercentage += 30;
            CopyMES(Content_SourceDirectory);
            CompletedPercentage += 30;
            CopyTMAX(Content_SourceDirectory);
            CompletedPercentage = 100;
        }

        public void CopyMES(string Content_SourceDirectory)
        {
            // Extract MES content to MES_Directory
            try
            {
                ZipFile.ExtractToDirectory(Path.Combine(Content_SourceDirectory, "MES.zip"), MES_TargetDirectory);
                Define.Logger.AddLog("BASIC", $"Extract MES.zip to {MES_TargetDirectory} Success");
            }
            catch (Exception ex)
            {
                Define.Logger.AddLog("BASIC", $"{ex.Message}", IsError : true);
            }

            Directory.CreateDirectory(Path.Combine(MES_TargetDirectory, "EQUIP"));
        }
        public static void CopyAGENT(string Content_SourceDirectory)
        {
            // Extract AGENT content to AGENT_Directory

            try
            {
                ZipFile.ExtractToDirectory(Path.Combine(Content_SourceDirectory, "AGENT.zip"), AGENT_TargetDirectory);
                Define.Logger.AddLog("BASIC", $"Extract AGENT.zip to {AGENT_TargetDirectory} Success");
            }
            catch (Exception ex)
            {
                Define.Logger.AddLog("BASIC", $"{ex.Message}", IsError : true);
            }

        }
        public static void CopyTMAX(string Content_SourceDirectory)
        {
            //Extract TMAX content to  TMAX_Directory
            try
            {
                ZipFile.ExtractToDirectory(Path.Combine(Content_SourceDirectory, "TMAX.zip"), TMAX_TargetDirectory);
                Define.Logger.AddLog("BASIC", $"Extract TMAX.zip to {TMAX_TargetDirectory} Success");
            }
            catch (Exception ex)
            {
                Define.Logger.AddLog("BASIC", $"{ex.Message}", IsError: true);
            }
        }
    }
}
