using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpgraderTests;

namespace UpgraderUnitTests
{
    //[TestClass]
    //public class T30_Main
    //{
    //    private Pawns _pwans;
    //    private DeployAction _action;

    //    [TestInitialize()]
    //    public void Initialize()
    //    {
    //        _pwans = new Pawns();
    //        _pwans.ReadDirectory(ConstantsUT.SourceOldPath, false);
    //        _pwans.ReadDirectory(ConstantsUT.SourceNewPath, true);

    //        // empty both test folders
    //        _pwans.EmptyDirectory(ConstantsUT.TestSourcePath);
    //        _pwans.EmptyDirectory(ConstantsUT.TestTargetPath);

    //        _action = new DeployAction(ConstantsUT.TestSourcePath, ConstantsUT.TestTargetPath, null, null, false);
    //    }

    //    [TestCleanup()]
    //    public void Cleanup()
    //    {
    //    }

    //    [TestMethod]
    //    public void Copy_Replacing_Old_File()
    //    {
    //        /* seed both source and target */
    //        _pwans.MoveToSource("an", "bn");
    //        _pwans.MoveToTarget("ao");

    //        _action.Act();

    //        /* both files has been replaced*/
    //        Assert.AreEqual(true, _pwans.TargetIs("an", "bn"));
    //        Assert.AreEqual(false, _pwans.TargetIs("ao", "bn"));
    //    }

    //    [TestMethod]
    //    public void Copy_Filling_Empty_Target()
    //    {
    //        /* seed source */
    //        _pwans.MoveToSource("an", "bn", "co", "dn", "eo");

    //        _action.Act();

    //        /* verify results */
    //        Assert.AreEqual(true, _pwans.TargetIs("an", "bn", "co", "dn", "eo"));
    //        Assert.AreEqual(false, _pwans.TargetIs("an", "bn", "cn", "dn", "eo"));
    //    }

    //    [TestMethod]
    //    public void Copy_Keeping_New_Files()
    //    {
    //        /* seed source and some newer filwes in target */
    //        _pwans.MoveToSource("ao", "bn", "cn", "dn", "eo");
    //        _pwans.MoveToTarget("an", "bn", "co", "dn", "en");

    //        _action.Act();

    //        /* no files has been changed on target */
    //        Assert.AreEqual(true, _pwans.TargetIs("an", "bn", "cn", "dn", "en"));
    //        Assert.AreEqual(false, _pwans.TargetIs("ao", "bn", "cn", "dn", "eo"));
    //    }

    //    [TestMethod]
    //    public void Copy_With_Manifest()
    //    {
    //        /* seed source */
    //        _pwans.MoveToSource("an", "bn", "cn", "dn", "en", "fn", "m1n");

    //        /* trigger update with a.txt as manifest */
    //        _action = new DeployAction(ConstantsUT.TestSourcePath, ConstantsUT.TestTargetPath, "m1.manifest", null, false);
    //        _action.Act();

    //        /* all file has been copied, but not cn which is absent in manifest */
    //        Assert.AreEqual(true, _pwans.TargetIs("an", "bn", "dn", "en", "fn"));
    //    }

    //    [TestMethod]
    //    public void Copy_With_Manifest_OverWrite()
    //    {
    //        /* seed source */
    //        _pwans.MoveToSource("ao", "bn", "co", "do", "en", "fn", "m2n");
    //        _pwans.MoveToTarget("an", "bn", "cn", "dn", "en", "fn");

    //        /* trigger update with a.txt as manifest */
    //        _action = new DeployAction(ConstantsUT.TestSourcePath, ConstantsUT.TestTargetPath, "m2.manifest", null, false);
    //        _action.Act();

    //        /* all file has been copied, but not cn which is absent in manifest */
    //        Assert.AreEqual(true, _pwans.TargetIs("ao", "bn", "cn", "do", "en", "fn"));
    //    }

    //    [TestMethod]
    //    public void Copy_NoCopyFiles()
    //    {
    //        /* seed source */
    //        _pwans.MoveToSource("DeployerLibn", "ao", "bn", "cn", "do", "eo", "fn", "m3n");
    //        _pwans.MoveToTarget("an", "bn", "co", "dn", "en", "fn");

    //        /* trigger update with a.txt as manifest */
    //        _action = new DeployAction(ConstantsUT.TestSourcePath, ConstantsUT.TestTargetPath, "m3.manifest", null, false);
    //        _action.Act();

    //        /* all file has been copied, but not cn which is absent in manifest */
    //        Assert.AreEqual(true, _pwans.TargetIs("ao", "bn", "co", "do", "eo", "fn"));
    //    }
    //}
}