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
            colorsThemeButton?.onClick.AddListener(() => SelectTheme(Core.CardTheme.Colors));
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
            challengeColorsButton?.onClick.AddListener(() => SelectChallengeTheme(Core.CardTheme.Colors));
            challengePrincessButton?.onClick.AddListener(() => SelectChallengeTheme(Core.CardTheme.Princess));
            challengeCarsButton?.onClick.AddListener(() => SelectChallengeTheme(Core.CardTheme.Cars));
            challengeThemeBackButton?.onClick.AddListener(ShowGameTypePanel);

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
                UpdateAchievementsButton();
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

        private void ShowStartPanel()
        {
            if (menuPanel != null) menuPanel.SetActive(true);
            HideAllPanels();
            if (startPanel != null) startPanel.SetActive(true);
            UpdateStartPanelTexts();
            UpdateAchievementsButton();
        }

        private void UpdateStartPanelTexts()
        {
            if (playButtonText != null) playButtonText.text = Localization.Get("play");
            if (optionsButtonText != null) optionsButtonText.text = Localization.Get("options");
            if (leaderboardButtonText != null) leaderboardButtonText.text = Localization.Get("scores");
            if (quitButton != null) quitButton.GetComponentInChildren<Text>().text = Localization.Get("quit");
        }

        private void UpdateAchievementsButton()
        {
            var gpgs = Core.GPGSManager.Instance;
            bool show = gpgs != null && gpgs.IsAuthenticated;

            if (_achievementsBtn == null && show && startPanel != null)
            {
                // Create button below Quit (or overlay at bottom-right)
                var canvas = GetComponentInParent<Canvas>() ?? GetComponent<Canvas>();
                if (canvas != null)
                {
                    _achievementsBtn = new GameObject("AchievementsBtn");
                    _achievementsBtn.transform.SetParent(startPanel.transform, false);
                    var img = _achievementsBtn.AddComponent<Image>();
                    img.color = new Color(0.5f, 0.2f, 0.8f, 1f);
                    img.sprite = RoundedButtonHelper.GetRoundedSprite();
                    img.type = Image.Type.Sliced;
                    var btn = _achievementsBtn.AddComponent<Button>();
                    var rect = _achievementsBtn.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.2f, 0.04f);
                    rect.anchorMax = new Vector2(0.8f, 0.24f);
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;

                    var txtObj = new GameObject("Text");
                    txtObj.transform.SetParent(_achievementsBtn.transform, false);
                    var txt = txtObj.AddComponent<Text>();
                    txt.text = "🏆 Achievements";
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

                    btn.onClick.AddListener(() => Core.GPGSManager.Instance?.ShowAchievements());

                    // Move Quit button down to make room
                    if (quitButton != null)
                    {
                        var quitRect = quitButton.GetComponent<RectTransform>();
                        quitRect.anchorMin = new Vector2(0.2f, -0.2f);
                        quitRect.anchorMax = new Vector2(0.8f, 0f);
                    }
                }
            }

            if (_achievementsBtn != null)
                _achievementsBtn.SetActive(show);
        }

        private void ShowStartLeaderboard()
        {
            var gpgs = Core.GPGSManager.Instance;
            if (gpgs != null && gpgs.IsAuthenticated)
            {
                // Show choice: Global vs Local
                HideAllPanels();
                if (startLeaderboardPanel != null) startLeaderboardPanel.SetActive(true);
                if (startLeaderboardText != null)
                    startLeaderboardText.text = "";
                ShowLeaderboardChoice();
                return;
            }

            // Not authenticated — show local directly
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
            if (optionsTitle != null) optionsTitle.text = Localization.Get("options");
            if (languageButtonText != null) languageButtonText.text = Localization.Get("languageOption");
            if (creditsButton != null)
                creditsButton.GetComponentInChildren<Text>().text = Localization.Get("credits");
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
        }

        private void ShowLanguagePanel()
        {
            HideAllPanels();
            if (languagePanel != null) languagePanel.SetActive(true);
            if (languageTitle != null) languageTitle.text = "Choose Language / Wybierz język";
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
            if (challengeThemeTitle != null)
                challengeThemeTitle.text = Localization.Get("chooseTheme");
            SetThemeButtonSprite(challengeCarsButton, "CarCards");
            SetThemeButtonSprite(challengePrincessButton, "PrincessCards");
            SetThemeButtonLabel(challengeColorsButton, Localization.Get("themeColors"));
            StyleAsCard(challengeCarsButton, new Vector2(0.02f, 0.4f), new Vector2(0.32f, 0.75f));
            StyleAsCard(challengePrincessButton, new Vector2(0.35f, 0.4f), new Vector2(0.65f, 0.75f));
            StyleAsCard(challengeColorsButton, new Vector2(0.68f, 0.4f), new Vector2(0.98f, 0.75f));
            EnsureLockedCards(challengeThemePanel.transform);
        }

        private void SelectChallengeTheme(Core.CardTheme theme)
        {
            var config = GameManager.Instance.Config;
            if (config != null) config.theme = theme;
            _singlePlayer = true;
            ShowNamesPanel();
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
            GameManager.Instance.StartGame();
        }

        private void ShowModePanel()
        {
            HideAllPanels();
            if (modePanel != null) modePanel.SetActive(true);
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
            if (themeTitle != null)
                themeTitle.text = Localization.Get("chooseTheme");
            SetThemeButtonSprite(carsThemeButton, "CarCards");
            SetThemeButtonSprite(princessThemeButton, "PrincessCards");
            SetThemeButtonLabel(colorsThemeButton, Localization.Get("themeColors"));
            StyleAsCard(carsThemeButton, new Vector2(0.02f, 0.4f), new Vector2(0.32f, 0.75f));
            StyleAsCard(princessThemeButton, new Vector2(0.35f, 0.4f), new Vector2(0.65f, 0.75f));
            StyleAsCard(colorsThemeButton, new Vector2(0.68f, 0.4f), new Vector2(0.98f, 0.75f));
            EnsureLockedCards(themePanel.transform);
        }

        private void SelectTheme(Core.CardTheme theme)
        {
            var config = GameManager.Instance.Config;
            if (config != null) config.theme = theme;
            ShowNamesPanel();
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
            // Add locked placeholder cards if not already created
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
                rect.anchorMin = new Vector2(xStart, 0.05f);
                rect.anchorMax = new Vector2(xEnd, 0.35f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                // Question mark
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
