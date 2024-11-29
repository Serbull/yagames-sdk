using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using YaGamesSDK;
using Newtonsoft.Json;
using System.Collections;

public class YaGames : MonoBehaviour
{
    private static YaGames Instance;

    private static Action _rewardedAdCallback;

    [SerializeField] private bool _sendGameReadyOnStart = true;
    [Space]
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
#if !UNITY_EDITOR
        if (_sendGameReadyOnStart)
        {
            SendGameReady();
        }
        //LoadGame();

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

    #region GameReady

    private static bool _isGameReadySent;

    [DllImport("__Internal")]
    private static extern void SendGameReadyExtern();

    public static void SendGameReady()
    {
        if (_isGameReadySent)
            return;

        Debug.Log("[YaGamesSDK] Send GameReady");
        _isGameReadySent = true;
        SendGameReadyExtern();
    }
    #endregion

    #region Ads

    private bool _grandReward;
    private bool _rewardedAdHidden = true;
    private bool _interstitialAdHidden = true;
    private static InterstitialAdTimerPopup _interstitialAdTimerPopup;
    private static float _baseTimeScale;
    private static float _baseVolume;

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
        CacheGameAdValues();

        Debug.Log($"[YandexSDK] Show interstitial Ad.");
#if !UNITY_EDITOR
        ShowInterstitialAdExtern();
#endif
    }

    public static void ShowInterstitialAdWithTimer(int time = 2)
    {
        CacheGameAdValues();

        Time.timeScale = 0;
        var popup = Resources.Load<InterstitialAdTimerPopup>("Prefabs/InterstitialAdTimerPopup");
        _interstitialAdTimerPopup = Instantiate(popup);
        _interstitialAdTimerPopup.Initialize(ContinueGameAfterAds, time);

        Instance.StartCoroutine(InterstitialAdTimer(time));
    }

    private static IEnumerator InterstitialAdTimer(int time)
    {
        yield return new WaitForSecondsRealtime(time);

#if UNITY_EDITOR
        _interstitialAdTimerPopup.AdClosed();
#else
        ShowInterstitialAdExtern();
#endif
    }

    public static void ShowRewardedAd(Action callback)
    {
        CacheGameAdValues();

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
        Time.timeScale = _baseTimeScale;
        AudioListener.volume = _baseVolume;
        AudioListener.pause = false;
    }

    private static void CacheGameAdValues()
    {
        _baseTimeScale = Time.timeScale;
        _baseVolume = AudioListener.volume;
    }

    #endregion

    #region AdsCallbacks

    public void InterstitialAdOpened()
    {
        Time.timeScale = 0;
        AudioListener.volume = 0;
        AudioListener.pause = true;
        _interstitialAdHidden = false;
    }

    public void RewardedAdOpened()
    {
        Time.timeScale = 0;
        AudioListener.volume = 0;
        AudioListener.pause = true;
        _rewardedAdHidden = false;
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

    private static string _leaderboardData;

    public static bool IsLeaderboardLoaded => _leaderboardData != null;
    public static string LeaderboardData => _leaderboardData;

    [DllImport("__Internal")]
    private static extern void SetLeaderboardScoreExtern(string leaderboardName, int score);

    [DllImport("__Internal")]
    private static extern void LoadLeaderboardExtern(string leaderboardName, bool includeUser, int quantityAround, int quantityTop);

    public static void SetLeaderboadScore(string leaderboardName, int score)
    {
        Debug.Log($"[YandexSDK] Set Leaderboard: '{leaderboardName}' score: {score}");
#if !UNITY_EDITOR
        SetLeaderboardScoreExtern(leaderboardName, score);
#endif
    }

    public static void LoadLeaderboard(string leaderboardName, bool includeUser = true, int quantityAround = 10, int quantityTop = 10)
    {
        Debug.Log($"[YandexSDK] Load Leaderboard: '{leaderboardName}'");
#if !UNITY_EDITOR
        LoadLeaderboardExtern(leaderboardName, includeUser, quantityAround, quantityTop);
#endif
    }

    //not work
    public void LeaderboardLoaded(string data)
    {
        Debug.Log($"[YandexSDK] Leaderboard loaded: {data}");
        _leaderboardData = data;
    }

#endregion

#region Review

    private static bool _isReviewAvailable;

    public static bool IsReviewAvailable => _isReviewAvailable;

    public static event Action OnReviewStateChanged;
    public static event Action<bool> OnReviewFinish;

    [DllImport("__Internal")]
    private static extern void CheckCanReviewExtern();

    [DllImport("__Internal")]
    private static extern void ShowReviewExtern();

    public static void ShowReview()
    {
        Debug.Log("[YandexSDK] Review requested.");
        if (_isReviewAvailable)
        {
            ShowReviewExtern();
        }
    }

    public static void OpenReviewForReward(Action callback, int currencyCount, Sprite currencyIcon)
    {
        if (!_isReviewAvailable) return;

        var popup = Instantiate(Resources.Load<RateUsForRewardPopup>("Prefabs/RateUsForRewardPopup"));
        popup.Initialize(callback, currencyCount, currencyIcon);
    }

    public void ReviewAvailable() //from lib
    {
        Debug.Log("[YandexSDK] Review is available.");
        if (!_isReviewAvailable)
        {
            _isReviewAvailable = true;
            OnReviewStateChanged?.Invoke();
        }
    }

    public void ReviewNotAvailable(object reason) //from lib
    {
        var reasonString = reason as string;
        Debug.Log("[YandexSDK] Review not available: " + reasonString);
        if (_isReviewAvailable)
        {
            _isReviewAvailable = false;
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
#else
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
#endif
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