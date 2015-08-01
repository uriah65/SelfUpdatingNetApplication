using System;

namespace Upgrader
{
    public class UpgradeException : Exception
    {
        public UpgradeException(string mesage, Exception innerException = null) : base(mesage, innerException)
        {
        }
    }
}