namespace Upgrader
{
    internal interface IUpdateOperations
    {
        bool IsUpgraderUpdateRequired();

        void UpgradeUpdater();

        bool IsApplicationUpdateRequired(bool exceptUpgrader);

        void UpdateApplication(bool exceptUpgrader);

        void LaunchApplication(string[] args = null);

        void LaunchUpgrader(string[] args);
    }
}