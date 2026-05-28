# Bugs & TODO

## Naprawione

- [x] ~~Karty zasłaniane przez panel UI na trudnym poziomie~~ — kamera auto-skaluje się do rozmiaru gridu
- [x] ~~Piotruś/Joker niewidoczny po odkryciu (biały na białym)~~ — fallback kolor czarny
- [x] ~~Sprite'y kart różnych rozmiarów~~ — SpriteDrawMode.Sliced wymusza jednolity rozmiar
- [x] ~~Panel zebranych kart nie otwierał się~~ — przyciski "Karty" zamiast world raycast
- [x] ~~Build cache .utmp w repo~~ — dodany do .gitignore
- [x] ~~back_card.png ładowany jako karta do gry~~ — wykluczony z LoadPrincessSprites
- [x] ~~Zebrane pary nachodzą na karty na mobile~~ — karty znikają, widoczne tylko w panelu "Karty"

## Znane problemy

- [ ] **GameConfig** — paleta kolorów ma 8 kolorów, ale Hard (5×6) wymaga 14 par. Kolory się powtarzają.
- [ ] **PairCollector.cs** — zebrane karty mogą wychodzić poza ekran przy dużej liczbie par na Hard.
- [ ] **MainMenu.cs** — `Player1Name`/`Player2Name` są statyczne — nie resetują się między sesjami Unity.
- [ ] **Application.Quit()** — nie działa w edytorze Unity, tylko w buildzie.

## TODO (priorytet wysoki)

- [ ] Dodać więcej kolorów do palety (min. 15 dla Hard)
- [ ] Dźwięki: flip karty, znalezienie pary, Piotruś/Joker, game over
- [ ] Wizualne wyróżnienie karty Piotruś/Joker w trybie kolorów

## TODO (priorytet średni)

- [ ] Online multiplayer — implementacja `OnlineGameMode` (placeholder istnieje)
- [ ] Save/Load wyników — lokalny leaderboard (JSON)
- [ ] Safe Area — padding UI dla notchy/zaokrągleń na mobile
- [ ] Efekt cząsteczkowy przy znalezieniu pary
- [ ] Timer opcjonalny — ograniczenie czasu na turę
- [ ] Więcej motywów kart (zwierzęta, pojazdy, itp.)

## TODO (priorytet niski)

- [ ] Więcej niż 2 graczy
- [ ] Statystyki gracza (% trafień, najszybsza gra)
- [ ] Animacja tasowania kart przed rozdaniem
- [ ] Podgląd wszystkich kart na 2 sekundy przed startem (opcjonalny)

## Lekcje z projektu DemonsAndAngels (zastosowane)

| Problem w D&A | Rozwiązanie w MagicPairs |
|---------------|--------------------------|
| Polling w Update() | Event-driven: GameEvents static bus |
| `.material.color` leak | MaterialPropertyBlock + SpriteRenderer |
| SceneSetup nie idempotentny | Sprawdzanie istniejących obiektów |
| Shader.Find() nie działa w buildach | Materiał z prefaba / Resources |
| Singleton bez ochrony | GameManager z null check |
| Martwy kod | Brak nieużywanych klas |
| Brak interfejsów | IGameMode (local/single/online) |
| Magic numbers | GameConfig ScriptableObject |
| TMPro problemy | UnityEngine.UI.Text (wbudowany) |
| asmdef problemy | Brak asmdef — Assembly-CSharp default |
