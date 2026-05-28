using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class CollectedCardsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text titleText;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject cardSlotPrefab;
        [SerializeField] private Button player1Button;
        [SerializeField] private Button player2Button;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Close);
            player1Button?.onClick.AddListener(() => ShowPlayer(0));
            player2Button?.onClick.AddListener(() => ShowPlayer(1));
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnGameOver -= OnGameOver;
        }

        private void OnGameStarted()
        {
            if (player1Button != null)
            {
                player1Button.gameObject.SetActive(true);
                player1Button.GetComponentInChildren<Text>().text = Localization.Get("cards");
            }
            if (player2Button != null)
            {
                player2Button.gameObject.SetActive(true);
                player2Button.GetComponentInChildren<Text>().text = Localization.Get("cards");
            }
            Close();
        }

        private void OnGameOver(int _)
        {
            if (player1Button != null) player1Button.gameObject.SetActive(false);
            if (player2Button != null) player2Button.gameObject.SetActive(false);
            Close();
        }

        private void ShowPlayer(int playerIndex)
        {
            var collector = FindAnyObjectByType<PairCollector>();
            if (collector == null) return;

            var cards = playerIndex == 0 ? collector.Player1Cards : collector.Player2Cards;
            if (cards.Count == 0) return;

            ShowCards(playerIndex, cards);
        }

        private void ShowCards(int playerIndex, IReadOnlyList<CardController> cards)
        {
            if (panel == null) return;
            panel.SetActive(true);

            string name = playerIndex == 0 ? MainMenu.Player1Name : MainMenu.Player2Name;
            int pairCount = cards.Count / 2;
            bool pl = Localization.CurrentLanguage == Language.Polish;
            if (titleText != null)
                titleText.text = $"{name} - {pairCount} {(pl ? (pairCount == 1 ? "para" : "par") : (pairCount == 1 ? "pair" : "pairs"))}";

            if (closeButton != null)
                closeButton.GetComponentInChildren<Text>().text = pl ? "Zamknij" : "Close";

            // Clear old slots
            foreach (Transform child in contentContainer)
                Destroy(child.gameObject);

            // Show both cards of each pair
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var slot = Instantiate(cardSlotPrefab, contentContainer);
                slot.SetActive(true);

                var img = slot.GetComponent<Image>();
                if (card.Data.HasSprite)
                {
                    img.sprite = card.Data.faceSprite;
                    img.color = Color.white;
                }
                else
                {
                    img.sprite = null;
                    img.color = card.Data.faceColor;
                }
            }
        }

        private void Close()
        {
            if (panel != null) panel.SetActive(false);
        }
    }
}
