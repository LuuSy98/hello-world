using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInstaller.Helpers
{
    public interface IHelper
    {
        bool IsDone { get; }
        int CompletedPercentage { get; }
        int PercentageOfTotal { get; }
        void Run();
    }
}
