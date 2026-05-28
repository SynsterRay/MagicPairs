# Bugs & TODO

## Znane problemy

- [ ] **GameConfig** — paleta kolorów ma 8 kolorów, ale Hard (5×6) wymaga 14 par. Kolory się powtarzają (`i % palette.Length`), co może mylić graczy.
- [ ] **PairCollector.cs** — pozycjonowanie zebranych kart może wychodzić poza ekran przy dużej liczbie par na Hard.
- [ ] **CardGrid.cs** — grid wymaga nieparzystej liczby kart (dla Piotrusia). Przy parzystym gridRows×gridCols (np. 4×4=16, 4×5=20, 5×6=30) jest OK (nieparzyste po odjęciu 1 dla Piotrusia = parzysta liczba par).
- [ ] **MainMenu.cs** — `Player1Name`/`Player2Name` są statyczne — nie resetują się między sesjami Unity.
- [ ] **Application.Quit()** — nie działa w edytorze Unity, tylko w buildzie.

## TODO (priorytet wysoki)

- [ ] Dodać więcej kolorów do palety (min. 15 dla Hard)
- [ ] Dźwięki: flip karty, znalezienie pary, Piotruś/Joker, game over
- [ ] Wizualne wyróżnienie karty Piotruś/Joker (inny rewers lub symbol)

## TODO (priorytet średni)

- [ ] Online multiplayer — implementacja `OnlineGameMode` (placeholder istnieje)
- [ ] Save/Load wyników — lokalny leaderboard (JSON)
- [ ] Safe Area — padding UI dla notchy/zaokrągleń na mobile
- [ ] Efekt cząsteczkowy przy znalezieniu pary
- [ ] Animacja "pulse" przy dotknięciu karty (feedback dotykowy)
- [ ] Timer opcjonalny — ograniczenie czasu na turę
- [ ] Kamera orthographic size dostosowana do poziomu trudności

## TODO (priorytet niski)

- [ ] Motywy graficzne (zwierzęta, kształty zamiast kolorów)
- [ ] Więcej niż 2 graczy
- [ ] Statystyki gracza (% trafień, najszybsza gra)
- [ ] Animacja tasowania kart przed rozdaniem
- [ ] Podgląd wszystkich kart na 2 sekundy przed startem (opcjonalny)

## Lekcje z projektu DemonsAndAngels (zastosowane)

| Problem w D&A | Rozwiązanie w MagicPairs |
|---------------|--------------------------|
| Polling w Update() (PlayerHealthBar) | Event-driven: GameEvents static bus |
| `.material.color` leak (AttackFeedback) | MaterialPropertyBlock w CardAnimator |
| SceneSetup nie idempotentny | Sprawdzanie istniejących obiektów przed tworzeniem |
| Shader.Find() nie działa w buildach | Materiał tworzony w editorze, przypisany do prefaba |
| Singleton bez ochrony (DialogueUI) | GameManager z proper null check |
| Martwy kod (LootTable, GameManager pusty) | Brak nieużywanych klas |
| Brak interfejsów | IGameMode dla separacji local/single/online |
| Magic numbers | GameConfig ScriptableObject |
| TMPro problemy z pakietem | UnityEngine.UI.Text (wbudowany) |
| asmdef problemy z referencjami | Brak asmdef — Assembly-CSharp default |
