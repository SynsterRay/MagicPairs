using UnityEngine;
using System.Collections.Generic;

namespace MagicPairs.Core
{
    public enum ShopItemType { CardTheme, PowerUpPeek, PowerUpShuffle, PowerUpFreeze, CoinPack }

    [System.Serializable]
    public class ShopItem
    {
        public string id;
        public ShopItemType type;
        public int coinPrice;       // 0 = real money (IAP)
        public string iapProductId; // null = coins only
        public CardTheme? theme;    // for card themes
        public int quantity;        // for power-ups / coin packs
    }

    public static class ShopCatalog
    {
        private const string UnlockedKey = "MagicPairs_UnlockedThemes";

        public static readonly List<ShopItem> Items = new()
        {
            // Card themes (expensive — encourage IAP)
            new() { id = "theme_animals", type = ShopItemType.CardTheme, coinPrice = 500, theme = (CardTheme)3, quantity = 1 },
            new() { id = "theme_water_world", type = ShopItemType.CardTheme, coinPrice = 800, theme = (CardTheme)4, quantity = 1 },
            new() { id = "theme_space", type = ShopItemType.CardTheme, coinPrice = 1200, theme = (CardTheme)5, quantity = 1 },

            // Power-ups (moderate)
            new() { id = "peek_3", type = ShopItemType.PowerUpPeek, coinPrice = 80, quantity = 3 },
            new() { id = "shuffle_3", type = ShopItemType.PowerUpShuffle, coinPrice = 100, quantity = 3 },
            new() { id = "freeze_3", type = ShopItemType.PowerUpFreeze, coinPrice = 120, quantity = 3 },

            // Coin packs (IAP — prices in coins represent value, purchase via real money)
            new() { id = "coins_100", type = ShopItemType.CoinPack, coinPrice = 0, iapProductId = "com.magicpairs.coins100", quantity = 100 },
            new() { id = "coins_500", type = ShopItemType.CoinPack, coinPrice = 0, iapProductId = "com.magicpairs.coins500", quantity = 500 },
            new() { id = "coins_1500", type = ShopItemType.CoinPack, coinPrice = 0, iapProductId = "com.magicpairs.coins1500", quantity = 1500 },
        };

        public static bool IsThemeUnlocked(CardTheme theme)
        {
            // Base themes always unlocked
            if (theme == CardTheme.Colors || theme == CardTheme.Princess || theme == CardTheme.Cars)
                return true;
            int unlocked = PlayerPrefs.GetInt(UnlockedKey, 0);
            return (unlocked & (1 << (int)theme)) != 0;
        }

        public static void UnlockTheme(CardTheme theme)
        {
            int unlocked = PlayerPrefs.GetInt(UnlockedKey, 0);
            unlocked |= (1 << (int)theme);
            PlayerPrefs.SetInt(UnlockedKey, unlocked);
            PlayerPrefs.Save();
        }

        public static bool TryPurchase(ShopItem item)
        {
            if (item.type == ShopItemType.CoinPack)
                return false; // IAP handled separately

            if (!PlayerWallet.CanAfford(item.coinPrice))
                return false;

            PlayerWallet.Spend(item.coinPrice);

            switch (item.type)
            {
                case ShopItemType.CardTheme:
                    if (item.theme.HasValue) UnlockTheme(item.theme.Value);
                    break;
                case ShopItemType.PowerUpPeek:
                case ShopItemType.PowerUpShuffle:
                case ShopItemType.PowerUpFreeze:
                    // Power-ups stored in PlayerPrefs for persistence
                    string key = $"MagicPairs_Shop_{item.type}";
                    int current = PlayerPrefs.GetInt(key, 0);
                    PlayerPrefs.SetInt(key, current + item.quantity);
                    PlayerPrefs.Save();
                    break;
            }

            return true;
        }

        public static int GetStoredPowerUps(ShopItemType type)
        {
            string key = $"MagicPairs_Shop_{type}";
            return PlayerPrefs.GetInt(key, 0);
        }

        public static void ConsumeStoredPowerUp(ShopItemType type)
        {
            string key = $"MagicPairs_Shop_{type}";
            int current = PlayerPrefs.GetInt(key, 0);
            if (current > 0)
            {
                PlayerPrefs.SetInt(key, current - 1);
                PlayerPrefs.Save();
            }
        }
    }
}
