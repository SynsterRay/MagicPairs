using UnityEngine;
using MagicPairs.Core;
using MagicPairs.GameFlow;

namespace MagicPairs.Audio
{
    public class SFXManager : MonoBehaviour
    {
        [Header("Clips")]
        [SerializeField] private AudioClip cardFlip;
        [SerializeField] private AudioClip pairMatch;
        [SerializeField] private AudioClip mismatch;
        [SerializeField] private AudioClip joker;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip gameOver;
        [SerializeField] private AudioClip levelComplete;

        [Header("Volume")]
        [SerializeField] [Range(0f, 1f)] private float volume = 0.7f;

        private AudioSource _source;

        private void Awake()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            LoadClipsFromResources();
        }

        private void LoadClipsFromResources()
        {
            if (cardFlip == null) cardFlip = Resources.Load<AudioClip>("Audio/Card_Flip");
            if (pairMatch == null) pairMatch = Resources.Load<AudioClip>("Audio/Pair_Match");
            if (mismatch == null) mismatch = Resources.Load<AudioClip>("Audio/Pair_Mismatch");
            if (joker == null) joker = Resources.Load<AudioClip>("Audio/Joker");
            if (buttonClick == null) buttonClick = Resources.Load<AudioClip>("Audio/Button_Click");
            if (levelComplete == null) levelComplete = Resources.Load<AudioClip>("Audio/Level_Complete");
        }

        private void OnEnable()
        {
            GameEvents.OnPairMatched += OnMatch;
            GameEvents.OnPairMismatched += OnMismatch;
            GameEvents.OnPiotrusFlipped += OnJoker;
            GameEvents.OnGameOver += OnGameOver;
            ChallengeMode.OnLevelComplete += OnLevelComplete;
        }

        private void OnDisable()
        {
            GameEvents.OnPairMatched -= OnMatch;
            GameEvents.OnPairMismatched -= OnMismatch;
            GameEvents.OnPiotrusFlipped -= OnJoker;
            GameEvents.OnGameOver -= OnGameOver;
            ChallengeMode.OnLevelComplete -= OnLevelComplete;
        }

        private void OnMatch(int _, int __) => Play(pairMatch);
        private void OnMismatch() => Play(mismatch);
        private void OnJoker(int _) => Play(joker);
        private void OnGameOver(int _) => Play(gameOver);
        private void OnLevelComplete(int _) => Play(levelComplete);

        public void PlayFlip() => Play(cardFlip);
        public void PlayButton() => Play(buttonClick);
        public void PlayLevelComplete() => Play(levelComplete);

        private void Play(AudioClip clip)
        {
            if (clip != null && _source != null)
                _source.PlayOneShot(clip, volume);
        }
    }
}
