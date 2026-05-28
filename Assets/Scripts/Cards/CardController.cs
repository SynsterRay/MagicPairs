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
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private MaterialPropertyBlock _propBlock;
        private Renderer _renderer;

        public void Initialize(CardData data, Color backColor)
        {
            Data = data;
            _animator = GetComponent<CardAnimator>();
            _renderer = GetComponentInChildren<Renderer>();
            _propBlock = new MaterialPropertyBlock();
            SetColor(backColor);
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
            });
        }

        public void FlipBack(Color backColor)
        {
            State = CardState.Animating;
            _animator.PlayFlip(backColor, () =>
            {
                State = CardState.FaceDown;
            });
        }

        public void SetMatched()
        {
            State = CardState.Matched;
        }

        private void SetColor(Color color)
        {
            if (_renderer == null) return;
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(BaseColorId, color);
            _propBlock.SetColor(ColorId, color);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
