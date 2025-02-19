using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YaGamesSDK
{
    public class CloudSaves
    {
        [DllImport("__Internal")]
        private static extern void SaveGameExtern(string data);

        [DllImport("__Internal")]
        private static extern void LoadGameExtern();

        public static event Action OnDataLoaded;

        private static bool _isDataLoaded;
        private static string _data;

        public static bool IsDataLoaded
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return _isDataLoaded;
#endif
            }
        }

        public static string Data
        {
            get
            {
#if UNITY_EDITOR
                return PlayerPrefs.GetString("userData");
#else
                return _data;
#endif
            }
        }

        public static void LoadGame()
        {
#if !UNITY_EDITOR
            LoadGameExtern();
#endif
        }

        public static void SaveGame(string data)
        {
#if UNITY_EDITOR
            PlayerPrefs.SetString("userData", data);
#else
            SaveGameExtern(data);
#endif
        }

        public void DataLoaded(string data)
        {
            _isDataLoaded = true;
            _data = data;
            OnDataLoaded?.Invoke();
        }
    }
}
