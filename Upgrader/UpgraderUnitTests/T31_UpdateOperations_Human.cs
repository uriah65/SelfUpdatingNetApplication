//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using UpgraderTests;
//using System.Collections.Generic;

//namespace UpgraderUnitTests
//{
//    [TestClass]
//    public class T31_UpdateOperations_Human
//    {
//        private Pawns _dummies;
//        private UpdateOperations _operatiions;

//        [TestInitialize()]
//        public void Initialize()
//        {
//            _dummies = new Pawns();
//            _dummies.ReadDirectory(ConstantsUT.SourceOldPath, false);
//            _dummies.ReadDirectory(ConstantsUT.SourceNewPath, true);

//            // empty both test folders
//            _dummies.EmptyDirectory(ConstantsUT.TestSourcePath);
//            _dummies.EmptyDirectory(ConstantsUT.TestTargetPath);

//            //_action = new DeployAction(ConstantsUT.TestSourcePath, ConstantsUT.TestTargetPath, null, null, false);
//        }

//        [TestCleanup()]
//        public void Cleanup()
//        {
//        }

//        [TestMethod]
//        public void PostExecute_Arguments_PassThrough()
//        {
//            //return;
//            //_dummies.MoveToSource("fn");

//            //_action = new DeployAction(ConstantsUT.TestSourcePath, ConstantsUT.TestTargetPath, null, new List<string>() { "f.exe", "a1", "a2", "a3" }, false);
//            //_action.Act();

//            ///* we expect to see f.exe window with arguments a1, a2 and a3 */
//            //_action.ExecuteAfter();
//        }
//    }
//}
