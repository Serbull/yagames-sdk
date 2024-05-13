using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using YandexGames;
using Newtonsoft.Json;

public class YandexSDK : MonoBehaviour
{
    private static YandexSDK Instance;

    private static Action _rewardedAdCallback;

    [SerializeField] private bool _showInterstitialOnRepeat;
    [SerializeField] private float _interstialRepeatTimer = 60;

    private float _currentInterstitialRepeatTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Initialize();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Initialize()
    {
        //LoadGame();

#if !UNITY_EDITOR
        //RestorePurchases();
        CheckCanReviewExtern();
        LoadFlagsExtern();
#endif

        if (_showInterstitialOnRepeat)
        {
            _currentInterstitialRepeatTimer = _interstialRepeatTimer;
        }
    }

    private void Update()
    {
        if (_grandReward && _rewardedAdHidden)
        {
            _grandReward = false;
            _rewardedAdCallback?.Invoke();
            _rewardedAdCallback = null;
        }

        if (_showInterstitialOnRepeat && _interstitialAdHidden && _rewardedAdHidden)
        {
            if (_currentInterstitialRepeatTimer > 0)
            {
                _currentInterstitialRepeatTimer -= Time.deltaTime;
            }
            else
            {
                _currentInterstitialRepeatTimer = _interstialRepeatTimer;
                ShowInterstitialAdWithTimer();
            }
        }
    }

    #region Ads

    private bool _grandReward;
    private bool _rewardedAdHidden = true;
    private bool _interstitialAdHidden = true;
    private static InterstitialAdTimerPopup _interstitialAdTimerPopup;

    [DllImport("__Internal")]
    private static extern void ShowInterstitialAdExtern();

    [DllImport("__Internal")]
    private static extern void ShowRewardedAdExtern();

    [DllImport("__Internal")]
    private static extern void ShowBannerAdExtern();

    [DllImport("__Internal")]
    private static extern void HideBannerAdExtern();

    public static void ShowInterstitialAd()
    {
        Debug.Log($"[YandexSDK] Show interstitial Ad.");
#if !UNITY_EDITOR
        ShowInterstitialAdExtern();
#endif
    }

    public static void ShowInterstitialAdWithTimer(int time = 3)
    {
        Time.timeScale = 0;
        var popup = Resources.Load<InterstitialAdTimerPopup>("Prefabs/InterstitialAdTimerPopup");
        _interstitialAdTimerPopup = Instantiate(popup);
        _interstitialAdTimerPopup.Initialize(ContinueGameAfterAds, time);
    }

    public static void ShowRewardedAd(Action callback)
    {
        Debug.Log($"[YandexSDK] Show rewarded Ad.");
        _rewardedAdCallback = callback;
#if UNITY_EDITOR
        Instance.RewardedAdGranded();
#else
        ShowRewardedAdExtern();
#endif
    }

    public static void ShowBannerAd()
    {
        Debug.Log($"[YandexSDK] Show banner.");
#if !UNITY_EDITOR
        ShowBannerAdExtern();
#endif
    }

    public static void HideBannerAd()
    {
        Debug.Log($"[YandexSDK] Hide banner.");
#if !UNITY_EDITOR
        HideBannerAdExtern();
#endif
    }

    private static void ContinueGameAfterAds()
    {
        _interstitialAdTimerPopup = null;
        Time.timeScale = 1;
    }

    #endregion

    #region AdsCallbacks

    public void InterstitialAdOpened()
    {
        Time.timeScale = 0;
        _interstitialAdHidden = false;
    }

    public void InterstitialAdClosed()
    {
        _interstitialAdHidden = true;
        _currentInterstitialRepeatTimer = _interstialRepeatTimer;
        if (_interstitialAdTimerPopup != null)
        {
            _interstitialAdTimerPopup.AdClosed();
        }
        else
        {
            ContinueGameAfterAds();
        }
    }

    public void RewardedAdOpened()
    {
        Time.timeScale = 0;
        _rewardedAdHidden = false;
    }

    public void RewardedAdClosed()
    {
        _rewardedAdHidden = true;
        ContinueGameAfterAds();
    }

    public void RewardedAdGranded()
    {
        _grandReward = true;
    }

    public void RewardedAdNotReady()
    {

    }

    #endregion

    #region Leaderboard

    [DllImport("__Internal")]
    private static extern void SetLeaderboardScoreExtern(string leaderboardName, int score);

    public static void SetLeaderboadScore(string leaderboardName, int score)
    {
        Debug.Log($"[YandexSDK] Set Leaderboard: '{leaderboardName}' score: {score}");
#if !UNITY_EDITOR
        SetLeaderboardScoreExtern(leaderboardName, score);
#endif
    }

    #endregion

    #region Review

    private static bool _reviewIsAvailable;
    private static bool _reviewPopupShown;

    public static bool ReviewIsAvailable => _reviewIsAvailable;

    public static event Action OnReviewStateChanged;
    public static event Action<bool> OnReviewFinish;

    [DllImport("__Internal")]
    private static extern void CheckCanReviewExtern();

    [DllImport("__Internal")]
    private static extern void ShowReviewExtern();

    public static void ShowReview()
    {
        Debug.Log("[YandexSDK] Review requested.");
        if (_reviewIsAvailable)
        {
            ShowReviewExtern();
        }
    }

    public static void ShowReviewForReward(Action callback)
    {
        if (!_reviewIsAvailable) return;

        if (_reviewPopupShown) return;

        _reviewPopupShown = true;
        var popup = Instantiate(Resources.Load<RateUsForRewardPopup>("Prefabs/RateUsForRewardPopup"));
        popup.Initialize(callback);
    }

    public void ReviewAvailable() //from lib
    {
        Debug.Log("[YandexSDK] Review is available.");
        if (!_reviewIsAvailable)
        {
            _reviewIsAvailable = true;
            OnReviewStateChanged?.Invoke();
        }
    }

    public void ReviewNotAvailable(object reason) //from lib
    {
        var reasonString = reason as string;
        Debug.Log("[YandexSDK] Review not available: " + reasonString);
        if (_reviewIsAvailable)
        {
            _reviewIsAvailable = false;
            OnReviewStateChanged?.Invoke();
        }
    }

    public void ReviewFinishSuccessful() //from lib
    {
        Debug.Log("[YandexSDK] Review finish: succesful");
        OnReviewFinish?.Invoke(true);
        ReviewNotAvailable("already requested");
    }

    public void ReviewFinishCancel() //from lib
    {
        Debug.Log("[YandexSDK] Review finish: cancel");
        OnReviewFinish?.Invoke(false);
        ReviewNotAvailable("already requested");
    }
    #endregion

    #region Purchasing

    private static readonly List<string> _restoredProducts = new();

    public static bool IsPurchasesRestored { get; private set; }
    public static string[] RestoredProducts => _restoredProducts.ToArray();

    public static event Action OnPurchasesRestored;
    public static event Action<string> OnPurchaseSuccessful;

    [DllImport("__Internal")]
    private static extern string GetProductPriceExtern(string productId);

    [DllImport("__Internal")]
    private static extern void PurchaseConsumableExtern(string productId);

    [DllImport("__Internal")]
    private static extern void PurchaseNonConsumableExtern(string productId);

    [DllImport("__Internal")]
    private static extern void RestorePurchasesExtern();

    public static string GetProductPrice(string productId)
    {
#if UNITY_EDITOR
        return "-";
#endif
#pragma warning disable CS0162 // Unreachable code detected
        return GetProductPriceExtern(productId);
#pragma warning restore CS0162 // Unreachable code detected
    }

    public static void RestorePurchases()
    {
        if (IsPurchasesRestored) return;

        RestorePurchasesExtern();
    }

    public static void Purchase(string productId, bool consumable)
    {
#if UNITY_EDITOR
        Instance.PurchaseSuccessful(productId);
#else
        if (consumable) PurchaseConsumableExtern(productId);
        else PurchaseNonConsumableExtern(productId);
#endif
    }

    public void PurchaseSuccessful(object productIdString)
    {
        var productId = productIdString as string;
        Debug.Log("[YandexSDK] Purchase successful: " + productId);
        OnPurchaseSuccessful?.Invoke(productId);
    }

    public void PurchaseFailed(object productIdString)
    {
        var productId = productIdString as string;
        Debug.Log("[YandexSDK] Purchase failed: " + productId);
    }

    public void PurchaseRestored(object productIdString)
    {
        var productId = productIdString as string;
        Debug.Log("[YandexSDK] Purchase restored: " + productId);
        if (!_restoredProducts.Contains(productId))
        {
            _restoredProducts.Add(productId);
        }
    }

    public void PurchasesRestored()
    {
        Debug.Log("[YandexSDK] Purchases successful restored.");
        IsPurchasesRestored = true;
        OnPurchasesRestored?.Invoke();
    }

    #endregion

    #region Flags

    private static Dictionary<string, string> _flags = new();
    public static bool IsFlagsLoaded { get; private set; }
    public static event Action OnFlagsLoaded;

    [DllImport("__Internal")]
    private static extern string LoadFlagsExtern();

    public void FlagsLoaded(string flags) //from lib
    {
        Debug.Log($"[YandexSDK] Flags loaded: {flags}");
        _flags = JsonConvert.DeserializeObject<Dictionary<string, string>>(flags);

        foreach (var item in _flags)
        {
            if (item.Key == "interstitialInterval")
            {
                _interstialRepeatTimer = int.Parse(item.Value);
                _currentInterstitialRepeatTimer = _interstialRepeatTimer;
                Debug.Log($"[YandexSDK] Change interstitial interval: {_interstialRepeatTimer}");
            }
        }

        IsFlagsLoaded = true;
        OnFlagsLoaded?.Invoke();
    }

    public static int GetFlag(string flag, int defalutValue)
    {
#if UNITY_EDITOR
        return defalutValue;
#endif

        if (_flags.ContainsKey(flag))
        {
            if (int.TryParse(_flags[flag], out int result))
            {
                return result;
            }
            else
            {
                Debug.LogWarning($"[YandexSDK] Cannot (int) parse flag: {flag} -> {_flags[flag]}");
                return defalutValue;
            }
        }
        else
        {
            Debug.LogWarning($"[YandexSDK] Not exist flag: {flag}");
            return defalutValue;
        }
    }

    #endregion

    #region Language

    [DllImport("__Internal")]
    private static extern string GetLanguageExtern();

    public static string GetLanguage(string defaultLanguage = "ru")
    {
#if UNITY_EDITOR
        return defaultLanguage;
#else
        return GetLanguageExtern();
#endif
    }

#endregion

    //#region CloudSaves

    //    public static bool UserPrefsLoaded { get; private set; }

    //    [DllImport("__Internal")]
    //    private static extern void SaveGameExtern(string data);

    //    [DllImport("__Internal")]
    //    private static extern void LoadGameExtern();

    //    public static void LoadGame()
    //    {
    //#if UNITY_EDITOR
    //        UserPrefs.SetUserData(PlayerPrefs.GetString("userData"));
    //        UserPrefsLoaded = true;
    //#else
    //        LoadGameExtern();
    //#endif
    //    }

    //    public static void SaveGame()
    //    {
    //#if UNITY_EDITOR
    //        PlayerPrefs.SetString("userData", UserPrefs.GetSaveData());
    //#else
    //        SaveGameExtern(UserPrefs.GetSaveData());
    //#endif
    //    }

    //    public void UserDataLoaded(string data)
    //    {
    //        UserPrefs.SetUserData(data);
    //        UserPrefsLoaded = true;
    //    }

    //#endregion
}