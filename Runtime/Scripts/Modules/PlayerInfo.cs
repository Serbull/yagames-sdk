using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace YaGamesSDK
{
    public class PlayerInfo
    {
        [DllImport("__Internal")]
        private static extern string GetPlayerInfoExtern();

        public struct PlayerData
        {
            public string name;
            public string id;
        }

        public static PlayerData GetPlayerData()
        {
#if UNITY_EDITOR
            return new PlayerData();
#else
            var json = GetPlayerInfoExtern();
            return JsonConvert.DeserializeObject<PlayerData>(json);
#endif
        }
    }
}
