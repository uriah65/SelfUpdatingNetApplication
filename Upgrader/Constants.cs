using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Upgrader
{
    public static class Constants
    {
        public const string LAUNCHED_FROM_APP = "/autoupdate-from-application";
        public const string LAUNCHED_FROM_UPDATER = "/autoupdate-from-updater";

        public static string UPGRADER_EXE_FILE = "Upgrader.exe";
        public static string UPGRADER_CONFIGURATION_FILE = UPGRADER_EXE_FILE + ".config";

        public static readonly string MESSAGE_START_MAINAPP = $"{UPGRADER_EXE_FILE} is out of date, please start main application.";
        public static readonly string MESSAGE_CANNOT_COMPLETE_UPDATE = $"Cannot complete update of the main application. Please restart application.";


        internal static string ExecutionDirectory { get; private set; }
        internal static string ApplicationExe { get; private set; }
        internal static string DeploymentBaseDirectory { get; private set; }
        internal static bool AllowOffline { get; private set; } = false; //todo in settings

        internal static Tracer Tracer = new Tracer();


        public static void LoadConfiguration()
        {
            Tracer.Trace("LoadConfiguration started.");
                       
            ExecutionDirectory = Helpers.GetExecutionDirectory();

            XElement xdoc = XElement.Load(ExecutionDirectory + UPGRADER_CONFIGURATION_FILE);
            var els = xdoc.Descendants("setting");

            ApplicationExe = els.SingleOrDefault(e => e.Attribute("name").Value == "ApplicationExe")?.Value;
            DeploymentBaseDirectory = els.SingleOrDefault(e => e.Attribute("name").Value == "DeploymentBaseDirectory")?.Value;

            if (string.IsNullOrWhiteSpace(ApplicationExe))
            {
                throw new UpgradeException("Please specify 'ApplicationExe' setting in the configuration file.");
            }

            if (string.IsNullOrWhiteSpace(DeploymentBaseDirectory))
            {
                throw new UpgradeException("Please specify 'DeploymentBaseDirectory' setting in the configuration file.");
            }

            if (Directory.Exists(DeploymentBaseDirectory) == false)
            {
                if (AllowOffline == false)
                {
                    throw new UpgradeException("Application deployment directory is unavailable.");
                }
            }

            Tracer.Trace("LoadConfiguration finished.");
        }
    }
}