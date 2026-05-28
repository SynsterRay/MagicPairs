using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class TurnIndicator : MonoBehaviour
    {
        [SerializeField] private Text turnText;
        [SerializeField] private Color player1Color = Color.white;
        [SerializeField] private Color player2Color = Color.white;

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
            string name = playerIndex == 0 ? MainMenu.Player1Name : MainMenu.Player2Name;
            turnText.text = Localization.Get("turn", name);
            turnText.color = playerIndex == 0 ? player1Color : player2Color;
        }

        private void ShowPiotrusWarning(int playerIndex)
        {
            if (turnText == null) return;
            string name = playerIndex == 0 ? MainMenu.Player1Name : MainMenu.Player2Name;
            turnText.text = Localization.Get("piotrus", name);
            turnText.color = Color.black;
        }
    }
}
