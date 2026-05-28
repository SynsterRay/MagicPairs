# Magic Pairs — Dokumentacja Projektu

## Przegląd

Gra karciana Memory dla dzieci — Unity 6 z URP, docelowo na urządzenia mobilne (Android/iOS). Dwóch graczy na zmianę odkrywa karty szukając par o tym samym kolorze.

## Stan projektu (2026-05-28)

### Zaimplementowane

| System | Status | Opis |
|--------|--------|------|
| Siatka kart | ✅ | Dynamiczny grid 4×4 z kolorowymi kartami |
| Flip animation | ✅ | Animacja odkrywania karty (scale X) |
| Deal animation | ✅ | Karty "lecą" z talii na swoje pozycje |
| Mechanika par | ✅ | Odkrycie dwóch kart tego samego koloru = para |
| Karta Piotruś | ✅ | Specjalna czarna karta — utrata kolejki |
| System tur | ✅ | Hot-seat dla 2 graczy, zmiana po błędzie |
| Punktacja | ✅ | Zliczanie par per gracz |
| Zbieranie par | ✅ | Animacja przesuwania znalezionych par pod nick gracza |
| Menu startowe | ✅ | Wybór języka (PL/EN), wprowadzenie imion graczy |
| Lokalizacja | ✅ | System PL/EN dla wszystkich tekstów UI |
| Game Over | ✅ | Panel z wynikiem i przyciskiem "Zagraj ponownie" |
| Editor Setup | ✅ | Idempotentny SceneSetup (jedno kliknięcie) |
| Touch Input | ✅ | Obsługa dotyku i myszy |

### Niezaimplementowane

- [ ] Multiplayer online (placeholder `OnlineGameMode` istnieje)
- [ ] Wybór trudności (Easy 3×4, Medium 4×5, Hard 5×6)
- [ ] Efekty dźwiękowe (SFX)
- [ ] Animacje cząsteczkowe przy znalezieniu pary
- [ ] Save/Load wyników (leaderboard)
- [ ] Safe Area dla notchy (mobile)
- [ ] Różne motywy graficzne kart

## Architektura

### Namespace'y

```
MagicPairs.Core       — GameManager, GameEvents, GameConfig, Localization
MagicPairs.Cards      — CardController, CardAnimator, CardGrid, CardData, PairCollector
MagicPairs.GameFlow   — IGameMode, LocalGameMode, OnlineGameMode (placeholder)
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
| Interface (IGameMode) | Separacja trybu gry | Łatwe dodanie online w przyszłości |
| ScriptableObject | `GameConfig` | Konfiguracja bez zmian w kodzie |
| Idempotentny Setup | SceneSetup sprawdza istniejące obiekty | Unikanie duplikatów |

### Kluczowe decyzje

1. **Brak TMPro** — Unity 6 ma problemy z paczką TextMeshPro w nowych projektach. Użyto `UnityEngine.UI.Text`.
2. **Brak asmdef** — usunięto assembly definitions, bo powodowały problemy z referencjami pakietów.
3. **Unlit shader** — karty używają URP/Unlit zamiast Lit, żeby kolor był zawsze widoczny bez oświetlenia.
4. **Scale flip** — animacja flipu przez skalowanie X (1→0→1) zamiast rotacji Y, bo Quad ma backface culling.

### Flow gry

```
MainMenu (imiona, język) → Start
  → GameManager.StartGame() → GameEvents.OnGameStarted
    → CardGrid.BuildGrid() — spawn kart z animacją deal
    → LocalGameMode.StartGame() — reset tur i punktów
    → TurnManager ustawia Gracz 1
  → Gracz klika kartę → TouchInputHandler → IGameMode.OnCardSelected()
    → Flip animation → sprawdzenie:
      - Piotruś → utrata kolejki
      - Pierwsza karta → czekaj na drugą
      - Druga karta:
        - Match → PairCollector animuje karty pod nick gracza, +1 punkt
        - Mismatch → flip back, zmiana tury
  → Wszystkie pary znalezione → GameOver (winner)
    → "Zagraj ponownie" → restart
```

## Jak uruchomić

1. Otwórz projekt w Unity 6
2. Zainstaluj pakiet **Input System** (Package Manager → Unity Registry)
3. Włącz nowy Input System: Edit → Project Settings → Player → Active Input Handling → **Both** lub **Input System Package**
4. Otwórz `Assets/Scenes/MainScene`
5. Jeśli scena pusta: **MagicPairs → Setup Scene**
6. Play

## Jak przebudować scenę od zera

1. File → New Scene → Save As `Assets/Scenes/MainScene`
2. **MagicPairs → Setup Scene**
3. Gotowe (idempotentne — można uruchomić wielokrotnie)

## Sterowanie

| Akcja | Input |
|-------|-------|
| Odkryj kartę | Dotknij / LPM |
| Menu startowe | Automatycznie przy starcie |

## Konfiguracja (GameConfig ScriptableObject)

Plik: `Assets/ScriptableObjects/GameConfig.asset`

| Parametr | Domyślna | Opis |
|----------|----------|------|
| gridRows | 4 | Liczba wierszy |
| gridCols | 4 | Liczba kolumn |
| colorPalette | 8 kolorów | Kolory par |
| piotrusColor | czarny | Kolor karty Piotruś |
| cardBackColor | ciemny fiolet | Kolor rewersu |
| flipDuration | 0.3s | Czas animacji flipu |
| mismatchDelay | 1.0s | Czas pokazania błędnej pary |
| piotrusDelay | 1.5s | Czas pokazania Piotrusia |

## Historia zmian

### 2026-05-28
- Stworzenie projektu Unity 6 z URP
- Implementacja pełnej mechaniki Memory
- Karta Piotruś (utrata kolejki)
- System tur dla 2 graczy (hot-seat)
- Animacje: deal, flip (scale X), collect
- Menu startowe z wyborem języka i imion
- Lokalizacja PL/EN
- PairCollector — zbieranie par pod nickiem gracza
- Editor tool: SceneSetup (idempotentny)
- GitHub repo: https://github.com/SynsterRay/MagicPairs
