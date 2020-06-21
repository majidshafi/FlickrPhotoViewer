namespace PhotoViewerApplication
{
    /// <summary>
    /// Class to hold Photo related data
    /// </summary>
    public class PhotoViewerCollection : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PhotoViewerCollection()
        {

        }

        #endregion
        private string photoUrl = string.Empty;

        #region Properties
        /// <summary>
        /// Get or set the photo url
        /// </summary>
        public string PhotoUrl
        {
            get => photoUrl;
            set
            {
                Set(nameof(PhotoUrl), ref photoUrl, value);
            }
        }

        private string tweets = string.Empty;

        /// <summary>
        /// Get or set the Search Text
        /// </summary>
        public string Tweets
        {
            get => tweets;
            set
            {
                Set(ref tweets, value);
            }

        }


        #endregion
    }
}
