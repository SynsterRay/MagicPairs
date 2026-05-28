using UnityEngine;
using TMPro;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class TurnIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private Color player1Color = new(0.2f, 0.6f, 1f);
        [SerializeField] private Color player2Color = new(1f, 0.4f, 0.4f);

        private void OnEnable()
        {
            GameEvents.OnTurnChanged += UpdateTurn;
            GameEvents.OnPiotrusFlipped += ShowPiotrusWarning;
        }

        private void OnDisable()
        {
            GameEvents.OnTurnChanged -= UpdateTurn;
            GameEvents.OnPiotrusFlipped -= ShowPiotrusWarning;
        }

        private void UpdateTurn(int playerIndex)
        {
            if (turnText == null) return;
            turnText.text = $"Tura: Gracz {playerIndex + 1}";
            turnText.color = playerIndex == 0 ? player1Color : player2Color;
        }

        private void ShowPiotrusWarning(int playerIndex)
        {
            if (turnText == null) return;
            turnText.text = $"Piotruś! Gracz {playerIndex + 1} traci kolejkę!";
            turnText.color = Color.black;
        }
    }
}
