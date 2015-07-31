using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader;

namespace UpgraderTests
{
    /// <summary>
    /// Summary description for U_Controller_MainApp
    /// </summary>
    [TestClass]
    public class U_Controller_MainApp
    {
        private string[] UPDATER_ARGS = new string[] { Upgrader.Constants.LAUNCHED_FROM_UPDATER };
    
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Application_Controller_HappyPath()
        {
            /* happy path, both Updater and App do not require update. Nothing happens, App restart is not required.*/
            FakeOperations operations = new FakeOperations(false, false);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(null);
            operations.AssertCodingSequence("");
            Assert.AreEqual(false, restart);
        }

        [TestMethod]
        public void Application_Controller_HappyPath_ThroughUpdater()
        {
            /* happy path, both Updater and App do not require update. Nothing happens, App restart is not required.*/
            FakeOperations operations = new FakeOperations(false, false);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(UPDATER_ARGS);
            operations.AssertCodingSequence("");
            Assert.AreEqual(false, restart);
        }

        [TestMethod]
        public void Application_Controller_Updater()
        {
            /* updater requires update, since application was started first, we update updater, launch it and quit. */
            FakeOperations operations = new FakeOperations(false, true);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(null);
            operations.AssertCodingSequence(FakeOperations.UPDATE_UPDATER, FakeOperations.LAUNCH_UPDATER);
            Assert.AreEqual(true, restart);
        }

        [TestMethod]
        [ExpectedException(typeof(UpgradeException))]
        public void Application_Controller_Updater_ThroughUpdater()
        {
            /* updater requires update, since application was started through UPADTER, - exception happened. User should have started main application.*/
            FakeOperations operations = new FakeOperations(false, true);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(UPDATER_ARGS);
            operations.AssertCodingSequence("");
            Assert.AreEqual(false, restart);
        }

        [TestMethod]
        public void Application_Controller_Both()
        {
            /* Both updater and application require update, since application was started first, we update updater, launch it to update application and quit. */
            FakeOperations operations = new FakeOperations(true, true);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(null);
            operations.AssertCodingSequence(FakeOperations.UPDATE_UPDATER, FakeOperations.LAUNCH_UPDATER);
            Assert.AreEqual(true, restart);
        }

        [TestMethod]
        [ExpectedException(typeof(UpgradeException))]
        public void Application_Controller_Both_ThroughUpdater()
        {
            /* Both updater and application require update, since application was started through UPADTER, - exception happened. User should have started main application.*/
            FakeOperations operations = new FakeOperations(true, true);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(UPDATER_ARGS);
            operations.AssertCodingSequence("");
            Assert.AreEqual(false, restart);
        }

        [TestMethod]
        public void Application_Controller_App()
        {
            /* Application requires update, since application was started first we launch updater to update application and quit. */
            FakeOperations operations = new FakeOperations(true, false);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(null);
            operations.AssertCodingSequence( FakeOperations.LAUNCH_UPDATER);
            Assert.AreEqual(true, restart);
        }

        [TestMethod]
        [ExpectedException(typeof(UpgradeException))]
        public void Application_Controller_App_ThroughUpdater()
        {
            /* Application requires update, since application was started through UPADTER, - exception happened. User should have started main application.*/
            FakeOperations operations = new FakeOperations(true, false);
            Upgrader.Program._operations = operations;
            bool restart = Upgrader.Program.IsApplicationRestart_Inner(UPDATER_ARGS);
            operations.AssertCodingSequence("");
            Assert.AreEqual(false, restart);
        }
    }
}
