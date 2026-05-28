using UnityEngine;
using UnityEngine.InputSystem;
using MagicPairs.Cards;
using MagicPairs.GameFlow;

namespace MagicPairs.Input
{
    public class TouchInputHandler : MonoBehaviour
    {
        private UnityEngine.Camera _cam;

        private void Start()
        {
            _cam = UnityEngine.Camera.main;
        }

        private IGameMode GetActiveMode()
        {
            var challenge = GetComponent<ChallengeMode>();
            if (challenge != null && challenge.enabled) return challenge;
            var single = GetComponent<SinglePlayerMode>();
            if (single != null && single.enabled) return single;
            var local = GetComponent<LocalGameMode>();
            if (local != null && local.enabled) return local;
            return null;
        }

        private void Update()
        {
            if (_cam == null) return;

            bool tapped = false;
            Vector2 screenPos = Vector2.zero;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                tapped = true;
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
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
                    GetActiveMode()?.OnCardSelected(card);
            }
        }
    }
}
