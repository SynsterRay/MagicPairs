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
            panelImg.color = new Color(1f, 1f, 1f, 0.97f);
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

            // Reward icon/text
            string rewardName = _todayReward switch
            {
                PowerUpType.Peek => $"🔍 {Localization.Get("peek")}",
                PowerUpType.Shuffle => $"🔄 {Localization.Get("shuffle")}",
                PowerUpType.Freeze => $"❄️ {Localization.Get("freeze")}",
                _ => ""
            };
            int rewardCount = _streak >= 7 ? 2 : 1;
            string rewardDisplay = rewardCount > 1 ? $"{rewardName} x{rewardCount}" : rewardName;

            _rewardText = CreateText("Reward", rewardDisplay, panel.transform,
                new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.7f), 36);
            _rewardText.color = new Color(0.1f, 0.5f, 0.2f);

            // Streak info
            string streakInfo = Localization.Get("dailyStreak", _streak);
            _streakText = CreateText("Streak", streakInfo, panel.transform,
                new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.4f), 22);
            _streakText.color = new Color(0.5f, 0.5f, 0.5f);

            // Claim button
            var claimGo = new GameObject("ClaimBtn");
            claimGo.transform.SetParent(panel.transform, false);
            var claimImg = claimGo.AddComponent<Image>();
            claimImg.color = new Color(0.1f, 0.7f, 0.3f, 1f);
            claimImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            claimImg.type = Image.Type.Sliced;
            _claimButton = claimGo.AddComponent<Button>();
            var claimRect = claimGo.GetComponent<RectTransform>();
            claimRect.anchorMin = new Vector2(0.2f, 0.05f);
            claimRect.anchorMax = new Vector2(0.8f, 0.23f);
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

            // Grant power-up(s)
            var powerUp = FindAnyObjectByType<PowerUpManager>();
            if (powerUp != null)
            {
                int count = _streak >= 7 ? 2 : 1;
                for (int i = 0; i < count; i++)
                    powerUp.AddPowerUp(_todayReward);
            }

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
