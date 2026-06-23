using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MagicPairs.UI
{
    public class TutorialOverlay : MonoBehaviour
    {
        private const string ShownKey = "MagicPairs_TutorialShown";

        private GameObject _panel;
        private Canvas _canvas;
        private int _currentPage;
        private Image[] _dots;
        private GameObject _contentArea;

        private static readonly string[] Titles = {
            "Match the pairs!",
            "Watch out for the Joker!",
            "Game Modes",
            "Power-ups"
        };

        private static readonly string[] Descriptions = {
            "Flip cards and find matching pairs.\nRemember what you see!",
            "The Joker card makes you lose your turn.\nAvoid it!",
            "Play with a friend, take on the Challenge,\nor race against time!",
            "Earn Peek, Shuffle & Freeze\nin Challenge mode!"
        };

        private static readonly string[][] Icons = {
            new[] { "cars", "cars" },
            new[] { "challenge" },
            new[] { "arcade", "challenge", "time attack" },
            new[] { "peek_game", "shuffle_game", "freeze_game" }
        };

        private void Start()
        {
            _canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (PlayerPrefs.GetInt(ShownKey, 0) == 1) { Destroy(this); return; }
            StartCoroutine(ShowAfterSplash());
        }

        private IEnumerator ShowAfterSplash()
        {
            // Wait for splash screen to finish
            yield return new WaitForSeconds(2.5f);
            Show();
        }

        public void Show()
        {
            if (_canvas == null) return;
            if (_panel != null) Destroy(_panel);

            _currentPage = 0;
            CreatePanel();
            ShowPage(0);
        }

        private void CreatePanel()
        {
            _panel = new GameObject("TutorialPanel");
            _panel.transform.SetParent(_canvas.transform, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = Color.white;
            var rect = _panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            _panel.transform.SetAsLastSibling();

            // Content area
            _contentArea = new GameObject("Content");
            _contentArea.transform.SetParent(_panel.transform, false);
            var cRect = _contentArea.AddComponent<RectTransform>();
            cRect.anchorMin = new Vector2(0.05f, 0.25f);
            cRect.anchorMax = new Vector2(0.95f, 0.85f);
            cRect.offsetMin = Vector2.zero;
            cRect.offsetMax = Vector2.zero;

            // Dots indicator
            var dotsGo = new GameObject("Dots");
            dotsGo.transform.SetParent(_panel.transform, false);
            var dotsRect = dotsGo.AddComponent<RectTransform>();
            dotsRect.anchorMin = new Vector2(0.3f, 0.18f);
            dotsRect.anchorMax = new Vector2(0.7f, 0.22f);
            dotsRect.offsetMin = Vector2.zero;
            dotsRect.offsetMax = Vector2.zero;

            _dots = new Image[4];
            for (int i = 0; i < 4; i++)
            {
                var dot = new GameObject($"Dot{i}");
                dot.transform.SetParent(dotsGo.transform, false);
                var dImg = dot.AddComponent<Image>();
                dImg.color = new Color(0.3f, 0.1f, 0.5f, 0.3f);
                dImg.sprite = RoundedButtonHelper.GetRoundedSprite();
                dImg.type = Image.Type.Sliced;
                var dRect = dot.GetComponent<RectTransform>();
                float x = (float)i / 4f + 0.02f;
                dRect.anchorMin = new Vector2(x, 0.1f);
                dRect.anchorMax = new Vector2(x + 0.2f, 0.9f);
                dRect.offsetMin = Vector2.zero;
                dRect.offsetMax = Vector2.zero;
                _dots[i] = dImg;
            }

            // Next button
            var nextBtn = UIFactory.CreateIconButton("NextBtn", "play", _panel.transform,
                new Vector2(0.3f, 0.05f), new Vector2(0.7f, 0.16f));
            nextBtn.onClick.AddListener(NextPage);

            // Skip button
            var skipTxt = UIFactory.CreateText("SkipBtn", "Skip", _panel.transform,
                new Vector2(0.75f, 0.9f), new Vector2(0.95f, 0.97f), TextAnchor.MiddleRight, 24);
            skipTxt.color = new Color(0.5f, 0.3f, 0.7f, 0.7f);
            var skipBtn = skipTxt.gameObject.AddComponent<Button>();
            skipBtn.transition = Selectable.Transition.None;
            skipBtn.onClick.AddListener(Close);
        }

        private void ShowPage(int page)
        {
            // Clear content
            foreach (Transform child in _contentArea.transform)
                Destroy(child.gameObject);

            // Update dots
            for (int i = 0; i < _dots.Length; i++)
                _dots[i].color = i == page ? new Color(0.3f, 0.1f, 0.5f) : new Color(0.3f, 0.1f, 0.5f, 0.3f);

            // Title
            var title = UIFactory.CreateText("Title", Titles[page], _contentArea.transform,
                new Vector2(0f, 0.75f), new Vector2(1f, 0.95f), TextAnchor.MiddleCenter, 42);
            title.color = new Color(0.3f, 0.1f, 0.5f);

            // Icons row
            var icons = Icons[page];
            for (int i = 0; i < icons.Length; i++)
            {
                var ico = new GameObject($"Icon{i}");
                ico.transform.SetParent(_contentArea.transform, false);
                var img = ico.AddComponent<Image>();
                if (page == 0)
                {
                    // Load a matching pair card from resources
                    var cardSpr = Resources.Load<Sprite>("SpaceAnimalsCards/1");
                    if (cardSpr == null)
                    {
                        var tex = Resources.Load<Texture2D>("SpaceAnimalsCards/1");
                        if (tex != null) cardSpr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                    }
                    img.sprite = cardSpr;
                }
                else if (page == 1 && i == 0)
                {
                    // Load joker card from resources
                    var jokerSpr = Resources.Load<Sprite>("SpaceAnimalsCards/joker");
                    if (jokerSpr == null)
                    {
                        var tex = Resources.Load<Texture2D>("SpaceAnimalsCards/joker");
                        if (tex != null) jokerSpr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                    }
                    img.sprite = jokerSpr;
                }
                else
                {
                    img.sprite = UIIcons.Get(icons[i]);
                }
                img.preserveAspect = true;
                img.raycastTarget = false;
                var iRect = ico.GetComponent<RectTransform>();
                float xMin = (1f - icons.Length * 0.25f) / 2f + i * 0.25f;
                iRect.anchorMin = new Vector2(xMin, 0.35f);
                iRect.anchorMax = new Vector2(xMin + 0.22f, 0.72f);
                iRect.offsetMin = Vector2.zero;
                iRect.offsetMax = Vector2.zero;
            }

            // Description
            var desc = UIFactory.CreateText("Desc", Descriptions[page], _contentArea.transform,
                new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.32f), TextAnchor.MiddleCenter, 28);
            desc.color = new Color(0.3f, 0.3f, 0.3f);
        }

        private void NextPage()
        {
            _currentPage++;
            if (_currentPage >= 4)
                Close();
            else
                ShowPage(_currentPage);
        }

        private void Close()
        {
            PlayerPrefs.SetInt(ShownKey, 1);
            PlayerPrefs.Save();
            if (_panel != null) Destroy(_panel);
            Destroy(this);
        }
    }
}
