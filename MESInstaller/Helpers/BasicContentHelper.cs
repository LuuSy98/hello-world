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
            
        }

        public void ContentCopy(string Content_SourceDirectory)
        {
            DeployAGENT(Content_SourceDirectory);
            CompletedPercentage += 30;
            DeployTMAX(Content_SourceDirectory);
            CompletedPercentage += 30;

            if (HelperStatus.DeployEASAgent)
            {
                DeployEASAgent(Content_SourceDirectory);
            }

            CompletedPercentage = 100;
        }

        public void DeployEASAgent(string Content_SourceDirectory)
        {
            try
            {
                string executeFilePath = Path.Combine(Content_SourceDirectory, @"EAS_AGENT\setup.exe");
                System.Diagnostics.Process.Start(executeFilePath);
                Define.Logger.AddLog("BASIC", $"Run file {executeFilePath} Success");
            }
            catch (Exception ex)
            {
                Define.Logger.AddLog("BASIC", $"{ex.Message}", IsError : true);
            }
        }

        public static void DeployAGENT(string Content_SourceDirectory)
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
        public static void DeployTMAX(string Content_SourceDirectory)
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
