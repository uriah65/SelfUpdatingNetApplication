using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpgraderUnitTests;

namespace UpgraderTests
{
    class Pawns
    {
        public List<Pawn> Files { get; set; }

        public string Path { get; set; }

        public Pawns()
        {
            Files = new List<Pawn>();
        }

        public void ReadDirectory(string path, bool isNew)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                Pawn dummy = new Pawn();
                Files.Add(dummy);

                dummy.Name = file.Name;
                dummy.Nick = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                dummy.Nick += isNew ? "n" : "o";
                dummy.Path = file.FullName;
                dummy.LastWrite = file.LastWriteTime;
            }
        }

        #region MoveTo

        public List<string> MoveToSource(params string[] nicks)
        {
            return MoveTo(ConstantsUT.TestSourcePath, nicks);
        }

        public List<string> MoveToTarget(params string[] nicks)
        {
            return MoveTo(ConstantsUT.TestTargetPath, nicks);
        }

        private List<string> MoveTo(string path, params string[] nicks)
        {
            List<string> files = new List<string>();
            foreach (string nick in nicks)
            {
                Pawn pawn = Find(nick);
                files.Add(pawn.Name);
                string targetPath = path + @"\" + pawn.Name;
                File.Copy(pawn.Path, targetPath);
                Upgrader.FileOperations.SetFileReadonly(new FileInfo(targetPath));
            }
            return files;
        }

        #endregion MoveTo

        #region Contains


        public bool TargetIs(params string[] nicks)
        {
            List<string> nicksList = nicks.ToList();

            DirectoryInfo info = new DirectoryInfo(ConstantsUT.TestTargetPath);
            FileInfo[] fileInfos = info.GetFiles();

            if (nicks.Count() != fileInfos.Length)
            {
                /* declared and actual file count match */
                return false;
            }

            foreach (FileInfo file in fileInfos)
            {
                Pawn dummy = Files.Single(e => e.Name == file.Name && e.LastWrite == file.LastWriteTime);
                string nick = dummy.Nick;
                if (nicksList.Contains(nick))
                {
                    /* each real file was declared */
                    nicksList.Remove(nick);
                }
                else
                {
                    return false;
                }
            }

            if (nicksList.Count > 0)
            {
                return false;
            }

            return true;
        }


        #endregion Contains

        public Pawn Find(string nick)
        {
            return Files.Single(e => e.Nick == nick);
        }



        public void EmptyDirectory(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                Upgrader.FileOperations.CheckAndRemoveReadOnly(fileInfo);
                fileInfo.Delete();
            }

            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

    }
}
