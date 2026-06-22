using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;
using MagicPairs.GameFlow;

namespace MagicPairs.UI
{
    public class RateUsManager : MonoBehaviour
    {
        private const string GamesWonKey = "MagicPairs_GamesWon";
        private const string RatedKey = "MagicPairs_Rated";
        private const string DismissedKey = "MagicPairs_RateDismissed";
        private const int GamesBeforePrompt = 5;
        private const string StoreUrl = "https://play.google.com/store/apps/details?id=com.Mateusz_Bajak.MagicPairs";

        private GameObject _popup;

        private void OnEnable()
        {
            GameEvents.OnGameOver += OnGameOver;
            ChallengeMode.OnLevelComplete += OnLevelComplete;
            TimeAttackMode.OnTimeAttackComplete += OnTimeAttackComplete;
        }

        private void OnDisable()
        {
            GameEvents.OnGameOver -= OnGameOver;
            ChallengeMode.OnLevelComplete -= OnLevelComplete;
            TimeAttackMode.OnTimeAttackComplete -= OnTimeAttackComplete;
        }

        private void OnGameOver(int winnerIndex)
        {
            if (winnerIndex == 0) IncrementAndCheck();
        }

        private void OnLevelComplete(int _) => IncrementAndCheck();
        private void OnTimeAttackComplete(float _) => IncrementAndCheck();

        private void IncrementAndCheck()
        {
            if (PlayerPrefs.GetInt(RatedKey, 0) == 1) return;
            if (PlayerPrefs.GetInt(DismissedKey, 0) == 1) return;

            int won = PlayerPrefs.GetInt(GamesWonKey, 0) + 1;
            PlayerPrefs.SetInt(GamesWonKey, won);
            PlayerPrefs.Save();

            if (won == GamesBeforePrompt)
                ShowPopup();
        }

        private void ShowPopup()
        {
            var canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (canvas == null) return;

            _popup = new GameObject("RateUsPopup");
            _popup.transform.SetParent(canvas.transform, false);
            var bg = _popup.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.7f);
            var bgRect = _popup.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            _popup.transform.SetAsLastSibling();

            var panel = new GameObject("Panel");
            panel.transform.SetParent(_popup.transform, false);
            var panelImg = panel.AddComponent<Image>();
            panelImg.color = Color.white;
            panelImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            panelImg.type = Image.Type.Sliced;
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.35f);
            panelRect.anchorMax = new Vector2(0.9f, 0.65f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Title
            var title = UIFactory.CreateText("Title", "⭐⭐⭐⭐⭐", panel.transform,
                new Vector2(0.1f, 0.7f), new Vector2(0.9f, 0.95f), TextAnchor.MiddleCenter, 42);
            title.color = new Color(0.9f, 0.75f, 0.1f);

            // Message
            string msg = Localization.CurrentLanguage == Language.Polish
                ? "Podoba Ci się gra?\nOceń nas w Google Play!"
                : "Enjoying the game?\nRate us on Google Play!";
            var msgText = UIFactory.CreateText("Msg", msg, panel.transform,
                new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.7f), TextAnchor.MiddleCenter, 28);
            msgText.color = new Color(0.2f, 0.2f, 0.2f);

            // Rate button
            var rateBtn = UIFactory.CreateIconButton("RateBtn", "scores", panel.transform,
                new Vector2(0.55f, 0.08f), new Vector2(0.9f, 0.38f));
            rateBtn.onClick.AddListener(OnRate);

            // Later button
            var laterBtn = UIFactory.CreateIconButton("LaterBtn", "close", panel.transform,
                new Vector2(0.1f, 0.08f), new Vector2(0.45f, 0.38f));
            laterBtn.onClick.AddListener(OnLater);
        }

        private void OnRate()
        {
            PlayerPrefs.SetInt(RatedKey, 1);
            PlayerPrefs.Save();
            Application.OpenURL(StoreUrl);
            if (_popup != null) Destroy(_popup);
        }

        private void OnLater()
        {
            PlayerPrefs.SetInt(DismissedKey, 1);
            PlayerPrefs.Save();
            if (_popup != null) Destroy(_popup);
        }
    }
}
