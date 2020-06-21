using LinqToTwitter;
using PhotoViewerApplication.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoViewerApplication.Twitter
{
    public class MyTwitterModule
    {
        #region Private Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string twitterConsumerKey = string.Empty;
        private string twitterConsumerSecret = string.Empty;
        private string twitterAccessToken = string.Empty;
        private string twitterAccessTokenSecret = string.Empty;

        private SingleUserAuthorizer singleUserAuthorizer = null;
        private TwitterContext twitterContext = null;

        #endregion

        public MyTwitterModule()
        {
            ReadConfigurationData();
            AuthenticateUser();
        }

        private void AuthenticateUser()
        {
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
                log.Error(string.Format("Twitter Authorization Failed, exiting the application: Class {0} Method {1}", nameof(MyTwitterModule), nameof(AuthenticateUser)));
                MessageBox.Show(Resources.TwitterAuthenticationError, Resources.TwitterAuthenticationErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void ReadConfigurationData()
        {
            twitterConsumerKey = ConfigurationManager.AppSettings.Get(Constants.TWITTER_CONSUMER_KEY);
            twitterConsumerSecret = ConfigurationManager.AppSettings.Get(Constants.TWITTER_CONSUMER_SECRET);
            twitterAccessToken = ConfigurationManager.AppSettings.Get(Constants.TWITTER_ACCESS_TOKEN);
            twitterAccessTokenSecret = ConfigurationManager.AppSettings.Get(Constants.TWITTER_ACCESS_TOKEN_SECRET);

        }

        List<string> feeds = new List<string>();
        public List<string> GetTwitterFeeds(string searchText)
        {

            try
            {
                var sr = (from search in twitterContext.Search
                          where search.Type == SearchType.Search &&
                          search.Query == searchText
                          select search).SingleOrDefault();

                if (sr != null && sr.Statuses != null)
                {


                    foreach (Status s in sr.Statuses)
                    {
                        if (s.Lang == "en" && s.Retweeted == false && s.IncludeRetweets == false && s.ExcludeReplies == false)
                            feeds.Add(s.Text);
                    }


                }
            }
            catch(Exception ex)
            {

            }

            return feeds;

        }
    }
   
}
