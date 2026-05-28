namespace MagicPairs.Players
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public int score;
        public int playerIndex;

        public PlayerData(int index, string name)
        {
            playerIndex = index;
            playerName = name;
            score = 0;
        }
    }
}
