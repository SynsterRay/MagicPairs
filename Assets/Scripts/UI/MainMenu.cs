using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private InputField player1Input;
        [SerializeField] private InputField player2Input;
        [SerializeField] private Button startButton;
        [SerializeField] private Button languageButton;
        [SerializeField] private Text languageButtonText;
        [SerializeField] private Text player1Label;
        [SerializeField] private Text player2Label;
        [SerializeField] private Text startButtonText;
        [SerializeField] private Text titleText;

        public static string Player1Name { get; private set; } = "Gracz 1";
        public static string Player2Name { get; private set; } = "Gracz 2";

        private void Start()
        {
            startButton?.onClick.AddListener(OnStart);
            languageButton?.onClick.AddListener(OnToggleLanguage);
            menuPanel.SetActive(true);
            RefreshTexts();
        }

        private void OnToggleLanguage()
        {
            Localization.CurrentLanguage = Localization.CurrentLanguage == Language.Polish
                ? Language.English : Language.Polish;
            RefreshTexts();
        }

        private void RefreshTexts()
        {
            if (languageButtonText != null)
                languageButtonText.text = Localization.CurrentLanguage == Language.Polish ? "PL" : "EN";
            if (player1Label != null) player1Label.text = Localization.Get("player1Name");
            if (player2Label != null) player2Label.text = Localization.Get("player2Name");
            if (startButtonText != null) startButtonText.text = Localization.Get("start");
            if (titleText != null) titleText.text = "Magic Pairs";

            if (player1Input != null)
                player1Input.placeholder.GetComponent<Text>().text = Localization.Get("player1");
            if (player2Input != null)
                player2Input.placeholder.GetComponent<Text>().text = Localization.Get("player2");
        }

        private void OnStart()
        {
            Player1Name = string.IsNullOrWhiteSpace(player1Input.text)
                ? Localization.Get("player1") : player1Input.text;
            Player2Name = string.IsNullOrWhiteSpace(player2Input.text)
                ? Localization.Get("player2") : player2Input.text;

            menuPanel.SetActive(false);
            GameManager.Instance.StartGame();
        }
    }
}
