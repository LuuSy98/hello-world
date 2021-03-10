using System.IO;
using System.IO.Compression;
using System.Windows;

namespace MESInstaller.Helpers
{
    public class BasicContentHelper
    {
        // D:\MESContent
        //              |---TMAX.zip
        //              |---AGENT.zip
        //              |---MES.zip
        public static string TMAX_TargetDirectory { get; set; } = @"C:\TMAX";
        public static string AGENT_TargetDirectory { get; set; } = @"D:\Agent";
        public static string MES_TargetDirectory { get; set; } = @"D:\MES";

        public static void ContentCopy(string Content_SourceDirectory)
        {
            CopyAGENT(Content_SourceDirectory);
            CopyMES(Content_SourceDirectory);
            CopyTMAX(Content_SourceDirectory);
            MessageBox.Show("Copy and Extract done successfully.");
        }

        public static void CopyMES(string Content_SourceDirectory)
        {
            // Extract MES content to MES_Directory
            ZipFile.ExtractToDirectory(Path.Combine(Content_SourceDirectory, "MES.zip"), MES_TargetDirectory);

            Directory.CreateDirectory(Path.Combine(MES_TargetDirectory, "EQUIP"));
        }
        public static void CopyAGENT(string Content_SourceDirectory)
        {
            // Extract AGENT content to AGENT_Directory
            ZipFile.ExtractToDirectory(Path.Combine(Content_SourceDirectory, "AGENT.zip"), AGENT_TargetDirectory);
        }
        public static void CopyTMAX(string Content_SourceDirectory)
        {
            //Extract TMAX content to  TMAX_Directory
            ZipFile.ExtractToDirectory(Path.Combine(Content_SourceDirectory, "TMAX.zip"), TMAX_TargetDirectory);
        }
    }
}
