using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        private void Start()
        {
            pauseButton?.onClick.AddListener(ShowConfirm);
            yesButton?.onClick.AddListener(BackToMenu);
            noButton?.onClick.AddListener(HideConfirm);
            if (confirmPanel != null) confirmPanel.SetActive(false);
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameOver += OnGameOver;
        }

        private void OnDestroy()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnGameOver -= OnGameOver;
        }

        private void OnGameStarted()
        {
            if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        }

        private void OnGameOver(int _)
        {
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);
            HideConfirm();
        }

        private void ShowConfirm()
        {
            if (confirmPanel != null) confirmPanel.SetActive(true);
        }

        private void HideConfirm()
        {
            if (confirmPanel != null) confirmPanel.SetActive(false);
        }

        private void BackToMenu()
        {
            HideConfirm();
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Clear cards
            var grid = FindAnyObjectByType<Cards.CardGrid>();
            if (grid != null)
            {
                foreach (Transform child in grid.transform)
                    Destroy(child.gameObject);
            }

            // Return to menu
            var menu = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);
            if (menu != null) menu.ReturnToMenu();
        }
    }
}
