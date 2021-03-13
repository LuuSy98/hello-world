using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInstaller.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IPData : PropertyChangedNotifier, ICloneable
    {
        #region Properties
        [JsonProperty]
        public string IPString
        {
            get { return _IPString; }
            set
            {
                _IPString = value;
                OnPropertyChanged("IPString");
            }
        }

        [JsonProperty]
        public string SubnetMask
        {
            get { return _SubnetMask; }
            set
            {
                _SubnetMask = value;
                OnPropertyChanged("SubnetMask");
            }
        }

        [JsonProperty]
        public string DefaultGateway
        {
            get { return _DefaultGateway; }
            set
            {
                _DefaultGateway = value;
                OnPropertyChanged("DefaultGateway");
            }
        }

        [JsonProperty]
        public bool SkipDNS
        {
            get { return _SkipDNS; }
            set
            {
                _SkipDNS = value;
                OnPropertyChanged("SkipDNS");
            }
        }

        [JsonProperty]
        public string PreferredDNS
        {
            get { return _PreferredDNS; }
            set
            {
                _PreferredDNS = value;
                OnPropertyChanged("DefaultGateway");
            }
        }

        [JsonProperty]
        public string AlternateDNS
        {
            get { return _AlternateDNS; }
            set
            {
                _AlternateDNS = value;
                OnPropertyChanged("DefaultGateway");
            }
        }
        #endregion

        #region Privates
        private string _IPString = "172.16.161.";
        private string _SubnetMask = "255.255.255.0";
        private string _DefaultGateway = "172.16.161.1";
        private bool _SkipDNS = true;
        private string _PreferredDNS = "";
        private string _AlternateDNS = "";

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
