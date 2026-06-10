using UnityEngine;
using MagicPairs.Core;

namespace MagicPairs.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private const string MenuMusicKey = "MagicPairs_MenuMusic";
        private const string GameMusicKey = "MagicPairs_GameMusic";

        private AudioSource _menuSource;
        private AudioSource _gameSource;

        private static bool _menuMusicOn;
        private static bool _gameMusicOn;
        private static bool _initialized;

        private static void EnsureInit()
        {
            if (_initialized) return;
            _initialized = true;
            try
            {
                _menuMusicOn = PlayerPrefs.GetInt(MenuMusicKey, 1) == 1;
                _gameMusicOn = PlayerPrefs.GetInt(GameMusicKey, 1) == 1;
            }
            catch
            {
                _menuMusicOn = true;
                _gameMusicOn = true;
                _initialized = false;
            }
        }

        public static bool MenuMusicOn
        {
            get { EnsureInit(); return _menuMusicOn; }
            set
            {
                _menuMusicOn = value;
                _initialized = true;
                PlayerPrefs.SetInt(MenuMusicKey, value ? 1 : 0);
                PlayerPrefs.Save();
                OnSettingsChanged?.Invoke();
            }
        }

        public static bool GameMusicOn
        {
            get { EnsureInit(); return _gameMusicOn; }
            set
            {
                _gameMusicOn = value;
                _initialized = true;
                PlayerPrefs.SetInt(GameMusicKey, value ? 1 : 0);
                PlayerPrefs.Save();
                OnSettingsChanged?.Invoke();
            }
        }

        public static event System.Action OnSettingsChanged;

        private void Awake()
        {
            EnsureInit();

            _menuSource = gameObject.AddComponent<AudioSource>();
            _menuSource.loop = true;
            _menuSource.playOnAwake = false;
            _menuSource.volume = 0.4f;
            _menuSource.clip = Resources.Load<AudioClip>("Audio/Menu_Music");

            _gameSource = gameObject.AddComponent<AudioSource>();
            _gameSource.loop = true;
            _gameSource.playOnAwake = false;
            _gameSource.volume = 0.3f;
            _gameSource.clip = Resources.Load<AudioClip>("Audio/In_Game_Music");
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += OnGameStarted;
            OnSettingsChanged += ApplySettings;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            OnSettingsChanged -= ApplySettings;
        }

        private void Start()
        {
            PlayMenuMusic();
        }

        private void OnGameStarted()
        {
            StopMenuMusic();
            PlayGameMusic();
        }

        public void PlayMenuMusic()
        {
            _gameSource.Stop();
            if (_menuMusicOn && _menuSource.clip != null)
                _menuSource.Play();
            else
                _menuSource.Stop();
        }

        private void PlayGameMusic()
        {
            if (_gameMusicOn && _gameSource.clip != null)
                _gameSource.Play();
            else
                _gameSource.Stop();
        }

        private void StopMenuMusic()
        {
            _menuSource.Stop();
        }

        public void StopGameMusic()
        {
            _gameSource.Stop();
        }

        private void ApplySettings()
        {
            if (_menuSource.isPlaying && !_menuMusicOn)
                _menuSource.Stop();
            else if (!_menuSource.isPlaying && _menuMusicOn && !_gameSource.isPlaying && _menuSource.clip != null)
                _menuSource.Play();

            if (_gameSource.isPlaying && !_gameMusicOn)
                _gameSource.Stop();
            else if (!_gameSource.isPlaying && _gameMusicOn && !_menuSource.isPlaying && _gameSource.clip != null)
                _gameSource.Play();
        }
    }
}
