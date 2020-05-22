using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitAutomation
{
    public class DirectoryFileAnalyzer
    {
        private const float SingleFileSizeLimit = 100f;  // MB
        private const float DirectorySizeLimit = 2f; // GB
        public DirectoryFileAnalyzer()
        {
        }

        // Get directory size: gigabytes
        private static float GetDirectorySize(string fileName)
        {
            float size = 0;

            if(Directory.Exists(fileName))
            {
                var dirInfo = new DirectoryInfo(fileName);
                // the hidden files are excluded and files in hidden directory
                var fiInfos = dirInfo.GetFiles("*", SearchOption.AllDirectories).Where(fi => !fi.Attributes.HasFlag(FileAttributes.Hidden) && !fi.Directory.Attributes.HasFlag(FileAttributes.Hidden));

                foreach (FileInfo fiInfo in fiInfos)
                {
                    size += fiInfo.Length;
                }

                size = size / (1024f * 1024f * 1024f);
            }

            return size;
        }

        // Get file size in Megabytes
        private static float GetFileSize(string fileName)
        {
            float size = 0;

            if (File.Exists(fileName))
            {
                var fileInfo = new FileInfo(fileName);
                size = fileInfo.Length / (1024 * 1024f);
            }

            return size;
        }

        // The hidden files are excluded and the files in hidden folders are excluded as well
        private static List<FileInfo> GetFilesInDirSubdir(string directoryName)
        {
            if(Directory.Exists(directoryName))
            {
                var dirInfo = new DirectoryInfo(directoryName);
                return dirInfo.GetFiles("*", SearchOption.AllDirectories).Where(f=>!f.Attributes.HasFlag(FileAttributes.Hidden) && !f.Directory.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }

            return new List<FileInfo>();
        }

        // Hidden files are not included
        private static List<FileInfo> GetFilesInDirectory(string directory)
        {
            if(Directory.Exists(directory))
            {
                var dirInfo = new DirectoryInfo(directory);
                return dirInfo.GetFiles().Where(f=>!f.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }

            return new List<FileInfo>();
        }

        // Hidden directories are not included
        private static List<DirectoryInfo> GetSubdirectory(string directory)
        {
            if(Directory.Exists(directory))
            {
                var dirInfo = new DirectoryInfo(directory);
                return dirInfo.GetDirectories().Where(d=>!d.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }

            return new List<DirectoryInfo>();
        }

        private static List<DirectoryInfo> GetHiddenSubdirectories(string dir)
        {
            if(Directory.Exists(dir))
            {
                var dirInfo = new DirectoryInfo(dir);
                return dirInfo.GetDirectories("*", SearchOption.AllDirectories).Where(d => d.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }

            return new List<DirectoryInfo>();
        }

        private static List<FileInfo> GetHiddenFilesInDirectory(string dir)
        {
            if(Directory.Exists(dir))
            {
                var dirInfo = new DirectoryInfo(dir);
                return dirInfo.GetFiles("*", SearchOption.AllDirectories).Where(f => f.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }

            return new List<FileInfo>();
        }

        private static List<FileInfo> GetOversizedFilesInDirectory(string dir)
        {
            List<FileInfo> oversizedFileList = new List<FileInfo>();
            if(Directory.Exists(dir))
            {
                var dirInfo = new DirectoryInfo(dir);
                var flist = dirInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach(var f in flist)
                {
                    if (GetFileSize(f.FullName) > SingleFileSizeLimit)
                        oversizedFileList.Add(f);
                }
            }

            return oversizedFileList;
        }

        public static void CreateCommitInDirectory(in List<CommitStruct> commitsList, string directory)
        {
            float size = GetDirectorySize(directory);
            // Directory size less than 2 GB 
            if(size < DirectorySizeLimit)
            {
                // Search hidden directory
                var hiddenDirs = GetHiddenSubdirectories(directory);
                // Search hidden files
                var hiddenFiles = GetHiddenFilesInDirectory(directory);
                // Search the oversized files
                var oversizedFiles = GetOversizedFilesInDirectory(directory);

                DirCommitStruct dirCommit = new DirCommitStruct(directory, oversizedFiles, hiddenDirs, hiddenFiles);
                CommitStruct commit = new CommitStruct(directory, dirCommit, new List<FileInfo>(), CommitEnumeration.DIRCOMMIT);

                commitsList.Add(commit);

                return;
            }

            // The current directory size if over 2GB
            // Handle subdirectories in the current directory
            var subdirInfos = GetSubdirectory(directory);
            foreach (var dirInfo in subdirInfos)
            {
                CreateCommitInDirectory(commitsList, dirInfo.FullName);
            }

            // Handle the files in the current directory
            // Get the unhidden files in the current directory
            var files = GetFilesInDirectory(directory);

            // Only one file in the directory
            if(files.Count == 1)
            {
                DirCommitStruct dirCommit = new DirCommitStruct(null, null, null, null);
                string commitMessage = $"{directory}: {files.First().Name}";
                CommitStruct commit = new CommitStruct(commitMessage, dirCommit, new List<FileInfo> { files.First()}, CommitEnumeration.SINGLEFILECOMMIT);
                commitsList.Add(commit);
                return;
            }

            float fileBundleSize = 0;
            List<FileInfo> fileBundleList = new List<FileInfo>();

            int count = 0;
            foreach(var f in files)
            {
                var sz = GetFileSize(f.FullName);
                // The file bundle size is over 2GB
                if((fileBundleSize + sz)/1024f > DirectorySizeLimit)
                {
                    DirCommitStruct dirCommit = new DirCommitStruct(null, null, null, null);

                    string commitMessage = $"{directory}: file bundle {++count}";
                    CommitStruct commit = new CommitStruct(commitMessage, dirCommit, new List<FileInfo>(fileBundleList), CommitEnumeration.FILEBUNDLECOMMIT);

                    commitsList.Add(commit);

                    fileBundleList.Clear();
                    fileBundleSize = 0;
                }

                if(sz < SingleFileSizeLimit)
                {
                    fileBundleSize += sz;
                    fileBundleList.Add(f);
                }
            }

            if(fileBundleList.Count > 0)
            {
                DirCommitStruct dirCommit = new DirCommitStruct(null, null, null, null);

                string commitMessage = $"{directory}: file bundle {++count}";
                CommitStruct commit = new CommitStruct(commitMessage, dirCommit, new List<FileInfo>(fileBundleList), CommitEnumeration.FILEBUNDLECOMMIT);

                commitsList.Add(commit);
            }            

            return;
        }
    }
}
