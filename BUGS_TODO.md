# Bugs & TODO

## Fixed

- [x] ~~Cards hidden by UI panel on Hard difficulty~~ — camera auto-scales to grid size
- [x] ~~Joker invisible after reveal (white on white)~~ — fallback black color
- [x] ~~Card sprites different sizes~~ — SpriteDrawMode.Sliced forces uniform size
- [x] ~~Collected cards panel not opening~~ — "Cards" buttons instead of world raycast
- [x] ~~Build cache .utmp in repo~~ — added to .gitignore
- [x] ~~back_card.png loaded as playable card~~ — excluded from LoadPrincessSprites
- [x] ~~Collected pairs overlapping cards on mobile~~ — cards hidden, visible only in "Cards" panel
- [x] ~~Null reference on menu return~~ — StopAllCoroutines + disable game modes before destroying cards
- [x] ~~Names panel inconsistent between Arcade/Challenge~~ — unified to single panel
- [x] ~~Leaderboard entries misaligned~~ — removed blank line, PadLeft for numbers

## Known Issues

- [ ] **GameConfig** — color palette has 8 colors, but Hard (5×6) requires 14 pairs. Colors repeat.
- [ ] **PairCollector.cs** — collected cards may overflow screen on Hard with many pairs.
- [ ] **MainMenu.cs** — `Player1Name`/`Player2Name` are static — don't reset between Unity sessions.
- [ ] **Application.Quit()** — doesn't work in Unity Editor, only in builds.

## TODO (High Priority)

- [ ] Add more colors to palette (min. 15 for Hard)
- [ ] Visual distinction for Joker card in Colors theme

## TODO (Medium Priority)

- [ ] Online multiplayer — implement `OnlineGameMode` (placeholder exists)
- [ ] Safe Area — UI padding for notches/rounded corners on mobile
- [ ] Optional timer — time limit per turn
- [ ] More card themes (animals, vehicles, etc.)

## TODO (Low Priority)

- [ ] More than 2 players
- [ ] Player statistics (hit rate, fastest game)
- [ ] Card shuffle animation before deal
- [ ] Preview all cards for 2 seconds before start (optional)

## Lessons from DemonsAndAngels Project (Applied)

| Problem in D&A | Solution in MagicPairs |
|----------------|------------------------|
| Polling in Update() | Event-driven: GameEvents static bus |
| `.material.color` leak | MaterialPropertyBlock + SpriteRenderer |
| SceneSetup not idempotent | Checks for existing objects |
| Shader.Find() fails in builds | Material from prefab / Resources |
| Singleton without protection | GameManager with null check |
| Dead code | No unused classes |
| No interfaces | IGameMode (local/single/online) |
| Magic numbers | GameConfig ScriptableObject |
| TMPro issues | UnityEngine.UI.Text (built-in) |
| asmdef issues | No asmdef — Assembly-CSharp default |
