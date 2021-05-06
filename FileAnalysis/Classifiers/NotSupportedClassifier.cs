using System.IO;

namespace FileAnalysis.Classifiers
{
    /// <summary>
    /// A generic classifier - When all other classifiers fail, this will match any input file
    /// </summary>
    public class NotSupportedClassifier : FileClassifier
    {
        #region Construction

        public NotSupportedClassifier()
            : base( null )
        {
        }

        #endregion Construction

        #region FileClassifier Implementation

        public override bool Validate( Stream inputFile )
        {
            return true;
        }

        public override FileClassificationTypes FileClassificationType => FileClassificationTypes.NOT_SUPPORTED;

        #endregion FileClassifier Implementation
    }
}
