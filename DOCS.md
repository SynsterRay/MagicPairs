# Magic Pairs — Dokumentacja Projektu

## Przegląd

Gra karciana Memory dla dzieci — Unity 6 z URP, docelowo na urządzenia mobilne (Android/iOS). Dwóch graczy na zmianę odkrywa karty szukając par o tym samym kolorze.

## Stan projektu (2026-05-28)

### Zaimplementowane

| System | Status | Opis |
|--------|--------|------|
| Siatka kart | ✅ | Dynamiczny grid (3×4 / 4×5 / 5×6) z kolorowymi kartami |
| Flip animation | ✅ | Animacja odkrywania karty (scale X) |
| Deal animation | ✅ | Karty "lecą" z talii na swoje pozycje |
| Mechanika par | ✅ | Odkrycie dwóch kart tego samego koloru = para |
| Karta Piotruś/Joker | ✅ | Specjalna czarna karta — utrata kolejki |
| System tur | ✅ | Hot-seat dla 2 graczy, zmiana po błędzie |
| Single Player (AI) | ✅ | Gracz vs komputer z pamięcią (~60% skuteczności) |
| Tryb Wyzwanie | ✅ | Progresywny single-player z rosnącą trudnością i punktacją |
| Punktacja | ✅ | Zliczanie par (Arcade) / mnożnik serii (Wyzwanie) |
| Leaderboard | ✅ | Top 10 wyników zapisywanych w PlayerPrefs |
| Zbieranie par | ✅ | Ukrywanie znalezionych par z efektem cząsteczkowym |
| Efekt cząsteczkowy | ✅ | Burst 20 cząsteczek w kolorze karty przy odkryciu pary |
| Animacja punktów | ✅ | Floating popup "+100 x3!" z punch scale i fade out |
| Podgląd zebranych kart | ✅ | Panel z siatką zebranych par po kliknięciu "Karty" |
| Przerwanie gry | ✅ | Przycisk ✕ → potwierdzenie → powrót do menu |
| Menu startowe | ✅ | Ekran główny → Opcje/Graj → Arcade/Wyzwanie → ... |
| Opcje | ✅ | Panel z wyborem języka i credits |
| Lokalizacja | ✅ | System PL/EN (Piotruś/Joker) |
| Typy kart | ✅ | Kolory (klasyczny) lub Księżniczki (obrazki PNG) |
| Poziomy trudności | ✅ | ★ Easy (3×4), ★★ Medium (4×5), ★★★ Hard (5×6) |
| Nawigacja menu | ✅ | Przyciski cofania ← na każdym etapie |
| Przycisk wyjścia | ✅ | ✕ na ekranie startowym |
| Game Over | ✅ | Panel z wynikiem i przyciskiem "Zagraj ponownie" |
| Editor Setup | ✅ | Idempotentny SceneSetup (jedno kliknięcie) |
| Touch Input | ✅ | Obsługa dotyku i myszy |

### Niezaimplementowane

- [ ] Multiplayer online (placeholder `OnlineGameMode` istnieje)
- [ ] Efekty dźwiękowe (SFX)
- [ ] Safe Area dla notchy (mobile)
- [ ] Więcej motywów kart
- [ ] Więcej kolorów w palecie (dla Hard potrzeba 14, jest 8)
- [ ] Timer option

## Releases

| Wersja | Data | Opis |
|--------|------|------|
| v1.0 | 2026-05-28 | Pierwsza wersja APK |
| v1.1 | 2026-05-28 | Fix: kamera auto-scale, karty nie zasłaniane przez UI, Piotruś widoczny |
| v1.2 | 2026-05-28 | Fix back_card as playable card, hide matched pairs on mobile |
| v1.21 | 2026-05-28 | Score labels closer to center, universal back arrow |
| v1.3 | 2026-05-28 | Options menu, Challenge mode, leaderboard, particle effects |

## Architektura

### Namespace'y

```
MagicPairs.Core       — GameManager, GameEvents, GameConfig, Localization, Leaderboard
MagicPairs.Cards      — CardController, CardAnimator, CardGrid, CardData, PairCollector, MatchEffect
MagicPairs.GameFlow   — IGameMode, LocalGameMode, SinglePlayerMode, ChallengeMode, OnlineGameMode (placeholder)
MagicPairs.Players    — PlayerData, ScoreTracker
MagicPairs.UI         — MainMenu, ScoreDisplay, TurnIndicator, GameOverPanel, ChallengeUI, ScorePopup, CollectedCardsPanel, PauseButton
MagicPairs.Input      — TouchInputHandler
MagicPairs.Editor     — SceneSetup
```

### Wzorce projektowe

| Wzorzec | Użycie |
|---------|--------|
| Event Bus | `GameEvents` — statyczne eventy C# |
| Static Events | `ChallengeMode.OnChallengeScoreChanged` — dedykowane eventy trybu |
| MaterialPropertyBlock | Kolory kart (brak leaków materiałów) |
| Interface (IGameMode) | Separacja trybu gry: Local / SinglePlayer / Challenge / Online |
| ScriptableObject | `GameConfig` — konfiguracja bez zmian w kodzie |
| Idempotentny Setup | SceneSetup sprawdza istniejące obiekty |
| PlayerPrefs persistence | Leaderboard — zapis/odczyt wyników |

### Flow menu

```
Ekran startowy (logo + przyciski):
├── Graj / Play
│   ├── Arcade
│   │   ├── Tryb (2 Graczy / 1 Gracz vs AI)
│   │   ├── Trudność (★ / ★★ / ★★★)
│   │   ├── Typ kart (🎨 Kolory / 👸 Księżniczki)
│   │   └── Imiona → Start
│   └── Wyzwanie / Challenge
│       ├── Typ kart (🎨 Kolory / 👸 Księżniczki)
│       └── Imię → Start
├── Opcje / Options
│   ├── Język / Language (Polski / English)
│   └── Autor / Credits
└── ✕ (wyjście)

← "Cofnij" — na każdym panelu
✕ przerwanie gry — prawy górny róg podczas rozgrywki (z potwierdzeniem)
"Karty" — podgląd zebranych par gracza (lewy/prawy przycisk)
```

### Flow gry (Arcade)

```
MainMenu → Start
  → GameManager.StartGame() → GameEvents.OnGameStarted
    → CardGrid.BuildGrid() — spawn kart z animacją deal
    → LocalGameMode/SinglePlayerMode.StartGame() — reset tur i punktów
  → Gracz klika kartę → TouchInputHandler → IGameMode.OnCardSelected()
    → Flip animation → sprawdzenie:
      - Piotruś/Joker → utrata kolejki
      - Match → PairCollector ukrywa karty + MatchEffect (particles), +1 punkt
      - Mismatch → flip back, zmiana tury
  → Wszystkie pary znalezione → GameOver (winner)
```

### Flow gry (Wyzwanie)

```
MainMenu → Wyzwanie → Typ kart → Imię → Start
  → ChallengeMode.StartGame() — level 1 (5 kart = 2 pary + Piotruś)
  → Gracz vs AI na zmianę
  → Match:
    - Gracz: streak++, score += 100 × min(streak, 5)
    - AI: brak punktów
  → Mismatch:
    - Gracz: streak--, score -= 25 × |streak| (min streak = -3)
  → Level complete (gracz znalazł ostatnią parę):
    - Bonus: 500 × numer levelu
    - Panel "Dalej" → następny level (+1 para, większy grid)
  → Game Over (AI znalazło ostatnią parę):
    - Zapis do leaderboard
    - Panel z wynikiem + przycisk "Wyniki"
```

### Tryb Wyzwanie — szczegóły

**Progresja levelów:**
- Level 1: 5 kart (2 pary + Piotruś), grid 2×3
- Level 2: 7 kart (3 pary), grid 3×3
- Level N: 2+N-1 par, max 8 (Colors) lub 14 (Princess)
- Po osiągnięciu max kart: AI staje się trudniejsze (+5% pamięci/level)

**System punktacji:**
- Trafienie: +100 × mnożnik (streak x1 do x5)
- Pomyłka: streak -1 (min -3), kara -25 × |streak|
- Piotruś: streak = 0
- Bonus za level: +500 × numer levelu
- Odbudowa: po serii pomyłek, trafienie daje bazowe 100 pkt

**AI w Wyzwaniu:**
- Bazowa pamięć: 30% (łatwe na początku)
- Po max kartach: +5% pamięci na każdy kolejny level
- Max pamięć: 95%

### AI (SinglePlayerMode — Arcade)

- Gracz 0 = człowiek, Gracz 1 = komputer
- AI pamięta odkryte karty (`Dictionary<colorIndex, List<CardController>>`)
- `aiMemoryChance = 0.6f` — 60% szans na użycie pamięci
- `aiThinkDelay = 1.0s` — opóźnienie przed ruchem
- Po znalezieniu pary AI dostaje kolejną turę

### Leaderboard

- Top 10 wyników
- Zapis: PlayerPrefs (JSON)
- Dane: imię gracza, wynik, osiągnięty level
- Dostępny po zakończeniu Wyzwania (przycisk "Wyniki")

## Jak uruchomić

1. Otwórz projekt w Unity 6
2. Zainstaluj pakiet **Input System** (Package Manager → Unity Registry)
3. Włącz nowy Input System: Edit → Project Settings → Player → Active Input Handling → **Both**
4. Otwórz `Assets/Scenes/MainScene`
5. Jeśli scena pusta: **MagicPairs → Setup Scene**
6. Play

## Jak przebudować scenę od zera

1. Ctrl+A w Hierarchy → Delete
2. **MagicPairs → Setup Scene**
3. Ctrl+S
4. Gotowe (idempotentne)

## Sterowanie

| Akcja | Input |
|-------|-------|
| Odkryj kartę | Dotknij / LPM |
| Nawigacja menu | Przyciski na ekranie |

## Konfiguracja (GameConfig ScriptableObject)

Plik: `Assets/ScriptableObjects/GameConfig.asset`

| Parametr | Domyślna | Opis |
|----------|----------|------|
| gridRows | 4 | Zmieniane przez wybór trudności / level |
| gridCols | 4 | Zmieniane przez wybór trudności / level |
| colorPalette | 8 kolorów | Kolory par |
| piotrusColor | czarny | Kolor karty Piotruś/Joker |
| cardBackColor | ciemny fiolet | Kolor rewersu |
| flipDuration | 0.3s | Czas animacji flipu |
| mismatchDelay | 1.0s | Czas pokazania błędnej pary |
| piotrusDelay | 1.5s | Czas pokazania Piotrusia |

## Historia zmian

### 2026-05-28 (wieczór)
- Ekran startowy z przyciskami Graj / Opcje / ✕
- Panel Opcje z wyborem języka i credits
- Tryb Wyzwanie (Challenge) — progresywny single-player vs AI
- System punktacji z mnożnikiem serii (streak x1-x5)
- Kara za pomyłki (streak spada, odejmowanie punktów)
- Leaderboard — top 10 wyników (PlayerPrefs)
- Wybór typu kart w trybie Wyzwanie
- Efekt cząsteczkowy (particle burst) przy odkryciu pary
- Animacja floating popup z punktami (+100 x3!)
- Fix: wskaźnik tury niewidoczny (przesunięty pod TopBar, ciemne kolory)
- Fix: Challenge mode — złą liczbę kart przy pierwszym uruchomieniu
- Fix: TouchInputHandler nie rozpoznawał ChallengeMode

### 2026-05-28
- Stworzenie projektu Unity 6 z URP
- Implementacja pełnej mechaniki Memory
- Karta Piotruś/Joker (utrata kolejki)
- System tur dla 2 graczy (hot-seat)
- Tryb Single Player (AI z pamięcią)
- Animacje: deal, flip (scale X), collect
- Menu krokowe: język → tryb → trudność → imiona
- Przyciski cofania ← i wyjścia ✕
- 3 poziomy trudności (★/★★/★★★)
- Lokalizacja PL/EN
- PairCollector — zbieranie par pod nickiem gracza
- Editor tool: SceneSetup (idempotentny)
- Logo jednorożca w menu
- Background gry z pliku background_game.png (50% opacity)
- Typy kart: Kolory i Księżniczki (14 obrazków PNG + joker)
- Back card: dedykowany rewers w trybie Księżniczki
- Karty powiększone (1.0×1.4)
- Biały pasek górny z wynikami graczy
- Panel autora/credits
- Przygotowanie pod Android (ikona aplikacji)
- GitHub repo: https://github.com/SynsterRay/MagicPairs
