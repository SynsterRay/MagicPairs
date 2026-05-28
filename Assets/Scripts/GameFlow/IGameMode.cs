namespace MagicPairs.GameFlow
{
    public interface IGameMode
    {
        int CurrentPlayerIndex { get; }
        void StartGame();
        void OnCardSelected(Cards.CardController card);
    }
}
