using System;
using System.Collections.Generic;
using System.IO;

namespace GitAutomation
{
    public enum CommitEnumeration
    {
        Invalid,
        DIRCOMMIT,  // The whole directory as one commit
        FILEBUNDLECOMMIT,   // More than one files as one commit
        SINGLEFILECOMMIT    // Signle file as as one commit
    }

    public struct DirCommitStruct
    {
        public string commitObject;
        public List<FileInfo> overSizedFileInfos;
        public List<DirectoryInfo> hiddenDirInfos;
        public List<FileInfo> hiddenFileInfos;

        public DirCommitStruct(string commitObject, List<FileInfo> overSizedFileInfos, List<DirectoryInfo> hiddenDirInfos, List<FileInfo> hiddenFileInfos)
        {
            this.commitObject = commitObject;
            this.overSizedFileInfos = overSizedFileInfos;
            this.hiddenDirInfos = hiddenDirInfos;
            this.hiddenFileInfos = hiddenFileInfos;
        }
    }

    public struct CommitStruct
    {
        public CommitEnumeration commitType;
        public string commitMesage;
        public DirCommitStruct dirCommit;
        public List<FileInfo> fileInfos;

        public CommitStruct(CommitEnumeration commitType, string commitMesage, DirCommitStruct dirCommit, List<FileInfo> fileInfos)
        {
            this.commitType = commitType;
            this.commitMesage = commitMesage;
            this.dirCommit = dirCommit;
            this.fileInfos = fileInfos;
        }
    }
}
