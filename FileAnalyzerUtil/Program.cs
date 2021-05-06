using FileAnalysis;
using FileAnalysis.Analyzers;
using System;
using System.Collections.Generic;
using System.IO;

using Xnx.Common.Extensions;

namespace FileAnalyzerUtil
{
    class Program
    {
        #region Helpers

        private static void ShowHelpText()
        {
            var year = DateTime.Now.Year;

            var helpText = new string[]
            {
                "fa - A simple file analysis utility",
                $"(c) {year} KL Discovery - All Rights Reserved.\n",
                "Synopsis: Provide an input path and an output file, and fa will produce a .CSV report",
                "providing the file type of each file and the MD5 hash.\n",
                "Usage:",
                "fa [-options] source_path output_file[.csv]\n",
                "Options:",
                "-r\tRecurses the folder structure specified by [source_path].",
                "\tOtherwise, only the [source_path] folder is scanned.\n",
                "-n\tInclude non-supported file types. Otherwise, a non-supported file",
                "\twill be ignored. Non-supported files are labeled NOT_SUPPORTED",
                "\tin the output file.\n",
                "-h\tInclude a header in the output file.\n",
                "Note that multiple options can be combined as in -rnh or -r -n -h",
            };

            helpText.ForEach( msg =>
            {
                Console.WriteLine( msg );
            } );
            Console.WriteLine();
        }

        private static bool IsCommandLineSwitch( string arg )
        {
            bool isSwitch = false;

            if( !arg.IsNullOrEmpty() )
            {
                isSwitch = arg.StartsWith( '-' );
            }

            return isSwitch;
        }

        private static FileAnalyzerOptionTypes ParseCommandLineSwitch( char optionSwitch )
        {
            var options = FileAnalyzerOptionTypes.None;

            switch( optionSwitch )
            {
                default:
                    throw new Exception( $"The command line switch -{optionSwitch} is invalid." );
                case 'r':
                case 'R':
                    options = FileAnalyzerOptionTypes.RecurseSubdirectories;
                    break;
                case 'n':
                case 'N':
                    options = FileAnalyzerOptionTypes.IncludeNotSupported;
                    break;
                case 'h':
                case 'H':
                    options = FileAnalyzerOptionTypes.IncludeHeader;
                    break;
            }

            return options;
        }

        private static FileAnalyzerOptions ParseCommandLineArguments( string[] args )
        {
            var switchOptions = FileAnalyzerOptionTypes.None;
            var commandLineParams = new List<string>();

            if( !args.IsNullOrEmpty() )
            {
                foreach( var arg in args )
                {
                    if( IsCommandLineSwitch( arg ) )
                    {
                        if( arg.Length > 1 )
                        {
                            var switches = arg.Substring( 1 ).ToCharArray();
                            switches.ForEach( s => switchOptions |= ParseCommandLineSwitch( s ) );
                        }
                    }
                    else
                    {
                        commandLineParams.Add( arg );
                    }
                }
            }

            string inputPath, outputFilespec;
            if( commandLineParams.Count >= 1 )
            {
                inputPath = commandLineParams[0];

                if( commandLineParams.Count >= 2 )
                {
                    outputFilespec = commandLineParams[1];
                    if( outputFilespec.EndsWith( '\\' ) )
                    {
                        throw new Exception( $"{outputFilespec} is a folder. You must specify an output file name." );
                    }

                    if( !outputFilespec.ToLower().EndsWith( ".csv" ) )
                    {
                        outputFilespec += ".csv";
                    }

                    if( commandLineParams.Count == 2 )
                    {
                        return new FileAnalyzerOptions( inputPath, outputFilespec, switchOptions );
                    }
                    else
                    {
                        throw new Exception( "Too many arguments specified." );
                    }
                }
                else
                {
                    throw new Exception( "An Output File must be specified." );
                }
            }
            else
            {
                throw new Exception( "An Input Path must be specified." );
            }
        }

        private static void ValidateOptions( FileAnalyzerOptions options )
        {
            if( File.Exists( options.OutputFilespec ) )
            {
                Console.WriteLine( $"Output file {options.OutputFilespec} exists. Do you wish to overwrite it? (Y/N)" );

                ConsoleKeyInfo ki;
                do
                {
                    ki = Console.ReadKey( true );
                } while( ki.Key != ConsoleKey.Y && ki.Key != ConsoleKey.N );
                if( ki.Key == ConsoleKey.Y )
                {
                    File.Delete( options.OutputFilespec );
                }
                else
                {
                    throw new Exception( "Output file already exists. Please choose a new file and try again." );
                }
            }
        }

        private static void WriteOutputFile( FileAnalyzerOptions options, FileResult[] fileResults )
        {
            using( var stream = new StreamWriter( options.OutputFilespec ) )
            {
                try
                {
                    if( (options.Switches & FileAnalyzerOptionTypes.IncludeHeader) == FileAnalyzerOptionTypes.IncludeHeader )
                    {
                        stream.WriteLine( "Path / Filename,File Type Descriptor,MD5 Hash" );
                    }
                    foreach( var result in fileResults )
                    {
                        stream.WriteLine( $"{result.Filespec},{result.FileType},{result.HashValue}" );
                    }
                }
                catch( Exception ex )
                {
                    throw;
                }
                finally
                {
                    stream.Close();
                }
            }
        }

        #endregion Helpers

        static void Main( string[] args )
        {
            bool showHelp = args.IsNullOrEmpty();

            if( !showHelp )
            {
                try
                {
                    var options = ParseCommandLineArguments( args );
                    ValidateOptions( options );
                    var analyzer = new BulkFileAnalyzer( options );
                    analyzer.OnLogMessage += Console.WriteLine;

                    var results = analyzer.ProcessFiles();

                    Console.WriteLine( "Writing output file..." );
                    WriteOutputFile( options, results );
                    Console.WriteLine( $"\nWrote {results.Length} records to {options.OutputFilespec}\n" );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( $"\nERROR! {ex.Message}\n" );
                    showHelp = true;
                }
            }

            if( showHelp )
            {
                ShowHelpText();
            }
        }
    }
}
