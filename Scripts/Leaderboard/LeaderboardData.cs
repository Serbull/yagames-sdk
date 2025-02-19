
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
            public Player player;
            public string avatarUrl;
        }

        public float lastFetchTime;

        public Leaderboard leaderboard;
        public Entry[] entries;
        public int userRank;
    }
}
