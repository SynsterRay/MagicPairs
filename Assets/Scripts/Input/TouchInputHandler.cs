using UnityEngine;
using UnityEngine.InputSystem;
using MagicPairs.Cards;
using MagicPairs.GameFlow;

namespace MagicPairs.Input
{
    public class TouchInputHandler : MonoBehaviour
    {
        private UnityEngine.Camera _cam;
        private IGameMode _gameMode;

        private void Start()
        {
            _cam = UnityEngine.Camera.main;
            _gameMode = GetComponent<IGameMode>() ?? GetComponentInParent<IGameMode>();
            if (_gameMode == null)
                _gameMode = FindAnyObjectByType<LocalGameMode>();
        }

        private void Update()
        {
            if (_gameMode == null || _cam == null) return;

            bool tapped = false;
            Vector2 screenPos = Vector2.zero;

            // Touch
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                tapped = true;
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            // Mouse fallback (editor/desktop)
            else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                tapped = true;
                screenPos = Mouse.current.position.ReadValue();
            }

            if (!tapped) return;

            Ray ray = _cam.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var card = hit.collider.GetComponent<CardController>();
                if (card != null)
                    _gameMode.OnCardSelected(card);
            }
        }
    }
}
