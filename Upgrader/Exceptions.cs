using System;

namespace Upgrader
{
    public class UpgradeException : Exception
    {
        public UpgradeException(string mesage, Exception innerException) : base(mesage, innerException)
        {
        }
    }

    public class UpgradeUpgraderException : UpgradeException
    {
        public UpgradeUpgraderException(string message, Exception innerException) :base(message, innerException)
        {            
        }
    }

}