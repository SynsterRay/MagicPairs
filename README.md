# Magic Pairs

A memory card game for kids — Unity 6 with URP, targeting mobile (Android/iOS).

## How to Play

- Cards are placed face-down in a grid
- Two players take turns flipping 2 cards at a time
- If the cards match → pair found, player scores a point and continues
- If the cards don't match → they flip back, turn passes to the other player
- **Joker card** — a special black card with no pair. Whoever flips it immediately loses their turn
- The game ends when all pairs are found
- The player with the most pairs wins

## Game Modes

- **Arcade** — 2 players (hot-seat) or 1 player vs AI
- **Challenge** — progressive single-player mode with scoring, streak multipliers, and leaderboard

## Features

- 🎮 Step-by-step menu: game type → mode → difficulty → card theme → names
- 🃏 Card deal animation from deck + flip animation
- 🏆 Scoring system with visual pair collection
- 🃏 Joker card — lose your turn
- 🤖 Single Player vs AI (with memory system)
- ⭐ 3 difficulty levels (★ Easy 3×4, ★★ Medium 4×5, ★★★ Hard 5×6)
- 🎨 Card themes: Colors or Princesses (14 PNG illustrations)
- 🔥 Challenge mode: streak multiplier (x1–x5), level progression, leaderboard
- ✨ Particle effects on pair match + floating score popup
- 🌍 Full localization: Polish / English
- 🔊 Sound effects: card flip, pair match, mismatch, joker, level complete
- 📱 Touch & mouse input
- ← Back navigation on every screen

## Requirements

- Unity 6 (6000.x) with URP
- Package: Input System
- Target: Android / iOS / Desktop

## How to Run (Development)

1. Open the project in Unity 6
2. Install **Input System** (Package Manager → Unity Registry)
3. Open `Assets/Scenes/MainScene`
4. If the scene is empty: **MagicPairs → Setup Scene**
5. Press Play

## Controls

- Tap / click a card to flip it

## Project Structure

```
Assets/Scripts/
├── Core/       — GameManager, GameEvents, GameConfig, Localization, Leaderboard
├── Cards/      — CardController, CardAnimator, CardGrid, CardData, PairCollector, MatchEffect
├── GameFlow/   — IGameMode, LocalGameMode, SinglePlayerMode, ChallengeMode
├── Players/    — PlayerData, ScoreTracker
├── UI/         — MainMenu, ScoreDisplay, TurnIndicator, GameOverPanel, ChallengeUI, ScorePopup
├── Input/      — TouchInputHandler
├── Audio/      — SFXManager
└── Editor/     — SceneSetup
```

## Documentation

- [DOCS.md](DOCS.md) — full technical documentation
- [BUGS_TODO.md](BUGS_TODO.md) — known issues and roadmap

## License

© 2026 Mateusz Bajak
