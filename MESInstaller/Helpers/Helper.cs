using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInstaller.Helpers
{
    public class Helper : IHelper
    {
        public bool IsDone { get; protected set; }

        public int CompletedPercentage { get; protected set; }

        public int PercentageOfTotal { get; protected set; }

        public void Run()
        {
            Task task = new Task(() => {
                Execute();

                CompletedPercentage = 100;
                IsDone = true;
            });

            task.Start();
        }

        protected virtual void Execute()
        {
            return;
        }
    }
}
