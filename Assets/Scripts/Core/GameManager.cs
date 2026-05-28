using UnityEngine;

namespace MagicPairs.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameConfig config;

        public GameConfig Config => config;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            GameEvents.FireGameStarted();
        }
    }
}
