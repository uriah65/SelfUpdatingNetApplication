using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upgrader
{
    public class UpgradeException : Exception
    {
        public UpgradeException(string mesage, Exception innerException = null) : base(mesage, innerException)
        {
        }
    }
}
