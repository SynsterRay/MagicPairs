# Bugs & TODO

## Znane problemy

- [ ] **PairCollector.cs** — pozycjonowanie zebranych kart może wychodzić poza ekran przy dużej liczbie par (grid > 4×4). Dodać scrollowanie lub zmniejszanie skali.
- [ ] **CardGrid.cs** — grid wymaga nieparzystej liczby kart (dla Piotrusia). Przy parzystym gridRows×gridCols jedna pozycja będzie pusta.
- [ ] **LocalGameMode.cs** — jeśli gracz kliknie Piotrusia jako drugą kartę po wybraniu pierwszej, pierwsza karta też wraca (zamierzone, ale może być nieintuicyjne).
- [ ] **MainMenu.cs** — `Player1Name`/`Player2Name` są statyczne — nie resetują się między sesjami Unity.

## TODO (priorytet wysoki)

- [ ] Wybór trudności (Easy 3×4, Medium 4×5, Hard 5×6) w menu startowym
- [ ] Dźwięki: flip karty, znalezienie pary, Piotruś, game over
- [ ] Wizualne wyróżnienie karty Piotruś (inny rewers lub symbol)

## TODO (priorytet średni)

- [ ] Online multiplayer — implementacja `OnlineGameMode` (placeholder istnieje)
- [ ] Save/Load wyników — lokalny leaderboard (JSON)
- [ ] Safe Area — padding UI dla notchy/zaokrągleń na mobile
- [ ] Efekt cząsteczkowy przy znalezieniu pary
- [ ] Animacja "pulse" przy dotknięciu karty (feedback dotykowy)
- [ ] Timer opcjonalny — ograniczenie czasu na turę

## TODO (priorytet niski)

- [ ] Motywy graficzne (zwierzęta, kształty zamiast kolorów)
- [ ] Więcej niż 2 graczy
- [ ] Tryb single-player (vs AI)
- [ ] Statystyki gracza (% trafień, najszybsza gra)

## Lekcje z projektu DemonsAndAngels (zastosowane)

| Problem w D&A | Rozwiązanie w MagicPairs |
|---------------|--------------------------|
| Polling w Update() (PlayerHealthBar) | Event-driven: GameEvents static bus |
| `.material.color` leak (AttackFeedback) | MaterialPropertyBlock w CardAnimator |
| SceneSetup nie idempotentny | Sprawdzanie istniejących obiektów przed tworzeniem |
| Shader.Find() nie działa w buildach | Materiał tworzony w editorze, przypisany do prefaba |
| Singleton bez ochrony (DialogueUI) | GameManager z proper null check |
| Martwy kod (LootTable, GameManager pusty) | Brak nieużywanych klas |
| Brak interfejsów | IGameMode dla separacji local/online |
| Magic numbers | GameConfig ScriptableObject |
| TMPro problemy z pakietem | UnityEngine.UI.Text (wbudowany) |
| asmdef problemy z referencjami | Brak asmdef — Assembly-CSharp default |
