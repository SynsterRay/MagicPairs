using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class GameBackground : MonoBehaviour
    {
        private Image _image;

        private void Awake() => _image = GetComponent<Image>();

        private void OnEnable() => GameEvents.OnGameStarted += UpdateBackground;
        private void OnDisable() => GameEvents.OnGameStarted -= UpdateBackground;

        private void UpdateBackground()
        {
            if (_image == null) return;
            var config = GameManager.Instance?.Config;
            if (config == null) return;

            string path = config.theme == CardTheme.Cars
                ? "Backgrounds/background_cars"
                : "Backgrounds/background_game";

            var tex = Resources.Load<Texture2D>(path);
            if (tex != null)
                _image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}
