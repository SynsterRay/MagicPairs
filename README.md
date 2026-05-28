# Magic Pairs

Gra karciana Memory dla dzieci — Unity 6, mobile (Android/iOS).

## Zasady gry

- Karty ułożone rewersem do góry w siatce
- Dwóch graczy na zmianę odkrywa po 2 karty
- Jeśli karty mają ten sam kolor → para znaleziona, gracz zdobywa punkt i gra dalej
- Jeśli karty się nie zgadzają → wracają na miejsce, kolejka przechodzi do drugiego gracza
- **Karta Piotruś** — specjalna karta bez pary. Kto ją odkryje, natychmiast traci kolejkę
- Gra kończy się gdy wszystkie pary zostaną znalezione
- Wygrywa gracz z większą liczbą par

## Wymagania

- Unity 6 (6000.x) z URP
- Target: Android / iOS

## Jak uruchomić

1. Otwórz projekt w Unity 6
2. Otwórz `Assets/Scenes/GameScene`
3. Jeśli scena pusta: **MagicPairs → Setup Scene**
4. Play

## Sterowanie

- Dotknij/kliknij kartę aby ją odkryć

## Tryby gry

- **Offline 2 graczy** — hot-seat na jednym urządzeniu
- **Online 2 graczy** — placeholder (do implementacji)

## Struktura projektu

```
Assets/Scripts/
├── Core/       — GameManager, GameEvents, GameConfig
├── Cards/      — CardController, CardAnimator, CardGrid, CardData
├── GameFlow/   — IGameMode, LocalGameMode, TurnManager
├── Players/    — PlayerData, ScoreTracker
├── UI/         — HUD, ScoreDisplay, TurnIndicator, GameOverPanel
├── Input/      — TouchInputHandler
└── Editor/     — SceneSetup, CardPrefabSetup
```
