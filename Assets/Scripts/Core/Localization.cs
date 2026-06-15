using System.Collections.Generic;

namespace MagicPairs.Core
{
    public enum Language { Polish, English }

    public static class Localization
    {
        private static Language _currentLanguage = Language.English;
        private const string LangKey = "MagicPairs_Language";

        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                UnityEngine.PlayerPrefs.SetInt(LangKey, (int)value);
                UnityEngine.PlayerPrefs.Save();
            }
        }

        static Localization()
        {
            try { _currentLanguage = (Language)UnityEngine.PlayerPrefs.GetInt(LangKey, (int)Language.English); }
            catch { _currentLanguage = Language.English; }
        }

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
            { "player1Name", new[] { "Imię Gracza 1", "Player 1 Name" } },
            { "player2Name", new[] { "Imię Gracza 2", "Player 2 Name" } },
            { "score", new[] { "{0}: {1}", "{0}: {1}" } },
            { "mode1P", new[] { "1 Gracz", "1 Player (vs AI)" } },
            { "mode2P", new[] { "2 Graczy", "2 Players" } },
            { "themeColors", new[] { "🎨 Kolory", "🎨 Colors" } },
            { "themePrincess", new[] { "👸 Księżniczki", "👸 Princesses" } },
            { "themeCars", new[] { "🚗 Samochody", "🚗 Cars" } },
            { "chooseTheme", new[] { "Wybierz typ kart", "Choose card type" } },
            { "cards", new[] { "Karty", "Cards" } },
            { "play", new[] { "Graj", "Play" } },
            { "options", new[] { "Opcje", "Options" } },
            { "quit", new[] { "Wyjdź", "Quit" } },
            { "languageOption", new[] { "Język", "Language" } },
            { "arcade", new[] { "Arcade", "Arcade" } },
            { "challenge", new[] { "Wyzwanie", "Challenge" } },
            { "yourName", new[] { "Twoje imię", "Your name" } },
            { "level", new[] { "Poziom", "Level" } },
            { "leaderboard", new[] { "Tabela wyników", "Leaderboard" } },
            { "scores", new[] { "Wyniki", "Scores" } },
            { "credits", new[] { "Autor", "Credits" } },
            { "chooseLanguage", new[] { "Wybierz język", "Choose Language" } },
            { "chooseGameType", new[] { "Wybierz tryb gry", "Choose game type" } },
            { "chooseMode", new[] { "Wybierz tryb", "Choose mode" } },
            { "chooseDifficulty", new[] { "Wybierz poziom trudności", "Choose difficulty" } },
            { "backToMenu", new[] { "Wróć do menu?", "Back to menu?" } },
            { "yes", new[] { "Tak", "Yes" } },
            { "no", new[] { "Nie", "No" } },
            { "close", new[] { "Zamknij", "Close" } },
            { "next", new[] { "Dalej", "Next" } },
            { "levelComplete", new[] { "Poziom {0} ukończony!", "Level {0} complete!" } },
            { "challengeOver", new[] { "Koniec! Wynik: {0}", "Game Over! Score: {0}" } },
            { "noScores", new[] { "Brak wyników", "No scores yet" } },
            { "pairs", new[] { "{0} - {1} par", "{0} - {1} pairs" } },
            { "pair", new[] { "{0} - 1 para", "{0} - 1 pair" } },
            { "peek", new[] { "Podgląd", "Peek" } },
            { "shuffle", new[] { "Tasuj", "Shuffle" } },
            { "freeze", new[] { "Zamróź", "Freeze" } },
            { "timeAttack", new[] { "Na czas", "Time Attack" } },
            { "timeAttackWin", new[] { "Ukończono! Pozostało: {0:F1}s", "Complete! Time left: {0:F1}s" } },
            { "timeAttackLose", new[] { "Czas minął!", "Time's up!" } },
            { "menuMusic", new[] { "Muzyka menu", "Menu Music" } },
            { "gameMusic", new[] { "Muzyka w grze", "Game Music" } },
            { "globalScores", new[] { "Globalne wyniki", "Global Scores" } },
            { "localScores", new[] { "Lokalne wyniki", "Local Scores" } },
            { "adPowerUp", new[] { "Bonus", "Bonus" } },
            { "adNotReady", new[] { "Niedostępne", "Not ready" } },
            { "dailyBonus", new[] { "Codzienna nagroda!", "Daily Bonus!" } },
            { "dailyStreak", new[] { "Seria: {0} dni", "Streak: {0} days" } },
            { "claim", new[] { "Odbierz", "Claim" } },
            { "coins", new[] { "Monety", "Coins" } },
            { "shop", new[] { "Sklep", "Shop" } },
            { "buy", new[] { "Kup", "Buy" } },
            { "notEnough", new[] { "Za mało monet", "Not enough coins" } },
            { "purchased", new[] { "Kupiono!", "Purchased!" } },
            { "achievements", new[] { "🏆 Osiągnięcia", "🏆 Achievements" } },
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
