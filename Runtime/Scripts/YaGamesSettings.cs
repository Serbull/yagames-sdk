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

        [Header("Other")]
        public int BuildVersion;
        public bool ReplaceIndexHtml = true;
        [ShowIf(nameof(ReplaceIndexHtml))]
        public bool ShowInterstitialOnGameStart = true;
        public bool ArchiveBuild = true;
    }
}
