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

        private GameFlow.LocalGameMode _local;
        private GameFlow.SinglePlayerMode _single;
        private GameFlow.ChallengeMode _challenge;
        private GameFlow.TimeAttackMode _timeAttack;
        private Cards.CardGrid _grid;
        private ChallengeUI _challengeUI;
        private MainMenu _menu;

        private void Start()
        {
            _local = FindAnyObjectByType<GameFlow.LocalGameMode>(FindObjectsInactive.Include);
            _single = FindAnyObjectByType<GameFlow.SinglePlayerMode>(FindObjectsInactive.Include);
            _challenge = FindAnyObjectByType<GameFlow.ChallengeMode>(FindObjectsInactive.Include);
            _timeAttack = FindAnyObjectByType<GameFlow.TimeAttackMode>(FindObjectsInactive.Include);
            _grid = FindAnyObjectByType<Cards.CardGrid>(FindObjectsInactive.Include);
            _challengeUI = FindAnyObjectByType<ChallengeUI>(FindObjectsInactive.Include);
            _menu = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);

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
            if (confirmPanel != null) confirmPanel.SetActive(false);
        }

        private void OnGameOver(int _)
        {
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);
            HideConfirm();
        }

        private void ShowConfirm()
        {
            // Immediately stop all game modes so AI/timers don't continue
            StopGameModes();
            if (confirmPanel != null) confirmPanel.SetActive(true);
            var texts = confirmPanel.GetComponentsInChildren<Text>();
            foreach (var t in texts)
            {
                if (t.gameObject.name == "ConfirmText")
                    t.text = Localization.Get("backToMenu");
            }
            if (yesButton != null)
            {
                var t = yesButton.GetComponentInChildren<Text>();
                if (t != null) t.text = Localization.Get("yes");
            }
            if (noButton != null)
            {
                var t = noButton.GetComponentInChildren<Text>();
                if (t != null) t.text = Localization.Get("no");
            }
        }

        private void HideConfirm()
        {
            if (confirmPanel != null) confirmPanel.SetActive(false);
        }

        private void StopGameModes()
        {
            if (_local != null) { _local.StopAllCoroutines(); _local.enabled = false; }
            if (_single != null) { _single.StopAllCoroutines(); _single.enabled = false; }
            if (_challenge != null) { _challenge.StopAllCoroutines(); _challenge.enabled = false; }
            if (_timeAttack != null) { _timeAttack.StopAllCoroutines(); _timeAttack.enabled = false; }
        }

        private void BackToMenu()
        {
            HideConfirm();
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Clear cards
            if (_grid != null)
            {
                var children = new System.Collections.Generic.List<GameObject>();
                foreach (Transform child in _grid.transform)
                    if (child != null) children.Add(child.gameObject);
                foreach (var go in children)
                    if (go != null) Destroy(go);
            }

            // Return to menu
            if (_challengeUI != null) _challengeUI.SendMessage("HideAll", SendMessageOptions.DontRequireReceiver);
            if (_menu != null) _menu.ReturnToMenu();
        }
    }
}
