using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicPairs.Core;

namespace MagicPairs.Cards
{
    public class PairCollector : MonoBehaviour
    {
        [SerializeField] private float collectDelay = 0.5f;
        [SerializeField] private float moveSpeed = 0.4f;
        [SerializeField] private float cardScale = 0.75f;

        private List<CardController> _player1Cards = new();
        private List<CardController> _player2Cards = new();

        private void OnEnable()
        {
            GameEvents.OnGameStarted += Reset;
            GameEvents.OnPairMatched += OnPairMatched;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= Reset;
            GameEvents.OnPairMatched -= OnPairMatched;
        }

        private void Reset()
        {
            _player1Cards.Clear();
            _player2Cards.Clear();
        }

        private void OnPairMatched(int playerIndex, int colorIndex)
        {
            var grid = GetComponent<CardGrid>();
            if (grid == null) return;

            var matched = new List<CardController>();
            foreach (var card in grid.Cards)
            {
                if (card != null && card.State == CardState.Matched
                    && card.Data.colorIndex == colorIndex && !IsCollected(card))
                {
                    matched.Add(card);
                }
            }

            if (matched.Count >= 2)
                StartCoroutine(CollectPair(playerIndex, matched[0], matched[1]));
        }

        private bool IsCollected(CardController card)
        {
            return _player1Cards.Contains(card) || _player2Cards.Contains(card);
        }

        private IEnumerator CollectPair(int playerIndex, CardController a, CardController b)
        {
            yield return new WaitForSeconds(collectDelay);

            var list = playerIndex == 0 ? _player1Cards : _player2Cards;
            list.Add(a);
            list.Add(b);

            // Disable colliders
            var colA = a.GetComponent<Collider>();
            var colB = b.GetComponent<Collider>();
            if (colA != null) colA.enabled = false;
            if (colB != null) colB.enabled = false;

            // Reposition all collected cards for this player
            RepositionCards(playerIndex);
        }

        private void RepositionCards(int playerIndex)
        {
            var cam = UnityEngine.Camera.main;
            if (cam == null) return;

            float orthoSize = cam.orthographicSize;
            float aspect = cam.aspect;
            float halfWidth = orthoSize * aspect;

            var list = playerIndex == 0 ? _player1Cards : _player2Cards;
            int pairCount = list.Count / 2;

            float scaledWidth = 0.7f * cardScale;
            float scaledHeight = 1.0f * cardScale;
            float rowSpacing = scaledHeight + 0.15f;

            // P1 under left score label, P2 under right score label
            float centerX = playerIndex == 0 ? -halfWidth * 0.5f : halfWidth * 0.5f;

            // Start from vertical middle of screen, going downward
            float startY = 0f;

            for (int i = 0; i < pairCount; i++)
            {
                float y = startY - i * rowSpacing;

                Vector3 posA = new Vector3(centerX - scaledWidth * 0.5f - 0.04f, y, 0f);
                Vector3 posB = new Vector3(centerX + scaledWidth * 0.5f + 0.04f, y, 0f);

                int cardIdxA = i * 2;
                int cardIdxB = i * 2 + 1;

                if (cardIdxA < list.Count)
                    StartCoroutine(MoveAndShrink(list[cardIdxA].transform, posA));
                if (cardIdxB < list.Count)
                    StartCoroutine(MoveAndShrink(list[cardIdxB].transform, posB));
            }
        }

        private IEnumerator MoveAndShrink(Transform t, Vector3 target)
        {
            Vector3 startPos = t.position;
            Vector3 startScale = t.localScale;
            Vector3 endScale = new Vector3(0.7f * cardScale, 1.0f * cardScale, 1f);
            float elapsed = 0f;

            while (elapsed < moveSpeed)
            {
                elapsed += Time.deltaTime;
                float ratio = Mathf.SmoothStep(0f, 1f, elapsed / moveSpeed);
                t.position = Vector3.Lerp(startPos, target, ratio);
                t.localScale = Vector3.Lerp(startScale, endScale, ratio);
                yield return null;
            }

            t.position = target;
            t.localScale = endScale;
        }
    }
}
