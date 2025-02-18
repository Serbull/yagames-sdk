using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace YaGamesSDK
{
    public interface ILeaderboards
    {
        void LeaderboardLoaded(string data);
    }

    public class Leaderboards : ILeaderboards
    {
        private static readonly List<LeaderboardData> _leaderboardDatas = new();

        public event Action<string> OnLeaderboardLoaded;

        [DllImport("__Internal")]
        private static extern void SetLeaderboardScoreExtern(string leaderboardName, int score);

        [DllImport("__Internal")]
        private static extern void LoadLeaderboardExtern(string leaderboardName, bool includeUser, int quantityAround, int quantityTop);

        public void SetScore(string leaderboardName, int score)
        {
            YaGames.Log($"Set Leaderboard: '{leaderboardName}' score: {score}");
#if !UNITY_EDITOR
        SetLeaderboardScoreExtern(leaderboardName, score);
#endif
        }

        public void Load(string leaderboardName, bool includeUser = true, int quantityAround = 10, int quantityTop = 10)
        {
            YaGames.Log($"Load Leaderboard: '{leaderboardName}'");
#if !UNITY_EDITOR
        LoadLeaderboardExtern(leaderboardName, includeUser, quantityAround, quantityTop);
#endif
        }

        void ILeaderboards.LeaderboardLoaded(string data)
        {
            var dataClass = JsonConvert.DeserializeObject<LeaderboardData>(data);
            var leaderboardId = GetLeaderboadId(dataClass.leaderboard.name);
            if (leaderboardId == -1)
            {
                _leaderboardDatas.Add(dataClass);
            }

            dataClass.lastLoadTime = Time.unscaledTime;
            OnLeaderboardLoaded?.Invoke(dataClass.leaderboard.name);
        }

        private int GetLeaderboadId(string leaderboardName)
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

        public LeaderboardData GetData(string leaderboardName)
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

        public bool IsLoaded(string leaderboardName)
        {
            return GetData(leaderboardName) != null;
        }
    }
}
