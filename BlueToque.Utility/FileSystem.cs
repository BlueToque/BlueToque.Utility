using System;
using System.IO;
using System.Linq;

namespace BlueToque.Utility
{
    public static class FileSystem
    {
        //
        // Summary:
        //     Get the size of a directory in bytes
        //
        // Parameters:
        //   directoryInfo:
        public static long GetSize(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
                return -1L;

            try
            {
                return directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Sum((FileInfo a) => a.Length);
            }
            catch (Exception ex)
            {
                Trace.TraceError("FileSystem.GetSize: Error calculating size of directory:\r\n{0}", ex);
                return 0L;
            }
        }

        //
        // Summary:
        //     Delete all the files in a given path, and optionally all folders as well
        //
        // Parameters:
        //   path:
        //
        //   deleteFolders:
        public static void DeleteFiles(string path, bool deleteFolders = false)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
                DeleteFile(file);

            if (deleteFolders)
            {
                string[] directories = Directory.GetDirectories(path);
                foreach (string diretory in directories)
                    DeleteDirectory(diretory);
            }
        }

        //
        // Parameters:
        //   file:
        public static bool DeleteFile(string file)
        {
            try
            {
                if (file.IsNullOrEmpty())
                    return false;

                if (!File.Exists(file))
                    return false;

                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //
        // Summary:
        //     Copy the source directory to the destination directory This may throw exceptions
        //     for file locks and other problems It will return true if the copy of all files
        //     in the source was successfull
        //
        // Parameters:
        //   source:
        //
        //   destination:
        //
        //   copySubDirectories:
        //
        //   overWrite:
        //
        //   copyIfNewer:
        //
        // Returns:
        //     True if all files copied successfully
        public static bool CopyDirectory(string source, string destination, bool copySubDirectories = true, bool overWrite = false, bool copyIfNewer = true)
        {
            if (source.IsNullOrEmpty() || destination.IsNullOrEmpty())
            {
                Trace.TraceWarning("FileSystem.CopyDirectory: source or destination is null");
                return false;
            }

            DirectoryInfo directoryInfo = new(source);
            if (!directoryInfo.Exists)
            {
                Trace.TraceError("Source directory does not exist or could not be found: " + source);
                return false;
            }

            Paths.EnsureDirectoryExists(destination);

            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                string text = Path.Combine(destination, fileInfo.Name);
                if (copyIfNewer)
                {
                    FileInfo fileInfo2 = new(text);
                    if (fileInfo2.Exists && fileInfo2.LastWriteTime >= fileInfo.LastWriteTime)
                        continue;
                }

                fileInfo.CopyTo(text, overWrite);
            }

            if (!copySubDirectories)
                return true;

            bool result = true;
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            foreach (DirectoryInfo directoryInfo2 in directories)
            {
                if (!CopyDirectory(directoryInfo2.FullName, Path.Combine(destination, directoryInfo2.Name), copySubDirectories, overWrite, copyIfNewer))
                    result = false;
            }

            return result;
        }

        //
        // Summary:
        //     Delete a directory
        //
        // Parameters:
        //   diretory:
        //
        //   recursive:
        public static bool DeleteDirectory(string diretory, bool recursive = true)
        {
            try
            {
                if (diretory.IsNullOrEmpty())
                    return false;

                if (!Directory.Exists(diretory))
                    return false;

                Directory.Delete(diretory, recursive);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("FileSystem.DeleteDirectory: Error copying files\r\n{0}", ex);
                return false;
            }
        }
    }

}
