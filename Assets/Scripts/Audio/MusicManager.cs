using UnityEngine;
using MagicPairs.Core;

namespace MagicPairs.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private const string MenuMusicKey = "MagicPairs_MenuMusic";
        private const string GameMusicKey = "MagicPairs_GameMusic";
        private const float CrossfadeDuration = 0.1f;

        private AudioSource _menuSourceA;
        private AudioSource _menuSourceB;
        private AudioSource _gameSourceA;
        private AudioSource _gameSourceB;

        private AudioClip _menuClip;
        private AudioClip _gameClip;

        private float _menuVolume = 0.4f;
        private float _gameVolume = 0.3f;

        private bool _menuPlaying;
        private bool _gamePlaying;

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

            _menuClip = Resources.Load<AudioClip>("Audio/Menu_Music");
            _gameClip = Resources.Load<AudioClip>("Audio/In_Game_Music");

            _menuSourceA = CreateSource(_menuClip, _menuVolume);
            _menuSourceB = CreateSource(_menuClip, _menuVolume);
            _gameSourceA = CreateSource(_gameClip, _gameVolume);
            _gameSourceB = CreateSource(_gameClip, _gameVolume);
        }

        private AudioSource CreateSource(AudioClip clip, float volume)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = clip;
            src.loop = false;
            src.playOnAwake = false;
            src.volume = volume;
            return src;
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

        private void Update()
        {
            if (_menuPlaying && _menuClip != null)
                HandleCrossfade(_menuSourceA, _menuSourceB, _menuClip, _menuVolume);
            if (_gamePlaying && _gameClip != null)
                HandleCrossfade(_gameSourceA, _gameSourceB, _gameClip, _gameVolume);
        }

        private void HandleCrossfade(AudioSource srcA, AudioSource srcB, AudioClip clip, float maxVol)
        {
            float clipLen = clip.length;
            float fadeStart = clipLen - CrossfadeDuration;

            if (srcA.isPlaying && srcA.time >= fadeStart)
            {
                float t = (srcA.time - fadeStart) / CrossfadeDuration;
                srcA.volume = Mathf.Lerp(maxVol, 0f, t);

                if (!srcB.isPlaying)
                {
                    srcB.time = 0f;
                    srcB.volume = 0f;
                    srcB.Play();
                }
                srcB.volume = Mathf.Lerp(0f, maxVol, t);
            }
            else if (srcB.isPlaying && srcB.time >= fadeStart)
            {
                float t = (srcB.time - fadeStart) / CrossfadeDuration;
                srcB.volume = Mathf.Lerp(maxVol, 0f, t);

                if (!srcA.isPlaying)
                {
                    srcA.time = 0f;
                    srcA.volume = 0f;
                    srcA.Play();
                }
                srcA.volume = Mathf.Lerp(0f, maxVol, t);
            }
            else if (!srcA.isPlaying && !srcB.isPlaying)
            {
                srcA.volume = maxVol;
                srcA.Play();
            }
        }

        private void OnGameStarted()
        {
            StopMenuMusic();
            PlayGameMusic();
        }

        public void PlayMenuMusic()
        {
            StopGame();
            if (_menuMusicOn && _menuClip != null)
            {
                _menuPlaying = true;
                _menuSourceA.volume = _menuVolume;
                _menuSourceA.time = 0f;
                _menuSourceA.Play();
            }
        }

        private void PlayGameMusic()
        {
            if (_gameMusicOn && _gameClip != null)
            {
                _gamePlaying = true;
                _gameSourceA.volume = _gameVolume;
                _gameSourceA.time = 0f;
                _gameSourceA.Play();
            }
        }

        private void StopMenuMusic()
        {
            _menuPlaying = false;
            _menuSourceA.Stop();
            _menuSourceB.Stop();
        }

        private void StopGame()
        {
            _gamePlaying = false;
            _gameSourceA.Stop();
            _gameSourceB.Stop();
        }

        public void StopGameMusic()
        {
            StopGame();
        }

        private void ApplySettings()
        {
            if (!_menuMusicOn) StopMenuMusic();
            if (!_gameMusicOn) StopGame();

            if (_menuMusicOn && !_gamePlaying && !_menuPlaying)
                PlayMenuMusic();
        }
    }
}
