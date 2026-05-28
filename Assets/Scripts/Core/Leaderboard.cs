using UnityEngine;
using System.Collections.Generic;

namespace MagicPairs.Core
{
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public int level;
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries = new();
    }

    public static class Leaderboard
    {
        private const string SaveKey = "MagicPairs_Leaderboard";
        private const int MaxEntries = 10;

        private static LeaderboardData _data;

        public static IReadOnlyList<LeaderboardEntry> Entries
        {
            get
            {
                Load();
                return _data.entries;
            }
        }

        public static void AddEntry(string name, int score, int level)
        {
            Load();
            _data.entries.Add(new LeaderboardEntry { playerName = name, score = score, level = level });
            _data.entries.Sort((a, b) => b.score.CompareTo(a.score));
            if (_data.entries.Count > MaxEntries)
                _data.entries.RemoveRange(MaxEntries, _data.entries.Count - MaxEntries);
            Save();
        }

        public static bool IsHighScore(int score)
        {
            Load();
            return _data.entries.Count < MaxEntries || score > _data.entries[_data.entries.Count - 1].score;
        }

        private static void Load()
        {
            if (_data != null) return;
            string json = PlayerPrefs.GetString(SaveKey, "");
            _data = string.IsNullOrEmpty(json) ? new LeaderboardData() : JsonUtility.FromJson<LeaderboardData>(json);
            if (_data == null) _data = new LeaderboardData();
        }

        private static void Save()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_data));
            PlayerPrefs.Save();
        }
    }
}
