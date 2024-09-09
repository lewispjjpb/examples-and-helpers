using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.UI;


public class RewardedAds : MonoBehaviour
{
    private GameManager gameManager;

    #if UNITY_ANDROID
    private string _adUnitId = "your android app id here";
    #elif UNITY_IPHONE
    private string _adUnitId = "your ios app id here";
    #else
    private string _adUnitId = "unused";
    #endif

    private RewardedAd _rewardedAd;
    private Button button;
    private bool isRetrying = false;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //Find my button that controls the ad view to be able to control its active status
        button = gameObject.GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(ShowRewardedAd);
        MobileAds.Initialize((InitializationStatus initStatus) => {
            LoadRewardedAd();
        });
    }

    /// Loads the rewarded ad.
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                    "with error : " + error);
                    // Retry loading the ad after 10 seconds
                    if (!isRetrying)
                    {
                        isRetrying = true;
                        StartCoroutine(RetryLoadAd());
                    }
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                        + ad.GetResponseInfo());

                _rewardedAd = ad;
                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
                // Inform the UI that the ad is ready.

                //once the ad is successfully loaded, enable the button in the UI
                button.interactable = true;
                RegisterReloadHandler(ad);
                isRetrying = false;
            });
    }

    private IEnumerator RetryLoadAd()
    {
        yield return new WaitForSeconds(10);
        LoadRewardedAd();
    }


    public void ShowRewardedAd()
    {

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // This is where you will determine what reward the user gets if they watch the ad
                // in my game Paddle Baddle, watching the ad gives the user two more balls to start with
                gameManager.ballCount += 2;
            });
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
        }

        // Inform the UI that the ad is not ready.
        button.interactable = false;
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                        "with error : " + error);
        };
    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                        "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

    public void DestroyAd()
        {
            if (_rewardedAd != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            // Inform the UI that the ad is not ready.
            button.interactable = false;
        }

}