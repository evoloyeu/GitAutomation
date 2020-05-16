using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitAutomation
{
    public class DirectoryFileAnalyzer
    {
        public DirectoryFileAnalyzer()
        {
        }

        // Get directory size: gigabytes
        public static float GetDirectorySize(string fileName)
        {
            float size = 0;

            if(Directory.Exists(fileName))
            {
                var dirInfo = new DirectoryInfo(fileName);
                foreach (FileInfo fiInfo in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    size += fiInfo.Length;
                }

                size = size / (1024f * 1024f * 1024f);
            }

            return size;
        }

        // Get file size in Megabytes
        public static float GetFileSize(string fileName)
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
        public static List<FileInfo> GetFilesInDirectory(string directoryName)
        {
            if(Directory.Exists(directoryName))
            {
                var dirInfo = new DirectoryInfo(directoryName);
                return dirInfo.GetFiles("*", SearchOption.AllDirectories).Where(f=>!f.Attributes.HasFlag(FileAttributes.Hidden) && !f.FullName.Contains(".git")).ToList();
            }

            return new List<FileInfo>();
        }
    }
}
