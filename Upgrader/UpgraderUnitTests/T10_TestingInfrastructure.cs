using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpgraderTests;

namespace UpgraderUnitTests
{
    [TestClass]
    public class T10_TestingInfrastructure
    {
        private Pawns _dummies;

        [TestInitialize()]
        public void Initialize()
        {
            // filling dummy files collection
            // fill A
            // fill B
            // delete C
            _dummies = new Pawns();
            _dummies.ReadDirectory(ConstantsUT.SourceOldPath, false);
            _dummies.ReadDirectory(ConstantsUT.SourceNewPath, true);

            // empty both test folders
            _dummies.EmptyDirectory(ConstantsUT.TestSourcePath);
            _dummies.EmptyDirectory(ConstantsUT.TestTargetPath);
        }

        [TestCleanup()]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void TestMoveMethods()
        {

            _dummies.MoveToSource("ao", "bn");
            _dummies.MoveToTarget("an", "en");
            _dummies.TargetIs("an", "dn");

            // empty directory get filled

            // single file get overridden (with read only  flag)

            // new file stays

            // exception on the file that absent in source

            // exception on the file that is locked in destination
        }




    }
}
