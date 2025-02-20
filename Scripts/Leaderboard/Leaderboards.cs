using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace YaGamesSDK
{
    public class Leaderboards
    {
        private static readonly List<LeaderboardData> _leaderboardDatas = new();

        public static event Action<string> OnLeaderboardLoaded;

        [DllImport("__Internal")]
        private static extern void SetLeaderboardScoreExtern(string leaderboardName, int score, string extraData);

        [DllImport("__Internal")]
        private static extern void LoadLeaderboardExtern(string leaderboardName, bool includeUser, int quantityAround, int quantityTop);

        public static void SetScore(string leaderboardName, int score, string extraData = null)
        {
            YaGames.Log($"Set Leaderboard: '{leaderboardName}' score: {score} extraData: '{extraData}'");
#if !UNITY_EDITOR
            SetLeaderboardScoreExtern(leaderboardName, score, extraData);
#endif
        }

        public static void Fetch(string leaderboardName, bool includeUser = true, int quantityAround = 10, int quantityTop = 10)
        {
            YaGames.Log($"Fetch Leaderboard: '{leaderboardName}'");
#if !UNITY_EDITOR
            LoadLeaderboardExtern(leaderboardName, includeUser, quantityAround, quantityTop);
#endif
        }

        public void LeaderboardLoaded(string data)
        {
            var dataClass = JsonConvert.DeserializeObject<LeaderboardData>(data);
            var leaderboardId = GetLeaderboadId(dataClass.leaderboard.name);
            if (leaderboardId == -1)
            {
                _leaderboardDatas.Add(dataClass);
            }

            dataClass.lastFetchTime = Time.unscaledTime;
            OnLeaderboardLoaded?.Invoke(dataClass.leaderboard.name);
        }

        private static int GetLeaderboadId(string leaderboardName)
        {
            for (int i = 0; i < _leaderboardDatas.Count; i++)
            {
                if (_leaderboardDatas[i].leaderboard.name == leaderboardName)
                {
                    return i;
                }
            }

            return -1;
        }

        public static LeaderboardData GetData(string leaderboardName)
        {
            foreach (var data in _leaderboardDatas)
            {
                if (data.leaderboard.name == leaderboardName)
                {
                    return data;
                }
            }

            return null;
        }

        public static bool IsLoaded(string leaderboardName)
        {
            return GetData(leaderboardName) != null;
        }
    }
}
