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
        private Sprite _backSprite;
        private static Audio.SFXManager _sfxCache;

        public void Initialize(CardData data, Color backColor)
        {
            Data = data;
            _backColor = backColor;
            _animator = GetComponent<CardAnimator>();
            if (_sfxCache == null) _sfxCache = FindAnyObjectByType<Audio.SFXManager>();

            // Load back sprite for Princess theme
            if (data.HasSprite)
            {
                _backSprite = Resources.Load<Sprite>("PrincessCards/back_card");
                if (_backSprite == null)
                {
                    var tex = Resources.Load<Texture2D>("PrincessCards/back_card");
                    if (tex != null)
                        _backSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }

                if (_backSprite != null)
                    _animator.SetSprite(_backSprite);
                else
                    _animator.SetColor(backColor);
            }
            else
            {
                _animator.SetColor(backColor);
            }

            State = CardState.FaceDown;
        }

        public bool CanFlip => State == CardState.FaceDown;

        public void Flip()
        {
            if (!CanFlip) return;
            State = CardState.Animating;
            _sfxCache?.PlayFlip();
            _animator.PlayFlip(Data.faceColor, () =>
            {
                State = CardState.FaceUp;
                if (Data.isPiotrus && !Data.HasSprite)
                    _animator.ShowJokerSymbol();
                OnFlipComplete?.Invoke(this);
            }, Data.faceSprite);
        }

        public void FlipBack(Color backColor)
        {
            if (_animator == null || !_animator.isActiveAndEnabled) return;
            State = CardState.Animating;
            _animator.HideJokerSymbol();
            _animator.PlayFlipBack(backColor, () =>
            {
                if (this == null) return;
                State = CardState.FaceDown;
                if (_backSprite != null)
                    _animator.SetSprite(_backSprite);
            });
        }

        public void SetMatched()
        {
            State = CardState.Matched;
        }
    }
}
