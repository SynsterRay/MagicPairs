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
| Punktacja | ✅ | Zliczanie par per gracz |
| Zbieranie par | ✅ | Animacja przesuwania znalezionych par pod nick gracza |
| Podgląd zebranych kart | ✅ | Panel z siatką zebranych par po kliknięciu "Karty" |
| Przerwanie gry | ✅ | Przycisk ✕ → potwierdzenie → powrót do menu |
| Menu startowe | ✅ | Krokowe: język → tryb → trudność → typ kart → imiona → start |
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
- [ ] Animacje cząsteczkowe przy znalezieniu pary
- [ ] Save/Load wyników (leaderboard)
- [ ] Safe Area dla notchy (mobile)
- [ ] Różne motywy graficzne kart
- [ ] Więcej kolorów w palecie (dla Hard potrzeba 14, jest 8)

## Architektura

### Namespace'y

```
MagicPairs.Core       — GameManager, GameEvents, GameConfig, Localization
MagicPairs.Cards      — CardController, CardAnimator, CardGrid, CardData, PairCollector
MagicPairs.GameFlow   — IGameMode, LocalGameMode, SinglePlayerMode, OnlineGameMode (placeholder)
MagicPairs.Players    — PlayerData, ScoreTracker
MagicPairs.UI         — MainMenu, ScoreDisplay, TurnIndicator, GameOverPanel
MagicPairs.Input      — TouchInputHandler
MagicPairs.Editor     — SceneSetup
```

### Wzorce projektowe

| Wzorzec | Użycie | Lekcja z DemonsAndAngels |
|---------|--------|--------------------------|
| Event Bus | `GameEvents` — statyczne eventy C# | Zamiast pollingu w Update() |
| MaterialPropertyBlock | Kolory kart | Zamiast `.material.color` (leak) |
| Interface (IGameMode) | Separacja trybu gry | Local / SinglePlayer / Online |
| ScriptableObject | `GameConfig` | Konfiguracja bez zmian w kodzie |
| Idempotentny Setup | SceneSetup sprawdza istniejące obiekty | Unikanie duplikatów |

### Flow menu

```
1. Wybierz język (Polski / English)
2. Wybierz tryb (2 Graczy / 1 Gracz vs AI)
3. Wybierz trudność (★ / ★★ / ★★★)
4. Wybierz typ kart (🎨 Kolory / 👸 Księżniczki)
5. Wpisz imiona → Start

← "Cofnij" — wycentrowany przycisk na dole każdego panelu (oprócz języka)
✕ wyjście z gry — na ekranie wyboru języka
✕ przerwanie gry — prawy górny róg podczas rozgrywki (z potwierdzeniem)
"Karty" — podgląd zebranych par gracza (lewy/prawy przycisk)
```

### Flow gry

```
MainMenu → Start
  → GameManager.StartGame() → GameEvents.OnGameStarted
    → CardGrid.BuildGrid() — spawn kart z animacją deal
    → LocalGameMode/SinglePlayerMode.StartGame() — reset tur i punktów
  → Gracz klika kartę → TouchInputHandler → IGameMode.OnCardSelected()
    → Flip animation → sprawdzenie:
      - Piotruś/Joker → utrata kolejki
      - Pierwsza karta → czekaj na drugą
      - Druga karta:
        - Match → PairCollector animuje karty pod nick gracza, +1 punkt
        - Mismatch → flip back, zmiana tury
  → W trybie AI: komputer automatycznie wybiera karty z pamięcią
  → Wszystkie pary znalezione → GameOver (winner)
    → "Zagraj ponownie" → restart
```

### AI (SinglePlayerMode)

- Gracz 0 = człowiek, Gracz 1 = komputer
- AI pamięta odkryte karty (`Dictionary<colorIndex, List<CardController>>`)
- Pamięć obejmuje karty odkryte przez obu graczy (AI "widzi" co gracz odkrywa)
- `aiMemoryChance = 0.6f` — 60% szans na użycie pamięci przy wyborze
- W 40% przypadków AI wybiera losowo, nawet jeśli "wie" gdzie jest para (symulacja niedoskonałej pamięci)
- `aiThinkDelay = 1.0s` — opóźnienie przed ruchem (naturalność)
- Po znalezieniu pary AI dostaje kolejną turę
- Logika wyboru:
  1. Pierwsza karta: sprawdza czy zna pełną parę → jeśli tak (i 60% roll), wybiera jedną z nich
  2. Druga karta: sprawdza czy zna partnera pierwszej → jeśli tak (i 60% roll), wybiera go
  3. W przeciwnym razie: losowy wybór z dostępnych kart

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
| gridRows | 4 | Zmieniane przez wybór trudności |
| gridCols | 4 | Zmieniane przez wybór trudności |
| colorPalette | 8 kolorów | Kolory par |
| piotrusColor | czarny | Kolor karty Piotruś/Joker |
| cardBackColor | ciemny fiolet | Kolor rewersu |
| flipDuration | 0.3s | Czas animacji flipu |
| mismatchDelay | 1.0s | Czas pokazania błędnej pary |
| piotrusDelay | 1.5s | Czas pokazania Piotrusia |

## Historia zmian

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
- Logo jednorożca w menu (zamiast tekstu)
- Background gry z pliku background_game.png (50% opacity)
- Białe tło paneli UI z ciemnym tekstem bold
- Biały tekst wskaźnika tury (na tle gry)
- Typy kart: Kolory (klasyczny) i Księżniczki (14 obrazków PNG + joker)
- Back card: dedykowany rewers karty w trybie Księżniczki (back_card.png)
- Karty powiększone (1.0×1.4) dla lepszej widoczności
- Biały pasek górny z wynikami graczy, background poniżej
- Panel autora/credits w menu głównym
- Przygotowanie pod Android (ikona aplikacji)
- GitHub repo: https://github.com/SynsterRay/MagicPairs
