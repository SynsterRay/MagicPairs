using UnityEngine;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.GameFlow
{
    /// <summary>
    /// Placeholder for future online multiplayer implementation.
    /// Will sync card selections and turns over network.
    /// </summary>
    public class OnlineGameMode : MonoBehaviour, IGameMode
    {
        public int CurrentPlayerIndex => _localPlayerIndex;

        private int _localPlayerIndex;

        public void StartGame()
        {
            // TODO: Connect to server, wait for opponent, sync seed for shuffle
            Debug.Log("[OnlineGameMode] Not implemented. Use LocalGameMode for now.");
        }

        public void OnCardSelected(CardController card)
        {
            // TODO: Send card selection to server, wait for confirmation
            Debug.Log("[OnlineGameMode] Not implemented.");
        }
    }
}
