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
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }

        private void OnDestroy()
        {
            if (Instance == this) GameEvents.ClearAll();
        }

        public void StartGame()
        {
            GameEvents.FireGameStarted();
        }
    }
}
