﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Upgrader
{
    public static class FileOperations
    {
        #region Arguments

        //todo: remove from here...
        public static bool HasArg(string[] args, string pattern)
        {
            if (args == null)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (arg == pattern)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Arguments

        #region File Helpers 

        public static string GetExecutionDirectory()
        {
            string directory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            directory = Path.GetDirectoryName(directory).EnsureSlash();
            return directory;
        }

        public static string EnsureSlash(this string path)
        {
            if (path.EndsWith(@"\") == false)
            {
                path += @"\";
            }
            return path;
        }

        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }

        public static void CheckAndRemoveReadOnly(FileInfo fileInfo)
        {
            if (IsFileReadonly(fileInfo))
            {
                SetFileNotReadonly(fileInfo);
            }
        }

        public static bool IsFileReadonly(FileInfo fileInfo)
        {
            return fileInfo.IsReadOnly;
        }

        public static void SetFileReadonly(FileInfo fileInfo)
        {
            fileInfo.IsReadOnly = true;
        }

        public static void SetFileNotReadonly(FileInfo fileInfo)
        {
            fileInfo.IsReadOnly = false;
        }


        public static Process ExecuteCommand(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command);
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = true;
            Process process = Process.Start(startInfo);
            return process;
        }

        #endregion File Helpers
    }
}
