using System.Collections;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class BannerAds : MonoBehaviour
{
    // Start is called before the first frame update
    #if UNITY_ANDROID
    private string _adUnitId = "your-android-id-here";
    #elif UNITY_IPHONE
    private string _adUnitId = "your-ios-id-here";
    #else
    private string _adUnitId = "unused";
    #endif

    BannerView _bannerView;
    private bool isRetrying = false;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            this.LoadBannerAd();
        });
    }



    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if(_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        _bannerView.LoadAd(adRequest);
        ListenToAdEvents();
    }

    private IEnumerator RetryBannerAd()
    {
        // Wait for a specified amount of time before retrying.
        yield return new WaitForSeconds(10f); // You can adjust the delay as needed.

        // Retry loading the banner ad.
        LoadBannerAd();

        // Reset the retry flag.
        isRetrying = false;
    }
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
            if (!isRetrying)
            {
                isRetrying = true;
                StartCoroutine(RetryBannerAd());
            }
            };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Banner view paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
}
