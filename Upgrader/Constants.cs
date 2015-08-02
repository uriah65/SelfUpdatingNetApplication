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

        //public static read-only string MESSAGE_START_MAINAPP = $"{UPGRADER_EXE_FILE} is out of date, please start main application.";
        public static readonly string MESSAGE_CANNOT_COMPLETE_UPDATE = $"Cannot complete update of the main application. Please restart application.";

        internal static string WorkingDirectory { get; private set; }
        internal static string ApplicationExe { get; private set; }
        internal static string InstallationDirectory { get; private set; }
        internal static bool AllowOffline { get; private set; }

        internal static Tracer Tracer = new Tracer();

        public static void LoadConfiguration()
        {
            Tracer.Trace("LoadConfiguration started.");

            WorkingDirectory = FileOperations.GetExecutionDirectory();

            /* reading upgrader configuration file */
            XElement xdoc = XElement.Load(WorkingDirectory + UPGRADER_CONFIGURATION_FILE);
            var els = xdoc.Descendants("setting");

            /* getting values */
            ApplicationExe = els.SingleOrDefault(e => e.Attribute("name").Value == "ApplicationExe")?.Value;
            InstallationDirectory = els.SingleOrDefault(e => e.Attribute("name").Value == "InstallationDirectory")?.Value;
            var allowOfflineStr = els.SingleOrDefault(e => e.Attribute("name").Value == "AllowOffLine")?.Value;
            AllowOffline = string.Compare(allowOfflineStr, "true", true) == 0; 

            /* verify values */
            if (string.IsNullOrWhiteSpace(ApplicationExe))
            {
                throw new UpgradeException("Please specify 'ApplicationExe' parameter in the Upgrader.exe.config file.", null);
            }

            if (string.IsNullOrWhiteSpace(InstallationDirectory))
            {
                throw new UpgradeException("Please specify 'InstallationDirectory' setting in the Upgrader.exe.config file.", null);
            }

            if (Directory.Exists(InstallationDirectory) == false)
            {
                if (AllowOffline == false)
                {
                    throw new UpgradeException("InstallationDirectory directory isn't available.", null);
                }
            }

            /* coerce */
            InstallationDirectory = InstallationDirectory.EnsureSlash();

            Tracer.Trace("LoadConfiguration finished.");
        }
    }
}