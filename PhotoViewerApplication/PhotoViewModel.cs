using GalaSoft.MvvmLight.Command;
using PhotoViewerApplication.Properties;
using PhotoViewerApplication.Twitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhotoViewerApplication
{
    /// <summary>
    /// View Model for Photo viewer application
    /// </summary>
    public class PhotoViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Flicker Object
        /// </summary>
        private MyFlickerModule myFlicker = null;

        private MyTwitterModule myTwitter = null;

        /// <summary>
        /// List of Photo URL's
        /// </summary>
        private List<string> photoUrlList = new List<string>();

        /// <summary>
        /// List of Tweets
        /// </summary>
        private List<string> photoTweets = new List<string>();

        #endregion

        #region Properties
        private string searchText = string.Empty;

        /// <summary>
        /// Get or set the Search Text
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set
            {
                Set(ref searchText, value);
            }

        }
        
        private int? numerOfPhotos;

        /// <summary>
        /// Get or set the number of photos
        /// </summary>
        public int? NumberOfPhotos
        {
            get => numerOfPhotos;
            set
            {

                Set(ref numerOfPhotos, value);

            }
        }

        private ObservableCollection<PhotoViewerCollection> photoCollection = new ObservableCollection<PhotoViewerCollection>();

        /// <summary>
        /// Collection of Photo URL's to populate in the list
        /// </summary>
        public ObservableCollection<PhotoViewerCollection> PhotoCollections
        {
            get => photoCollection;
            set
            {
                Set(ref photoCollection, value);
            }
        }
     
        private bool isTaskDone;

        /// <summary>
        /// Get or set the value to indicate whether url loading is done or not
        /// </summary>
        public bool IsTaskDone
        {
            get => isTaskDone;
            set => Set(ref isTaskDone, value);
        }

        private bool isIndicatorBusy = false;

        /// <summary>
        /// Get or set the value to indicate whether to show busy indicator or not
        /// </summary>
        public bool IsIndicatorBusy
        {
            get => isIndicatorBusy;
            set { Set(ref isIndicatorBusy, value); }
        }

        private string busyContentMessage = string.Empty;

        /// <summary>
        /// Get or set the busy content message
        /// </summary>
        public string BusyContentMessage
        {
            get => busyContentMessage;
            set { Set(ref busyContentMessage, value); }
        }

        #endregion

        #region Command Handler

        /// <summary>
        /// Gets the command to fetch photos from Flickr
        /// </summary>
        public ICommand FetchImageCommand { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PhotoViewModel()
        {
            InitializeData();
        }
        #endregion

        #region Private Functions

        /// <summary>
        /// Initializes the flicker object and command handler
        /// </summary>
        private void InitializeData()
        {
            log.Info(string.Format("Data Initialization: Class {0} Method {1}", nameof(PhotoViewModel), nameof(InitializeData)));
            myTwitter = new MyTwitterModule();
            myFlicker = new MyFlickerModule();
            FetchImageCommand = new RelayCommand(async () => await FetchPhotosAndTweets());
        }

        /// <summary>
        /// Validate the data(search text and number of photos)
        /// </summary>
        /// <returns></returns>
        private bool ValidateData()
        {
            bool status = false;
            log.Info(string.Format("Data Validation: Class {0} Method {1}", nameof(PhotoViewModel), nameof(ValidateData)));

            if (string.IsNullOrEmpty(SearchText))
            {
                MessageBox.Show(Resources.SearchTextErrorMessage, Resources.SearchTextCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                log.Debug(string.Format("Search Text is Empty, Class {0} Method {1}", nameof(PhotoViewModel), nameof(ValidateData)));
                SearchText = string.Empty;
            }
            else if (NumberOfPhotos == 0)
            {
                MessageBox.Show(Resources.NumOfPhotosZeroErrorMessage, Resources.NumOfPhotosCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                log.Debug(string.Format("Number of Photos should be > 0 & < 4000, Class {0} Method {1}", nameof(PhotoViewModel), nameof(ValidateData)));
                NumberOfPhotos = 0;
            }
            else if (NumberOfPhotos > Constants.MAX_PHOTOS)
            {
                MessageBox.Show(Resources.NumOfPhotosExceed, Resources.NumOfPhotosCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                log.Debug(string.Format("Number of Photos is > 4000, Class {0} Method {1}", nameof(PhotoViewModel), nameof(ValidateData)));
                NumberOfPhotos = 0;
            }
            else
            {
                log.Debug(string.Format("Validation success, Class {0} Method {1}", nameof(PhotoViewModel), nameof(ValidateData)));
                status = true;
            }
            return status;
        }

        /// <summary>
        /// Fetches the photo url from the flicker & tweets from Twitter
        /// </summary>
        /// <returns></returns>
        private async Task FetchPhotosAndTweets()
        {
            log.Info(string.Format("Fetching Photos & Tweets: Class {0} Method {1}", nameof(PhotoViewModel), nameof(FetchPhotosAndTweets)));
            IsTaskDone = false;
            try
            {
                if (ValidateData())
                {
                    IsIndicatorBusy = true;
                    BusyContentMessage = Resources.PhotosDownloadingMessage;

                    if (myFlicker != null && myTwitter != null)
                    {
                        await Application.Current.Dispatcher.Invoke(async () =>
                        {
                            // Clear Photo Containers
                            PhotoCollections.Clear();
                            photoUrlList.Clear();
                            photoTweets.Clear();

                            // Get Photo URL's
                            photoUrlList = myFlicker.GetPhotoUrlList(NumberOfPhotos.Value, SearchText);
                            // Get Tweets
                            photoTweets = myTwitter.GetTwitterFeeds(SearchText);
                            string twitterFeeds = "";

                            // Take first two tweets for UI purpose
                            if (photoTweets != null && photoTweets.Count > 0)
                                {
                                    int count = 0;
                                    foreach (string tw in photoTweets)
                                    {
                                        if (count < 2)
                                        {
                                            twitterFeeds += tw + ",";
                                            count++;
                                        }
                                    }
                                }
                                else
                                {
                                    twitterFeeds = Resources.NoTweetsMessage;
                                    photoTweets = new List<string>();
                                }

                                if (photoUrlList != null && photoUrlList.Count > 0)
                                {
                                    foreach (string url in photoUrlList)
                                    {
                                        PhotoCollections.Add(new PhotoViewerCollection { PhotoUrl = url, Tweets = twitterFeeds });
                                        await Task.Delay(10);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(Resources.NoPhotosMessage, Resources.NoPhotosCaptionMessage, MessageBoxButton.OK, MessageBoxImage.Information);
                                    photoUrlList = new List<string>();
                                }

                            });
                            IsTaskDone = true;
                    }
                    IsIndicatorBusy = false;
                    IsTaskDone = true;
                }
                else
                {
                    log.Error(string.Format("Validation of Data Failed : Class {0} Method {1}", nameof(PhotoViewModel), nameof(FetchPhotosAndTweets)));
                }
            }
            catch(Exception ex)
            {
                log.Error(string.Format("Exception occured in FetchPhotosAndTweets : Class {0} Method {1} Exception {2}", nameof(PhotoViewModel), nameof(FetchPhotosAndTweets),ex.Message.ToString()));
            }
        }
        #endregion
    }
}
