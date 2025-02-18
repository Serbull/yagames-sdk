
namespace YaGamesSDK
{
    public class LeaderboardData
    {
        public class Leaderboard
        {
            public string name;
        }

        public class Entries
        {
            public class Player
            {
                public string publicName;
            }

            public int rank;
            public int score;
            public Player player;
        }

        public float lastLoadTime;

        public Leaderboard leaderboard;
        public Entries[] entries;
        public int userRank;
    }
}
