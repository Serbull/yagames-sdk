
namespace YaGamesSDK
{
    public class LeaderboardData
    {
        public class Leaderboard
        {
            public string name;
        }

        public class Entry
        {
            public class Player
            {
                public string publicName;
            }

            public int rank;
            public int score;
            public string formattedScore;
            public Player player;
            public string avatarUrl;
            public string extraData;
        }

        public float lastFetchTime;

        public Leaderboard leaderboard;
        public Entry[] entries;
        public int userRank;
    }
}
