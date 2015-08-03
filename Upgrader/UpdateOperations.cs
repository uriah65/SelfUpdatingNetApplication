#define SMALL_CASE_OFF

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Upgrader
{
    public class UpdateOperations : IUpdateOperations
    {
        #region IUpdateOperations

        public bool IsApplicationUpdateRequired(bool exceptUpgrader)
        {
            List<string> files = GetApplicationFiles(exceptUpgrader);
            files = GetOlderOrMissing(files, Constants.InstallationDirectory, Constants.WorkingDirectory);

            return files.Any();
        }

        public bool IsUpgraderUpdateRequired()
        {
            List<string> files = GetUpgraderFiles();
            files = GetOlderOrMissing(files, Constants.InstallationDirectory, Constants.WorkingDirectory);
            return files.Any();
        }

        public void UpdateApplication(bool exceptUpgrader)
        {
            List<string> files = GetApplicationFiles(exceptUpgrader);
            CopyFilesWithCheck(files, Constants.InstallationDirectory, Constants.WorkingDirectory);
        }

        public void UpgradeUpdater()
        {
            List<string> files = GetUpgraderFiles();
            CopyFilesWithCheck(files, Constants.InstallationDirectory, Constants.WorkingDirectory);
        }

        public void LaunchApplication(string[] args)
        {
            //todo: pass args[]
            Launch(Constants.ApplicationExe, Constants.LAUNCHED_FROM_UPDATER);
        }

        public void LaunchUpgrader(string[] args = null)
        {
            //todo: pass args[]
            Launch(Constants.UPGRADER_EXE_FILE, Constants.LAUNCHED_FROM_APP);
        }

        private void Launch(string file, string arguments)
        {
            file = Constants.WorkingDirectory + file;
            if (File.Exists(file) == false)
            {
                throw new UpgradeException($"File {file} wasn't found.", null);
            }

            Process process = new Process();
            process.StartInfo.WorkingDirectory = Constants.WorkingDirectory;
            process.StartInfo.FileName = file;
            process.StartInfo.Arguments = Constants.LAUNCHED_FROM_APP; //todo: pass args[]
            process.Start();
        }

        #endregion IUpdateOperations

        #region Operation Actions

        public List<string> GetUpgraderFiles()
        {
            List<string> files = new List<string>() { Constants.UPGRADER_EXE_FILE, Constants.UPGRADER_CONFIGURATION_FILE };
            return files;
        }

        public List<string> GetApplicationFiles(bool exceptUpgrader)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Constants.InstallationDirectory);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            List<string> files = fileInfos.Select(e => e.Name).ToList();

            if (files == null)
            {
                throw new UpgradeException($"Application installation directory '{Constants.InstallationDirectory}' is empty.", null);
            }

            if (exceptUpgrader)
            {
                files = files.Except(GetUpgraderFiles(), StringComparer.OrdinalIgnoreCase).ToList();
            }

            return files;
        }

        internal void CopyFilesWithCheck(List<string> files, string from, string to)
        {
            from = from.EnsureSlash();
            to = to.EnsureSlash();

            /* filter out only old files */
            files = GetOlderOrMissing(files, from, to);
            if (files.Any() == false)
            {
                return;
            }

            /* make sure that files to be overwritten are not locked */
            AssetNotLocked(files, to);

            /* actually copy */
            ReplaceOlderFiles(files, from, to);
        }

        private void AssetNotLocked(List<string> files, string folder)
        {
            /* 6 attempts allow to wait for 1+2++ ..+ 6 = 21 seconds for all attempts. */
            int attempts = 6;
            int delay = 1000;

            bool hasLocked = false;
            List<string> lockedFiles = null;

            for (int i = 0; i < attempts; i++)
            {
                lockedFiles = GetLocked(files, folder);
                hasLocked = lockedFiles != null && lockedFiles.Count > 0;
                if (hasLocked == false)
                {
                    break;
                }
                Constants.Tracer.Trace($"Discovered {lockedFiles.Count()} locked files.");
                Thread.Sleep(delay++);
            }

            if (hasLocked)
            {
                string message = string.Format("The following files are in use: ", Environment.NewLine);
                for (int i = 0; i < Math.Min(3, lockedFiles.Count); i++)
                {
                    message += lockedFiles[i] + ", ";
                }

                throw new UpgradeException(message, null);
            }
        }

        private List<string> GetOlderOrMissing(List<string> files, string from, string to)
        {
            bool overWrite = false;

            List<string> results = new List<string>();

            foreach (string file in files)
            {
                string sourceFile = from + file;
                string targetFile = to + file;
                FileInfo sourceInfo = new FileInfo(sourceFile);
                if (sourceInfo.Exists == false)
                {
                    throw new UpgradeException("Source file '" + sourceFile + "' was not found.", null);
                }

                FileInfo targetInfo = new FileInfo(targetFile);
                if (targetInfo.Exists == false)
                {
                    /* file is missing in target */
                    results.Add(file);
                }
                else
                {
                    if (overWrite || sourceInfo.LastWriteTime > targetInfo.LastWriteTime)
                    {
                        /* base contains newer file */
                        results.Add(file);
                    }
                }
            }

            return results;
        }

        private List<string> GetLocked(List<string> files, string folder)
        {
            List<string> locked = new List<string>();

            foreach (string file in files)
            {
                string fullPath = folder + file;
                FileInfo fileInfo = new FileInfo(fullPath);
                if (fileInfo.Exists)
                {
                    bool isLocked = false;
                    try
                    {
                        /* we must remove read only flag, because file will cause an exception if read-only file as if it is locked  */
                        FileOperations.CheckAndRemoveReadOnly(fileInfo);
                        isLocked = FileOperations.IsFileLocked(fullPath);
                    }
                    catch
                    {
                        isLocked = true;
                    }

                    if (isLocked)
                    {
                        /* add file to locked collection */
                        locked.Add(file);
                    }
                }
            }

            return locked;
        }

        private void ReplaceOlderFiles(List<string> files, string from, string to)
        {
            foreach (string file in files)
            {
                string basePath = from + file;
                string targetPath = to + file;

                bool toReplace = true;
                FileInfo targetInfo = new FileInfo(targetPath);
                if (targetInfo.Exists)
                {
                    FileInfo sourceInfo = new FileInfo(basePath);
                    toReplace = sourceInfo.LastWriteTime > targetInfo.LastWriteTime;
                    if (toReplace)
                    {
                        FileOperations.CheckAndRemoveReadOnly(targetInfo);
                    }
                }

                if (toReplace)
                {
                    Constants.Tracer.Trace($"Copying '{basePath}' to '{targetPath}' ");

                    /* actual file copy */
                    File.Copy(basePath, targetPath, true);

                    /* get new Info and remove read only, if source was read only */
                    targetInfo = new FileInfo(targetPath);
                    FileOperations.CheckAndRemoveReadOnly(targetInfo);
                }
            }
        }

        #endregion Operation Actions
    }
}

//private List<string> GetLocked(string folderPath, List<string> files)
//{
//    List<string> locked = new List<string>();
//    //List<string> executables =  files.Where(e => e.EndsWith(".exe")).ToList();
//    List<string> executables =  files;

//    foreach (string file in executables)
//    {
//        string exePath = folderPath + file;
//        FileInfo fileInfo = new FileInfo(exePath);
//        if (fileInfo.Exists == false)
//        {
//            /* we might be looking for locked files, but they not yet has been copied to the target directory.
//               such 'non-existing' files considered not to be locked. */
//            continue;
//        }

//        bool isLocked = false;
//        try
//        {
//            /* read only can cause an exception if read-only file is locked at the same time */
//            FileOperations.CheckAndRemoveReadOnly(fileInfo);

//            isLocked = FileOperations.IsFileLocked(exePath);
//        }
//        catch
//        {
//            isLocked = true;
//        }

//        if (isLocked)
//        {
//            /* add file to locked collection */
//            locked.Add(file);
//        }
//    }

//    return locked;
//}