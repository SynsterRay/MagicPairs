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
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameOver += OnGameOver;
        }

        private void OnDisable()
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
            var texts = confirmPanel.GetComponentsInChildren<Text>();
            foreach (var t in texts)
            {
                if (t.gameObject.name == "ConfirmText")
                    t.text = Localization.Get("backToMenu");
            }
            if (yesButton != null)
                yesButton.GetComponentInChildren<Text>().text = Localization.Get("yes");
            if (noButton != null)
                noButton.GetComponentInChildren<Text>().text = Localization.Get("no");
        }

        private void HideConfirm()
        {
            if (confirmPanel != null) confirmPanel.SetActive(false);
        }

        private void BackToMenu()
        {
            HideConfirm();
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Stop all game mode coroutines first
            var local = FindAnyObjectByType<GameFlow.LocalGameMode>(FindObjectsInactive.Include);
            var single = FindAnyObjectByType<GameFlow.SinglePlayerMode>(FindObjectsInactive.Include);
            var challenge = FindAnyObjectByType<GameFlow.ChallengeMode>(FindObjectsInactive.Include);
            var timeAttack = FindAnyObjectByType<GameFlow.TimeAttackMode>(FindObjectsInactive.Include);
            if (local != null) { local.StopAllCoroutines(); local.enabled = false; }
            if (single != null) { single.StopAllCoroutines(); single.enabled = false; }
            if (challenge != null) { challenge.StopAllCoroutines(); challenge.enabled = false; }
            if (timeAttack != null) { timeAttack.StopAllCoroutines(); timeAttack.enabled = false; }

            // Clear cards
            var grid = FindAnyObjectByType<Cards.CardGrid>();
            if (grid != null)
            {
                var children = new System.Collections.Generic.List<GameObject>();
                foreach (Transform child in grid.transform)
                    if (child != null) children.Add(child.gameObject);
                foreach (var go in children)
                    if (go != null) Destroy(go);
            }

            // Return to menu
            var challengeUI = FindAnyObjectByType<ChallengeUI>(FindObjectsInactive.Include);
            if (challengeUI != null) challengeUI.SendMessage("HideAll", SendMessageOptions.DontRequireReceiver);

            var menu = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);
            if (menu != null) menu.ReturnToMenu();
        }
    }
}
