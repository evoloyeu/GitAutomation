using System;
using System.IO;
 
namespace GitAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("Hello World!");
            var currentDirectory = Directory.GetCurrentDirectory();
            var dirInfo = new DirectoryInfo(@"/Volumes/Seagate Backup Plus Drive/Work/LiAoShow");
            long size = 0;
            foreach(FileInfo fiInfo in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                size += fiInfo.Length;
            }

            Console.WriteLine($"Directory Size: {size/(1024f*1024f*1024f)} GB");*/
            var directory = @"/Volumes/Seagate Backup Plus Drive/Work/LiAoShow/李敖有话说/视频";
            //Print directory size
            var size = DirectoryFileAnalyzer.GetDirectorySize(directory);
            Console.WriteLine($"{directory}: size: {size} GB");
            

            // Print File Size
            foreach(var fileInfo in DirectoryFileAnalyzer.GetFilesInDirectory(directory))
            {
                Console.WriteLine($"size:{DirectoryFileAnalyzer.GetFileSize(fileInfo.FullName)} MB : {fileInfo.Name}");
            }
        }
    }
}
