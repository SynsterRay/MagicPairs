using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;
using MagicPairs.GameFlow;

namespace MagicPairs.UI
{
    public class DailyBonus : MonoBehaviour
    {
        private const string LastClaimKey = "MagicPairs_DailyLastClaim";
        private const string StreakKey = "MagicPairs_DailyStreak";

        private GameObject _popup;
        private Text _titleText;
        private Text _rewardText;
        private Text _streakText;
        private Button _claimButton;

        private int _streak;
        private PowerUpType _todayReward;

        private void Start()
        {
            CheckDailyBonus();
        }

        private void CheckDailyBonus()
        {
            string lastClaim = PlayerPrefs.GetString(LastClaimKey, "");
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");

            if (lastClaim == today) return; // Already claimed today

            // Check streak
            if (!string.IsNullOrEmpty(lastClaim))
            {
                var lastDate = System.DateTime.Parse(lastClaim);
                var diff = (System.DateTime.Now.Date - lastDate.Date).Days;
                _streak = diff == 1 ? PlayerPrefs.GetInt(StreakKey, 0) + 1 : 1;
            }
            else
            {
                _streak = 1;
            }

            // Determine reward: rotate Peek → Shuffle → Freeze
            _todayReward = (PowerUpType)((_streak - 1) % 3);

            ShowPopup();
        }

        private void ShowPopup()
        {
            var canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (canvas == null) return;

            _popup = new GameObject("DailyBonusPopup");
            _popup.transform.SetParent(canvas.transform, false);

            // Background overlay
            var bg = _popup.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.7f);
            var bgRect = _popup.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Panel
            var panel = new GameObject("Panel");
            panel.transform.SetParent(_popup.transform, false);
            var panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(1f, 1f, 1f, 1f);
            panelImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            panelImg.type = Image.Type.Sliced;
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.12f, 0.3f);
            panelRect.anchorMax = new Vector2(0.88f, 0.7f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Title
            _titleText = CreateText("Title", Localization.Get("dailyBonus"), panel.transform,
                new Vector2(0.1f, 0.7f), new Vector2(0.9f, 0.95f), 32);
            _titleText.color = new Color(0.2f, 0.2f, 0.2f);

            // Power-up icon + name
            string puIconName = _todayReward switch
            {
                PowerUpType.Peek => "peek",
                PowerUpType.Shuffle => "shuffle",
                PowerUpType.Freeze => "freeze",
                _ => "peek"
            };
            string puName = _todayReward switch
            {
                PowerUpType.Peek => Localization.Get("peek"),
                PowerUpType.Shuffle => Localization.Get("shuffle"),
                PowerUpType.Freeze => Localization.Get("freeze"),
                _ => ""
            };
            int rewardCount = _streak >= 7 ? 2 : 1;
            int coinReward = Mathf.Min(_streak * 10, 70);

            var puIcon = new GameObject("PUIcon");
            puIcon.transform.SetParent(panel.transform, false);
            var puImg = puIcon.AddComponent<Image>();
            puImg.sprite = UIIcons.Get(puIconName);
            puImg.preserveAspect = true;
            puImg.raycastTarget = false;
            var puRect = puIcon.GetComponent<RectTransform>();
            puRect.anchorMin = new Vector2(0.08f, 0.52f);
            puRect.anchorMax = new Vector2(0.32f, 0.72f);
            puRect.offsetMin = Vector2.zero;
            puRect.offsetMax = Vector2.zero;

            string puLabel = rewardCount > 1 ? $"{puName} x{rewardCount}" : puName;
            _rewardText = CreateText("PUName", puLabel, panel.transform,
                new Vector2(0.34f, 0.52f), new Vector2(0.92f, 0.72f), 38);
            _rewardText.color = new Color(0.1f, 0.65f, 0.2f);
            _rewardText.alignment = TextAnchor.MiddleLeft;

            // Coins icon + gold amount text
            var coinsIcon = new GameObject("CoinsIcon");
            coinsIcon.transform.SetParent(panel.transform, false);
            var ciImg = coinsIcon.AddComponent<Image>();
            ciImg.sprite = UIIcons.Get("coins");
            ciImg.preserveAspect = true;
            ciImg.raycastTarget = false;
            var ciRect = coinsIcon.GetComponent<RectTransform>();
            ciRect.anchorMin = new Vector2(0.08f, 0.30f);
            ciRect.anchorMax = new Vector2(0.32f, 0.50f);
            ciRect.offsetMin = Vector2.zero;
            ciRect.offsetMax = Vector2.zero;

            var coinText = CreateText("CoinAmount", $"+{coinReward}", panel.transform,
                new Vector2(0.34f, 0.30f), new Vector2(0.92f, 0.50f), 30);
            coinText.color = new Color(0.85f, 0.65f, 0.1f);
            coinText.alignment = TextAnchor.MiddleLeft;

            // Streak info
            string streakInfo = Localization.Get("dailyStreak", _streak);
            _streakText = CreateText("Streak", streakInfo, panel.transform,
                new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.4f), 22);
            _streakText.color = new Color(0.5f, 0.5f, 0.5f);

            // Claim button (icon)
            var claimGo = new GameObject("ClaimBtn");
            claimGo.transform.SetParent(panel.transform, false);
            var claimImg = claimGo.AddComponent<Image>();
            claimImg.sprite = UIIcons.Get("claim");
            claimImg.preserveAspect = true;
            claimImg.color = Color.white;
            _claimButton = claimGo.AddComponent<Button>();
            _claimButton.transition = Selectable.Transition.None;
            var claimRect = claimGo.GetComponent<RectTransform>();
            claimRect.anchorMin = new Vector2(0.3f, 0.03f);
            claimRect.anchorMax = new Vector2(0.7f, 0.28f);
            claimRect.offsetMin = Vector2.zero;
            claimRect.offsetMax = Vector2.zero;

            var btnText = CreateText("BtnText", Localization.Get("claim"), claimGo.transform,
                new Vector2(0f, 0f), new Vector2(1f, 1f), 28);
            btnText.color = Color.white;

            _claimButton.onClick.AddListener(ClaimReward);
        }

        private void ClaimReward()
        {
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            PlayerPrefs.SetString(LastClaimKey, today);
            PlayerPrefs.SetInt(StreakKey, _streak);
            PlayerPrefs.Save();

            // Grant coins: 10 × streak day (10, 20, 30... max 70)
            int coinReward = Mathf.Min(_streak * 10, 70);
            PlayerWallet.Add(coinReward);

            // Grant power-up(s) — store in PlayerPrefs so they persist until Challenge starts
            int count = _streak >= 7 ? 2 : 1;
            for (int i = 0; i < count; i++)
            {
                var storeType = _todayReward switch
                {
                    PowerUpType.Peek => Core.ShopItemType.PowerUpPeek,
                    PowerUpType.Shuffle => Core.ShopItemType.PowerUpShuffle,
                    PowerUpType.Freeze => Core.ShopItemType.PowerUpFreeze,
                    _ => Core.ShopItemType.PowerUpPeek
                };
                string key = $"MagicPairs_Shop_{storeType}";
                int current = PlayerPrefs.GetInt(key, 0);
                PlayerPrefs.SetInt(key, current + 1);
            }
            PlayerPrefs.Save();

            GPGSManager.Instance?.EventDailyBonus();

            if (_popup != null) Destroy(_popup);
        }

        private Text CreateText(string name, string text, Transform parent, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
            return txt;
        }
    }
}
