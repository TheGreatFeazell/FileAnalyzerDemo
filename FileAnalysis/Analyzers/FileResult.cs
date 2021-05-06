using System;

namespace FileAnalysis.Analyzers
{
    public class FileResult
    {
        #region Construction

        public FileResult(string filespec, FileClassificationTypes fileType, string hashValue)
        {
            this.Filespec = filespec;
            this.FileType = fileType;
            this.HashValue = hashValue;
        }

        #endregion Construction

        #region Accessors

        public string Filespec { get; }
        public FileClassificationTypes FileType { get; }
        public String HashValue { get; }

        #endregion Accessors
    }
}
