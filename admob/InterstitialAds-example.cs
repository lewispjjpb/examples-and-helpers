using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

/**
Add this file to a Game Object and import that reference into your controlling script.  When you want to show an add, call:
  if (interstitialObject)
  {
      InterstitialAdScript interstitialAdScript = interstitialObject.GetComponent<InterstitialAdScript>();
      if (interstitialAdScript.AdLoadedStatus)
      {
          interstitialAdScript.ShowInterstitialAd();
      }
  }
*/

public class InterstitialAdScript : MonoBehaviour
{
    public bool AdLoadedStatus;
    // private string _adUnitId;

    #if UNITY_ANDROID
    private string _adUnitId = "your-android-ad-id-here";
    #elif UNITY_IPHONE
    private string _adUnitId = "your-ios-ad-id-here";
    #else
    private string _adUnitId = "unused";
    #endif
    InterstitialAd _interstitialAd;


    public void Start()
    {
        // Initialize the Google Mobile Ads SDK
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
                _interstitialAd.Destroy();
                _interstitialAd = null;
        }


        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                    "with error : " + error);
                    return;
                }


                _interstitialAd = ad;
                RegisterEventHandlers(ad);
                Debug.Log("ad loaded");
                // Inform the UI that the ad is ready.
                AdLoadedStatus = true;
            });

    }

    public void ShowInterstitialAd()
    {
        Debug.Log("_interstitialAd " + _interstitialAd);
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            LoadInterstitialAd();
        }
        AdLoadedStatus = false;
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (_interstitialAd != null)
        {
            Debug.Log("Destroying interstitial ad.");
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        // Inform the UI that the ad is not ready.
        AdLoadedStatus = false;
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                        "with error : " + error);
            LoadInterstitialAd();
        };
    }
}
