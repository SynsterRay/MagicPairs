using System.Collections.Generic;

namespace MagicPairs.Core
{
    public enum Language { Polish, English }

    public static class Localization
    {
        public static Language CurrentLanguage { get; set; } = Language.Polish;

        private static readonly Dictionary<string, string[]> Texts = new()
        {
            // index 0 = Polish, 1 = English
            { "player1", new[] { "Gracz 1", "Player 1" } },
            { "player2", new[] { "Gracz 2", "Player 2" } },
            { "computer", new[] { "Komputer", "Computer" } },
            { "turn", new[] { "Tura: {0}", "Turn: {0}" } },
            { "piotrus", new[] { "Piotruś! {0} traci kolejkę!", "Joker! {0} loses a turn!" } },
            { "wins", new[] { "Wygrywa {0}!", "{0} wins!" } },
            { "draw", new[] { "Remis!", "Draw!" } },
            { "playAgain", new[] { "Zagraj ponownie", "Play Again" } },
            { "start", new[] { "Start", "Start" } },
            { "language", new[] { "Język", "Language" } },
            { "player1Name", new[] { "Imię Gracza 1", "Player 1 Name" } },
            { "player2Name", new[] { "Imię Gracza 2", "Player 2 Name" } },
            { "score", new[] { "{0}: {1}", "{0}: {1}" } },
            { "mode1P", new[] { "1 Gracz (vs AI)", "1 Player (vs AI)" } },
            { "mode2P", new[] { "2 Graczy", "2 Players" } },
            { "themeColors", new[] { "🎨 Kolory", "🎨 Colors" } },
            { "themePrincess", new[] { "👸 Księżniczki", "👸 Princesses" } },
            { "chooseTheme", new[] { "Wybierz typ kart", "Choose card type" } },
            { "cards", new[] { "Karty", "Cards" } },
            { "play", new[] { "Graj", "Play" } },
            { "options", new[] { "Opcje", "Options" } },
            { "languageOption", new[] { "Język", "Language" } },
            { "arcade", new[] { "Arcade", "Arcade" } },
            { "challenge", new[] { "Wyzwanie", "Challenge" } },
            { "yourName", new[] { "Twoje imię", "Your name" } },
            { "level", new[] { "Poziom", "Level" } },
            { "leaderboard", new[] { "Tabela wyników", "Leaderboard" } },
        };

        public static string Get(string key)
        {
            if (Texts.TryGetValue(key, out var values))
                return values[(int)CurrentLanguage];
            return key;
        }

        public static string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
