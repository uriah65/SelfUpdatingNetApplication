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
            files = GetOlderFiles(files);
            return files.Any();
        }

        public bool IsUpgraderUpdateRequired()
        {
            List<string> files = GetUpgraderFiles();
            files = GetOlderFiles(files);
            return files.Any();
        }

        public void UpdateApplication(bool exceptUpgrader)
        {
            List<string> files = GetApplicationFiles(exceptUpgrader);
            files = GetOlderFiles(files);
            CopyFilesWithCheck(files);
        }

        public void UpgradeUpdater()
        {
            List<string> files = GetUpgraderFiles();
            files = GetOlderFiles(files);
            CopyFilesWithCheck(files);
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
#if SMALL_CASE
            files = files.Select(e => e.ToLowerInvariant()).ToList();
#endif
            return files;
        }

        public List<string> GetApplicationFiles(bool exceptUpgrader)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Constants.InstallationDirectory);
            FileInfo[] fileInfos = dirInfo.GetFiles();
#if SMALL_CASE
            List<string> files = fileInfos.Select(e => e.Name.ToLowerInvariant()).ToList();
#else
            List<string> files = fileInfos.Select(e => e.Name).ToList();
#endif

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

        private void CopyFilesWithCheck(List<string> files)
        {
            /* 6 attempts allow to wait for 1+2++ ..+ 6 = 21 seconds for all attempts. */
            int attempts = 6;
            int delay = 1000; 

            bool hasLocked = false;
            List<string> lockedFiles = null;

            for (int i = 0; i < attempts; i++)
            {
                lockedFiles = GetLocked(Constants.WorkingDirectory, files);
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

            CopyNewerFiles(Constants.InstallationDirectory, Constants.WorkingDirectory, files);
        }

        private List<string> GetOlderFiles(List<string> files)
        {
            bool overWrite = false;
            string baseDirectory = Constants.InstallationDirectory;
            string targetDirectory = Constants.WorkingDirectory;

            List<string> results = new List<string>();

            foreach (string file in files)
            {
                string basePath = baseDirectory + file;
                string targetPath = targetDirectory + file;
                FileInfo baseInfo = new FileInfo(basePath);
                if (baseInfo.Exists == false)
                {
                    throw new UpgradeException("Base file '" + basePath + "' was not found.", null);
                }

                FileInfo targetInfo = new FileInfo(targetPath);
                if (targetInfo.Exists == false)
                {
                    /* file is missing in target */
                    results.Add(file);
                }
                else
                {
                    if (overWrite || baseInfo.LastWriteTime > targetInfo.LastWriteTime)
                    {
                        /* base contains newer file */
                        results.Add(file);
                    }
                }
            }

            return results;
        }

        private List<string> GetLocked(string path, List<string> files)
        {
            List<string> locked = new List<string>();
#if SMALL_CASE
            List<string> executables = files.Where(e => e.ToLower().EndsWith(".exe")).ToList();
#else
            List<string> executables = files.Where(e => e.EndsWith(".exe")).ToList();
#endif
            foreach (string file in executables)

            {
                //string exePath = path + @"\" + file; //todo: check
                string exePath = path + file;
                FileInfo fileInfo = new FileInfo(exePath);
                if (fileInfo.Exists == false)
                {
                    /* we might be looking for locked files, but they not yet has been copied to the target directory.
                       such 'non-existing' files considered not to be locked. */
                    continue;
                }

                bool isLocked = false;
                try
                {
                    /* read only can cause an exception if read-only file is locked at the same time */
                    FileOperations.CheckAndRemoveReadOnly(fileInfo);

                    isLocked = FileOperations.IsFileLocked(exePath);
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

            return locked;
        }

        private void CopyNewerFiles(string baseDirectory, string targetDirectory, List<string> files)
        {
            foreach (string file in files)
            {
                string basePath = baseDirectory + file;
                string targetPath = targetDirectory + file;

                FileInfo targetInfo = new FileInfo(targetPath);
                if (targetInfo.Exists)
                {
                    FileOperations.CheckAndRemoveReadOnly(targetInfo);
                }

                Constants.Tracer.Trace($"Copying '{basePath}' to '{targetPath}' ");

                File.Copy(basePath, targetPath, true);

                /* remove read only, if source was read only */
                targetInfo = new FileInfo(targetPath);
                FileOperations.CheckAndRemoveReadOnly(targetInfo);
            }
        }

        #endregion Operation Actions
    }
}