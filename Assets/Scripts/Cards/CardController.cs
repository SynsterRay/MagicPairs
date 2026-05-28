using UnityEngine;
using System;

namespace MagicPairs.Cards
{
    public enum CardState { FaceDown, Animating, FaceUp, Matched }

    public class CardController : MonoBehaviour
    {
        public CardData Data { get; private set; }
        public CardState State { get; private set; } = CardState.FaceDown;

        public event Action<CardController> OnFlipComplete;

        private CardAnimator _animator;
        private Color _backColor;

        public void Initialize(CardData data, Color backColor)
        {
            Data = data;
            _backColor = backColor;
            _animator = GetComponent<CardAnimator>();
            _animator.SetColor(backColor);
            State = CardState.FaceDown;
        }

        public bool CanFlip => State == CardState.FaceDown;

        public void Flip()
        {
            if (!CanFlip) return;
            State = CardState.Animating;
            _animator.PlayFlip(Data.faceColor, () =>
            {
                State = CardState.FaceUp;
                OnFlipComplete?.Invoke(this);
            }, Data.faceSprite);
        }

        public void FlipBack(Color backColor)
        {
            State = CardState.Animating;
            _animator.PlayFlipBack(backColor, () =>
            {
                State = CardState.FaceDown;
            });
        }

        public void SetMatched()
        {
            State = CardState.Matched;
        }
    }
}
