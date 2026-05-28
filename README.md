# Magic Pairs

Gra karciana Memory dla dzieci — Unity 6, mobile (Android/iOS).

## Zasady gry

- Karty ułożone rewersem do góry w siatce 4×4
- Dwóch graczy na zmianę odkrywa po 2 karty
- Jeśli karty mają ten sam kolor → para znaleziona, gracz zdobywa punkt i gra dalej
- Jeśli karty się nie zgadzają → wracają na miejsce, kolejka przechodzi do drugiego gracza
- **Karta Piotruś** — specjalna czarna karta bez pary. Kto ją odkryje, natychmiast traci kolejkę
- Gra kończy się gdy wszystkie pary zostaną znalezione
- Wygrywa gracz z większą liczbą par
- Znalezione pary animują się pod nick gracza

## Wymagania

- Unity 6 (6000.x) z URP
- Pakiet: Input System
- Target: Android / iOS / Desktop

## Jak uruchomić

1. Otwórz projekt w Unity 6
2. Zainstaluj **Input System** (Package Manager → Unity Registry)
3. Otwórz `Assets/Scenes/MainScene`
4. Jeśli scena pusta: **MagicPairs → Setup Scene**
5. Play

## Sterowanie

- Dotknij/kliknij kartę aby ją odkryć

## Funkcje

- 🎮 Menu startowe z wyborem języka (PL/EN) i imion graczy
- 🃏 Animacja rozdawania kart z talii
- 🔄 Animacja odkrywania (flip)
- 🏆 System punktacji z wizualnym zbieraniem par
- 🃏 Karta Piotruś — utrata kolejki
- 🌍 Lokalizacja PL/EN
- 🔌 Placeholder na multiplayer online

## Dokumentacja

- [DOCS.md](DOCS.md) — pełna dokumentacja techniczna
- [BUGS_TODO.md](BUGS_TODO.md) — znane problemy i plan rozwoju

## Struktura projektu

```
Assets/Scripts/
├── Core/       — GameManager, GameEvents, GameConfig, Localization
├── Cards/      — CardController, CardAnimator, CardGrid, CardData, PairCollector
├── GameFlow/   — IGameMode, LocalGameMode, OnlineGameMode (placeholder)
├── Players/    — PlayerData, ScoreTracker
├── UI/         — MainMenu, ScoreDisplay, TurnIndicator, GameOverPanel
├── Input/      — TouchInputHandler
└── Editor/     — SceneSetup
```
