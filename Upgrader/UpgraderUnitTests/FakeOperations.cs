using Upgrader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpgraderTests
{
    class FakeOperations : IUpdateOperations
    {
        bool _appRequireUpdate;
        bool _updaterRequireUpdate;

        public FakeOperations(bool appRequireUpdate, bool updaterRequireUpdate)
        {
            _callingSequence = "";
            _appRequireUpdate = appRequireUpdate;
            _updaterRequireUpdate = updaterRequireUpdate;
        }

        #region Calling Sequence

        public const string LAUNCH_APP = "La";
        public const string LAUNCH_UPDATER = "Lu";
        public const string UPDATE_APP = "Ua";
        public const string UPDATE_UPDATER = "Uu";

        private string _callingSequence = "";

        public string ObtaineCodeSequence()
        {
            string temp = _callingSequence;
            _callingSequence = "";
            return temp;
        }


        public void AssertCodingSequence(params string[] patterns)
        {
            string expected = "";
            foreach (string pattern in patterns)
            {
                expected += pattern;
            }

            string actual = ObtaineCodeSequence();

            Assert.AreEqual<string>(actual, expected, "Calling sequence is unexpected");

        }

        #endregion Calling Sequence

        #region IUpdateOperations

        public bool IsApplicationUpdateRequired(bool exceptUpgrader)
        {
            return _appRequireUpdate;
        }

        public bool IsUpgraderUpdateRequired()
        {
            return _updaterRequireUpdate;
        }

        public void LaunchApplication(string[] args = null)
        {
            _callingSequence += LAUNCH_APP;
        }

        public void LaunchUpgrader(string[] args)
        {
            _callingSequence += LAUNCH_UPDATER;
        }

        public void UpdateApplication(bool exceptUpgrader)
        {
            _callingSequence += UPDATE_APP;
        }

        public void UpgradeUpdater()
        {
            _callingSequence += UPDATE_UPDATER;
        }

        #endregion IUpdateOperations
    }
}
