using System.Collections.Generic;

namespace MagicPairs.Core
{
    public enum Language { Polish, English, Spanish, Portuguese, German, French, Hindi, Chinese, Japanese }

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

        // index: 0=PL, 1=EN, 2=ES, 3=PT, 4=DE, 5=FR, 6=HI, 7=ZH, 8=JA
        private static readonly Dictionary<string, string[]> Texts = new()
        {
            { "player1", new[] { "Gracz 1", "Player 1", "Jugador 1", "Jogador 1", "Spieler 1", "Joueur 1", "खिलाड़ी 1", "玩家1", "プレイヤー1" } },
            { "player2", new[] { "Gracz 2", "Player 2", "Jugador 2", "Jogador 2", "Spieler 2", "Joueur 2", "खिलाड़ी 2", "玩家2", "プレイヤー2" } },
            { "computer", new[] { "Komputer", "Computer", "Ordenador", "Computador", "Computer", "Ordinateur", "कंप्यूटर", "电脑", "コンピュータ" } },
            { "turn", new[] { "Tura: {0}", "Turn: {0}", "Turno: {0}", "Turno: {0}", "Zug: {0}", "Tour: {0}", "बारी: {0}", "回合: {0}", "ターン: {0}" } },
            { "piotrus", new[] { "Piotruś! {0} traci kolejkę!", "Joker! {0} loses a turn!", "¡Comodín! ¡{0} pierde turno!", "Coringa! {0} perde a vez!", "Joker! {0} verliert eine Runde!", "Joker! {0} perd un tour!", "जोकर! {0} की बारी गई!", "小丑！{0}失去一回合！", "ジョーカー！{0}のターンを失う！" } },
            { "wins", new[] { "Wygrywa {0}!", "{0} wins!", "¡{0} gana!", "{0} venceu!", "{0} gewinnt!", "{0} gagne!", "{0} जीता!", "{0}赢了！", "{0}の勝ち！" } },
            { "draw", new[] { "Remis!", "Draw!", "¡Empate!", "Empate!", "Unentschieden!", "Égalité!", "ड्रॉ!", "平局！", "引き分け！" } },
            { "playAgain", new[] { "Zagraj ponownie", "Play Again", "Jugar de nuevo", "Jogar novamente", "Nochmal spielen", "Rejouer", "फिर खेलें", "再玩一次", "もう一度" } },
            { "start", new[] { "Start", "Start", "Iniciar", "Iniciar", "Start", "Démarrer", "शुरू", "开始", "スタート" } },
            { "player1Name", new[] { "Imię Gracza 1", "Player 1 Name", "Nombre Jugador 1", "Nome Jogador 1", "Name Spieler 1", "Nom Joueur 1", "खिलाड़ी 1 का नाम", "玩家1名字", "プレイヤー1の名前" } },
            { "player2Name", new[] { "Imię Gracza 2", "Player 2 Name", "Nombre Jugador 2", "Nome Jogador 2", "Name Spieler 2", "Nom Joueur 2", "खिलाड़ी 2 का नाम", "玩家2名字", "プレイヤー2の名前" } },
            { "score", new[] { "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}" } },
            { "mode1P", new[] { "1 Gracz", "1 Player (vs AI)", "1 Jugador", "1 Jogador", "1 Spieler", "1 Joueur", "1 खिलाड़ी", "单人模式", "1人プレイ" } },
            { "mode2P", new[] { "2 Graczy", "2 Players", "2 Jugadores", "2 Jogadores", "2 Spieler", "2 Joueurs", "2 खिलाड़ी", "双人模式", "2人プレイ" } },
            { "themeColors", new[] { "🎨 Kolory", "🎨 Colors", "🎨 Colores", "🎨 Cores", "🎨 Farben", "🎨 Couleurs", "🎨 रंग", "🎨 颜色", "🎨 カラー" } },
            { "themePrincess", new[] { "👸 Księżniczki", "👸 Princesses", "👸 Princesas", "👸 Princesas", "👸 Prinzessinnen", "👸 Princesses", "👸 राजकुमारी", "👸 公主", "👸 プリンセス" } },
            { "themeCars", new[] { "🚗 Samochody", "🚗 Cars", "🚗 Coches", "🚗 Carros", "🚗 Autos", "🚗 Voitures", "🚗 कारें", "🚗 汽车", "🚗 車" } },
            { "themeWaterWorld", new[] { "🐬 Wodny Świat", "🐬 Water World", "🐬 Mundo Acuático", "🐬 Mundo Aquático", "🐬 Wasserwelt", "🐬 Monde Aquatique", "🐬 जल दुनिया", "🐬 水世界", "🐬 水の世界" } },
            { "chooseTheme", new[] { "Wybierz typ kart", "Choose card type", "Elige tipo de carta", "Escolha o tipo", "Kartentyp wählen", "Choisir le type", "कार्ड चुनें", "选择卡片类型", "カードを選ぶ" } },
            { "cards", new[] { "Karty", "Cards", "Cartas", "Cartas", "Karten", "Cartes", "कार्ड", "卡片", "カード" } },
            { "play", new[] { "Graj", "Play", "Jugar", "Jogar", "Spielen", "Jouer", "खेलें", "开始", "プレイ" } },
            { "options", new[] { "Opcje", "Options", "Opciones", "Opções", "Optionen", "Options", "सेटिंग्स", "设置", "設定" } },
            { "quit", new[] { "Wyjdź", "Quit", "Salir", "Sair", "Beenden", "Quitter", "बाहर", "退出", "終了" } },
            { "languageOption", new[] { "Język", "Language", "Idioma", "Idioma", "Sprache", "Langue", "भाषा", "语言", "言語" } },
            { "arcade", new[] { "Arcade", "Arcade", "Arcade", "Arcade", "Arcade", "Arcade", "आर्केड", "街机", "アーケード" } },
            { "challenge", new[] { "Wyzwanie", "Challenge", "Desafío", "Desafio", "Herausforderung", "Défi", "चुनौती", "挑战", "チャレンジ" } },
            { "yourName", new[] { "Twoje imię", "Your name", "Tu nombre", "Seu nome", "Dein Name", "Ton nom", "आपका नाम", "你的名字", "あなたの名前" } },
            { "level", new[] { "Poziom", "Level", "Nivel", "Nível", "Level", "Niveau", "लेवल", "关卡", "レベル" } },
            { "leaderboard", new[] { "Tabela wyników", "Leaderboard", "Clasificación", "Ranking", "Bestenliste", "Classement", "लीडरबोर्ड", "排行榜", "ランキング" } },
            { "scores", new[] { "Wyniki", "Scores", "Puntos", "Pontos", "Punkte", "Scores", "स्कोर", "分数", "スコア" } },
            { "credits", new[] { "Autor", "Credits", "Créditos", "Créditos", "Credits", "Crédits", "क्रेडिट", "制作", "クレジット" } },
            { "chooseLanguage", new[] { "Wybierz język", "Choose Language", "Elige idioma", "Escolha idioma", "Sprache wählen", "Choisir la langue", "भाषा चुनें", "选择语言", "言語を選ぶ" } },
            { "chooseGameType", new[] { "Wybierz tryb gry", "Choose game type", "Elige modo de juego", "Escolha o modo", "Spielmodus wählen", "Choisir le mode", "गेम मोड चुनें", "选择游戏模式", "モードを選ぶ" } },
            { "chooseMode", new[] { "Wybierz tryb", "Choose mode", "Elige modo", "Escolha o modo", "Modus wählen", "Choisir le mode", "मोड चुनें", "选择模式", "モード選択" } },
            { "chooseDifficulty", new[] { "Wybierz poziom trudności", "Choose difficulty", "Elige dificultad", "Escolha dificuldade", "Schwierigkeit wählen", "Choisir la difficulté", "कठिनाई चुनें", "选择难度", "難易度を選ぶ" } },
            { "backToMenu", new[] { "Wróć do menu?", "Back to menu?", "¿Volver al menú?", "Voltar ao menu?", "Zurück zum Menü?", "Retour au menu?", "मेनू पर वापस?", "返回菜单？", "メニューに戻る？" } },
            { "yes", new[] { "Tak", "Yes", "Sí", "Sim", "Ja", "Oui", "हाँ", "是", "はい" } },
            { "no", new[] { "Nie", "No", "No", "Não", "Nein", "Non", "नहीं", "否", "いいえ" } },
            { "close", new[] { "Zamknij", "Close", "Cerrar", "Fechar", "Schließen", "Fermer", "बंद", "关闭", "閉じる" } },
            { "next", new[] { "Dalej", "Next", "Siguiente", "Próximo", "Weiter", "Suivant", "अगला", "下一关", "次へ" } },
            { "levelComplete", new[] { "Poziom {0} ukończony!", "Level {0} complete!", "¡Nivel {0} completado!", "Nível {0} completo!", "Level {0} geschafft!", "Niveau {0} terminé!", "लेवल {0} पूरा!", "第{0}关完成！", "レベル{0}クリア！" } },
            { "challengeOver", new[] { "Koniec! Wynik: {0}", "Game Over! Score: {0}", "¡Fin! Puntos: {0}", "Fim! Pontos: {0}", "Ende! Punkte: {0}", "Fin! Score: {0}", "खेल खत्म! स्कोर: {0}", "结束！分数：{0}", "ゲームオーバー！スコア：{0}" } },
            { "noScores", new[] { "Brak wyników", "No scores yet", "Sin puntuaciones", "Sem pontuações", "Keine Punkte", "Aucun score", "कोई स्कोर नहीं", "暂无分数", "スコアなし" } },
            { "pairs", new[] { "{0} - {1} par", "{0} - {1} pairs", "{0} - {1} pares", "{0} - {1} pares", "{0} - {1} Paare", "{0} - {1} paires", "{0} - {1} जोड़े", "{0} - {1}对", "{0} - {1}ペア" } },
            { "pair", new[] { "{0} - 1 para", "{0} - 1 pair", "{0} - 1 par", "{0} - 1 par", "{0} - 1 Paar", "{0} - 1 paire", "{0} - 1 जोड़ा", "{0} - 1对", "{0} - 1ペア" } },
            { "peek", new[] { "Podgląd", "Peek", "Espiar", "Espiar", "Spähen", "Aperçu", "झांकना", "偷看", "のぞく" } },
            { "shuffle", new[] { "Tasuj", "Shuffle", "Mezclar", "Embaralhar", "Mischen", "Mélanger", "शफल", "洗牌", "シャッフル" } },
            { "freeze", new[] { "Zamróź", "Freeze", "Congelar", "Congelar", "Einfrieren", "Geler", "फ्रीज", "冻结", "フリーズ" } },
            { "timeAttack", new[] { "Na czas", "Time Attack", "Contrarreloj", "Contra o tempo", "Zeitangriff", "Contre-la-montre", "टाइम अटैक", "限时挑战", "タイムアタック" } },
            { "timeAttackWin", new[] { "Ukończono! Pozostało: {0:F1}s", "Complete! Time left: {0:F1}s", "¡Completo! Tiempo: {0:F1}s", "Completo! Tempo: {0:F1}s", "Geschafft! Zeit: {0:F1}s", "Terminé! Temps: {0:F1}s", "पूरा! समय: {0:F1}s", "完成！剩余：{0:F1}秒", "クリア！残り：{0:F1}秒" } },
            { "timeAttackLose", new[] { "Czas minął!", "Time's up!", "¡Se acabó el tiempo!", "Tempo esgotado!", "Zeit abgelaufen!", "Temps écoulé!", "समय खत्म!", "时间到！", "タイムアップ！" } },
            { "menuMusic", new[] { "Muzyka menu", "Menu Music", "Música menú", "Música menu", "Menümusik", "Musique menu", "मेनू संगीत", "菜单音乐", "メニュー音楽" } },
            { "gameMusic", new[] { "Muzyka w grze", "Game Music", "Música juego", "Música jogo", "Spielmusik", "Musique jeu", "गेम संगीत", "游戏音乐", "ゲーム音楽" } },
            { "globalScores", new[] { "Globalne wyniki", "Global Scores", "Puntos globales", "Pontos globais", "Globale Punkte", "Scores globaux", "वैश्विक स्कोर", "全球排名", "グローバルスコア" } },
            { "localScores", new[] { "Lokalne wyniki", "Local Scores", "Puntos locales", "Pontos locais", "Lokale Punkte", "Scores locaux", "स्थानीय स्कोर", "本地排名", "ローカルスコア" } },
            { "adPowerUp", new[] { "Bonus", "Bonus", "Bonus", "Bônus", "Bonus", "Bonus", "बोनस", "奖励", "ボーナス" } },
            { "adNotReady", new[] { "Niedostępne", "Not ready", "No disponible", "Indisponível", "Nicht verfügbar", "Indisponible", "उपलब्ध नहीं", "未就绪", "準備中" } },
            { "dailyBonus", new[] { "Codzienna nagroda!", "Daily Bonus!", "¡Bonus diario!", "Bônus diário!", "Täglicher Bonus!", "Bonus quotidien!", "दैनिक बोनस!", "每日奖励！", "デイリーボーナス！" } },
            { "dailyStreak", new[] { "Seria: {0} dni", "Streak: {0} days", "Racha: {0} días", "Sequência: {0} dias", "Serie: {0} Tage", "Série: {0} jours", "स्ट्रीक: {0} दिन", "连续：{0}天", "連続：{0}日" } },
            { "claim", new[] { "Odbierz", "Claim", "Reclamar", "Resgatar", "Abholen", "Réclamer", "प्राप्त करें", "领取", "受け取る" } },
            { "coins", new[] { "Monety", "Coins", "Monedas", "Moedas", "Münzen", "Pièces", "सिक्के", "金币", "コイン" } },
            { "shop", new[] { "Sklep", "Shop", "Tienda", "Loja", "Shop", "Boutique", "दुकान", "商店", "ショップ" } },
            { "buy", new[] { "Kup", "Buy", "Comprar", "Comprar", "Kaufen", "Acheter", "खरीदें", "购买", "購入" } },
            { "notEnough", new[] { "Za mało monet", "Not enough coins", "Monedas insuficientes", "Moedas insuficientes", "Nicht genug Münzen", "Pas assez de pièces", "पर्याप्त सिक्के नहीं", "金币不足", "コイン不足" } },
            { "purchased", new[] { "Kupiono!", "Purchased!", "¡Comprado!", "Comprado!", "Gekauft!", "Acheté!", "खरीदा!", "已购买！", "購入済み！" } },
            { "achievements", new[] { "🏆 Osiągnięcia", "🏆 Achievements", "🏆 Logros", "🏆 Conquistas", "🏆 Erfolge", "🏆 Succès", "🏆 उपलब्धियाँ", "🏆 成就", "🏆 実績" } },
        };

        public static string Get(string key)
        {
            if (Texts.TryGetValue(key, out var values))
            {
                int idx = (int)CurrentLanguage;
                if (idx < values.Length) return values[idx];
                return values.Length > 1 ? values[1] : values[0]; // fallback to English
            }
            return key;
        }

        public static string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
