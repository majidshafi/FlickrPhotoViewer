# FlickrPhotoViewer
Display Photos from Flickr and associated tweets for search pattern

---------------------------------------------------------------------------------------------------------------------
Please clone the code from the repo and build the solution.
Nuget packages are already included with the solution.

_____________________________________________________________________________________________________________________

Functionality is described as:
  - Uses Flickr and Twitter API Keys for my account.
  - Keys are places in App.Config

After running the application, please follow the below steps:
  - Enter Search Text to search photos from Flickr
  - Enter Number of Photos (Currently it is allowed till 4000 as defined MAX limit from Flickr)
  - Hit Fetch Photos button and photos will be downloaded from Flickr(128*128).
  - Same Search text is used to fetch tweets from Twiter and are displayed along images.
  
 Following validations are taken care:
 - Check if Flickr user is authenticated
 - Check if Twitter user is authenticated
 - Check for photo limit betweeen 1-4000
 - Check if no photo is avaialble from flickr
 - Check if no tweet is associated with search pattern
 
 Platform Usage
  - WPF Application targetting 4.7.2 .NET
  - Configuration : AnyCPU
  - Nugets Used:
    - Extended WPF took kit : For busy Indicator
    - FlickrNet : Access Flickr API's
    - LinqToTwitter : Access Twitter API's
    - log4net : Logging mechanism
    - MvvmLight : WPF Model
    
