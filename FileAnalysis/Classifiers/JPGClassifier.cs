namespace FileAnalysis.Classifiers
{
    public class JPGClassifier : FileClassifier
    {
        #region Construction

        public JPGClassifier()
            : base( new byte[] { 0xff, 0xd8 } )
        {

        }

        #endregion Construction

        #region FileClassifier Implemenation

        public override FileClassificationTypes FileClassificationType => FileClassificationTypes.JPG;

        #endregion FileClassifier Implemenation
    }
}
