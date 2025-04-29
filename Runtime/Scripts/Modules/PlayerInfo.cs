using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace YaGamesSDK
{
    public class PlayerInfo
    {
        public enum AvatarSize { Small, Medium, Large }

        [DllImport("__Internal")]
        private static extern string GetPlayerInfoExtern();

        public struct PlayerData
        {
            public string name;
            public string id;
            public string avatarUrl;
        }

        //public static PlayerData GetPlayerData(AvatarSize avatarSize = AvatarSize.Medium)
        public static PlayerData GetPlayerData()
        {
#if UNITY_EDITOR
            return new PlayerData();
#else
            //var json = GetPlayerInfoExtern(avatarSize.ToString().ToLower());
            var json = GetPlayerInfoExtern();
            return JsonConvert.DeserializeObject<PlayerData>(json);
#endif
        }
    }
}
