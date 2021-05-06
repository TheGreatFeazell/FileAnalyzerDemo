using System;

namespace FileAnalysis
{
    [Flags]
    public enum FileAnalyzerOptionTypes
    {
        None = 0x00,
        RecurseSubdirectories = 0x01,
        IncludeNotSupported = 0x02,
        IncludeHeader = 0x04,
    }
}
