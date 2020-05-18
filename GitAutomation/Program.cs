using System;
using System.IO;
using System.Collections.Generic;
 
namespace GitAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = @"/Volumes/Seagate Backup Plus Drive/Work/rexesources";
            
            List<CommitStruct> commitsList = new List<CommitStruct>();
            DirectoryFileAnalyzer.CreateCommitInDirectory(commitsList, directory);
            foreach(var commit in commitsList)
            {
                Console.WriteLine($"Type: {commit.commitType.ToString()}\t, commit message: {commit.commitMesage}");
            }

            return;
        }
    }
}
