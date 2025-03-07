using UnityEngine;

namespace YaGamesSDK.Core
{
    public class YaGamesSettings : ScriptableObject
    {
        private static YaGamesSettings _instance;

        public static YaGamesSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<YaGamesSettings>("YaGamesSettings");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = YaGamesUtils.CreateSettingsFile();
                    }
#endif
                }

                return _instance;
            }
        }

        [Header("Interstitial")]
        public bool ShowInterstitialOnRepeat;
        [ShowIf(nameof(ShowInterstitialOnRepeat))]
        public float InterstitialRepeatTime = 60;
        [ShowIf(nameof(ShowInterstitialOnRepeat))]
        public string InterstitialRepeatFlag = "interstitialInterval";

        [Header("Localization")]
        public string EditorDefaultLanguage = "en";

        [Header("Debug")]
        public bool ShowBuildTime = true;
        [HideInInspector]
        public string BuildTime;
    }
}
