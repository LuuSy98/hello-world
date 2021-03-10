using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MESInstaller.Models
{
    public class Define
    {
        public static readonly List<string> LINE_LIST = new List<string>
        {
            "IR_LINE",
            "VCM_LINE",
        };

        public static readonly List<string> VCM_MACHINE_LIST = new List<string>
        {
            "V200_CarrierLoading",
            "V200_SolderPaste",
            "V200_WeldingBossPin",
            "V200_Jetting441",
            "V300_USPCutting",
            "V500_CarrierBaseAssemble",
            "V300_FullAssyAssemble",
            "V300_USPBonding",
            "V300_FullBonding",
            "V700_FunctionInspection",
        };

        // Todo: Modify this list
        public static readonly List<string> IR_MACHINE_LIST = new List<string>
        {
            "LensAssemble_OLD",
            "LensAssemble_NEW",
            "Dispensing_VeryOLD",
            "Dispensing_OLD",
            "Dispensing_NEW",
        };
    }
}
