#region

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.UT
{
    public class FileSystemUtilTests
    {
        [Test]
        public void DeleteDirectoryTree()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            //Dont delete root folder
            CreateDirectoryTree(path, 4);

            FileSystemUtil.DeleteDirectoryTree(path, false);
            Directory.Exists(path).ShouldBeTrue();
            Directory.GetFiles(path).ShouldBeEmpty();
            Directory.GetDirectories(path).ShouldBeEmpty();

            //delete root dir
            CreateDirectoryTree(path, 4);
            FileSystemUtil.DeleteDirectoryTree(path, true);
            Directory.Exists(path).ShouldBeFalse();
        }

        private void CreateDirectoryTree(string path, int degree)
        {
            for (var i = 0; i < degree; i++)
            {
                var subDir = Path.Combine(path, i.ToString());
                Directory.CreateDirectory(subDir);
                for (var j = 0; j < degree/2; j++)
                {
                    var file = Path.Combine(subDir, j + ".txt");
                    using (var s = File.Create(file))
                    {
                        s.Close();
                    }
                }
                CreateDirectoryTree(subDir, degree-1);
            }
        }

        [Test]
        public void DeleteDirectory_DeletesLocalDirCopy()
        {
            var path = CreateLocalDirectory();
            Thread.Sleep(1000);

            FileSystemUtil.DeleteDirectoryContent(path);

            Thread.Sleep(1000);
            var fse = Directory.GetFileSystemEntries(path);
            fse.Any().ShouldBeFalse();

            Directory.Delete(path);
        }

        [Test]
        public void DeleteAllDirectoryFiles_DeletesAllFiles()
        {
            var path = CreateDirectoryWithFiles();

            FileSystemUtil.DeleteAllDirectoryFiles(path, "txt, json");

            Assert.True(Directory.GetFileSystemEntries(path).Count() == 3);

            Assert.True(File.Exists(Path.Combine(path, "1.txt")));
            Assert.True(File.Exists(Path.Combine(path, "2.txt")));
            Assert.True(File.Exists(Path.Combine(path, "6.json")));

            Directory.Delete(path, true);
        }

        [Test]
        public void DeleteAllDirectoryFiles_ExcludesFilesFromDeletion()
        {
            var path = CreateDirectoryWithFiles();

            FileSystemUtil.DeleteAllDirectoryFiles(path);

            Assert.True(!Directory.GetFileSystemEntries(path).Any());
            Directory.Delete(path);
        }

        private string CreateDirectoryWithFiles()
        {
            var root = GetSubDirectory();
            Directory.CreateDirectory(root);

            var files = new[]
            {
                Path.Combine(root, "1.txt"),
                Path.Combine(root, "2.txt"),
                Path.Combine(root, "3.html"),
                Path.Combine(root, "4.html"),
                Path.Combine(root, "5.html"),
                Path.Combine(root, "6.json")
            };

            foreach (var file in files)
                using (var t = File.Create(file))
                {
                }

            return root;
        }

        private static string GetSubDirectory()
        {
            return @"C:\Automation\UnitTesting\" + DateTime.UtcNow.ToTimeStamp();
        }

        private string CreateLocalDirectory()
        {
            var root = GetSubDirectory();

            var subDirs = new[]
            {
                Path.Combine(root, "1"),
                Path.Combine(root, "2"),
                Path.Combine(root, "3")
            };
            foreach (var dir in subDirs)
                Directory.CreateDirectory(dir);

            var filesDir = subDirs[1];
            var files = new[]
            {
                Path.Combine(filesDir, "1.txt"),
                Path.Combine(filesDir, "2.xml"),
                Path.Combine(filesDir, "3.jpg"),
                Path.Combine(filesDir, "4.html"),
                Path.Combine(root, "1.txt"),
                Path.Combine(root, "2.html"),
                Path.Combine(root, "3.json")
            };

            foreach (var file in files)
                using (var t = File.Create(file))
                {
                }

            return root;
        }

        [Test]
        public void DeleteFile_Deletes()
        {
            var file = Path.GetTempFileName();

            FileSystemUtil.DeleteFile(file);
            File.Exists(file).ShouldBeFalse();
        }

        [Test]
        public void DeleteFile_Returns()
        {
            var file = Path.GetTempFileName();

            FileSystemUtil.DeleteFile(file + "SSS");
            File.Exists(file).ShouldBeTrue();

            FileSystemUtil.DeleteFile(null);
            File.Exists(file).ShouldBeTrue();

            File.Delete(file);
        }


        [Test]
        public void DeleteDirectory_Deletes()
        {
            var path = CreateTempFolderPath();
            FileSystemUtil.DeleteFile(path);
            Directory.Exists(path).ShouldBeFalse();
        }

        [Test]
        public void DeleteDirectory_Returns()
        {
            var path = CreateTempFolderPath();
            Directory.CreateDirectory(path);

            FileSystemUtil.DeleteDirectory(path + "CCC");
            Directory.Exists(path).ShouldBeTrue();

            FileSystemUtil.DeleteDirectory(null);
            Directory.Exists(path).ShouldBeTrue();

            Directory.Delete(path);
        }

        private static string CreateTempFolderPath()
        {
            return Path.GetTempPath() + DateTime.Now.ToTimeStamp();
        }


        [Test]
        public void RelativeToAbsolute_NonBackslashedPath()
        {
            const string relativePath = "Plugins";
            var result = FileSystemUtil.RelativePathToAbsolutePath(relativePath);

            var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            expectedPath.ShouldEqual(result);
        }

        [Test]
        public void RelativeToAbsolute_PathWithDot()
        {
            var relativePath = ".";
            var result = FileSystemUtil.RelativePathToAbsolutePath(relativePath);

            var expectedPath = AppDomain.CurrentDomain.BaseDirectory;
            expectedPath.ShouldEqual(result);

            
            relativePath = @"Relative\.\Path";

            result = FileSystemUtil.RelativePathToAbsolutePath(relativePath);
            expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Relative\Path");
            expectedPath.ShouldEqual(result);
        }

        [Test]
        public void CreateDirectoryIfNotExists()
        {
            var path = CreateTempFolderPath();

            FileSystemUtil.CreateDirectoryIfNotExists(path);
            Directory.Exists(path).ShouldBeTrue();

            FileSystemUtil.CreateDirectoryIfNotExists(path);
            Directory.Exists(path).ShouldBeTrue();

            Directory.Delete(path);
        }
    }
}