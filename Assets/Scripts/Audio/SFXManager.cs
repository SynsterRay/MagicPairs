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

        private void Play(AudioClip clip)
        {
            if (clip != null && _source != null)
                _source.PlayOneShot(clip, volume);
        }
    }
}
