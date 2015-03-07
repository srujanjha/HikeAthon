using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hikeathon
{
    public enum ActivationRequestResults
    {
        AllEnabled,
        AskMeLater,
        NoAndDontAskAgain,
        NotAvailableYet
    };

    public class ActivateSensorCoreStatus
    {
        public ActivationRequestResults ActivationRequestResult;
        public bool Ongoing = false;

    }
}
