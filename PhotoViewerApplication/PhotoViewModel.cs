using GalaSoft.MvvmLight.Command;
using PhotoViewerApplication.Properties;
using PhotoViewerApplication.Twitter;
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
            FetchImageCommand = new RelayCommand(async () => await FetchPhotos());
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
                MessageBox.Show(Resources.NumOfPhotosErrorMessage, Resources.NumOfPhotosCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                log.Debug(string.Format("Number of Photos is not numeric, Class {0} Method {1}", nameof(PhotoViewModel), nameof(ValidateData)));
                NumberOfPhotos = 0;
            }
            else if (NumberOfPhotos > 4000)
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
        /// Fetches the photo url from the flicker
        /// </summary>
        /// <returns></returns>
        private async Task FetchPhotos()
        {
            log.Info(string.Format("Fetching Photos: Class {0} Method {1}", nameof(PhotoViewModel), nameof(FetchPhotos)));
            IsTaskDone = false;
            if (ValidateData())
            {
                IsIndicatorBusy = true;
                BusyContentMessage = Resources.PhotosDownloadingMessage;

                if (myFlicker != null)
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                         PhotoCollections.Clear();
                         photoUrlList.Clear();
                        photoTweets.Clear();

                        photoUrlList = myFlicker.GetPhotoUrlList(NumberOfPhotos.Value, SearchText);
                        photoTweets = myTwitter.GetTwitterFeeds(SearchText);
                        string fd = "";
                        if (photoTweets != null && photoTweets.Count > 0)
                        {
                            int count = 0;
                            foreach(string tw in photoTweets)
                            {
                                if (count < 2)
                                {
                                    fd += tw + ",";
                                    count++;
                                }
                            }
                        }
                        else
                        {
                            fd = string.Format("Couldn't find any tweets associated with {0}", SearchText);
                        }

                        if (photoUrlList != null && photoUrlList.Count > 0)
                        {
                            foreach (string url in photoUrlList)
                            {
                                PhotoCollections.Add(new PhotoViewerCollection { PhotoUrl = url, Tweets = fd });
                                await Task.Delay(10);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Couldn't find any photos", "asdfa", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                       
                    });
                    IsTaskDone = true;
                }
                IsIndicatorBusy = false;
                IsTaskDone = true;
            }
            else
            {
                log.Error(string.Format("Validation of Data Failed : Class {0} Method {1}", nameof(PhotoViewModel), nameof(FetchPhotos)));
            }
        }
        #endregion
    }
}
