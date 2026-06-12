using UnityEngine;
using System.Collections;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.GameFlow
{
    public class TimeAttackMode : MonoBehaviour, IGameMode
    {
        public int CurrentPlayerIndex => 0; // Always player

        [SerializeField] private float startTime = 60f;
        [SerializeField] private float mismatchPenalty = 3f;
        [SerializeField] private float matchBonus = 2f;

        private CardController _firstPick;
        private bool _waitingForResult;
        private int _pairsFound;
        private int _totalPairs;
        private float _timeRemaining;
        private bool _running;
        private GameConfig _config;

        public float TimeRemaining => _timeRemaining;
        public float StartTime => startTime;
        public int PairsFound => _pairsFound;
        public int TotalPairs => _totalPairs;

        public static event System.Action<float> OnTimeChanged; // timeRemaining
        public static event System.Action<int, int> OnPairsChanged; // found, total
        public static event System.Action<float> OnTimeAttackComplete; // finalTime (time left)
        public static event System.Action OnTimeUp; // ran out of time

        public static bool IsTimeAttackMode { get; set; }

        private void OnEnable() => GameEvents.OnGameStarted += OnGameStarted;
        private void OnDisable()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            StopAllCoroutines();
        }

        private void OnGameStarted()
        {
            _config = GameManager.Instance.Config;
            _totalPairs = _config.TotalSlots / 2; // No joker in Time Attack
            _pairsFound = 0;
            _firstPick = null;
            _waitingForResult = false;
            _timeRemaining = startTime;
            _running = true;
            OnTimeChanged?.Invoke(_timeRemaining);
            OnPairsChanged?.Invoke(_pairsFound, _totalPairs);
        }

        private void Update()
        {
            if (!_running) return;

            _timeRemaining -= Time.deltaTime;
            OnTimeChanged?.Invoke(_timeRemaining);

            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _running = false;
                OnTimeUp?.Invoke();
                Core.GPGSManager.Instance?.EventTimeAttackTimeout();
            }
        }

        public void OnCardSelected(CardController card)
        {
            if (!_running) return;
            if (_waitingForResult) return;
            if (!card.CanFlip) return;

            card.Flip();
            card.OnFlipComplete += OnCardFlipped;
        }

        private void OnCardFlipped(CardController card)
        {
            card.OnFlipComplete -= OnCardFlipped;

            if (card.Data.isPiotrus)
            {
                _waitingForResult = true;
                _timeRemaining = Mathf.Max(0f, _timeRemaining - mismatchPenalty * 2f);
                GameEvents.FirePiotrusFlipped(0);
                OnTimeChanged?.Invoke(_timeRemaining);
                StartCoroutine(HandlePiotrus(card));
                return;
            }

            if (_firstPick == null)
            {
                _firstPick = card;
            }
            else
            {
                _waitingForResult = true;
                if (_firstPick.Data.colorIndex == card.Data.colorIndex)
                    StartCoroutine(HandleMatch(_firstPick, card));
                else
                    StartCoroutine(HandleMismatch(_firstPick, card));
            }
        }

        private IEnumerator HandleMatch(CardController a, CardController b)
        {
            a.SetMatched();
            b.SetMatched();
            _pairsFound++;
            _timeRemaining += matchBonus;

            GameEvents.FirePairMatched(0, a.Data.colorIndex);
            OnPairsChanged?.Invoke(_pairsFound, _totalPairs);
            OnTimeChanged?.Invoke(_timeRemaining);

            yield return new WaitForSeconds(0.4f);

            _firstPick = null;
            _waitingForResult = false;

            if (_pairsFound >= _totalPairs)
            {
                _running = false;
                OnTimeAttackComplete?.Invoke(_timeRemaining);
            }
        }

        private IEnumerator HandleMismatch(CardController a, CardController b)
        {
            yield return new WaitForSeconds(0.8f);

            a.FlipBack(_config.cardBackColor);
            b.FlipBack(_config.cardBackColor);

            _timeRemaining = Mathf.Max(0f, _timeRemaining - mismatchPenalty);
            GameEvents.FirePairMismatched();
            OnTimeChanged?.Invoke(_timeRemaining);

            _firstPick = null;
            _waitingForResult = false;
        }

        private IEnumerator HandlePiotrus(CardController card)
        {
            yield return new WaitForSeconds(1f);
            if (card != null) card.FlipBack(_config.cardBackColor);
            if (_firstPick != null)
            {
                _firstPick.FlipBack(_config.cardBackColor);
                _firstPick = null;
            }
            _waitingForResult = false;
        }

        public void StartGame() { /* triggered by OnGameStarted */ }
    }
}
