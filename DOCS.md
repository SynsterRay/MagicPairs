# Magic Pairs — Project Documentation

## Overview

A memory card game for kids — Unity 6 with URP, targeting mobile (Android/iOS). Two players take turns flipping cards looking for matching pairs.

## Project Status (2026-06-08)

### Implemented

| System | Status | Description |
|--------|--------|-------------|
| Card Grid | ✅ | Dynamic grid (3×4 / 4×5 / 5×6) with colored cards |
| Flip Animation | ✅ | Card reveal animation (scale X) |
| Deal Animation | ✅ | Cards fly from deck to their positions |
| Pair Mechanics | ✅ | Flipping two cards of the same color = pair |
| Joker Card | ✅ | Special black card with ☠ symbol — lose your turn |
| Turn System | ✅ | Hot-seat for 2 players, switch on mismatch |
| Single Player (AI) | ✅ | Player vs computer with memory (~60% accuracy) |
| Challenge Mode | ✅ | Progressive single-player with increasing difficulty and scoring |
| Scoring | ✅ | Pair counting (Arcade) / streak multiplier (Challenge) |
| Leaderboard | ✅ | Top 10 scores saved in PlayerPrefs |
| Pair Collection | ✅ | Hiding found pairs with particle effect |
| Particle Effect | ✅ | Burst of 20 particles in card color on pair match |
| Score Popup | ✅ | Floating popup "+100 x3!" with punch scale and fade out |
| Collected Cards View | ✅ | Panel with grid of collected pairs via "Cards" button |
| Pause/Quit | ✅ | ✕ button → confirmation → return to menu |
| Main Menu | ✅ | Start screen → Options/Play → Arcade/Challenge → ... |
| Options | ✅ | Panel with language selection and credits |
| Localization | ✅ | Full PL/EN system — all buttons, panels, messages |
| Card Themes | ✅ | Colors (classic), Princesses (PNG), Cars (PNG) |
| Difficulty Levels | ✅ | ★ Easy (3×4), ★★ Medium (4×5), ★★★ Hard (5×6) |
| Color Palette | ✅ | 15 distinct colors — supports Hard mode (14 pairs) |
| Menu Navigation | ✅ | Back buttons ← on every step |
| Quit Button | ✅ | ✕ on start screen |
| Game Over | ✅ | Panel with result and "Play Again" button |
| Editor Setup | ✅ | Idempotent SceneSetup (one click) |
| Sound Effects | ✅ | Card flip, pair match, mismatch, joker, level complete, button click |
| Touch Input | ✅ | Touch and mouse support |
| Safe Area | ✅ | UI adapts to notches and rounded corners on mobile |
| Ads (AdMob) | ✅ | Banner (adaptive, bottom) + Interstitial (after completed games) + Rewarded (second chance, power-up bonus) |
| Second Chance | ✅ | Watch rewarded ad to restart current level after game over |
| Power-ups | ✅ | Peek, Shuffle, Freeze — earned every 3 levels or at streak x5 |
| Time Attack | ✅ | Solo mode — find all pairs before time runs out, no Joker |
| Player Name Persistence | ✅ | Names saved in PlayerPrefs, pre-filled on next game |
| Google Play Games | ✅ | Sign-in, global leaderboard (Challenge + Time Attack) |

### Not Yet Implemented

- [ ] Online multiplayer (placeholder `OnlineGameMode` exists)
- [ ] More card themes (animals, vehicles, etc.)
- [ ] Daily Challenge

## Releases

| Version | Date | Description |
|---------|------|-------------|
| v1.0 | 2026-05-28 | First APK release |
| v1.1 | 2026-05-28 | Fix: camera auto-scale, cards not hidden by UI, Joker visible |
| v1.2 | 2026-05-28 | Fix: back_card as playable card, hide matched pairs on mobile |
| v1.21 | 2026-05-28 | Score labels closer to center, universal back arrow |
| v1.22 | 2026-05-28 | Options menu, Challenge mode, leaderboard, particle effects |
| v1.23 | 2026-05-29 | SFX, English docs, UI alignment fixes, null reference fixes |
| v1.3 | 2026-05-31 | 15-color palette, Joker ☠ symbol, Safe Area, ads fix, second chance fix |
| v1.31 | 2026-06-01 | Fix Android crash, remove custom Activity, unify menu layout |
| v1.4 | 2026-06-02 | Cars theme, UI overhaul (Fredoka One font, rounded buttons, shine overlay, bevel text, dynamic backgrounds) |
| v1.401 | 2026-06-08 | Fix button text overflow on mobile, remove (vs AI) from Polish, improve color palette |
| v1.402 | 2026-06-08 | Fix button text not vertically centered on mobile |
| v1.403 | 2026-06-09 | Reorder menu (Challenge>TimeAttack>Arcade, Cars>Princess>Colors), fix text overflow on mobile |
| v1.406 | 2026-06-11 | Google Play Games Services — global leaderboard, global/local scores choice |

## Architecture

### Namespaces

```
MagicPairs.Core       — GameManager, GameEvents, GameConfig, Localization, Leaderboard
MagicPairs.Cards      — CardController, CardAnimator, CardGrid, CardData, PairCollector, MatchEffect
MagicPairs.GameFlow   — IGameMode, LocalGameMode, SinglePlayerMode, ChallengeMode, PowerUpManager, TimeAttackMode, OnlineGameMode (placeholder)
MagicPairs.Players    — PlayerData, ScoreTracker
MagicPairs.UI         — MainMenu, ScoreDisplay, TurnIndicator, GameOverPanel, ChallengeUI, ScorePopup, CollectedCardsPanel, PauseButton
MagicPairs.Input      — TouchInputHandler
MagicPairs.Audio      — SFXManager
MagicPairs.Ads        — AdManager
MagicPairs.Editor     — SceneSetup
```

### Design Patterns

| Pattern | Usage |
|---------|-------|
| Event Bus | `GameEvents` — static C# events |
| Static Events | `ChallengeMode.OnChallengeScoreChanged` — dedicated mode events |
| MaterialPropertyBlock | Card colors (no material leaks) |
| Interface (IGameMode) | Game mode separation: Local / SinglePlayer / Challenge / Online |
| ScriptableObject | `GameConfig` — configuration without code changes |
| Idempotent Setup | SceneSetup checks for existing objects |
| PlayerPrefs Persistence | Leaderboard — save/load scores |

### Menu Flow

```
Start Screen (logo + buttons):
├── Play
│   ├── Arcade
│   │   ├── Mode (2 Players / 1 Player vs AI)
│   │   ├── Difficulty (★ / ★★ / ★★★)
│   │   ├── Card Type (🎨 Colors / 👸 Princesses)
│   │   └── Names → Start
│   └── Challenge
│       ├── Card Type (🎨 Colors / 👸 Princesses)
│       └── Name → Start
├── Options
│   ├── Language (Polski / English)
│   └── Credits
├── Scores (Leaderboard)
└── ✕ (quit)

← "Back" — on every panel
✕ quit game — top right corner during gameplay (with confirmation)
"Cards" — view collected pairs (left/right button)
```

### Game Flow (Arcade)

```
MainMenu → Start
  → GameManager.StartGame() → GameEvents.OnGameStarted
    → CardGrid.BuildGrid() — spawn cards with deal animation
    → LocalGameMode/SinglePlayerMode.StartGame() — reset turns and scores
  → Player taps card → TouchInputHandler → IGameMode.OnCardSelected()
    → Flip animation → check:
      - Joker → lose turn
      - Match → PairCollector hides cards + MatchEffect (particles), +1 point
      - Mismatch → flip back, switch turn
  → All pairs found → GameOver (winner)
```

### Game Flow (Challenge)

```
MainMenu → Challenge → Card Type → Name → Start
  → ChallengeMode.StartGame() — level 1 (5 cards = 2 pairs + Joker)
  → Player vs AI taking turns
  → Match:
    - Player: streak++, score += 100 × min(streak, 5)
    - AI: no points
  → Mismatch:
    - Player: streak--, score -= 25 × |streak| (min streak = -3)
  → Level complete (player found last pair):
    - Bonus: 500 × level number
    - "Next" panel → next level (+1 pair, larger grid)
  → Game Over (AI found last pair):
    - Save to leaderboard
    - Panel with score + "Scores" button
```

### Challenge Mode — Details

**Level Progression:**
- Level 1: 5 cards (2 pairs + Joker), grid 2×3
- Level 2: 7 cards (3 pairs), grid 3×3
- Level N: 2+N-1 pairs, max 8 (Colors) or 14 (Princess)
- After reaching max cards: AI becomes harder (+4% memory/cycle)

**Scoring System:**
- Match: +100 × multiplier (streak x1 to x5)
- Mismatch: streak -1 (min -3), penalty -25 × |streak|
- Joker: streak = 0, penalty 50 + 25×level
- Level bonus: +500 × level number
- Recovery: after mismatches, a match gives base 100 pts

**AI in Challenge:**
- Base memory: 30% (easy at start)
- After max cards: +4% memory per cycle
- Max memory: 90%

**Power-ups (Challenge only):**
- 🔍 Peek — reveals 2 random cards for 1 second (start with 1)
- 🔄 Shuffle — randomizes positions of face-down cards, clears AI memory
- ❄️ Freeze — AI skips its next turn
- Earned: +1 every 3 levels (rotating), +1 random on streak x5
- Only usable on player's turn

### AI (SinglePlayerMode — Arcade)

- Player 0 = human, Player 1 = computer
- AI remembers revealed cards (`Dictionary<colorIndex, List<CardController>>`)
- `aiMemoryChance = 0.6f` — 60% chance to use memory
- `aiThinkDelay = 1.0s` — delay before move
- After finding a pair, AI gets another turn

### Leaderboard

- Top 10 scores
- Storage: PlayerPrefs (JSON)
- Data: player name, score, level reached
- Accessible after Challenge ends ("Scores" button) and from start screen

## How to Run

1. Open project in Unity 6
2. Install **Input System** package (Package Manager → Unity Registry)
3. Enable new Input System: Edit → Project Settings → Player → Active Input Handling → **Both**
4. Open `Assets/Scenes/MainScene`
5. If scene is empty: **MagicPairs → Setup Scene**
6. Play

## How to Rebuild Scene from Scratch

1. Ctrl+A in Hierarchy → Delete
2. **MagicPairs → Setup Scene**
3. Ctrl+S
4. Done (idempotent)

## Controls

| Action | Input |
|--------|-------|
| Flip card | Tap / Left Click |
| Menu navigation | On-screen buttons |

## Configuration (GameConfig ScriptableObject)

File: `Assets/ScriptableObjects/GameConfig.asset`

| Parameter | Default | Description |
|-----------|---------|-------------|
| gridRows | 4 | Changed by difficulty selection / level |
| gridCols | 4 | Changed by difficulty selection / level |
| colorPalette | 15 colors | Pair colors |
| piotrusColor | black | Joker card color |
| cardBackColor | dark purple | Card back color |
| flipDuration | 0.3s | Flip animation time |
| mismatchDelay | 1.0s | Time to show mismatched pair |
| piotrusDelay | 1.5s | Time to show Joker |

## Changelog

### 2026-06-17 (v1.41)
- Complete UI overhaul: all text buttons replaced with icon sprites
- Start panel: 3-column grid (Play top, Shop/Scores/Options, Achievements/Quit)
- Achievements icon button (bottom-left, only when GPGS authenticated)
- Loading screen with animated purple/gray dots between menu→game
- Shop redesigned: big icons side-by-side, green price tag badges with %OFF
- DailyBonus: power-up icon + name (left/right), coins icon + gold amount
- Themed game backgrounds (Cars/Princess) via world-space SpriteRenderer
- White gradient below score bar (ScreenSpaceCamera canvas, renders behind cards)
- White PowerUpBar above TopBar
- All game panels white: ChallengeOver, TimeAttack result, LevelComplete (green text)
- "BLOCKED" overlay on locked themes instead of gray tint
- Scores button reactive (GPGS only), post-game scores → GPGS global leaderboard
- Performance refactor: cached game mode refs in TouchInputHandler, static back sprite cache, font cache in ScorePopup, reduced TimeAttackUI per-frame allocs
- Removed dead code: OnlineGameMode.cs, GameBackground.CreatePanel(), unused fields
- Stripped 22 Debug.Log statements for production builds
- Fixed card sprite z-fighting artifact on upper rows

### 2026-06-12
- Ad monetization overhaul:
  - Added adaptive banner ad (bottom of screen, always visible)
  - Persistent interstitial counter (survives app restart via PlayerPrefs)
  - Rewarded ad button in Challenge power-up bar — watch ad to get random power-up
  - ShowRewarded now has failure callback — UI shows "Not ready" feedback
  - Second Chance button hides when ad fails instead of doing nothing
  - Interstitial only shown after completed games (not on mid-game quit)
  - Production Ad Unit ID placeholders (TODO: replace with real IDs)
- New localization keys: adPowerUp, adNotReady (PL/EN)

### 2026-06-11
- Google Play Games Services v2.1.0 integration
- Global leaderboard for Challenge and Time Attack modes
- Menu: global/local scores choice when signed in to Google Play
- GPGSManager singleton handles sign-in, score posting, and native leaderboard UI

### 2026-06-09
- Reorder game type buttons: Challenge (top) → Time Attack (middle) → Arcade (bottom)
- Reorder card theme buttons: Cars (top) → Princesses (middle) → Colors (bottom)
- Fix button text overflow on mobile — switched from HorizontalWrapMode.Overflow to Wrap with padding

### 2026-06-08
- Fixed theme button text ("Samochody", "Księżniczki") not visible on phone — buttons now auto-scale with resizeTextForBestFit
- Removed "(vs AI)" from Polish single-player arcade mode label
- Fixed "Poziom 1" text wrapping/splitting on mobile in Challenge mode (widened area + best fit)
- Replaced color palette with 15 maximally distinct colors (black, white, gray, maroon, dark green replace similar lime/gold/silver/teal/indigo)
- Fixed button text not vertically centered — switched from Wrap to Overflow mode

### 2026-06-02
- Cars theme (14 PNG cards + joker + custom back card)
- UI overhaul: Fredoka One font (bubbly, fairy-tale style)
- Rounded buttons with procedural sliced sprite + white shine overlay
- Button press feedback (color tint transition)
- Bevel effect on text (dual Shadow — dark below + white above)
- All font sizes doubled for mobile readability
- Dynamic backgrounds per theme (cars = road, default = fairy tale)
- Narrower menu buttons (60% width, cleaner layout)

### 2026-06-01
- Fixed Android crash on launch — removed custom EdgeToEdgeActivity and AndroidManifest override
- Removed androidx.core dependency (no longer needed without custom Activity)
- Fixed duplicate Gradle import in launcherTemplate
- Unity handles fullscreen/immersive mode natively (no custom Activity required)
- Unified menu panel layout — all panels same size, buttons aligned across screens
- Fixed difficulty buttons not centered between title and back button
- Fixed names panel Start button overlapping Player 2 input
- Dynamic Start button positioning (centered for single player, lower for two players)
- Quit button shows localized text ("Wyjdź"/"Quit") instead of ✕ symbol
- Logo repositioned to avoid being cut off at top
- Fixed duplicate line in MainMenu.cs (potential NullReferenceException)
- Fixed AAB signing — added buildTypes with signingConfig to launcherTemplate.gradle

### 2026-05-31
- Expanded color palette from 8 to 15 distinct colors (Hard mode fully playable)
- Joker card now shows red ☠ symbol when revealed in Colors theme
- Safe Area support — UI adapts to notches and rounded corners on mobile
- Fixed challenge level ending early (totalPairs mismatch with actual grid size)
- Fixed second chance: now restarts current level instead of advancing to next
- Ads reworked: interstitial on "Play Again" (Arcade), every 3 levels (Challenge), on menu return
- Reverted ad IDs to test mode for development
- Power-ups: Peek, Shuffle, Freeze with localized UI buttons
- Refactored FindAnyObjectByType calls — cached references in ChallengeUI and CardController
- Fixed power-up buttons remaining visible after returning to menu
- Time Attack mode: solo, 60s timer, +2s match, -3s mismatch, no Joker
- Time Attack leaderboard (sorted by time remaining)
- Time bonus/penalty popups (+2s / -3s) visible during gameplay
- Player names saved in PlayerPrefs, pre-filled on next game

### 2026-05-29
- Sound effects: card flip, pair match, mismatch, joker, level complete, button click
- Translated all documentation to English (Google Play preparation)
- UI alignment fixes for menu panels (consistent positioning across all screens)
- Unified names panel (Arcade single-player and Challenge share same panel)
- Fixed null reference errors when returning to menu mid-game
- Fixed leaderboard formatting (removed extra blank line, aligned numbers)
- Default language: English, default player names: "Player 1" / "Player 2"
- Menu panel fully opaque white background (no game elements bleeding through)

### 2026-05-28 (evening)
- Start screen with Play / Options / Scores / ✕ buttons
- Options panel with language selection and credits
- Leaderboard button on start screen (PlayerPrefs, persistent)
- Challenge mode — progressive single-player vs AI
- Card type selection in Challenge mode (Colors / Princesses)
- Scoring system with streak multiplier (x1-x5)
- Mismatch penalty (streak drops, point deduction)
- Leaderboard — top 10 scores (PlayerPrefs)
- Particle effect (burst) on pair match
- Floating score popup animation (+100 x3!)
- Full PL/EN localization for all buttons and panels
- Fix: turn indicator invisible (moved below TopBar, dark colors)
- Fix: Challenge mode wrong card count on first run
- Fix: TouchInputHandler not recognizing ChallengeMode
- Fix: buttons showing wrong language text

### 2026-05-28
- Created Unity 6 project with URP
- Full Memory game mechanics implementation
- Joker card (lose turn)
- Turn system for 2 players (hot-seat)
- Single Player mode (AI with memory)
- Animations: deal, flip (scale X), collect
- Step-by-step menu: language → mode → difficulty → names
- Back buttons ← and quit ✕
- 3 difficulty levels (★/★★/★★★)
- PL/EN localization
- PairCollector — collecting pairs under player name
- Editor tool: SceneSetup (idempotent)
- Unicorn logo in menu
- Game background from background_game.png (50% opacity)
- Card themes: Colors and Princesses (14 PNG images + joker)
- Back card: dedicated back image for Princess theme
- Cards enlarged (1.0×1.4)
- White top bar with player scores
- Credits panel
- Android preparation (app icon)
- GitHub repo: https://github.com/SynsterRay/MagicPairs
