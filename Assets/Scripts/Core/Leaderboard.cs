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
    public class TimeAttackEntry
    {
        public string playerName;
        public float timeLeft;
        public string difficulty;
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries = new();
    }

    [System.Serializable]
    public class TimeAttackData
    {
        public List<TimeAttackEntry> entries = new();
    }

    public static class Leaderboard
    {
        private const string SaveKey = "MagicPairs_Leaderboard";
        private const string TimeAttackKey = "MagicPairs_TimeAttack";
        private const int MaxEntries = 10;

        private static LeaderboardData _data;
        private static TimeAttackData _taData;

        public static IReadOnlyList<LeaderboardEntry> Entries
        {
            get
            {
                Load();
                return _data.entries;
            }
        }

        public static IReadOnlyList<TimeAttackEntry> TimeAttackEntries
        {
            get
            {
                LoadTimeAttack();
                return _taData.entries;
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

        public static void AddTimeAttackEntry(string name, float timeLeft, string difficulty)
        {
            LoadTimeAttack();
            _taData.entries.Add(new TimeAttackEntry { playerName = name, timeLeft = timeLeft, difficulty = difficulty });
            _taData.entries.Sort((a, b) => b.timeLeft.CompareTo(a.timeLeft)); // More time left = better
            if (_taData.entries.Count > MaxEntries)
                _taData.entries.RemoveRange(MaxEntries, _taData.entries.Count - MaxEntries);
            SaveTimeAttack();
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

        private static void LoadTimeAttack()
        {
            if (_taData != null) return;
            string json = PlayerPrefs.GetString(TimeAttackKey, "");
            _taData = string.IsNullOrEmpty(json) ? new TimeAttackData() : JsonUtility.FromJson<TimeAttackData>(json);
            if (_taData == null) _taData = new TimeAttackData();
        }

        private static void Save()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_data));
            PlayerPrefs.Save();
        }

        private static void SaveTimeAttack()
        {
            PlayerPrefs.SetString(TimeAttackKey, JsonUtility.ToJson(_taData));
            PlayerPrefs.Save();
        }
    }
}
