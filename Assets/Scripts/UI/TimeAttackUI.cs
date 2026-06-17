using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;
using MagicPairs.GameFlow;

namespace MagicPairs.UI
{
    public class TimeAttackUI : MonoBehaviour
    {
        private GameObject _panel;
        private Text _timerText;
        private Text _pairsText;
        private GameObject _resultPanel;
        private Text _resultText;
        private Button _menuButton;
        private Button _retryButton;

        private void Start()
        {
            CreateUI();
            HideAll();
        }

        private void OnEnable()
        {
            TimeAttackMode.OnTimeChanged += UpdateTimer;
            TimeAttackMode.OnPairsChanged += UpdatePairs;
            TimeAttackMode.OnTimeAttackComplete += ShowWin;
            TimeAttackMode.OnTimeUp += ShowLose;
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnPairMatched += OnMatch;
            GameEvents.OnPairMismatched += OnMismatch;
            GameEvents.OnPiotrusFlipped += OnJoker;
        }

        private void OnDisable()
        {
            TimeAttackMode.OnTimeChanged -= UpdateTimer;
            TimeAttackMode.OnPairsChanged -= UpdatePairs;
            TimeAttackMode.OnTimeAttackComplete -= ShowWin;
            TimeAttackMode.OnTimeUp -= ShowLose;
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnPairMatched -= OnMatch;
            GameEvents.OnPairMismatched -= OnMismatch;
            GameEvents.OnPiotrusFlipped -= OnJoker;
        }

        private void CreateUI()
        {
            var canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (canvas == null) return;

            // Timer panel (top)
            _panel = new GameObject("TimeAttackPanel");
            _panel.transform.SetParent(canvas.transform, false);
            var rect = _panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.85f);
            rect.anchorMax = new Vector2(0.9f, 0.93f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            _timerText = CreateText("TimerText", "60.0", _panel.transform,
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), 36);
            _timerText.color = new Color(0.15f, 0.15f, 0.15f);
            _pairsText = CreateText("PairsText", "0/0", _panel.transform,
                new Vector2(0.5f, 0f), new Vector2(1f, 1f), 28);
            _pairsText.color = new Color(0.15f, 0.15f, 0.15f);

            // Result panel
            _resultPanel = new GameObject("TimeAttackResult");
            _resultPanel.transform.SetParent(canvas.transform, false);
            var rpImg = _resultPanel.AddComponent<Image>();
            rpImg.color = new Color(1f, 1f, 1f, 1f);
            var rpRect = _resultPanel.GetComponent<RectTransform>();
            rpRect.anchorMin = new Vector2(0.1f, 0.3f);
            rpRect.anchorMax = new Vector2(0.9f, 0.7f);
            rpRect.offsetMin = Vector2.zero;
            rpRect.offsetMax = Vector2.zero;

            _resultText = CreateText("ResultText", "", _resultPanel.transform,
                new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.9f), 30);
            _resultText.color = new Color(0.15f, 0.15f, 0.15f);

            _menuButton = CreateButton("MenuBtn", "Menu", _resultPanel.transform,
                new Vector2(0.1f, 0.1f), new Vector2(0.45f, 0.4f));
            _menuButton.onClick.AddListener(OnMenu);

            _retryButton = CreateButton("RetryBtn", Localization.Get("playAgain"), _resultPanel.transform,
                new Vector2(0.55f, 0.1f), new Vector2(0.9f, 0.4f));
            _retryButton.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.3f, 1f);
            _retryButton.onClick.AddListener(OnRetry);

            _resultPanel.SetActive(false);
        }

        private void OnGameStarted()
        {
            if (!TimeAttackMode.IsTimeAttackMode) { HideAll(); return; }
            if (_panel != null) _panel.SetActive(true);
            if (_resultPanel != null) _resultPanel.SetActive(false);
        }

        private void HideAll()
        {
            if (_panel != null) _panel.SetActive(false);
            if (_resultPanel != null) _resultPanel.SetActive(false);
        }

        private float _lastDisplayedTime = -1f;

        private void UpdateTimer(float time)
        {
            if (_timerText == null) return;
            // Only update text when display value changes (avoid per-frame string alloc)
            float rounded = Mathf.Round(time * 10f) / 10f;
            if (rounded == _lastDisplayedTime) return;
            _lastDisplayedTime = rounded;
            _timerText.text = $"⏱ {time:F1}s";
            _timerText.color = time < 10f ? Color.red : new Color(0.15f, 0.15f, 0.15f);
        }

        private void UpdatePairs(int found, int total)
        {
            if (_pairsText != null)
                _pairsText.text = $"{found}/{total}";
        }

        private void OnMatch(int _, int __) => SpawnTimePopup("+2s", new Color(0.2f, 0.9f, 0.3f));
        private void OnMismatch() => SpawnTimePopup("-3s", new Color(0.9f, 0.2f, 0.2f));
        private void OnJoker(int _) => SpawnTimePopup("-6s", new Color(0.9f, 0.2f, 0.2f));

        private void SpawnTimePopup(string text, Color color)
        {
            if (!TimeAttackMode.IsTimeAttackMode) return;
            var canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (canvas == null) return;

            var go = new GameObject("TimePopup");
            go.transform.SetParent(canvas.transform, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = 44;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = color;
            txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f, 0.45f);
            rect.anchorMax = new Vector2(0.7f, 0.55f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            StartCoroutine(AnimatePopup(go, txt));
        }

        private System.Collections.IEnumerator AnimatePopup(GameObject go, Text txt)
        {
            var rect = go.GetComponent<RectTransform>();
            float t = 0f;
            Vector2 start = rect.anchoredPosition;
            while (t < 1f)
            {
                t += Time.deltaTime;
                rect.anchoredPosition = start + Vector2.up * (t * 60f);
                if (t > 0.6f)
                {
                    var c = txt.color;
                    c.a = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                    txt.color = c;
                }
                yield return null;
            }
            Destroy(go);
        }

        private void ShowWin(float timeLeft)
        {
            if (_resultPanel != null) _resultPanel.SetActive(true);
            if (_resultText != null)
                _resultText.text = Localization.Get("timeAttackWin", timeLeft);

            string diff = MainMenu.SelectedDifficulty.ToString();
            Leaderboard.AddTimeAttackEntry(MainMenu.Player1Name, timeLeft, diff);
            GPGSManager.Instance?.PostTimeAttackScore(timeLeft);
        }

        private void ShowLose()
        {
            if (_resultPanel != null) _resultPanel.SetActive(true);
            if (_resultText != null)
                _resultText.text = Localization.Get("timeAttackLose");
        }

        private void OnMenu()
        {
            HideAll();
            var adManager = FindAnyObjectByType<Ads.AdManager>();
            adManager?.TryShowInterstitialBetweenGames();
            var menu = FindAnyObjectByType<MainMenu>();
            menu?.ReturnToMenu();
        }

        private void OnRetry()
        {
            if (_resultPanel != null) _resultPanel.SetActive(false);
            GameManager.Instance.StartGame();
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

        private Button CreateButton(string name, string label, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.4f, 1f);
            img.sprite = RoundedButtonHelper.GetRoundedSprite();
            img.type = Image.Type.Sliced;
            go.AddComponent<Button>();
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;

            // Shine overlay
            var shine = new GameObject("Shine");
            shine.transform.SetParent(go.transform, false);
            var shineImg = shine.AddComponent<Image>();
            shineImg.sprite = RoundedButtonHelper.GetShineSprite();
            shineImg.type = Image.Type.Sliced;
            shineImg.color = Color.white;
            shineImg.raycastTarget = false;
            var shineRect = shine.GetComponent<RectTransform>();
            shineRect.anchorMin = Vector2.zero;
            shineRect.anchorMax = Vector2.one;
            shineRect.offsetMin = Vector2.zero;
            shineRect.offsetMax = Vector2.zero;

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = 28;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var tr = txtObj.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;

            return go.GetComponent<Button>();
        }
    }
}
