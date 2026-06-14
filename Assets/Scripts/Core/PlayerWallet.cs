using UnityEngine;

namespace MagicPairs.Core
{
    public static class PlayerWallet
    {
        private const string CoinsKey = "MagicPairs_Coins";
        private static int _coins = -1;

        public static event System.Action<int> OnCoinsChanged;

        public static int Coins
        {
            get
            {
                if (_coins < 0) _coins = PlayerPrefs.GetInt(CoinsKey, 0);
                return _coins;
            }
            private set
            {
                _coins = Mathf.Max(0, value);
                PlayerPrefs.SetInt(CoinsKey, _coins);
                PlayerPrefs.Save();
                OnCoinsChanged?.Invoke(_coins);
            }
        }

        public static void Add(int amount)
        {
            if (amount <= 0) return;
            Coins += amount;
        }

        public static bool Spend(int amount)
        {
            if (amount <= 0 || Coins < amount) return false;
            Coins -= amount;
            return true;
        }

        public static bool CanAfford(int amount) => Coins >= amount;
    }
}
