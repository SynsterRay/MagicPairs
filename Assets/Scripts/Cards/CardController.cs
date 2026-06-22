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
        private Sprite _backSprite;
        private static Audio.SFXManager _sfxCache;

        private static Sprite _backSpriteCache;
        private static string _backSpriteCacheFolder;

        public void Initialize(CardData data, Color backColor)
        {
            Data = data;
            _animator = GetComponent<CardAnimator>();
            if (_sfxCache == null) _sfxCache = FindAnyObjectByType<Audio.SFXManager>();

            // Load back sprite for sprite-based themes (cached per folder)
            if (data.HasSprite)
            {
                string folder = Core.GameManager.Instance?.Config?.theme switch
                {
                    Core.CardTheme.Cars => "CarCards",
                    Core.CardTheme.Dinos => "WaterWorldCards",
                    Core.CardTheme.Animals => "AnimalCards",
                    _ => "PrincessCards"
                };

                if (_backSpriteCache == null || _backSpriteCacheFolder != folder)
                {
                    _backSpriteCacheFolder = folder;
                    _backSpriteCache = Resources.Load<Sprite>($"{folder}/back_card");
                    if (_backSpriteCache == null)
                        _backSpriteCache = Resources.Load<Sprite>($"{folder}/cars_back");
                    if (_backSpriteCache == null)
                    {
                        var tex = Resources.Load<Texture2D>($"{folder}/back_card")
                               ?? Resources.Load<Texture2D>($"{folder}/cars_back");
                        if (tex != null)
                            _backSpriteCache = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                    }
                }
                _backSprite = _backSpriteCache;

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

        /// <summary>Reveal card for peek power-up without triggering game logic.</summary>
        public void PeekReveal()
        {
            if (!CanFlip) return;
            State = CardState.Animating;
            _animator.PlayFlip(Data.faceColor, () =>
            {
                State = CardState.FaceUp;
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
