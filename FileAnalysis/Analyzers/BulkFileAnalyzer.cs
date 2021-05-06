using System;
using System.Collections.Generic;
using System.IO;

using FileAnalysis.Classifiers;
using FileAnalysis.Interfaces;

namespace FileAnalysis.Analyzers
{
    public delegate void LoggingCallback( string message );

    /// <summary>
    /// Processes a set of files and attempts to identify the file's type by matching a byte pattern
    /// </summary>
    public class BulkFileAnalyzer
    {
        #region Fields

        private Exception _exception;

        #endregion Fields

        #region Construction

        public BulkFileAnalyzer( FileAnalyzerOptions options )
        {
            this.Options = options;
        }

        #endregion Construction

        #region Helpers

        protected void Info( string message )
        {
            if( this.OnLogMessage != null )
            {
                OnLogMessage( message );
            }
        }

        protected IFileClassifier[] GetClassifiers()
        {
            var classifiers = new List<IFileClassifier>();

            classifiers.Add( new PDFClassifier() );
            classifiers.Add( new JPGClassifier() );
            if( (this.Options.Switches & FileAnalyzerOptionTypes.IncludeNotSupported) == FileAnalyzerOptionTypes.IncludeNotSupported )
            {
                classifiers.Add( new NotSupportedClassifier() );
            }

            return classifiers.ToArray();
        }

        protected FileInfo[] GetFiles( string inputPath )
        {
            var files = new List<FileInfo>();
            try
            {
                Info( $"Getting files for analysis from {inputPath}..." );
                var di = new DirectoryInfo( inputPath );
                files.AddRange( di.GetFiles() );
                if( (this.Options.Switches & FileAnalyzerOptionTypes.RecurseSubdirectories) == FileAnalyzerOptionTypes.RecurseSubdirectories )
                {
                    foreach( var subDirectory in di.GetDirectories() )
                    {
                        files.AddRange( GetFiles( subDirectory.FullName ) );
                    }
                }
            }
            catch( Exception ex )
            {
                if( _exception == null )
                {
                    _exception = ex;
                    string msg = $"The specified Input Path is not valid.\nPath: {inputPath}\nException: {ex.Message}";
                    throw new Exception( msg, ex );
                }
                else
                {
                    throw;
                }
            }

            return files.ToArray();
        }

        protected void ProcessFile( Stream inputFile )
        {
        }

        #endregion Helpers

        #region Methods

        #endregion Methods

        public FileResult[] ProcessFiles()
        {
            var classifiers = GetClassifiers();
            var inputFiles = GetFiles( this.Options.InputPath );
            var results = new List<FileResult>();

            foreach( var file in inputFiles )
            {
                Info( $"Analyzing file {file.FullName}..." );
                foreach( var classifier in classifiers )
                {
                    string filespec = file.FullName;
                    var inputFile = file.OpenRead();
                    if( classifier.Validate( inputFile ) )
                    {
                        results.Add( new FileResult( filespec, classifier.FileClassificationType, classifier.GetHashCode( inputFile ) ) );
                    }
                }
            }

            Info( "Analysis complete.\n" );
            return results.ToArray();
        }

        #region Accessors

        public LoggingCallback OnLogMessage { get; set; }
        private FileAnalyzerOptions Options { get; }

        #endregion Accessors
    }
}
