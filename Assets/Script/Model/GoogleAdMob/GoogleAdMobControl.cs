using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using GoogleMobileAds.Common;
using System;

public class GoogleAdMobControl : MonoBehaviour
{
    /// <summary>
	/// 偵錯狀態用「測試用 Ad ID」
	/// 非測試狀態用「正式 Ad ID」
	/// </summary>
    private bool isDebug = true;

    //「測試用」初始化訊息以及相關廣告顯示訊息
    private Text AdsMessageText;
    //「測試用」橫幅廣告按鈕
    private Button BannerAdButton;
    //「測試用」插頁廣告按鈕
    private Button InterstitialAdButton;
    //「測試用」獎勵廣告按鈕
    private Button RewardAdButton;

    //自定義廣告型別
    public enum AdType
    {
        None = 0,
        Banner = 1,
        Interstitial = 2,
        Reward = 3
    }
    //現在執行的廣告型別
    private AdType currentAdType = AdType.None;

    //橫幅廣告相關元素
    private BannerView bannerView;

    //插頁廣告相關元素
    private InterstitialAd interstitial;

    //獎勵廣告相關元素
    private RewardedAd rewardedAd;

    #region 實際專案使用參數
    //廣告ID
    private int _adID;
    #endregion


    private void Awake()
    {
        //「測試用」取得測試物件
        if (GameObject.Find("/Canvas/Group/MessageGroup/ScrollView/Viewport/Content/Text").TryGetComponent<Text>(out Text _AdsMessageText))
        {
            AdsMessageText = _AdsMessageText;
        }
        if (GameObject.Find("/Canvas/Group/ItemGroup/ScrollView/Viewport/Content/BannerAdButton").TryGetComponent<Button>(out Button _BannerAdButton))
        {
            BannerAdButton = _BannerAdButton;
        }
        if (GameObject.Find("/Canvas/Group/ItemGroup/ScrollView/Viewport/Content/InterstitialAdButton").TryGetComponent<Button>(out Button _InterstitialAdButton))
        {
            InterstitialAdButton = _InterstitialAdButton;
        }
        if (GameObject.Find("/Canvas/Group/ItemGroup/ScrollView/Viewport/Content/RewardAdButton").TryGetComponent<Button>(out Button _RewardAdButton))
        {
            RewardAdButton = _RewardAdButton;
        }
    }

    // Use this for initialization
    void Start()
    {
        //初始化 橫幅廣告相關
        MobileAds.Initialize(HandleInitCompleteAction);

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 執行Log
    /// </summary>
    /// <param name="message"></param>
    private void SetLog(string message)
    {
        if (AdsMessageText == null) return;
        string currentMessgae = AdsMessageText.text;
        if (string.IsNullOrEmpty(currentMessgae))
        {
            AdsMessageText.text = message;
        }
        else
        {
            AdsMessageText.text = string.Format("{0}\n{1}", currentMessgae, message);
        }
    }


    ///// <summary>
    ///// 請求廣告類型 [實際專案]
    ///// </summary>
    ///// <param name="id">任務編號</param>
    ///// <param name="type">任務類型</param>
    //public void RequestAd(int id, int type)
    //{
    //    currentAdType = (AdType)type;
    //    switch (type)
    //    {
    //        case (int)AdType.None: //無廣告
    //            break;
    //        case (int)AdType.Banner: //橫幅廣告
    //            RequestBanner();
    //            break;
    //        case (int)AdType.Interstitial: //插頁式廣告
    //            break;
    //        case (int)AdType.Reward: //獎勵廣告
    //            break;
    //    }
    //}

    /// <summary>
    /// 請求廣告類型 [測試方法]
    /// </summary>
    /// <param name="id">任務編號</param>
    /// <param name="type">任務類型</param>
    public void RequestAd(int type)
    {
        currentAdType = (AdType)type;
        SetLog("RequestAd currentAdType:" + currentAdType.ToString());
        switch (type)
        {
            case (int)AdType.None: //無廣告
                break;
            case (int)AdType.Banner: //橫幅廣告
                RequestBanner();
                break;
            case (int)AdType.Interstitial: //插頁式廣告
                RequestInterstitial();
                break;
            case (int)AdType.Reward: //獎勵廣告
                break;
        }
    }


    #region 橫幅廣告事件

    /// <summary>
    /// 啟動橫幅
    /// </summary>
    private void RequestBanner()
    {
        string adUnitId = "";
        if (isDebug)
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
             adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
             adUnitId = "unexpected_platform";
#endif

        }
        else
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-8147047871025450/2704980731";
#elif UNITY_IPHONE
             adUnitId = "ca-app-pub-8147047871025450/5385956721";
#else
             adUnitId = "unexpected_platform";
#endif

        }
        SetLog("RequestBanner adUnitId:"+ adUnitId);

        //橫幅還存在則刪除
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }


        //建立橫幅廣告 (ID,大小,顯示位置) Create a 320x50 banner at the top of the screen.
        //大小規格 https://developers.google.com/android/reference/com/google/android/gms/ads/AdSize
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Center);

        //串接監聽
        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        //加載廣告
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    /// <summary>
    /// 關閉橫幅廣告
    /// </summary>
    public void DestroyBannerAd()
    {
        if (this.bannerView != null)
        {
            SetLog("DestroyBannerAd 關閉橫幅廣告");
            this.bannerView.Destroy();
        }
    }

    #endregion

    #region 插頁式廣告事件

    private void RequestInterstitial()
    {

        string adUnitId = "";
        if (isDebug)
        {
#if UNITY_ANDROID
             adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
         adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
         adUnitId = "unexpected_platform";
#endif

        }
        else
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-8147047871025450/8175771598";
#elif UNITY_IPHONE
             adUnitId = "ca-app-pub-8147047871025450/7437404991";
#else
             adUnitId = "unexpected_platform";
#endif

        }
        SetLog("RequestInterstitial adUnitId:" + adUnitId);

        //建立插頁式廣告 
        this.interstitial = new InterstitialAd(adUnitId);

        //串接監聽
        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        //加載廣告
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    #endregion

    #region 監聽事件

    /// <summary>
    /// 「監聽」初始化
    /// 需求使用：橫幅廣告
    /// </summary>
    /// <param name="initstatus"></param>
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            //初始化 橫幅廣告 
            SetLog("HandleBannerInitCompleteAction");
        });
    }

    /// <summary>
    /// 讀取廣告成功
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnAdLoaded(object sender, EventArgs e)
    {
        SetLog("HandleOnAdLoaded 讀取廣告成功");
        switch (currentAdType)
        {
            case AdType.None: //無廣告
                break;
            case AdType.Banner: //橫幅廣告
                //橫幅廣告不需要show()
                //this.bannerView.Show();
                break;
            case AdType.Interstitial: //插頁式廣告
                if (this.interstitial.IsLoaded())
                {
                    //顯示廣告
                    this.interstitial.Show();
                }
                break;
            case AdType.Reward: //獎勵廣告
                break;
        }
    }

    /// <summary>
    /// 讀取廣告失敗
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        SetLog("HandleOnAdFailedToLoad 讀取廣告失敗 :"+e.Message);
    }

    /// <summary>
    /// 廣告打開
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnAdOpened(object sender, EventArgs e)
    {
        SetLog("HandleOnAdOpened 廣告打開");
    }

    /// <summary>
    /// 廣告關閉
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnAdClosed(object sender, EventArgs e)
    {
        SetLog("HandleOnAdClosed 廣告關閉");
        switch (currentAdType)
        {
            case AdType.None: //無廣告
                break;
            case AdType.Banner: //橫幅廣告
                break;
            case AdType.Interstitial: //插頁式廣告
                if (this.interstitial != null)
                {
                    //顯示廣告
                    this.interstitial.Destroy();
                }
                break;
            case AdType.Reward: //獎勵廣告
                break;
        }
    }

    /// <summary>
    /// 點擊廣告離開APP
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOnAdLeavingApplication(object sender, EventArgs e)
    {
        SetLog("HandleOnAdLeavingApplication 點擊廣告離開APP");
    }

    #endregion


}
