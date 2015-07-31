﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Upgrader
{
    public class Operations : IUpdateOperations
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
            Process process = new Process();
            process.StartInfo.WorkingDirectory = Constants.ExecutionDirectory;
            process.StartInfo.FileName = Constants.ExecutionDirectory + Constants.ApplicationExe;
            process.StartInfo.Arguments = Constants.LAUNCHED_FROM_UPDATER; //todo: rest of args
            process.Start();
        }

        public void LaunchUpgrader(string[] args = null)
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = Constants.ExecutionDirectory;
            process.StartInfo.FileName = Constants.ExecutionDirectory + Constants.UPGRADER_EXE_FILE;
            process.StartInfo.Arguments = Constants.LAUNCHED_FROM_APP; //todo: rest of args
            process.Start();
        }

        #endregion IUpdateOperations

        #region Operation Actions

        public List<string> GetUpgraderFiles()
        {
            List<string> files = new List<string>() { Constants.UPGRADER_EXE_FILE, Constants.UPGRADER_CONFIGURATION_FILE };
            files = files.Select(e => e.ToLowerInvariant()).ToList();
            return files;
        }

        public List<string> GetApplicationFiles(bool exceptUpgrader)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Constants.DeploymentBaseDirectory);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            List<string> files = fileInfos.Select(e => e.Name.ToLowerInvariant()).ToList();

            if (files == null)
            {
                throw new UpgradeException("Base directory is empty.");
            }

            if (exceptUpgrader)
            {
                files = files.Except(GetUpgraderFiles()).ToList();
            }

            return files;
        }

        private void CopyFilesWithCheck(List<string> files)
        {
            List<string> locked = GetLocked(Constants.ExecutionDirectory, files);
            if (locked != null && locked.Count > 0)
            {
                string message = string.Format("Another version of the application is running.{0}{0}Locked files are: ", Environment.NewLine);
                for (int i = 0; i < Math.Min(3, locked.Count); i++)
                {
                    message += locked[i] + ", ";
                }

                throw new UpgradeException(message);
            }

            CopyNewerFiles(Constants.DeploymentBaseDirectory, Constants.ExecutionDirectory, files);
        }

        private List<string> GetOlderFiles(List<string> files)
        {
            bool overWrite = false;
            string baseDirectory = Constants.DeploymentBaseDirectory;
            string targetDirectory = Constants.ExecutionDirectory;

            List<string> results = new List<string>();

            foreach (string file in files)
            {
                string basePath = baseDirectory + @"\" + file;
                string targetPath = targetDirectory + @"\" + file;
                FileInfo baseInfo = new FileInfo(basePath);
                if (baseInfo.Exists == false)
                {
                    throw new UpgradeException("Base file '" + basePath + "' was not found.");
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
            List<string> executables = files.Where(e => e.ToLower().EndsWith(".exe")).ToList();
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
                    Helpers.CheckAndRemoveReadOnly(fileInfo);

                    isLocked = Helpers.IsFileLocked(exePath);
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
                string basePath = baseDirectory + @"\" + file;
                string targetPath = targetDirectory + @"\" + file;

                FileInfo targetInfo = new FileInfo(targetPath);
                if (targetInfo.Exists)
                {
                    Helpers.CheckAndRemoveReadOnly(targetInfo);
                }

                File.Copy(basePath, targetPath, true);

                /* remove read only, if source was read only */
                targetInfo = new FileInfo(targetPath);
                Helpers.CheckAndRemoveReadOnly(targetInfo);
            }
        }

        #endregion Operation Actions

    }
}