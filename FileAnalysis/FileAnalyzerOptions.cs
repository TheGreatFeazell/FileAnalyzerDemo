namespace FileAnalysis
{
    public class FileAnalyzerOptions
    {
        #region Construction

        public FileAnalyzerOptions(string inputPath, string outputFilespec, FileAnalyzerOptionTypes options)
        {
            this.InputPath = inputPath;
            this.OutputFilespec = outputFilespec;
            this.Switches = options;
        }

        #endregion Construction

        #region Accessors

        public string InputPath { get; }
        public string OutputFilespec { get; }
        public FileAnalyzerOptionTypes Switches { get; }

        #endregion Accessors
    }
}
