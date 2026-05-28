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
        [SerializeField] private GameObject languagePanel;
        [SerializeField] private GameObject modePanel;
        [SerializeField] private GameObject difficultyPanel;
        [SerializeField] private GameObject namesPanel;

        [Header("Language Panel")]
        [SerializeField] private Text languageTitle;
        [SerializeField] private Button polishButton;
        [SerializeField] private Button englishButton;
        [SerializeField] private Button quitButton;

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

        [Header("Names Panel")]
        [SerializeField] private InputField player1Input;
        [SerializeField] private InputField player2Input;
        [SerializeField] private Text player1Label;
        [SerializeField] private Text player2Label;
        [SerializeField] private Button startButton;
        [SerializeField] private Text startButtonText;
        [SerializeField] private Button namesBackButton;

        public static string Player1Name { get; private set; } = "Gracz 1";
        public static string Player2Name { get; private set; } = "Gracz 2";
        public static bool IsSinglePlayer { get; private set; }
        public static Difficulty SelectedDifficulty { get; private set; }

        private bool _singlePlayer;

        private void Start()
        {
            polishButton?.onClick.AddListener(() => SelectLanguage(Language.Polish));
            englishButton?.onClick.AddListener(() => SelectLanguage(Language.English));
            quitButton?.onClick.AddListener(() => Application.Quit());
            twoPlayersButton?.onClick.AddListener(() => SelectMode(false));
            singlePlayerButton?.onClick.AddListener(() => SelectMode(true));
            easyButton?.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
            mediumButton?.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
            hardButton?.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
            startButton?.onClick.AddListener(OnStart);
            modeBackButton?.onClick.AddListener(ShowLanguagePanel);
            difficultyBackButton?.onClick.AddListener(ShowModePanel);
            namesBackButton?.onClick.AddListener(ShowDifficultyPanel);

            ShowLanguagePanel();
        }

        private void ShowLanguagePanel()
        {
            if (menuPanel != null) menuPanel.SetActive(true);
            if (languagePanel != null) languagePanel.SetActive(true);
            if (modePanel != null) modePanel.SetActive(false);
            if (difficultyPanel != null) difficultyPanel.SetActive(false);
            if (namesPanel != null) namesPanel.SetActive(false);
            if (languageTitle != null) languageTitle.text = "Choose Language / Wybierz język";
        }

        private void SelectLanguage(Language lang)
        {
            Localization.CurrentLanguage = lang;
            ShowModePanel();
        }

        private void ShowModePanel()
        {
            if (languagePanel != null) languagePanel.SetActive(false);
            if (modePanel != null) modePanel.SetActive(true);
            if (difficultyPanel != null) difficultyPanel.SetActive(false);
            if (namesPanel != null) namesPanel.SetActive(false);
            if (modeTitle != null) modeTitle.text = Localization.CurrentLanguage == Language.Polish
                ? "Wybierz tryb" : "Choose mode";
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
            if (modePanel != null) modePanel.SetActive(false);
            if (difficultyPanel != null) difficultyPanel.SetActive(true);
            if (namesPanel != null) namesPanel.SetActive(false);
            if (difficultyTitle != null)
                difficultyTitle.text = Localization.CurrentLanguage == Language.Polish
                    ? "Wybierz poziom trudności" : "Choose difficulty";
        }

        private void SelectDifficulty(Difficulty diff)
        {
            SelectedDifficulty = diff;
            ApplyDifficulty(diff);
            ShowNamesPanel();
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
            if (difficultyPanel != null) difficultyPanel.SetActive(false);
            if (namesPanel != null) namesPanel.SetActive(true);

            if (player1Label != null) player1Label.text = Localization.Get("player1Name");
            if (player1Input != null)
                player1Input.placeholder.GetComponent<Text>().text = Localization.Get("player1");

            if (player2Label != null) player2Label.gameObject.SetActive(!_singlePlayer);
            if (player2Input != null) player2Input.gameObject.SetActive(!_singlePlayer);

            if (!_singlePlayer && player2Label != null)
                player2Label.text = Localization.Get("player2Name");
            if (!_singlePlayer && player2Input != null)
                player2Input.placeholder.GetComponent<Text>().text = Localization.Get("player2");

            if (startButtonText != null) startButtonText.text = Localization.Get("start");
        }

        private void OnStart()
        {
            IsSinglePlayer = _singlePlayer;

            Player1Name = string.IsNullOrWhiteSpace(player1Input.text)
                ? Localization.Get("player1") : player1Input.text;

            if (_singlePlayer)
                Player2Name = Localization.Get("computer");
            else
                Player2Name = string.IsNullOrWhiteSpace(player2Input.text)
                    ? Localization.Get("player2") : player2Input.text;

            var local = FindAnyObjectByType<GameFlow.LocalGameMode>(FindObjectsInactive.Include);
            var single = FindAnyObjectByType<GameFlow.SinglePlayerMode>(FindObjectsInactive.Include);

            if (local != null) local.enabled = !_singlePlayer;
            if (single != null) single.enabled = _singlePlayer;

            if (menuPanel != null) menuPanel.SetActive(false);
            GameManager.Instance.StartGame();
        }
    }
}
