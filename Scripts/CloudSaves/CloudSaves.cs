using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YaGamesSDK
{
    public class CloudSaves
    {
        public static event Action OnDataLoaded;

        public static bool IsDataLoaded { get; private set; }
        public static string Data { get; private set; }

        [DllImport("__Internal")]
        private static extern void SaveGameExtern(string data);

        [DllImport("__Internal")]
        private static extern void LoadGameExtern();

        public static void LoadGame()
        {
#if UNITY_EDITOR
            IsDataLoaded = true;
            Data = PlayerPrefs.GetString("userData");
            OnDataLoaded?.Invoke();
#else
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
            IsDataLoaded = true;
            Data = data;
            OnDataLoaded?.Invoke();
        }
    }
}
