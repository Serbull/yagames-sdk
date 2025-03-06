using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using YaGamesSDK;
using YaGamesSDK.Core;
using Newtonsoft.Json;
using System.Collections;
using System.Globalization;

public class YaGames : MonoBehaviour
{
    private static YaGames Instance;

    private static Action _rewardedAdCallback;

    [Header("CloudSaves")]
    [SerializeField] private bool _loadCloudSavesOnStart = false;
    [Header("Purchases")]
    [SerializeField] private bool _restorePurchasesOnStart = false;
    [Space]
    [SerializeField] private bool _sendGameReadyOnStart = true;

    private float _currentInterstitialRepeatTimer;

    private readonly static CloudSaves _cloudSaves = new();
    private readonly static Purchasing _purchasing = new();
    private readonly static Leaderboards _leaderboards = new();
    private static YaGamesSettings _settings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            _settings = YaGamesSettings.Instance;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (_loadCloudSavesOnStart)
        {
            CloudSaves.LoadGame();
        }

        if (_restorePurchasesOnStart)
        {
            Purchasing.RestorePurchases();
        }

        if (_sendGameReadyOnStart)
        {
            SendGameReady();
        }

#if !UNITY_EDITOR
        CheckCanReviewExtern();
        LoadFlagsExtern();
#endif

        if (_settings.ShowInterstitialOnRepeat)
        {
            _currentInterstitialRepeatTimer = _settings.InterstitialRepeatTime;
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

        if (_workoutInterstitialHiding && _interstitialAdHidden)
        {
            _workoutInterstitialHiding = false;
            _currentInterstitialRepeatTimer = _settings.InterstitialRepeatTime;
            if (_interstitialAdTimerPopup != null)
            {
                _interstitialAdTimerPopup.AdClosed();
            }
            else
            {
                ContinueGameAfterAds();
            }
        }

        if (_settings.ShowInterstitialOnRepeat && _interstitialAdHidden && _rewardedAdHidden)
        {
            if (_currentInterstitialRepeatTimer > 0)
            {
                _currentInterstitialRepeatTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                _currentInterstitialRepeatTimer = _settings.InterstitialRepeatTime;
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

        Log("Send GameReady");
        _isGameReadySent = true;
#if !UNITY_EDITOR
        SendGameReadyExtern();
#endif
    }

    #endregion

    #region Ads

    private bool _grandReward;
    private bool _rewardedAdHidden = true;
    private bool _interstitialAdHidden = true;
    private static InterstitialAdTimerPopup _interstitialAdTimerPopup;
    private static float _baseTimeScale;
    private static float _baseVolume;
    private bool _workoutInterstitialHiding;

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
        _workoutInterstitialHiding = true;
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

    #region JS_METHODS

    public void LeaderboardLoaded(string data)
    {
        Log($"Leaderboard loaded: {data}");
        _leaderboards.LeaderboardLoaded(data);
    }

    public void CloudSavesLoaded(string data)
    {
        Log($"CloudSaves loaded: {data}");
        _cloudSaves.DataLoaded(data);
    }

    //PURCHASING

    public void PurchasingPurchaseSuccessful(object productIdString)
    {
        var productId = productIdString as string;
        _purchasing.PurchaseSuccessful(productId);
    }

    public void PurchasingPurchaseFailed(object productIdString)
    {
        var productId = productIdString as string;
        _purchasing.PurchaseFailed(productId);
    }

    public void PurchasingProductRestored(object productIdString)
    {
        var productId = productIdString as string;
        _purchasing.PurchaseRestored(productId);
    }

    public void PurchasingAllProductsRestored()
    {
        _purchasing.AllProductsRestored();
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

        if (HasFlag(_settings.InterstitialRepeatFlag))
        {
            var time = GetFlag(_settings.InterstitialRepeatFlag, _settings.InterstitialRepeatTime);
            _settings.InterstitialRepeatTime = time;
            Log($"Change interstitial interval: {time}");
        }

        IsFlagsLoaded = true;
        OnFlagsLoaded?.Invoke();
    }

    public static bool HasFlag(string flag)
    {
        foreach (var item in _flags)
        {
            if (item.Key == flag)
            {
                return true;
            }
        }

        return false;
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

    public static float GetFlag(string flag, float defalutValue)
    {
#if UNITY_EDITOR
        return defalutValue;
#else
        if (_flags.ContainsKey(flag))
        {
            if (float.TryParse(_flags[flag], NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            else
            {
                Debug.LogWarning($"[YandexSDK] Cannot (float) parse flag: {flag} -> {_flags[flag]}");
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

    public static string GetLanguage()
    {
#if UNITY_EDITOR
        return _settings.EditorDefaultLanguage;
#else
        return GetLanguageExtern();
#endif
    }

    #endregion

    #region DeviceInfo

    private static string _deviceType;
    private static bool _isDeviceTouchable;

    [DllImport("__Internal")]
    private static extern string GetDeviceInfoExtern();

    public static bool IsDeviceTouchable
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            if (_deviceType == null)
            {
                _deviceType = GetDeviceInfoExtern();
                _isDeviceTouchable = _deviceType == "mobile" || _deviceType == "tablet";
            }

            return _isDeviceTouchable;
#endif
        }
    }

    #endregion

    public static void Log(string message)
    {
#if UNITY_EDITOR
        message = "<b><color=#ffbf00>[YandexSDK]</color></b> " + message;
#else
        message = "[YandexSDK] " + message;
#endif
        Debug.Log(message);
    }

    public static void LogError(string message)
    {
#if UNITY_EDITOR
        message = "<b><color=#ffbf00>[YandexSDK]</color></b> " + message;
#else
        message = "[YandexSDK] " + message;
#endif
        Debug.LogError(message);
    }
}
