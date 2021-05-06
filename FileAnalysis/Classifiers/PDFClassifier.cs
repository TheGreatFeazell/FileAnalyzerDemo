namespace FileAnalysis.Classifiers
{
    public class PDFClassifier : FileClassifier
    {
        #region Construction

        public PDFClassifier()
            : base( new byte[] { 0x25, 0x50, 0x44, 0x46 } ) // %pdf
        {
        }

        #endregion Construction

        #region FileClassifier Implementation

        public override FileClassificationTypes FileClassificationType => FileClassificationTypes.PDF;

        #endregion FileClassifier Implementation
    }
}
