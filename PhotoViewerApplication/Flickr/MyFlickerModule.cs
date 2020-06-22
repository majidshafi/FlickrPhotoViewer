using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using FlickrNet;
using PhotoViewerApplication.Properties;

namespace PhotoViewerApplication
{
    /// <summary>
    /// Flicker Implementation Module
    /// </summary>
    public class MyFlickerModule
    {
        #region Private Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Flicker API Key
        /// </summary>
        private string flickerAPIKey = string.Empty;

        /// <summary>
        /// Flicker Object
        /// </summary>
        private Flickr flickrObject = null;

        /// <summary>
        /// List of Photo URL's
        /// </summary>
        private List<string> photoUrls = new List<string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MyFlickerModule()
        {
            InitializeData();
        }

        #endregion

        #region Private Functions
        /// <summary>
        /// Data Initialization
        /// </summary>
        private void InitializeData()
        {
            log.Info(string.Format("InitializeData: Class {0} Method {1}", nameof(MyFlickerModule), nameof(InitializeData)));
            try
            {
                flickerAPIKey = ConfigurationManager.AppSettings.Get(Constants.FLICKER_API_KEY);
                flickrObject = new Flickr(flickerAPIKey);
                if (flickrObject != null)
                {
                    try
                    {
                        flickrObject.PhotosSearch(new PhotoSearchOptions {
                            PrivacyFilter = PrivacyFilter.PublicPhotos,MediaType = MediaType.Photos, PerPage=1});
                    }
                    catch(Exception ex)
                    {
                        log.Error(string.Format("Flicker Authorization Failed, exiting the application: Class {0} Method {1} exception {2}", nameof(MyFlickerModule), nameof(InitializeData),ex.Message.ToString()));
                        MessageBox.Show(Resources.AuthenticationError, Resources.AuthenticationErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                    
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Exception occured  Class {0} Method {1} Message {2}", nameof(MyFlickerModule), nameof(InitializeData),exception.Message.ToString()));
            }

        }

        #endregion

        #region Public Functions
        /// <summary>
        /// Returns the list of photo urls
        /// </summary>
        /// <param name="numPhotos">Number of photos to fetch</param>
        /// <param name="searchText">Search text used for photo</param>
        /// <returns></returns>
        public List<string> GetPhotoUrlList(int numPhotos, string searchText)
        {
            log.Info(string.Format("GetPhotoUrlList: Class {0} Method {1}", nameof(MyFlickerModule), nameof(InitializeData)));
            PhotoCollection photoCollection = null;
            PhotoSearchOptions options = null;
            try
            {
               options =  new PhotoSearchOptions
                {
                    Tags = searchText,
                    PerPage = numPhotos,
                    PrivacyFilter = PrivacyFilter.PublicPhotos,
                    MediaType = MediaType.Photos,
                    SafeSearch = SafetyLevel.Safe,
                    ContentType = ContentTypeSearch.PhotosAndScreenshots,
                    SortOrder = PhotoSearchSortOrder.InterestingnessDescending
                };

                if (options != null)
                {
                    photoCollection = flickrObject.PhotosSearch(options);
                    if (photoCollection?.Count > 0)
                    {
                        foreach (Photo photo in photoCollection)
                        {
                            if (photo != null && !(string.IsNullOrEmpty(photo.Medium800Url)))
                                photoUrls.Add(photo.LargeUrl);
                        }
                        return photoUrls;
                    }
                }
                else
                {
                    log.Error(string.Format("Options is null Class {0} Method {1}", nameof(MyFlickerModule), nameof(GetPhotoUrlList)));
                }
            }
            catch(Exception ex)
            {
                log.Error(string.Format("Exception occured  Class {0} Method {1} Message {2}", nameof(MyFlickerModule), nameof(GetPhotoUrlList), ex.Message.ToString()));
            }

            return null;
        }
        #endregion
    }
}
