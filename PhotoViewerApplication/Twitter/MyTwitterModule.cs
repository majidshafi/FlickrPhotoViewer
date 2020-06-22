using LinqToTwitter;
using PhotoViewerApplication.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace PhotoViewerApplication.Twitter
{
    /// <summary>
    /// Class to represent Twitter data
    /// </summary>
    public class MyTwitterModule
    {
        #region Private Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Consumer key
        /// </summary>
        private string twitterConsumerKey = string.Empty;

        /// <summary>
        /// Consumer secret
        /// </summary>
        private string twitterConsumerSecret = string.Empty;

        /// <summary>
        /// Access token
        /// </summary>
        private string twitterAccessToken = string.Empty;

        /// <summary>
        /// Access secret
        /// </summary>
        private string twitterAccessTokenSecret = string.Empty;

        /// <summary>
        /// Twitter User Object
        /// </summary>
        private SingleUserAuthorizer singleUserAuthorizer = null;

        /// <summary>
        /// Twitter Context
        /// </summary>
        private TwitterContext twitterContext = null;

        /// <summary>
        /// List of Tweets
        /// </summary>
        private List<string> feeds = new List<string>();


        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MyTwitterModule()
        {
            ReadConfigurationData();
            AuthenticateUser();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Calls Twitter API and checks if user is authenticated
        /// </summary>
        private void AuthenticateUser()
        {
            log.Info(string.Format("AuthenticateUser: Class {0} Method {1}", nameof(MyTwitterModule), nameof(AuthenticateUser)));
            try
            {
                singleUserAuthorizer = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = twitterConsumerKey,
                        ConsumerSecret = twitterConsumerSecret,
                        AccessToken = twitterAccessToken,
                        AccessTokenSecret = twitterAccessTokenSecret
                    }
                };
                twitterContext = new TwitterContext(singleUserAuthorizer);
                if (twitterContext != null)
                {
                    var searchResult = (from search in twitterContext.Search
                                        where search.Type == SearchType.Search &&
                                        search.Query == "rain"
                                        select search).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Twitter Authorization Failed, exiting the application: Class {0} Method {1} Exception {2}", nameof(MyTwitterModule), nameof(AuthenticateUser),ex.Message.ToString()));
                MessageBox.Show(Resources.TwitterAuthenticationError, Resources.TwitterAuthenticationErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Reads the configuration keys
        /// </summary>
        private void ReadConfigurationData()
        {
            log.Info(string.Format("ReadConfigurationData: Class {0} Method {1}", nameof(MyTwitterModule), nameof(ReadConfigurationData)));

            twitterConsumerKey = ConfigurationManager.AppSettings.Get(Constants.TWITTER_CONSUMER_KEY);
            twitterConsumerSecret = ConfigurationManager.AppSettings.Get(Constants.TWITTER_CONSUMER_SECRET);
            twitterAccessToken = ConfigurationManager.AppSettings.Get(Constants.TWITTER_ACCESS_TOKEN);
            twitterAccessTokenSecret = ConfigurationManager.AppSettings.Get(Constants.TWITTER_ACCESS_TOKEN_SECRET);

        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the list of tweets
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public List<string> GetTwitterFeeds(string searchText)
        {
            log.Info(string.Format("GetTwitterFeeds: Class {0} Method {1}", nameof(MyTwitterModule), nameof(GetTwitterFeeds)));
            try
            {
                var searchResponse = (from search in twitterContext.Search
                          where search.Type == SearchType.Search &&
                          search.Query == searchText
                          select search).SingleOrDefault();

                if (searchResponse != null && searchResponse.Statuses != null)
                {
                    foreach (Status status in searchResponse.Statuses)
                    {
                        if (status.Lang == "en" && status.Retweeted == false && status.IncludeRetweets == false && status.ExcludeReplies == false)
                        {
                            feeds.Add(status.Text);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(string.Format("Failed to get Twitter feeds: Class {0} Method {1} Exception {2}", nameof(MyTwitterModule), nameof(GetTwitterFeeds), ex.Message.ToString()));
            }
            return feeds;
        }
        #endregion
    }
}
