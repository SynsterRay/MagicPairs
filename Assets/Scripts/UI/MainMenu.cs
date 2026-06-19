using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public enum Difficulty { Easy, Medium, Hard }

    public class MainMenu : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject languagePanel;
        [SerializeField] private GameObject gameTypePanel;
        [SerializeField] private GameObject modePanel;
        [SerializeField] private GameObject difficultyPanel;
        [SerializeField] private GameObject namesPanel;

        [Header("Start Panel")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Text leaderboardButtonText;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Button creditsBackButton;
        [SerializeField] private Text playButtonText;
        [SerializeField] private Text optionsButtonText;
        [SerializeField] private GameObject startLeaderboardPanel;
        [SerializeField] private Text startLeaderboardText;
        [SerializeField] private Button startLeaderboardBackButton;

        [Header("Options Panel")]
        [SerializeField] private Text optionsTitle;
        [SerializeField] private Button languageButton;
        [SerializeField] private Text languageButtonText;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button optionsBackButton;
        [SerializeField] private Button menuMusicToggle;
        [SerializeField] private Text menuMusicToggleText;
        [SerializeField] private Button gameMusicToggle;
        [SerializeField] private Text gameMusicToggleText;

        [Header("Game Type Panel")]
        [SerializeField] private Text gameTypeTitle;
        [SerializeField] private Button arcadeButton;
        [SerializeField] private Text arcadeButtonText;
        [SerializeField] private Button challengeButton;
        [SerializeField] private Text challengeButtonText;
        [SerializeField] private Button timeAttackButton;
        [SerializeField] private Text timeAttackButtonText;
        [SerializeField] private Button gameTypeBackButton;

        [Header("Challenge Names Panel")]
        [SerializeField] private GameObject challengeNamesPanel;
        [SerializeField] private Text challengeNameLabel;
        [SerializeField] private InputField challengeNameInput;
        [SerializeField] private Button challengeStartButton;
        [SerializeField] private Text challengeStartText;
        [SerializeField] private Button challengeNamesBackButton;
        [SerializeField] private GameObject challengeThemePanel;
        [SerializeField] private Text challengeThemeTitle;
        [SerializeField] private Button challengeColorsButton;
        [SerializeField] private Button challengePrincessButton;
        [SerializeField] private Button challengeCarsButton;
        [SerializeField] private Button challengeThemeBackButton;

        [Header("Language Panel")]
        [SerializeField] private Text languageTitle;
        [SerializeField] private Button polishButton;
        [SerializeField] private Button englishButton;
        [SerializeField] private Button languageBackButton;

        [Header("Mode Panel")]
        [SerializeField] private Text modeTitle;
        [SerializeField] private Button twoPlayersButton;
        [SerializeField] private Button singlePlayerButton;
        [SerializeField] private Text twoPlayersText;
        [SerializeField] private Text singlePlayerText;
        [SerializeField] private Button modeBackButton;

        [Header("Difficulty Panel")]
        [SerializeField] private Text difficultyTitle;
        [SerializeField] private Button easyButton;
        [SerializeField] private Button mediumButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private Button difficultyBackButton;

        [Header("Theme Panel")]
        [SerializeField] private GameObject themePanel;
        [SerializeField] private Text themeTitle;
        [SerializeField] private Button colorsThemeButton;
        [SerializeField] private Button princessThemeButton;
        [SerializeField] private Button carsThemeButton;
        [SerializeField] private Button themeBackButton;

        [Header("Names Panel")]
        [SerializeField] private InputField player1Input;
        [SerializeField] private InputField player2Input;
        [SerializeField] private Text player1Label;
        [SerializeField] private Text player2Label;
        [SerializeField] private Button startButton;
        [SerializeField] private Text startButtonText;
        [SerializeField] private Button namesBackButton;

        public static string Player1Name { get; private set; } = "Player 1";
        public static string Player2Name { get; private set; } = "Player 2";
        public static bool IsSinglePlayer { get; private set; }
        public static bool IsChallengeMode { get; private set; }
        public static bool IsTimeAttackMode { get; private set; }
        public static Difficulty SelectedDifficulty { get; private set; }

        private bool _singlePlayer;

        private void Start()
        {
            playButton?.onClick.AddListener(ShowGameTypePanel);
            optionsButton?.onClick.AddListener(ShowOptionsPanel);
            leaderboardButton?.onClick.AddListener(ShowStartLeaderboard);
            startLeaderboardBackButton?.onClick.AddListener(ShowStartPanel);
            quitButton?.onClick.AddListener(() => Application.Quit());
            creditsButton?.onClick.AddListener(ShowCredits);
            creditsBackButton?.onClick.AddListener(ShowOptionsPanel);
            WireShopButton();
            optionsBackButton?.onClick.AddListener(ShowStartPanel);
            languageButton?.onClick.AddListener(ShowLanguagePanel);
            languageBackButton?.onClick.AddListener(ShowOptionsPanel);
            polishButton?.onClick.AddListener(() => SelectLanguage(Language.Polish));
            englishButton?.onClick.AddListener(() => SelectLanguage(Language.English));
            menuMusicToggle?.onClick.AddListener(ToggleMenuMusic);
            gameMusicToggle?.onClick.AddListener(ToggleGameMusic);
            arcadeButton?.onClick.AddListener(SelectArcade);
            challengeButton?.onClick.AddListener(SelectChallenge);
            timeAttackButton?.onClick.AddListener(SelectTimeAttack);
            gameTypeBackButton?.onClick.AddListener(ShowStartPanel);
            twoPlayersButton?.onClick.AddListener(() => SelectMode(false));
            singlePlayerButton?.onClick.AddListener(() => SelectMode(true));
            easyButton?.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
            mediumButton?.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
            hardButton?.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
            colorsThemeButton?.onClick.AddListener(() => SelectTheme(Core.CardTheme.Dinos));
            princessThemeButton?.onClick.AddListener(() => SelectTheme(Core.CardTheme.Princess));
            carsThemeButton?.onClick.AddListener(() => SelectTheme(Core.CardTheme.Cars));
            startButton?.onClick.AddListener(OnStart);
            modeBackButton?.onClick.AddListener(ShowGameTypePanel);
            difficultyBackButton?.onClick.AddListener(() =>
            {
                if (IsTimeAttackMode) ShowGameTypePanel();
                else ShowModePanel();
            });
            themeBackButton?.onClick.AddListener(ShowDifficultyPanel);
            namesBackButton?.onClick.AddListener(OnNamesBack);
            challengeStartButton?.onClick.AddListener(OnChallengeStart);
            challengeNamesBackButton?.onClick.AddListener(ShowChallengeThemePanel);
            challengeColorsButton?.onClick.AddListener(() => SelectChallengeTheme(Core.CardTheme.Dinos));
            challengePrincessButton?.onClick.AddListener(() => SelectChallengeTheme(Core.CardTheme.Princess));
            challengeCarsButton?.onClick.AddListener(() => SelectChallengeTheme(Core.CardTheme.Cars));
            challengeThemeBackButton?.onClick.AddListener(ShowGameTypePanel);

            WireLockedThemeButtons();
            CreateMenuStatusBar();

            Core.GPGSManager.OnAuthChanged += OnGPGSAuthChanged;
            ShowStartPanel();
        }

        private void OnDestroy()
        {
            Core.GPGSManager.OnAuthChanged -= OnGPGSAuthChanged;
        }

        private void OnGPGSAuthChanged()
        {
            if (startPanel != null && startPanel.activeSelf)
            {
                UpdateAchievementsButton();
                var gpgs = Core.GPGSManager.Instance;
                bool authenticated = gpgs != null && gpgs.IsAuthenticated;
                if (leaderboardButton != null)
                    leaderboardButton.gameObject.SetActive(authenticated);
            }
        }

        private void HideAllPanels()
        {
            if (startPanel != null) startPanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (languagePanel != null) languagePanel.SetActive(false);
            if (gameTypePanel != null) gameTypePanel.SetActive(false);
            if (modePanel != null) modePanel.SetActive(false);
            if (difficultyPanel != null) difficultyPanel.SetActive(false);
            if (themePanel != null) themePanel.SetActive(false);
            if (namesPanel != null) namesPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (challengeNamesPanel != null) challengeNamesPanel.SetActive(false);
            if (challengeThemePanel != null) challengeThemePanel.SetActive(false);
            if (startLeaderboardPanel != null) startLeaderboardPanel.SetActive(false);
        }

        private void WireShopButton()
        {
            var shopBtnObj = startPanel?.transform.Find("ShopBtn");
            if (shopBtnObj == null) return;
            var btn = shopBtnObj.GetComponent<Button>();
            btn?.onClick.AddListener(() =>
            {
                if (menuPanel != null) menuPanel.SetActive(false);
                var shop = FindAnyObjectByType<ShopUI>();
                shop?.Show(() => ShowStartPanel());
            });
        }

        private void WireLockedThemeButtons()
        {
            WireLockedBtn(themePanel, "LockedCard1");
            WireLockedBtn(themePanel, "LockedCard2");
            WireLockedBtn(challengeThemePanel, "ChLockedCard1");
            WireLockedBtn(challengeThemePanel, "ChLockedCard2");
        }

        private void WireLockedBtn(GameObject panel, string name)
        {
            if (panel == null) return;
            var obj = panel.transform.Find(name);
            if (obj == null) return;
            var btn = obj.GetComponent<Button>();
            btn?.onClick.AddListener(() =>
            {
                if (menuPanel != null) menuPanel.SetActive(false);
                var shop = FindAnyObjectByType<ShopUI>();
                shop?.Show(() => { ShowStartPanel(); ShowGameTypePanel(); });
            });
        }

        private GameObject _menuStatusBar;
        private Text _statusPeekText;
        private Text _statusShuffleText;
        private Text _statusFreezeText;
        private Text _statusCoinText;
        private GameObject _titleLogo;
        private GameObject[] _headerLogos;
        private int _activeHeaderIndex = -1;

        private static readonly string[] HeaderIcons = { "header_car", "header_dino", "header_monkey", "header_princess", "header_lion" };

        private void CreateMenuStatusBar()
        {
            if (menuPanel == null) return;

            // Find title logo reference
            _titleLogo = menuPanel.transform.Find("TitleLogo")?.gameObject;

            // Create header logos (different character per panel)
            _headerLogos = new GameObject[HeaderIcons.Length];
            for (int i = 0; i < HeaderIcons.Length; i++)
            {
                var sprite = UIIcons.Get(HeaderIcons[i]);
                if (sprite == null) continue;
                var go = new GameObject($"HeaderLogo_{HeaderIcons[i]}");
                go.transform.SetParent(menuPanel.transform, false);
                var img = go.AddComponent<Image>();
                img.sprite = sprite;
                img.preserveAspect = true;
                img.raycastTarget = false;
                var rect = go.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.25f, 0.72f);
                rect.anchorMax = new Vector2(0.75f, 0.93f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                go.SetActive(false);
                _headerLogos[i] = go;
            }

            // Status bar: power-ups + coins (top area, below logo)
            _menuStatusBar = new GameObject("MenuStatusBar");
            _menuStatusBar.transform.SetParent(menuPanel.transform, false);
            var barRect = _menuStatusBar.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0.05f, 0.93f);
            barRect.anchorMax = new Vector2(0.95f, 1.0f);
            barRect.offsetMin = Vector2.zero;
            barRect.offsetMax = Vector2.zero;

            _statusPeekText = CreateStatusIcon("peek_game", 0f, 0.22f);
            _statusShuffleText = CreateStatusIcon("shuffle_game", 0.26f, 0.48f);
            _statusFreezeText = CreateStatusIcon("freeze_game", 0.52f, 0.74f);
            _statusCoinText = CreateStatusIcon("coin_icon", 0.78f, 1.0f);

            _menuStatusBar.SetActive(false);
            Core.PlayerWallet.OnCoinsChanged += _ => UpdateMenuStatusBar();
        }

        private Text CreateStatusIcon(string iconName, float xMin, float xMax)
        {
            var go = new GameObject(iconName);
            go.transform.SetParent(_menuStatusBar.transform, false);
            var img = go.AddComponent<Image>();
            img.sprite = UIIcons.Get(iconName);
            img.preserveAspect = true;
            img.raycastTarget = false;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(xMin, 0.35f);
            rect.anchorMax = new Vector2(xMax, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var txtObj = new GameObject("Count");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = "0";
            txt.fontSize = 20;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.UpperCenter;
            txt.color = new Color(0.3f, 0.15f, 0.5f);
            txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = new Vector2(0f, -0.6f);
            txtRect.anchorMax = new Vector2(1f, 0f);
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            return txt;
        }

        private void UpdateMenuStatusBar()
        {
            int peek = Core.ShopCatalog.GetStoredPowerUps(Core.ShopItemType.PowerUpPeek) + 1;
            int shuffle = Core.ShopCatalog.GetStoredPowerUps(Core.ShopItemType.PowerUpShuffle);
            int freeze = Core.ShopCatalog.GetStoredPowerUps(Core.ShopItemType.PowerUpFreeze);
            if (_statusPeekText != null) _statusPeekText.text = peek.ToString();
            if (_statusShuffleText != null) _statusShuffleText.text = shuffle.ToString();
            if (_statusFreezeText != null) _statusFreezeText.text = freeze.ToString();
            if (_statusCoinText != null) _statusCoinText.text = Core.PlayerWallet.Coins.ToString();
        }

        private void ShowMenuStatusBar(bool showPony)
        {
            if (_menuStatusBar != null) _menuStatusBar.SetActive(true);
            if (!showPony) HideAllHeaders();
            if (_titleLogo != null) _titleLogo.SetActive(!showPony);
            UpdateMenuStatusBar();
        }

        private void ShowHeaderLogo(int index)
        {
            if (_headerLogos == null) return;
            for (int i = 0; i < _headerLogos.Length; i++)
                if (_headerLogos[i] != null) _headerLogos[i].SetActive(i == index);
            _activeHeaderIndex = index;
            if (_titleLogo != null) _titleLogo.SetActive(false);
        }

        private void HideAllHeaders()
        {
            if (_headerLogos == null) return;
            for (int i = 0; i < _headerLogos.Length; i++)
                if (_headerLogos[i] != null) _headerLogos[i].SetActive(false);
            _activeHeaderIndex = -1;
        }

        private void HideMenuStatusBar()
        {
            if (_menuStatusBar != null) _menuStatusBar.SetActive(false);
            HideAllHeaders();
            if (_titleLogo != null) _titleLogo.SetActive(true);
        }

        private void ShowStartPanel()
        {
            if (menuPanel != null) menuPanel.SetActive(true);
            HideAllPanels();
            if (startPanel != null) startPanel.SetActive(true);
            UpdateStartPanelTexts();
            UpdateAchievementsButton();
            ShowMenuStatusBar(false);

            // Show scores button only if authenticated to GPGS
            var gpgs = Core.GPGSManager.Instance;
            bool authenticated = gpgs != null && gpgs.IsAuthenticated;
            if (leaderboardButton != null)
                leaderboardButton.gameObject.SetActive(authenticated);
        }

        private void UpdateStartPanelTexts()
        {
            if (playButtonText != null) playButtonText.text = Localization.Get("play");
            if (optionsButtonText != null) optionsButtonText.text = Localization.Get("options");
            if (leaderboardButtonText != null) leaderboardButtonText.text = Localization.Get("scores");
            if (quitButton != null)
            {
                var quitTxt = quitButton.GetComponentInChildren<Text>();
                if (quitTxt != null) quitTxt.text = Localization.Get("quit");
            }
            var shopBtnObj = startPanel?.transform.Find("ShopBtn");
            if (shopBtnObj != null)
            {
                var txt = shopBtnObj.GetComponentInChildren<Text>();
                if (txt != null) txt.text = $"🪙 {Localization.Get("shop")}";
            }
        }

        private void UpdateAchievementsButton()
        {
            var gpgs = Core.GPGSManager.Instance;
            bool show = gpgs != null && gpgs.IsAuthenticated;

            if (_achievementsBtn == null && show && startPanel != null)
            {
                var btn = UIFactory.CreateIconButton("AchievementsBtn", "achievements",
                    startPanel.transform, new Vector2(0.02f, 0.02f), new Vector2(0.32f, 0.34f));
                _achievementsBtn = btn.gameObject;
                btn.onClick.AddListener(() => Core.GPGSManager.Instance?.ShowAchievements());
            }

            if (_achievementsBtn != null)
                _achievementsBtn.SetActive(show);
        }

        private void ShowStartLeaderboard()
        {
            var gpgs = Core.GPGSManager.Instance;
            if (gpgs != null && gpgs.IsAuthenticated)
            {
                gpgs.ShowLeaderboard();
                return;
            }

            // Not authenticated — show local scores as fallback
            ShowLocalLeaderboard();
        }

        private GameObject _leaderChoicePanel;
        private GameObject _achievementsBtn;

        private void ShowLeaderboardChoice()
        {
            if (_leaderChoicePanel != null) { _leaderChoicePanel.SetActive(true); return; }

            _leaderChoicePanel = new GameObject("LeaderChoicePanel");
            _leaderChoicePanel.transform.SetParent(startLeaderboardPanel.transform, false);
            var rect = _leaderChoicePanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.3f);
            rect.anchorMax = new Vector2(0.9f, 0.7f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var globalBtn = CreateChoiceButton("GlobalBtn", Localization.Get("globalScores"), _leaderChoicePanel.transform,
                new Vector2(0.05f, 0.55f), new Vector2(0.95f, 0.95f), new Color(0.8f, 0.6f, 0.1f, 1f));
            globalBtn.onClick.AddListener(() =>
            {
                _leaderChoicePanel.SetActive(false);
                ShowStartPanel();
                Core.GPGSManager.Instance?.ShowLeaderboard();
            });

            var localBtn = CreateChoiceButton("LocalBtn", Localization.Get("localScores"), _leaderChoicePanel.transform,
                new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.45f), new Color(0.3f, 0.5f, 0.8f, 1f));
            localBtn.onClick.AddListener(() =>
            {
                _leaderChoicePanel.SetActive(false);
                ShowLocalLeaderboard();
            });
        }

        private Button CreateChoiceButton(string name, string label, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.sprite = RoundedButtonHelper.GetRoundedSprite();
            img.type = Image.Type.Sliced;
            var btn = go.AddComponent<Button>();
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = 52;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 28;
            txt.resizeTextMaxSize = 52;
            var tr = txtObj.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(10f, 4f);
            tr.offsetMax = new Vector2(-10f, -4f);
            return btn;
        }

        private void ShowLocalLeaderboard()
        {
            HideAllPanels();
            if (startLeaderboardPanel != null) startLeaderboardPanel.SetActive(true);
            if (startLeaderboardText == null) return;

            var entries = Core.Leaderboard.Entries;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<size=32><b><color=#D4AF37>{Localization.Get("leaderboard").ToUpper()}</color></b></size>");
            sb.AppendLine();
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                sb.AppendLine($" {i + 1}. {e.playerName} — {e.score} (Lv.{e.level})");
            }
            if (entries.Count == 0)
                sb.AppendLine(Localization.Get("noScores"));

            var taEntries = Core.Leaderboard.TimeAttackEntries;
            if (taEntries.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"<size=28><b><color=#D4AF37>⏱ {Localization.Get("timeAttack").ToUpper()}</color></b></size>");
                sb.AppendLine();
                for (int i = 0; i < taEntries.Count; i++)
                {
                    var e = taEntries[i];
                    sb.AppendLine($" {i + 1}. {e.playerName} — {e.timeLeft:F1}s ({e.difficulty})");
                }
            }

            startLeaderboardText.text = sb.ToString();
        }

        private void ShowOptionsPanel()
        {
            HideAllPanels();
            if (optionsPanel != null) optionsPanel.SetActive(true);
            ShowMenuStatusBar(true);
            ShowHeaderLogo(3); // princess
            if (optionsTitle != null) optionsTitle.text = Localization.Get("options");
            if (languageButtonText != null) languageButtonText.text = Localization.Get("languageOption");
            if (creditsButton != null)
            {
                var creditsTxt = creditsButton.GetComponentInChildren<Text>();
                if (creditsTxt != null) creditsTxt.text = Localization.Get("credits");
            }
            UpdateMusicToggleTexts();
        }

        private void ToggleMenuMusic()
        {
            Audio.MusicManager.MenuMusicOn = !Audio.MusicManager.MenuMusicOn;
            UpdateMusicToggleTexts();
        }

        private void ToggleGameMusic()
        {
            Audio.MusicManager.GameMusicOn = !Audio.MusicManager.GameMusicOn;
            UpdateMusicToggleTexts();
        }

        private void UpdateMusicToggleTexts()
        {
            if (menuMusicToggleText != null)
                menuMusicToggleText.text = Localization.Get("menuMusic") + (Audio.MusicManager.MenuMusicOn ? " ✓" : " ✗");
            if (gameMusicToggleText != null)
                gameMusicToggleText.text = Localization.Get("gameMusic") + (Audio.MusicManager.GameMusicOn ? " ✓" : " ✗");

            // Swap icon sprites for on/off state
            if (menuMusicToggle != null)
            {
                var img = menuMusicToggle.GetComponent<Image>();
                var sprite = UIIcons.Get(Audio.MusicManager.MenuMusicOn ? "music" : "sound off");
                if (img != null && sprite != null) img.sprite = sprite;
            }
            if (gameMusicToggle != null)
            {
                var img = gameMusicToggle.GetComponent<Image>();
                var sprite = UIIcons.Get(Audio.MusicManager.GameMusicOn ? "sound on" : "sound off");
                if (img != null && sprite != null) img.sprite = sprite;
            }
        }

        private void ShowLanguagePanel()
        {
            HideAllPanels();
            if (languagePanel != null) languagePanel.SetActive(true);
            if (languageTitle != null) languageTitle.text = Localization.Get("chooseLanguage");
            CreateLanguageButtons();
        }

        private GameObject _langButtonsContainer;
        private static readonly (Language lang, string label)[] AllLanguages =
        {
            (Language.English, "English"),
            (Language.Polish, "Polski"),
            (Language.Spanish, "Español"),
            (Language.Portuguese, "Português"),
            (Language.German, "Deutsch"),
            (Language.French, "Français"),
            (Language.Hindi, "हिन्दी"),
            (Language.Chinese, "中文"),
            (Language.Japanese, "日本語"),
        };

        private void CreateLanguageButtons()
        {
            if (languagePanel == null) return;

            // Hide old static buttons
            if (polishButton != null) polishButton.gameObject.SetActive(false);
            if (englishButton != null) englishButton.gameObject.SetActive(false);

            if (_langButtonsContainer != null) { _langButtonsContainer.SetActive(true); return; }

            _langButtonsContainer = new GameObject("LangButtons");
            _langButtonsContainer.transform.SetParent(languagePanel.transform, false);
            var cRect = _langButtonsContainer.AddComponent<RectTransform>();
            cRect.anchorMin = new Vector2(0.05f, 0.05f);
            cRect.anchorMax = new Vector2(0.95f, 0.82f);
            cRect.offsetMin = Vector2.zero;
            cRect.offsetMax = Vector2.zero;

            int count = AllLanguages.Length;
            int cols = 3;
            int rows = (count + cols - 1) / cols;
            float spacing = 0.03f;
            float cellW = (1f - spacing * (cols + 1)) / cols;
            float cellH = (1f - spacing * (rows + 1)) / rows;

            for (int i = 0; i < count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                float x = spacing + col * (cellW + spacing);
                float y = 1f - spacing - (row + 1) * (cellH + spacing) + spacing;

                var (lang, label) = AllLanguages[i];
                var btn = UIFactory.CreateButton($"Lang_{lang}", label,
                    _langButtonsContainer.transform,
                    new Vector2(x, y), new Vector2(x + cellW, y + cellH),
                    new Color(0.3f, 0.15f, 0.6f, 1f));
                var capturedLang = lang;
                btn.onClick.AddListener(() => SelectLanguage(capturedLang));
            }
        }

        private void SelectLanguage(Language lang)
        {
            Localization.CurrentLanguage = lang;
            ShowOptionsPanel();
        }

        private void ShowCredits()
        {
            HideAllPanels();
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }

        private void ShowGameTypePanel()
        {
            HideAllPanels();
            if (gameTypePanel != null) gameTypePanel.SetActive(true);
            ShowMenuStatusBar(true);
            ShowHeaderLogo(1); // dino
            if (gameTypeTitle != null) gameTypeTitle.text = Localization.Get("chooseGameType");
            if (arcadeButtonText != null) arcadeButtonText.text = Localization.Get("arcade");
            if (challengeButtonText != null) challengeButtonText.text = Localization.Get("challenge");
            if (timeAttackButtonText != null) timeAttackButtonText.text = Localization.Get("timeAttack");
        }

        private void SelectArcade()
        {
            IsChallengeMode = false;
            IsTimeAttackMode = false;
            GameFlow.TimeAttackMode.IsTimeAttackMode = false;
            ShowModePanel();
        }

        private void SelectChallenge()
        {
            IsChallengeMode = true;
            IsTimeAttackMode = false;
            GameFlow.TimeAttackMode.IsTimeAttackMode = false;
            ShowChallengeThemePanel();
        }

        private void SelectTimeAttack()
        {
            IsChallengeMode = false;
            IsTimeAttackMode = true;
            GameFlow.TimeAttackMode.IsTimeAttackMode = true;
            _singlePlayer = true;
            ShowDifficultyPanel();
        }

        private void ShowChallengeThemePanel()
        {
            HideAllPanels();
            if (challengeThemePanel != null) challengeThemePanel.SetActive(true);
            ShowMenuStatusBar(true);
            ShowHeaderLogo(2); // monkey
            if (challengeThemeTitle != null)
                challengeThemeTitle.text = Localization.Get("chooseTheme");
        }

        private void SelectChallengeTheme(Core.CardTheme theme)
        {
            var config = GameManager.Instance.Config;
            if (config != null) config.theme = theme;
            _singlePlayer = true;
            StartGameDirectly();
        }

        private void ShowChallengeNamesPanel()
        {
            HideAllPanels();
            if (challengeNamesPanel != null) challengeNamesPanel.SetActive(true);
            if (challengeNameLabel != null) challengeNameLabel.text = Localization.Get("yourName");
            if (challengeNameInput != null)
                challengeNameInput.placeholder.GetComponent<Text>().text = Localization.Get("player1");
            if (challengeStartText != null) challengeStartText.text = Localization.Get("start");
        }

        private void OnChallengeStart()
        {
            IsSinglePlayer = true;
            Player1Name = string.IsNullOrWhiteSpace(challengeNameInput?.text)
                ? Localization.Get("player1") : challengeNameInput.text;
            Player2Name = Localization.Get("computer");

            var local = FindAnyObjectByType<GameFlow.LocalGameMode>(FindObjectsInactive.Include);
            var single = FindAnyObjectByType<GameFlow.SinglePlayerMode>(FindObjectsInactive.Include);
            var challenge = FindAnyObjectByType<GameFlow.ChallengeMode>(FindObjectsInactive.Include);

            if (local != null) local.enabled = false;
            if (single != null) single.enabled = false;
            if (challenge != null)
            {
                challenge.enabled = true;
                challenge.StartGame(); // Apply level 1 config before grid builds
            }

            if (menuPanel != null) menuPanel.SetActive(false);
            ShowLoadingAndStart();
        }

        private void ShowModePanel()
        {
            HideAllPanels();
            if (modePanel != null) modePanel.SetActive(true);
            ShowMenuStatusBar(true);
            ShowHeaderLogo(0); // car
            if (modeTitle != null) modeTitle.text = Localization.Get("chooseMode");
            if (twoPlayersText != null) twoPlayersText.text = Localization.Get("mode2P");
            if (singlePlayerText != null) singlePlayerText.text = Localization.Get("mode1P");
        }

        private void SelectMode(bool singlePlayer)
        {
            _singlePlayer = singlePlayer;
            ShowDifficultyPanel();
        }

        private void ShowDifficultyPanel()
        {
            HideAllPanels();
            if (difficultyPanel != null) difficultyPanel.SetActive(true);
            ShowMenuStatusBar(true);
            ShowHeaderLogo(4); // lion
            if (difficultyTitle != null) difficultyTitle.text = Localization.Get("chooseDifficulty");
        }

        private void SelectDifficulty(Difficulty diff)
        {
            SelectedDifficulty = diff;
            ApplyDifficulty(diff);
            ShowThemePanel();
        }

        private void ShowThemePanel()
        {
            HideAllPanels();
            if (themePanel != null) themePanel.SetActive(true);
            ShowMenuStatusBar(true);
            ShowHeaderLogo(2); // monkey
            if (themeTitle != null)
                themeTitle.text = Localization.Get("chooseTheme");
        }

        private void SelectTheme(Core.CardTheme theme)
        {
            var config = GameManager.Instance.Config;
            if (config != null) config.theme = theme;
            StartGameDirectly();
        }

        private void SetIconLabel(Button button, string label)
        {
            if (button == null) return;
            var existing = button.transform.Find("Label");
            Text txt;
            if (existing != null)
            {
                txt = existing.GetComponent<Text>();
            }
            else
            {
                var go = new GameObject("Label");
                go.transform.SetParent(button.transform, false);
                txt = go.AddComponent<Text>();
                txt.fontSize = 28;
                txt.fontStyle = FontStyle.Bold;
                txt.alignment = TextAnchor.MiddleCenter;
                txt.color = new Color(0.3f, 0.1f, 0.5f);
                txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                txt.resizeTextForBestFit = true;
                txt.resizeTextMinSize = 14;
                txt.resizeTextMaxSize = 28;
                var r = go.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, -0.3f);
                r.anchorMax = new Vector2(1f, 0f);
                r.offsetMin = Vector2.zero;
                r.offsetMax = Vector2.zero;
            }
            txt.text = label;
        }

        private void SetThemeButtonSprite(Button button, string folder)
        {
            if (button == null) return;
            var sprites = Resources.LoadAll<Sprite>(folder);
            Sprite card = null;
            foreach (var s in sprites)
            {
                if (!s.name.Contains("joker") && !s.name.Contains("back"))
                { card = s; break; }
            }
            if (card == null)
            {
                var textures = Resources.LoadAll<Texture2D>(folder);
                foreach (var tex in textures)
                {
                    if (tex.name.Contains("joker") || tex.name.Contains("back")) continue;
                    card = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    break;
                }
            }
            if (card == null) return;

            var img = button.GetComponent<Image>();
            img.sprite = card;
            img.color = Color.white;
            img.preserveAspect = true;
            // Hide text
            var txt = button.GetComponentInChildren<Text>();
            if (txt != null) txt.text = "";
        }

        private void SetThemeButtonLabel(Button button, string label)
        {
            if (button == null) return;
            var img = button.GetComponent<Image>();
            img.sprite = RoundedButtonHelper.GetRoundedSprite();
            img.type = Image.Type.Sliced;
            img.preserveAspect = false;
            img.color = new Color(0.1f, 0.4f, 0.9f, 1f); // Blue card for Colors theme
            var txt = button.GetComponentInChildren<Text>();
            if (txt != null) txt.text = label;
        }

        private void StyleAsCard(Button button, Vector2 anchorMin, Vector2 anchorMax)
        {
            if (button == null) return;
            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void EnsureLockedCards(Transform parent)
        {
            if (parent.Find("LockedCard1") != null) return;

            for (int i = 1; i <= 2; i++)
            {
                var locked = new GameObject($"LockedCard{i}");
                locked.transform.SetParent(parent, false);
                var img = locked.AddComponent<Image>();
                img.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                img.sprite = RoundedButtonHelper.GetRoundedSprite();
                img.type = Image.Type.Sliced;
                var rect = locked.GetComponent<RectTransform>();
                float xStart = i == 1 ? 0.02f : 0.35f;
                float xEnd = i == 1 ? 0.32f : 0.65f;
                rect.anchorMin = new Vector2(xStart, 0.10f);
                rect.anchorMax = new Vector2(xEnd, 0.42f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                var txtObj = new GameObject("Text");
                txtObj.transform.SetParent(locked.transform, false);
                var txt = txtObj.AddComponent<Text>();
                txt.text = "?";
                txt.fontSize = 72;
                txt.fontStyle = FontStyle.Bold;
                txt.alignment = TextAnchor.MiddleCenter;
                txt.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
                txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                var tr = txtObj.GetComponent<RectTransform>();
                tr.anchorMin = Vector2.zero;
                tr.anchorMax = Vector2.one;
                tr.offsetMin = Vector2.zero;
                tr.offsetMax = Vector2.zero;
            }
        }


        private void ApplyDifficulty(Difficulty diff)
        {
            var config = GameManager.Instance.Config;
            if (config == null) return;
            switch (diff)
            {
                case Difficulty.Easy:
                    config.gridRows = 3;
                    config.gridCols = 4;
                    break;
                case Difficulty.Medium:
                    config.gridRows = 4;
                    config.gridCols = 5;
                    break;
                case Difficulty.Hard:
                    config.gridRows = 5;
                    config.gridCols = 6;
                    break;
            }
        }

        private void ShowNamesPanel()
        {
            HideAllPanels();
            if (namesPanel != null) namesPanel.SetActive(true);

            // Load saved names
            string saved1 = PlayerPrefs.GetString("MagicPairs_Player1Name", "");
            string saved2 = PlayerPrefs.GetString("MagicPairs_Player2Name", "");

            if (player1Label != null)
                player1Label.text = _singlePlayer ? Localization.Get("yourName") : Localization.Get("player1Name");
            if (player1Input != null)
            {
                player1Input.placeholder.GetComponent<Text>().text = Localization.Get("player1");
                if (!string.IsNullOrEmpty(saved1)) player1Input.text = saved1;
            }

            if (player2Label != null) player2Label.gameObject.SetActive(!_singlePlayer);
            if (player2Input != null) player2Input.gameObject.SetActive(!_singlePlayer);

            if (!_singlePlayer && player2Label != null)
                player2Label.text = Localization.Get("player2Name");
            if (!_singlePlayer && player2Input != null)
            {
                player2Input.placeholder.GetComponent<Text>().text = Localization.Get("player2");
                if (!string.IsNullOrEmpty(saved2)) player2Input.text = saved2;
            }

            // Reposition start button: center between last input and back button
            if (startButton != null)
            {
                var rect = startButton.GetComponent<RectTransform>();
                if (_singlePlayer)
                {
                    rect.anchorMin = new Vector2(0.2f, 0.23f);
                    rect.anchorMax = new Vector2(0.8f, 0.43f);
                }
                else
                {
                    rect.anchorMin = new Vector2(0.2f, 0.06f);
                    rect.anchorMax = new Vector2(0.8f, 0.26f);
                }
            }

            if (startButtonText != null) startButtonText.text = Localization.Get("start");
        }

        private void OnNamesBack()
        {
            if (IsChallengeMode)
                ShowChallengeThemePanel();
            else
                ShowThemePanel();
        }

        private void OnStart()
        {
            IsSinglePlayer = _singlePlayer;

            Player1Name = string.IsNullOrWhiteSpace(player1Input?.text)
                ? Localization.Get("player1") : player1Input.text;

            if (_singlePlayer)
                Player2Name = Localization.Get("computer");
            else
                Player2Name = string.IsNullOrWhiteSpace(player2Input?.text)
                    ? Localization.Get("player2") : player2Input.text;

            // Save names for next time
            PlayerPrefs.SetString("MagicPairs_Player1Name", Player1Name);
            if (!_singlePlayer) PlayerPrefs.SetString("MagicPairs_Player2Name", Player2Name);
            PlayerPrefs.Save();

            var local = FindAnyObjectByType<GameFlow.LocalGameMode>(FindObjectsInactive.Include);
            var single = FindAnyObjectByType<GameFlow.SinglePlayerMode>(FindObjectsInactive.Include);
            var challenge = FindAnyObjectByType<GameFlow.ChallengeMode>(FindObjectsInactive.Include);
            var timeAttack = FindAnyObjectByType<GameFlow.TimeAttackMode>(FindObjectsInactive.Include);

            if (IsTimeAttackMode)
            {
                if (local != null) local.enabled = false;
                if (single != null) single.enabled = false;
                if (challenge != null) challenge.enabled = false;
                if (timeAttack != null) timeAttack.enabled = true;
            }
            else if (IsChallengeMode)
            {
                if (local != null) local.enabled = false;
                if (single != null) single.enabled = false;
                if (challenge != null)
                {
                    challenge.enabled = true;
                    challenge.StartGame();
                }
                if (timeAttack != null) timeAttack.enabled = false;
            }
            else
            {
                if (local != null) local.enabled = !_singlePlayer;
                if (single != null) single.enabled = _singlePlayer;
                if (challenge != null) challenge.enabled = false;
                if (timeAttack != null) timeAttack.enabled = false;
            }

            if (menuPanel != null) menuPanel.SetActive(false);
            ShowLoadingAndStart();
        }

        private void StartGameDirectly()
        {
            IsSinglePlayer = _singlePlayer;
            var gpgs = Core.GPGSManager.Instance;
            if (gpgs != null && gpgs.IsAuthenticated)
                Player1Name = Social.localUser.userName;
            else
                Player1Name = Localization.Get("player1");
            Player2Name = _singlePlayer ? Localization.Get("computer") : Localization.Get("player2");

            var local = FindAnyObjectByType<GameFlow.LocalGameMode>(FindObjectsInactive.Include);
            var single = FindAnyObjectByType<GameFlow.SinglePlayerMode>(FindObjectsInactive.Include);
            var challenge = FindAnyObjectByType<GameFlow.ChallengeMode>(FindObjectsInactive.Include);
            var timeAttack = FindAnyObjectByType<GameFlow.TimeAttackMode>(FindObjectsInactive.Include);

            if (IsTimeAttackMode)
            {
                if (local != null) local.enabled = false;
                if (single != null) single.enabled = false;
                if (challenge != null) challenge.enabled = false;
                if (timeAttack != null) timeAttack.enabled = true;
            }
            else if (IsChallengeMode)
            {
                if (local != null) local.enabled = false;
                if (single != null) single.enabled = false;
                if (challenge != null) { challenge.enabled = true; challenge.StartGame(); }
                if (timeAttack != null) timeAttack.enabled = false;
            }
            else
            {
                if (local != null) local.enabled = !_singlePlayer;
                if (single != null) single.enabled = _singlePlayer;
                if (challenge != null) challenge.enabled = false;
                if (timeAttack != null) timeAttack.enabled = false;
            }

            if (menuPanel != null) menuPanel.SetActive(false);
            ShowLoadingAndStart();
        }

        private void ShowLoadingAndStart()
        {
            var loading = FindAnyObjectByType<LoadingScreen>();
            if (loading != null)
                loading.Show(0.6f, () => GameManager.Instance.StartGame());
            else
                GameManager.Instance.StartGame();
        }

        public void ReturnToMenu()
        {
            ShowStartPanel();
            var music = FindAnyObjectByType<Audio.MusicManager>();
            music?.PlayMenuMusic();
        }
    }
}
