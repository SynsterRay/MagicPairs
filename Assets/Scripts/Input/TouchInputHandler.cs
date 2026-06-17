using UnityEngine;
using UnityEngine.InputSystem;
using MagicPairs.Cards;
using MagicPairs.GameFlow;

namespace MagicPairs.Input
{
    public class TouchInputHandler : MonoBehaviour
    {
        private UnityEngine.Camera _cam;
        private TimeAttackMode _timeAttack;
        private ChallengeMode _challenge;
        private SinglePlayerMode _single;
        private LocalGameMode _local;

        private void Start()
        {
            _cam = UnityEngine.Camera.main;
            _timeAttack = GetComponent<TimeAttackMode>();
            _challenge = GetComponent<ChallengeMode>();
            _single = GetComponent<SinglePlayerMode>();
            _local = GetComponent<LocalGameMode>();
        }

        private IGameMode GetActiveMode()
        {
            if (_timeAttack != null && _timeAttack.enabled) return _timeAttack;
            if (_challenge != null && _challenge.enabled) return _challenge;
            if (_single != null && _single.enabled) return _single;
            if (_local != null && _local.enabled) return _local;
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
