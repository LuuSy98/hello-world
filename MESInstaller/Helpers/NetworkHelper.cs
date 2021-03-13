using MESInstaller.Models;
using System.Diagnostics;

namespace MESInstaller.Helpers
{
    public class NetworkHelper : Helper
    {
        private IPData iPData = new IPData();
        public NetworkHelper(IPData iPData, int percentageOfTotal)
        {
            this.iPData = iPData;
            PercentageOfTotal = percentageOfTotal;
        }

        protected override void Execute()
        {
            SetIP(iPData.IPString, iPData.SubnetMask, iPData.DefaultGateway);
            CompletedPercentage += 50;

            if (!iPData.SkipDNS)
            {
                SetDNS(iPData.PreferredDNS, iPData.AlternateDNS);
            }
            CompletedPercentage = 100;
        }

        public static void SetIP(string IP, string subMask, string gateWay, string adapterName = "MES")
        {
            string strCmdArgs = $"interface ipv4 set address name=\"{adapterName}\" static {IP} {subMask} {gateWay}";

            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Verb = "runas";
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = strCmdArgs;

            process.Start();

            Define.Logger.AddLog("NETW", $"Setting adapter \"{adapterName}\" IP {IP} {subMask} {gateWay} Success!");
        }
        public static void SetDNS(string preferredDNS, string alternateDNS, string adapterName = "MES")
        {
            SetPreferredDNS(preferredDNS, adapterName);
            SetAlternateDNS(alternateDNS, adapterName);
        }

        public static void SetPreferredDNS(string preferredDNS, string adapterName = "MES")
        {
            string strCmdArgs = $"interface ipv4 set dns name=\"{adapterName}\" static {preferredDNS}";
            Process process = new Process();
            process.StartInfo.Verb = "runas";
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = strCmdArgs;

            process.Start();

            Define.Logger.AddLog("NETW", $"Setting adapter \"{adapterName}\" dns {preferredDNS} Success!");
        }

        public static void SetAlternateDNS(string alternateDNS, string adapterName = "MES")
        {
            string strCmdArgs = $"interface ipv4 set dns name=\"{adapterName}\" static {alternateDNS} index=2";
            Process process = new Process();
            process.StartInfo.Verb = "runas";
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = strCmdArgs;

            process.Start();

            Define.Logger.AddLog("NETW", $"Setting adapter \"{adapterName}\" dns index=2 {alternateDNS} Success!");
        }
    }
}
