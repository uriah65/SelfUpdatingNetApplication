using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader;

namespace UpgraderTests
{
    [TestClass]
    public class U_Controller_Updater
    {
        [TestMethod]
        public void Controller_Updater_HappyPath()
        {
            /* happy path, both Updater and App do not require update. Application launched.*/
            FakeOperations operations = new FakeOperations(false, false);
            Upgrader.Program._operations = operations;
            Upgrader.Program.Main_Inner();
            operations.AssertCodingSequence(FakeOperations.LAUNCH_APP);           
        }

        [TestMethod]
        public void Controller_Updater_AppUpdate()
        {
            /* application requires update. it get updated and then launched*/
            FakeOperations operations = new FakeOperations(true, false);
            Upgrader.Program._operations = operations;
            Upgrader.Program.Main_Inner();
            operations.AssertCodingSequence(FakeOperations.UPDATE_APP, FakeOperations.LAUNCH_APP);
        }

        [TestMethod]
        [ExpectedException(typeof(UpgradeUpgraderException))]
        public void Controller_Updater_UpdaterUpdate()
        {
            /* updater requires update, exception generated. User should have started main application first. */
            FakeOperations operations = new FakeOperations(false, true);
            Upgrader.Program._operations = operations;
            Upgrader.Program.Main_Inner();
            operations.AssertCodingSequence("");
        }

        [TestMethod]
        [ExpectedException(typeof(UpgradeUpgraderException))]
        public void Controller_Updater_UpdateBoth()
        {
            /* updater requires update, exception generated. User should have started main application first. */
            FakeOperations operations = new FakeOperations(true, true);
            Upgrader.Program._operations = operations;
            Upgrader.Program.Main_Inner();
            operations.AssertCodingSequence(FakeOperations.UPDATE_APP, FakeOperations.LAUNCH_APP);
        }

    }
}
