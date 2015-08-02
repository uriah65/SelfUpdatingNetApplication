using System;
using Upgrader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpgraderTests;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace UpgraderUnitTests
{
    [TestClass]
    public class T20_FileOperations
    {
        private Pawns _dummies;

        [TestInitialize()]
        public void Initialize()
        {
            // filling dummy files collection
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
        public void File_Locked_Unlocked()
        {
            _dummies.MoveToTarget("fn");
            string filePath = "";

            /* non existing file is not locked */
            filePath = ConstantsUT.TestTargetPath + @"\" + "nonexistinf.file";
            Assert.AreEqual(false, FileOperations.IsFileLocked(filePath));

            /* non running file is not locked */
            filePath = ConstantsUT.TestTargetPath + @"\" + "f.exe";
            FileOperations.SetFileNotReadonly(new FileInfo(filePath));
            Assert.AreEqual(false, FileOperations.IsFileLocked(filePath));

            /* running file is locked */
            Process process = FileOperations.ExecuteCommand(filePath, "");
            Assert.AreEqual(true, FileOperations.IsFileLocked(filePath));

            /* closing running process */
            Thread.Sleep(1000);
            process.CloseMainWindow();
            process.Close();

            /* non running file is not locked */
            Thread.Sleep(1000);
            Assert.AreEqual(false, FileOperations.IsFileLocked(filePath));
        }


        [TestMethod]
        public void ReadOnly_Set_Remove()
        {
            /* prepare a file */
            _dummies.MoveToSource("ao");
            string filePath = ConstantsUT.TestSourcePath + @"\a.txt";

            /* set RON and verify */
            FileOperations.SetFileReadonly(new FileInfo(filePath));
            Assert.AreEqual(true, FileOperations.IsFileReadonly(new FileInfo(filePath)));

            /* remove RON and verify */
            FileOperations.SetFileNotReadonly(new FileInfo(filePath));
            Assert.AreEqual(false, FileOperations.IsFileReadonly(new FileInfo(filePath)));
        }
    }
}
