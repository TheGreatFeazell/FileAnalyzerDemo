using System.IO;

namespace FileAnalysis.Interfaces
{
    /// <summary>
    /// A file classifier to identify file types from a supplied input stream
    /// </summary>
    public interface IFileClassifier
    {
        #region Methods

        bool Validate( Stream inputFile );
        string GetHashCode( Stream inputfile );

        #endregion Methods

        #region Accessors

        FileClassificationTypes FileClassificationType { get; }

        #endregion Accessors
    }
}
