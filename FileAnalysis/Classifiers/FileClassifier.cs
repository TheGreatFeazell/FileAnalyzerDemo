using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

using Xnx.Common.Extensions;

using FileAnalysis.Interfaces;

namespace FileAnalysis.Classifiers
{
    /// <summary>
    /// An abstraction for classifying files based on a byte pattern on the file's header
    /// </summary>
    public abstract class FileClassifier : IFileClassifier
    {
        #region Construction

        public FileClassifier( byte[] header )
        {
            this.Header = header;
        }

        #endregion Construction

        #region Helpers

        protected bool ValidateHeader( Stream inputFile )
        {
            bool valid = false;

            if( !this.Header.IsNullOrEmpty() )
            {
                try
                {
                    var buffer = new byte[this.Header.Length];
                    inputFile.Seek( 0, SeekOrigin.Begin );
                    inputFile.Read( buffer, 0, this.Header.Length );

                    valid = StructuralComparisons.StructuralEqualityComparer.Equals( buffer, this.Header );
                }
                catch( Exception ex )
                {
                    valid = false;
                }
            }

            return valid;
        }

        #endregion Helpers

        #region IFileClassifier

        public virtual bool Validate( Stream inputFile )
        {
            return ValidateHeader( inputFile );
        }

        public string GetHashCode( Stream inputStream )
        {
            string hash = null;

            using( var md5 = MD5.Create() )
            {
                var result = md5.ComputeHash( inputStream );
                hash = BitConverter.ToString( result );
            }

            return hash;
        }

        public abstract FileClassificationTypes FileClassificationType { get; }

        #endregion IFileClassifier

        #region Accessors

        private byte[] Header { get; }

        #endregion Accessors
    }
}
