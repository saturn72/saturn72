#region

using System;
using System.IO;
using System.Linq;
using System.Web;

#endregion

namespace Saturn72.Extensions
{
    public class FileSystemUtil
    {
        /// <summary>
        ///     converts relative path to absolute
        /// </summary>
        /// <param name="relativePath">relative path</param>
        /// <returns></returns>
        public static string RelativePathToAbsolutePath(string relativePath)
        {
            if (relativePath == "." )
                relativePath = string.Empty;
            if (relativePath.Contains(@"\.\"))
                relativePath = relativePath.Replace(@"\.\", @"\");

            return HttpContext.Current.IsNull()
                ? FileSystemRelativePathToAbsolutePath(relativePath)
                : WebRelativePathToAbsolutePath(relativePath);
        }

        private static string WebRelativePathToAbsolutePath(string subFolder)
        {
            var rPath = subFolder.Replace("\\", "/");

            while (rPath.StartsWith("/") || rPath.StartsWith("~"))
                rPath = rPath.Replace(0, 1, "");
            rPath = "~/" + rPath;

            return HttpContext.Current.Server.MapPath(rPath);
        }

        private static string FileSystemRelativePathToAbsolutePath(string subFolder)
        {
            var rPath = subFolder.Replace("/", "\\").RemoveAllInstances("~");
            while (rPath.StartsWith("\\"))
                rPath = rPath.Replace(0, 1, "");

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rPath);
        }

        /// <summary>
        ///     Deletes directory context
        /// </summary>
        /// <param name="path">path to directory</param>
        public static void DeleteDirectoryContent(string path)
        {
            if (!Directory.Exists(path))
                return;
            foreach (var dir in Directory.GetDirectories(path))
                Directory.Delete(dir, true);

            DeleteAllDirectoryFiles(path);
        }

        /// <summary>
        ///     Deletes all directory files
        /// </summary>
        /// <param name="path">path to directory</param>
        /// <param name="ignoredFileExtensions">
        ///     files extensions to be ignored, seperated comma delimited
        ///     <example>FileSystemUtil.DeleteAllDirectoryFiles(@"C:\temp", "xml, json, txt")</example>
        /// </param>
        public static void DeleteAllDirectoryFiles(string path, string ignoredFileExtensions = null)
        {
            if (!Directory.Exists(path))
                return;
            var ignoredExtensions = ignoredFileExtensions.HasValue()
                ? ignoredFileExtensions.Split(',').Where(x => x.HasValue()).Select(x => x.Trim())
                : new string[] {};
            var files =
                Directory.GetFiles(path).Where(f => !ignoredExtensions.Contains(Path.GetExtension(f).Substring(1)));

            foreach (var file in files)
                File.Delete(file);
        }

        /// <summary>
        ///     Checks if directory exists
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="throwIfNotExists">Should exception be thrown id not exists</param>
        /// <returns>true if exists, else false</returns>
        public static bool DirectoryExists(string path, bool throwIfNotExists = false)
        {
            var result = Directory.Exists(path);
            if (!result && throwIfNotExists)
                throw new DirectoryNotFoundException(path);

            return result;
        }

        /// <summary>
        ///     Checks if directory exists
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="throwIfNotExists">Should exception be thrown id not exists</param>
        /// <returns>true if exists, else false</returns>
        public static bool FileExists(string path, bool throwIfNotExists = false)
        {
            var result = File.Exists(path);
            if (!result && throwIfNotExists)
                throw new FileNotFoundException(path);

            return result;
        }

        public static void DeleteDirectoryTree(string path, bool deleteRoot = true)
        {
            if (!DirectoryExists(path))
                return;
            foreach (var dir in Directory.GetDirectories(path))
                DeleteDirectoryTree(dir, true);

            foreach (var file in Directory.GetFiles(path))
                File.Delete(file);

            if (deleteRoot)
                Directory.Delete(path);
        }

        /// <summary>
        ///     Deletes file. If the path not exists or illegal - it returns
        /// </summary>
        /// <param name="path">File's path</param>
        public static void DeleteFile(string path)
        {
            if (path.HasValue() && File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Deletes directory. If path illegal or not exists - it returns
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDirectory(string path)
        {
            if (path.HasValue() && Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (Directory.Exists(path))
                return;

            Directory.CreateDirectory(path);
        }

    }
}