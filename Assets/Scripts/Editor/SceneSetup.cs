using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace MagicPairs.Editor
{
    public static class SceneSetup
    {
        [MenuItem("MagicPairs/Setup Scene")]
        public static void SetupScene()
        {
            if (GameObject.Find("GameManager") != null)
            {
                if (!EditorUtility.DisplayDialog("MagicPairs",
                    "Scena już jest skonfigurowana. Chcesz ją przebudować?",
                    "Tak, przebuduj", "Anuluj"))
                    return;
                ClearScene();
            }

            CreateMaterials();
            CreateCardPrefab();
            CreateScene();
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("[MagicPairs] Scene setup complete!");
        }

        private static void ClearScene()
        {
            var all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in all)
            {
                if (go == null) continue;
                if (go.GetComponent<UnityEngine.Camera>() != null && go.CompareTag("MainCamera")) continue;
                if (go.GetComponent<Light>() != null) continue;
                Object.DestroyImmediate(go);
            }
        }

        private static void CreateMaterials()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Cards"))
                AssetDatabase.CreateFolder("Assets/Materials", "Cards");

            CreateMat("CardBackMat", new Color(0.15f, 0.1f, 0.3f));
        }

        private static Material CreateMat(string name, Color color)
        {
            string path = $"Assets/Materials/Cards/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var shader = Shader.Find("Universal Render Pipeline/Unlit")
                      ?? Shader.Find("Unlit/Color")
                      ?? Shader.Find("Standard");
            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.color = color;
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static void CreateCardPrefab()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Cards"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Cards");

            string prefabPath = "Assets/Prefabs/Cards/Card.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null) return;

            var card = GameObject.CreatePrimitive(PrimitiveType.Quad);
            card.name = "Card";
            card.transform.localScale = new Vector3(1.0f, 1.4f, 1f);

            Object.DestroyImmediate(card.GetComponent<MeshCollider>());
            var box = card.AddComponent<BoxCollider>();
            box.size = new Vector3(1f, 1f, 0.1f);

            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cards/CardBackMat.mat");
            card.GetComponent<MeshRenderer>().sharedMaterial = mat;

            card.AddComponent<Cards.CardController>();
            card.AddComponent<Cards.CardAnimator>();

            PrefabUtility.SaveAsPrefabAsset(card, prefabPath);
            Object.DestroyImmediate(card);
        }

        private static void CreateScene()
        {
            var cam = UnityEngine.Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.transform.position = new Vector3(0f, 0f, -10f);
                cam.transform.rotation = Quaternion.identity;
                cam.backgroundColor = new Color(0.12f, 0.12f, 0.18f);
            }

            CreateGameConfig();

            var gm = new GameObject("GameManager");
            var manager = gm.AddComponent<Core.GameManager>();
            gm.AddComponent<GameFlow.LocalGameMode>();
            var singleMode = gm.AddComponent<GameFlow.SinglePlayerMode>();
            singleMode.enabled = false;
            var challengeMode = gm.AddComponent<GameFlow.ChallengeMode>();
            challengeMode.enabled = false;
            gm.AddComponent<GameFlow.PowerUpManager>();
            var timeAttackMode = gm.AddComponent<GameFlow.TimeAttackMode>();
            timeAttackMode.enabled = false;
            gm.AddComponent<Cards.MatchEffect>();
            gm.AddComponent<Players.ScoreTracker>();
            gm.AddComponent<Input.TouchInputHandler>();
            gm.AddComponent<Audio.SFXManager>();
            gm.AddComponent<Audio.MusicManager>();
            gm.AddComponent<Ads.AdManager>();
            gm.AddComponent<Core.GPGSManager>();
            gm.AddComponent<Core.AchievementTracker>();

            var cardGrid = gm.AddComponent<Cards.CardGrid>();
            gm.AddComponent<Cards.PairCollector>();

            var config = AssetDatabase.LoadAssetAtPath<Core.GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
            var managerSo = new SerializedObject(manager);
            managerSo.FindProperty("config").objectReferenceValue = config;
            managerSo.ApplyModifiedProperties();

            var cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/Card.prefab");
            var gridSo = new SerializedObject(cardGrid);
            gridSo.FindProperty("cardPrefab").objectReferenceValue = cardPrefab;
            gridSo.ApplyModifiedProperties();

            CreateUI();

            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        private static void CreateGameConfig()
        {
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

            string path = "Assets/ScriptableObjects/GameConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<Core.GameConfig>(path) != null) return;

            var config = ScriptableObject.CreateInstance<Core.GameConfig>();
            AssetDatabase.CreateAsset(config, path);
        }

        private static void CreateUI()
        {
            var canvas = new GameObject("Canvas");
            var c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvas.AddComponent<GraphicRaycaster>();

            // Safe Area wrapper — all UI goes inside this
            var safeArea = new GameObject("SafeArea");
            safeArea.transform.SetParent(canvas.transform, false);
            var safeRect = safeArea.AddComponent<RectTransform>();
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;
            safeArea.AddComponent<UI.SafeArea>();

            // Score Display - top (on white bar)
            var topBar = new GameObject("TopBar");
            topBar.transform.SetParent(canvas.transform, false);
            var topBarImg = topBar.AddComponent<Image>();
            topBarImg.color = Color.white;
            var topBarRect = topBar.GetComponent<RectTransform>();
            topBarRect.anchorMin = new Vector2(0f, 0.82f);
            topBarRect.anchorMax = new Vector2(1f, 0.93f);
            topBarRect.offsetMin = Vector2.zero;
            topBarRect.offsetMax = Vector2.zero;

            var scorePanel = new GameObject("ScorePanel");
            scorePanel.transform.SetParent(topBar.transform, false);
            var scoreRect = scorePanel.AddComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0f, 0.3f);
            scoreRect.anchorMax = new Vector2(1f, 1f);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;

            var p1Score = CreateUIText("Player1Score", "Gracz 1: 0", scorePanel.transform,
                new Vector2(0.15f, 0f), new Vector2(0.5f, 1f), TextAnchor.MiddleCenter, 18);
            var p2Score = CreateUIText("Player2Score", "Gracz 2: 0", scorePanel.transform,
                new Vector2(0.5f, 0f), new Vector2(0.85f, 1f), TextAnchor.MiddleCenter, 18);

            var scoreDisplay = canvas.AddComponent<UI.ScoreDisplay>();
            var sdSo = new SerializedObject(scoreDisplay);
            sdSo.FindProperty("player1ScoreText").objectReferenceValue = p1Score;
            sdSo.FindProperty("player2ScoreText").objectReferenceValue = p2Score;
            sdSo.ApplyModifiedProperties();

            // Turn Indicator
            // Gray turn bar — bottom strip of TopBar (background for turn indicator)
            var turnBar = new GameObject("TurnBar");
            turnBar.transform.SetParent(topBar.transform, false);
            var turnBarImg = turnBar.AddComponent<Image>();
            turnBarImg.color = new Color(0.88f, 0.88f, 0.90f, 1f);
            var turnBarRect = turnBar.GetComponent<RectTransform>();
            turnBarRect.anchorMin = new Vector2(0f, 0f);
            turnBarRect.anchorMax = new Vector2(1f, 0.3f);
            turnBarRect.offsetMin = Vector2.zero;
            turnBarRect.offsetMax = Vector2.zero;

            // Turn Indicator — bottom of white TopBar, below scores
            var turnText = CreateUIText("TurnIndicator", "Tura: Gracz 1", topBar.transform,
                new Vector2(0.2f, 0f), new Vector2(0.8f, 0.3f), TextAnchor.MiddleCenter, 14);

            // Pause/Menu Button (top right corner)
            var pauseBtn = CreateButton("PauseBtn", "✕", canvas.transform,
                new Vector2(0.88f, 0.93f), new Vector2(0.98f, 0.99f));
            pauseBtn.GetComponent<Image>().color = new Color(0.7f, 0.2f, 0.2f, 0.9f);
            pauseBtn.SetActive(false);

            // Confirm panel
            var confirmPanel = new GameObject("ConfirmPanel");
            confirmPanel.transform.SetParent(canvas.transform, false);
            var cpImg = confirmPanel.AddComponent<Image>();
            cpImg.color = new Color(0f, 0f, 0f, 0.65f);
            cpImg.sprite = UI.RoundedButtonHelper.GetRoundedSprite();
            cpImg.type = Image.Type.Sliced;
            var cpRect = confirmPanel.GetComponent<RectTransform>();
            cpRect.anchorMin = new Vector2(0.15f, 0.35f);
            cpRect.anchorMax = new Vector2(0.85f, 0.65f);
            cpRect.offsetMin = Vector2.zero;
            cpRect.offsetMax = Vector2.zero;

            var confirmText = CreateUIText("ConfirmText", "Wróć do menu?", confirmPanel.transform,
                new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, 28);
            confirmText.color = Color.white;

            var yesBtn = CreateButton("YesBtn", "Tak", confirmPanel.transform,
                new Vector2(0.1f, 0.1f), new Vector2(0.45f, 0.45f));
            yesBtn.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.2f, 1f);

            var noBtn = CreateButton("NoBtn", "Nie", confirmPanel.transform,
                new Vector2(0.55f, 0.1f), new Vector2(0.9f, 0.45f));
            noBtn.GetComponent<Image>().color = new Color(0.7f, 0.2f, 0.2f, 1f);

            var pauseComp = canvas.AddComponent<UI.PauseButton>();
            var pauseSo = new SerializedObject(pauseComp);
            pauseSo.FindProperty("pauseButton").objectReferenceValue = pauseBtn.GetComponent<Button>();
            pauseSo.FindProperty("confirmPanel").objectReferenceValue = confirmPanel;
            pauseSo.FindProperty("yesButton").objectReferenceValue = yesBtn.GetComponent<Button>();
            pauseSo.FindProperty("noButton").objectReferenceValue = noBtn.GetComponent<Button>();
            pauseSo.ApplyModifiedProperties();

            // Collected Cards Panel (overlay when clicking collected pairs)
            var ccPanel = new GameObject("CollectedCardsPanel");
            ccPanel.transform.SetParent(canvas.transform, false);
            var ccImg = ccPanel.AddComponent<Image>();
            ccImg.color = new Color(1f, 1f, 1f, 0.95f);
            var ccRect = ccPanel.GetComponent<RectTransform>();
            ccRect.anchorMin = new Vector2(0.05f, 0.1f);
            ccRect.anchorMax = new Vector2(0.95f, 0.9f);
            ccRect.offsetMin = Vector2.zero;
            ccRect.offsetMax = Vector2.zero;
            ccPanel.SetActive(false);

            var ccTitle = CreateUIText("CCTitle", "Zebrane pary", ccPanel.transform,
                new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.98f), TextAnchor.MiddleCenter, 28);

            var ccCloseBtn = CreateButton("CCCloseBtn", "Zamknij", ccPanel.transform,
                new Vector2(0.3f, 0.02f), new Vector2(0.7f, 0.1f));
            ccCloseBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // Scroll content area
            var ccContent = new GameObject("CCContent");
            ccContent.transform.SetParent(ccPanel.transform, false);
            var ccContentRect = ccContent.AddComponent<RectTransform>();
            ccContentRect.anchorMin = new Vector2(0.05f, 0.05f);
            ccContentRect.anchorMax = new Vector2(0.95f, 0.86f);
            ccContentRect.offsetMin = Vector2.zero;
            ccContentRect.offsetMax = Vector2.zero;
            var grid2 = ccContent.AddComponent<GridLayoutGroup>();
            grid2.cellSize = new Vector2(120f, 170f);
            grid2.spacing = new Vector2(15f, 15f);
            grid2.childAlignment = TextAnchor.UpperCenter;

            // Card slot prefab (for displaying collected cards)
            var ccSlot = new GameObject("CardSlot");
            ccSlot.transform.SetParent(canvas.transform, false);
            var slotImg = ccSlot.AddComponent<Image>();
            slotImg.color = Color.white;
            ccSlot.SetActive(false);

            // Buttons to view collected cards (below score panel, left and right)
            var p1CollBtn = CreateButton("P1CollBtn", "Karty", canvas.transform,
                new Vector2(-0.01f, 0.82f), new Vector2(0.15f, 0.93f));
            p1CollBtn.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.9f, 1f);
            p1CollBtn.GetComponentInChildren<Text>().fontSize = 36;
            p1CollBtn.SetActive(false);

            var p2CollBtn = CreateButton("P2CollBtn", "Karty", canvas.transform,
                new Vector2(0.85f, 0.82f), new Vector2(1.01f, 0.93f));
            p2CollBtn.GetComponent<Image>().color = new Color(0.9f, 0.4f, 0.4f, 1f);
            p2CollBtn.GetComponentInChildren<Text>().fontSize = 36;
            p2CollBtn.SetActive(false);

            var ccComp = canvas.AddComponent<UI.CollectedCardsPanel>();
            var ccSo = new SerializedObject(ccComp);
            ccSo.FindProperty("panel").objectReferenceValue = ccPanel;
            ccSo.FindProperty("titleText").objectReferenceValue = ccTitle;
            ccSo.FindProperty("contentContainer").objectReferenceValue = ccContent.transform;
            ccSo.FindProperty("closeButton").objectReferenceValue = ccCloseBtn.GetComponent<Button>();
            ccSo.FindProperty("cardSlotPrefab").objectReferenceValue = ccSlot;
            ccSo.FindProperty("player1Button").objectReferenceValue = p1CollBtn.GetComponent<Button>();
            ccSo.FindProperty("player2Button").objectReferenceValue = p2CollBtn.GetComponent<Button>();
            ccSo.ApplyModifiedProperties();

            var turnIndicator = canvas.AddComponent<UI.TurnIndicator>();
            var tiSo = new SerializedObject(turnIndicator);
            tiSo.FindProperty("turnText").objectReferenceValue = turnText;
            tiSo.ApplyModifiedProperties();

            // Game Over Panel
            var goPanel = new GameObject("GameOverPanel");
            goPanel.transform.SetParent(canvas.transform, false);
            var goPanelImg = goPanel.AddComponent<Image>();
            goPanelImg.color = new Color(1f, 1f, 1f, 0.95f);
            var goPanelRect = goPanel.GetComponent<RectTransform>();
            goPanelRect.anchorMin = new Vector2(0.1f, 0.3f);
            goPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            goPanelRect.offsetMin = Vector2.zero;
            goPanelRect.offsetMax = Vector2.zero;

            var resultText = CreateUIText("ResultText", "Wygrywa!", goPanel.transform,
                new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, 42);

            var playAgainBtn = CreateButton("PlayAgainBtn", "Zagraj ponownie", goPanel.transform,
                new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.4f));

            var gameOverPanel = canvas.AddComponent<UI.GameOverPanel>();
            var gopSo = new SerializedObject(gameOverPanel);
            gopSo.FindProperty("panel").objectReferenceValue = goPanel;
            gopSo.FindProperty("resultText").objectReferenceValue = resultText;
            gopSo.FindProperty("playAgainButton").objectReferenceValue = playAgainBtn.GetComponent<Button>();
            gopSo.FindProperty("playAgainText").objectReferenceValue = playAgainBtn.GetComponentInChildren<Text>();
            gopSo.ApplyModifiedProperties();

            // --- Challenge UI ---
            // Score panel (top center, replaces score display in challenge mode)
            var chScorePanel = new GameObject("ChallengeScorePanel");
            chScorePanel.transform.SetParent(canvas.transform, false);
            var chScoreRect = chScorePanel.AddComponent<RectTransform>();
            chScoreRect.anchorMin = new Vector2(0.15f, 0.82f);
            chScoreRect.anchorMax = new Vector2(0.85f, 0.93f);
            chScoreRect.offsetMin = Vector2.zero;
            chScoreRect.offsetMax = Vector2.zero;

            var chLevelText = CreateUIText("ChLevelText", "Poziom 1", chScorePanel.transform,
                new Vector2(0f, 0f), new Vector2(0.38f, 1f), TextAnchor.MiddleCenter, 20);
            chLevelText.resizeTextForBestFit = true;
            chLevelText.resizeTextMinSize = 20;
            chLevelText.resizeTextMaxSize = 52;
            var chScoreText = CreateUIText("ChScoreText", "0", chScorePanel.transform,
                new Vector2(0.4f, 0f), new Vector2(0.72f, 1f), TextAnchor.MiddleCenter, 24);
            var chStreakText = CreateUIText("ChStreakText", "", chScorePanel.transform,
                new Vector2(0.74f, 0f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 22);
            chStreakText.color = new Color(0.9f, 0.5f, 0f);

            // Level Complete panel
            var lvlCompletePanel = new GameObject("LevelCompletePanel");
            lvlCompletePanel.transform.SetParent(canvas.transform, false);
            var lcImg = lvlCompletePanel.AddComponent<Image>();
            lcImg.color = new Color(0.1f, 0.5f, 0.2f, 0.85f);
            lcImg.sprite = UI.RoundedButtonHelper.GetRoundedSprite();
            lcImg.type = Image.Type.Sliced;
            var lcRect = lvlCompletePanel.GetComponent<RectTransform>();
            lcRect.anchorMin = new Vector2(0.15f, 0.35f);
            lcRect.anchorMax = new Vector2(0.85f, 0.65f);
            lcRect.offsetMin = Vector2.zero;
            lcRect.offsetMax = Vector2.zero;

            var lvlCompleteText = CreateUIText("LvlCompleteText", "Poziom ukończony!", lvlCompletePanel.transform,
                new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, 28);
            lvlCompleteText.color = Color.white;

            var nextLvlBtn = CreateButton("NextLvlBtn", "Dalej", lvlCompletePanel.transform,
                new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.4f));
            nextLvlBtn.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.3f, 1f);

            // Challenge Over panel
            var chOverPanel = new GameObject("ChallengeOverPanel");
            chOverPanel.transform.SetParent(canvas.transform, false);
            var coImg = chOverPanel.AddComponent<Image>();
            coImg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            var coRect = chOverPanel.GetComponent<RectTransform>();
            coRect.anchorMin = new Vector2(0.1f, 0.25f);
            coRect.anchorMax = new Vector2(0.9f, 0.75f);
            coRect.offsetMin = Vector2.zero;
            coRect.offsetMax = Vector2.zero;

            var chFinalScoreText = CreateUIText("ChFinalScore", "Koniec! Wynik: 0", chOverPanel.transform,
                new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, 30);
            chFinalScoreText.color = Color.white;

            var chMenuBtn = CreateButton("ChMenuBtn", "Menu", chOverPanel.transform,
                new Vector2(0.1f, 0.1f), new Vector2(0.45f, 0.4f));

            var chLeaderBtn = CreateButton("ChLeaderBtn", "Wyniki", chOverPanel.transform,
                new Vector2(0.55f, 0.1f), new Vector2(0.9f, 0.4f));
            chLeaderBtn.GetComponent<Image>().color = new Color(0.8f, 0.6f, 0.1f, 1f);

            var secondChanceBtn = CreateButton("SecondChanceBtn", "▶ Second Chance", chOverPanel.transform,
                new Vector2(0.15f, 0.42f), new Vector2(0.85f, 0.55f));
            secondChanceBtn.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.3f, 1f);
            secondChanceBtn.GetComponentInChildren<Text>().fontSize = 24;
            secondChanceBtn.SetActive(false);

            // Leaderboard panel
            var leaderPanel = new GameObject("LeaderboardPanel");
            leaderPanel.transform.SetParent(canvas.transform, false);
            var lpImg = leaderPanel.AddComponent<Image>();
            lpImg.color = new Color(1f, 1f, 1f, 0.95f);
            var lpRect = leaderPanel.GetComponent<RectTransform>();
            lpRect.anchorMin = new Vector2(0.05f, 0.1f);
            lpRect.anchorMax = new Vector2(0.95f, 0.9f);
            lpRect.offsetMin = Vector2.zero;
            lpRect.offsetMax = Vector2.zero;

            var leaderText = CreateUIText("LeaderText", "", leaderPanel.transform,
                new Vector2(0.05f, 0.15f), new Vector2(0.95f, 0.85f), TextAnchor.UpperLeft, 22);

            var leaderBackBtn = CreateButton("LeaderBackBtn", "←", leaderPanel.transform,
                new Vector2(0.3f, 0.02f), new Vector2(0.7f, 0.12f));
            leaderBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // Wire ChallengeUI
            var challengeUI = canvas.AddComponent<UI.ChallengeUI>();
            var cuSo = new SerializedObject(challengeUI);
            cuSo.FindProperty("scorePanel").objectReferenceValue = chScorePanel;
            cuSo.FindProperty("scoreText").objectReferenceValue = chScoreText;
            cuSo.FindProperty("streakText").objectReferenceValue = chStreakText;
            cuSo.FindProperty("levelText").objectReferenceValue = chLevelText;
            cuSo.FindProperty("levelCompletePanel").objectReferenceValue = lvlCompletePanel;
            cuSo.FindProperty("levelCompleteText").objectReferenceValue = lvlCompleteText;
            cuSo.FindProperty("nextLevelButton").objectReferenceValue = nextLvlBtn.GetComponent<Button>();
            cuSo.FindProperty("challengeOverPanel").objectReferenceValue = chOverPanel;
            cuSo.FindProperty("finalScoreText").objectReferenceValue = chFinalScoreText;
            cuSo.FindProperty("challengeMenuButton").objectReferenceValue = chMenuBtn.GetComponent<Button>();
            cuSo.FindProperty("leaderboardPanel").objectReferenceValue = leaderPanel;
            cuSo.FindProperty("leaderboardText").objectReferenceValue = leaderText;
            cuSo.FindProperty("leaderboardBackButton").objectReferenceValue = leaderBackBtn.GetComponent<Button>();
            cuSo.FindProperty("showLeaderboardButton").objectReferenceValue = chLeaderBtn.GetComponent<Button>();
            cuSo.FindProperty("secondChanceButton").objectReferenceValue = secondChanceBtn.GetComponent<Button>();
            cuSo.ApplyModifiedProperties();

            // Score Popup (floating +points animation)
            canvas.AddComponent<UI.ScorePopup>();
            canvas.AddComponent<UI.TimeAttackUI>();
            canvas.AddComponent<UI.DailyBonus>();
            canvas.AddComponent<UI.ShopUI>();

            // Main Menu Panel
            var menuPanel = new GameObject("MenuPanel");
            menuPanel.transform.SetParent(canvas.transform, false);
            var menuImg = menuPanel.AddComponent<Image>();
            menuImg.color = new Color(1f, 1f, 1f, 1f);
            var menuRect = menuPanel.GetComponent<RectTransform>();
            menuRect.anchorMin = Vector2.zero;
            menuRect.anchorMax = Vector2.one;
            menuRect.offsetMin = Vector2.zero;
            menuRect.offsetMax = Vector2.zero;

            // Title Logo
            var logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/logo_unicorn.png");
            var titleObj = new GameObject("TitleLogo");
            titleObj.transform.SetParent(menuPanel.transform, false);
            var titleImg = titleObj.AddComponent<Image>();
            titleImg.sprite = logoSprite;
            titleImg.preserveAspect = true;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.15f, 0.72f);
            titleRect.anchorMax = new Vector2(0.85f, 0.93f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            // Game Background (behind cards, on separate canvas)
            var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/background_game.png");
            var bgCanvas = new GameObject("BackgroundCanvas");
            var bgC = bgCanvas.AddComponent<Canvas>();
            bgC.renderMode = RenderMode.ScreenSpaceCamera;
            bgC.worldCamera = UnityEngine.Camera.main;
            bgC.planeDistance = 15f;
            bgC.sortingOrder = -1;
            bgCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();

            var bgObj = new GameObject("GameBackground");
            bgObj.transform.SetParent(bgCanvas.transform, false);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.sprite = bgSprite;
            bgImg.preserveAspect = false;
            bgImg.color = new Color(1f, 1f, 1f, 0.5f);
            bgObj.AddComponent<UI.GameBackground>();
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = new Vector2(1f, 0.85f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // --- Start Panel (main screen) ---
            var startPanel = new GameObject("StartPanel");
            startPanel.transform.SetParent(menuPanel.transform, false);
            var startPanelRect = startPanel.AddComponent<RectTransform>();
            startPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            startPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            startPanelRect.offsetMin = Vector2.zero;
            startPanelRect.offsetMax = Vector2.zero;

            var playBtn = CreateButton("PlayBtn", "Graj", startPanel.transform,
                new Vector2(0.2f, 0.76f), new Vector2(0.8f, 0.96f));
            playBtn.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.3f, 1f);

            var shopBtn = CreateButton("ShopBtn", "🪙 Sklep", startPanel.transform,
                new Vector2(0.2f, 0.52f), new Vector2(0.8f, 0.72f));
            shopBtn.GetComponent<Image>().color = new Color(0.9f, 0.7f, 0.1f, 1f);

            var optionsBtn = CreateButton("OptionsBtn", "Opcje", startPanel.transform,
                new Vector2(0.2f, 0.28f), new Vector2(0.8f, 0.48f));
            optionsBtn.GetComponent<Image>().color = new Color(0.3f, 0.5f, 0.8f, 1f);

            var leaderBtn = CreateButton("LeaderboardBtn", "Wyniki", startPanel.transform,
                new Vector2(0.2f, 0.04f), new Vector2(0.8f, 0.24f));
            leaderBtn.GetComponent<Image>().color = new Color(0.8f, 0.6f, 0.1f, 1f);

            var quitBtn = CreateButton("QuitBtn", "Wyjdź", startPanel.transform,
                new Vector2(0.2f, -0.20f), new Vector2(0.8f, 0.0f));
            quitBtn.GetComponent<Image>().color = new Color(0.6f, 0.15f, 0.15f, 1f);

            // --- Start Leaderboard Panel ---
            var startLeaderPanel = new GameObject("StartLeaderboardPanel");
            startLeaderPanel.transform.SetParent(menuPanel.transform, false);
            var slpRect = startLeaderPanel.AddComponent<RectTransform>();
            slpRect.anchorMin = new Vector2(0.05f, 0.05f);
            slpRect.anchorMax = new Vector2(0.95f, 1.0f);
            slpRect.offsetMin = Vector2.zero;
            slpRect.offsetMax = Vector2.zero;
            var slpImg = startLeaderPanel.AddComponent<Image>();
            slpImg.color = new Color(1f, 1f, 1f, 0.95f);

            var startLeaderText = CreateUIText("StartLeaderText", "", startLeaderPanel.transform,
                new Vector2(0.05f, 0.15f), new Vector2(0.95f, 0.85f), TextAnchor.UpperLeft, 22);

            var startLeaderBackBtn = CreateButton("StartLeaderBackBtn", "←", startLeaderPanel.transform,
                new Vector2(0.3f, 0.02f), new Vector2(0.7f, 0.12f));
            startLeaderBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Options Panel ---
            var optionsPanel = new GameObject("OptionsPanel");
            optionsPanel.transform.SetParent(menuPanel.transform, false);
            var optionsPanelRect = optionsPanel.AddComponent<RectTransform>();
            optionsPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            optionsPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            optionsPanelRect.offsetMin = Vector2.zero;
            optionsPanelRect.offsetMax = Vector2.zero;

            var optionsTitle = CreateUIText("OptionsTitle", "Opcje", optionsPanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 30);

            var langBtn = CreateButton("LanguageBtn", "Język", optionsPanel.transform,
                new Vector2(0.2f, 0.64f), new Vector2(0.8f, 0.8f));

            var menuMusicBtn = CreateButton("MenuMusicBtn", "Muzyka menu ✓", optionsPanel.transform,
                new Vector2(0.2f, 0.46f), new Vector2(0.8f, 0.62f));
            menuMusicBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.5f, 1f);

            var gameMusicBtn = CreateButton("GameMusicBtn", "Muzyka w grze ✓", optionsPanel.transform,
                new Vector2(0.2f, 0.28f), new Vector2(0.8f, 0.44f));
            gameMusicBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.5f, 1f);

            var creditsBtn = CreateButton("CreditsBtn", "Autor", optionsPanel.transform,
                new Vector2(0.2f, 0.10f), new Vector2(0.8f, 0.26f));
            creditsBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.5f, 1f);

            var optionsBackBtn = CreateButton("OptionsBackBtn", "←", optionsPanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            optionsBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Language Panel ---
            var langPanel = new GameObject("LanguagePanel");
            langPanel.transform.SetParent(menuPanel.transform, false);
            var langPanelRect = langPanel.AddComponent<RectTransform>();
            langPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            langPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            langPanelRect.offsetMin = Vector2.zero;
            langPanelRect.offsetMax = Vector2.zero;

            var langTitle = CreateUIText("LangTitle", "Choose Language / Wybierz język", langPanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 30);

            var plBtn = CreateButton("PolishBtn", "Polski", langPanel.transform,
                new Vector2(0.2f, 0.52f), new Vector2(0.8f, 0.68f));
            var enBtn = CreateButton("EnglishBtn", "English", langPanel.transform,
                new Vector2(0.2f, 0.26f), new Vector2(0.8f, 0.42f));

            var langBackBtn = CreateButton("LangBackBtn", "←", langPanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            langBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Credits Panel ---
            var creditsPanel = new GameObject("CreditsPanel");
            creditsPanel.transform.SetParent(menuPanel.transform, false);
            var creditsPanelRect = creditsPanel.AddComponent<RectTransform>();
            creditsPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            creditsPanelRect.anchorMax = new Vector2(0.9f, 0.75f);
            creditsPanelRect.offsetMin = Vector2.zero;
            creditsPanelRect.offsetMax = Vector2.zero;
            creditsPanel.SetActive(false);

            CreateUIText("CreditsTitle", "Credits", creditsPanel.transform,
                new Vector2(0f, 0.7f), new Vector2(1f, 0.95f), TextAnchor.MiddleCenter, 32);
            CreateUIText("CreditsAuthor", "Created by\nMateusz Bajak", creditsPanel.transform,
                new Vector2(0.1f, 0.3f), new Vector2(0.9f, 0.65f), TextAnchor.MiddleCenter, 28);

            var creditsBackBtn = CreateButton("CreditsBackBtn", "←", creditsPanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            creditsBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Game Type Panel ---
            var gameTypePanel = new GameObject("GameTypePanel");
            gameTypePanel.transform.SetParent(menuPanel.transform, false);
            var gameTypePanelRect = gameTypePanel.AddComponent<RectTransform>();
            gameTypePanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            gameTypePanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            gameTypePanelRect.offsetMin = Vector2.zero;
            gameTypePanelRect.offsetMax = Vector2.zero;

            var gameTypeTitle = CreateUIText("GameTypeTitle", "Wybierz tryb gry", gameTypePanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 30);

            var challengeBtn = CreateButton("ChallengeBtn", "Wyzwanie", gameTypePanel.transform,
                new Vector2(0.2f, 0.64f), new Vector2(0.8f, 0.8f));
            challengeBtn.GetComponent<Image>().color = new Color(0.8f, 0.4f, 0.1f, 1f);

            var timeAttackBtn = CreateButton("TimeAttackBtn", "Na czas", gameTypePanel.transform,
                new Vector2(0.2f, 0.38f), new Vector2(0.8f, 0.54f));
            timeAttackBtn.GetComponent<Image>().color = new Color(0.7f, 0.2f, 0.5f, 1f);

            var arcadeBtn = CreateButton("ArcadeBtn", "Arcade", gameTypePanel.transform,
                new Vector2(0.2f, 0.12f), new Vector2(0.8f, 0.28f));
            arcadeBtn.GetComponent<Image>().color = new Color(0.2f, 0.5f, 0.9f, 1f);

            var gameTypeBackBtn = CreateButton("GameTypeBackBtn", "←", gameTypePanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            gameTypeBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Challenge Names Panel ---
            var challengeNamesPanel = new GameObject("ChallengeNamesPanel");
            challengeNamesPanel.transform.SetParent(menuPanel.transform, false);
            var challengeNamesPanelRect = challengeNamesPanel.AddComponent<RectTransform>();
            challengeNamesPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            challengeNamesPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            challengeNamesPanelRect.offsetMin = Vector2.zero;
            challengeNamesPanelRect.offsetMax = Vector2.zero;

            var challengeNameLabel = CreateUIText("ChallengeNameLabel", "Twoje imię", challengeNamesPanel.transform,
                new Vector2(0.05f, 0.65f), new Vector2(0.95f, 0.82f), TextAnchor.MiddleLeft, 24);
            var challengeNameInput = CreateInputField("ChallengeNameInput", "Player", challengeNamesPanel.transform,
                new Vector2(0.05f, 0.46f), new Vector2(0.95f, 0.63f));

            var challengeStartBtn = CreateButton("ChallengeStartBtn", "Start", challengeNamesPanel.transform,
                new Vector2(0.2f, 0.14f), new Vector2(0.8f, 0.34f));
            challengeStartBtn.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.3f, 1f);

            var challengeNamesBackBtn = CreateButton("ChallengeNamesBackBtn", "←", challengeNamesPanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            challengeNamesBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Challenge Theme Panel ---
            var challengeThemePanel = new GameObject("ChallengeThemePanel");
            challengeThemePanel.transform.SetParent(menuPanel.transform, false);
            var challengeThemePanelRect = challengeThemePanel.AddComponent<RectTransform>();
            challengeThemePanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            challengeThemePanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            challengeThemePanelRect.offsetMin = Vector2.zero;
            challengeThemePanelRect.offsetMax = Vector2.zero;

            var challengeThemeTitle = CreateUIText("ChallengeThemeTitle", "Wybierz typ kart", challengeThemePanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 28);

            var chCarsBtn = CreateButton("ChCarsBtn", "🚗 Samochody", challengeThemePanel.transform,
                new Vector2(0.2f, 0.64f), new Vector2(0.8f, 0.8f));
            chCarsBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.8f, 1f);
            var chPrincessBtn = CreateButton("ChPrincessBtn", "👸 Księżniczki", challengeThemePanel.transform,
                new Vector2(0.2f, 0.38f), new Vector2(0.8f, 0.54f));
            chPrincessBtn.GetComponent<Image>().color = new Color(0.9f, 0.4f, 0.7f, 1f);
            var chColorsBtn = CreateButton("ChColorsBtn", "🎨 Kolory", challengeThemePanel.transform,
                new Vector2(0.2f, 0.12f), new Vector2(0.8f, 0.28f));

            var challengeThemeBackBtn = CreateButton("ChallengeThemeBackBtn", "←", challengeThemePanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            challengeThemeBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Mode Panel ---
            var modePanel = new GameObject("ModePanel");
            modePanel.transform.SetParent(menuPanel.transform, false);
            var modePanelRect = modePanel.AddComponent<RectTransform>();
            modePanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            modePanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            modePanelRect.offsetMin = Vector2.zero;
            modePanelRect.offsetMax = Vector2.zero;

            var modeTitle = CreateUIText("ModeTitle", "Wybierz tryb", modePanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 30);

            var twoPlayersBtn = CreateButton("TwoPlayersBtn", "2 Graczy", modePanel.transform,
                new Vector2(0.2f, 0.52f), new Vector2(0.8f, 0.68f));
            var singlePlayerBtn = CreateButton("SinglePlayerBtn", "1 Gracz (vs AI)", modePanel.transform,
                new Vector2(0.2f, 0.26f), new Vector2(0.8f, 0.42f));
            singlePlayerBtn.GetComponent<Image>().color = new Color(0.6f, 0.3f, 0.8f, 1f);

            var modeBackBtn = CreateButton("ModeBackBtn", "←", modePanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            modeBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Difficulty Panel ---
            var diffPanel = new GameObject("DifficultyPanel");
            diffPanel.transform.SetParent(menuPanel.transform, false);
            var diffPanelRect = diffPanel.AddComponent<RectTransform>();
            diffPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            diffPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            diffPanelRect.offsetMin = Vector2.zero;
            diffPanelRect.offsetMax = Vector2.zero;

            var diffTitle = CreateUIText("DiffTitle", "Wybierz poziom trudności", diffPanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 28);

            var easyBtn = CreateButton("EasyBtn", "★", diffPanel.transform,
                new Vector2(0.05f, 0.3f), new Vector2(0.3f, 0.7f));
            easyBtn.GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.3f, 1f);
            easyBtn.GetComponentInChildren<Text>().fontSize = 48;

            var mediumBtn = CreateButton("MediumBtn", "★★", diffPanel.transform,
                new Vector2(0.35f, 0.3f), new Vector2(0.65f, 0.7f));
            mediumBtn.GetComponent<Image>().color = new Color(0.9f, 0.7f, 0.1f, 1f);
            mediumBtn.GetComponentInChildren<Text>().fontSize = 40;

            var hardBtn = CreateButton("HardBtn", "★★★", diffPanel.transform,
                new Vector2(0.7f, 0.3f), new Vector2(0.95f, 0.7f));
            hardBtn.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.2f, 1f);
            hardBtn.GetComponentInChildren<Text>().fontSize = 32;

            // Labels under stars
            CreateUIText("EasyLabel", "3x4", diffPanel.transform,
                new Vector2(0.05f, 0.18f), new Vector2(0.3f, 0.3f), TextAnchor.MiddleCenter, 20);
            CreateUIText("MediumLabel", "4x5", diffPanel.transform,
                new Vector2(0.35f, 0.18f), new Vector2(0.65f, 0.3f), TextAnchor.MiddleCenter, 20);
            CreateUIText("HardLabel", "5x6", diffPanel.transform,
                new Vector2(0.7f, 0.18f), new Vector2(0.95f, 0.3f), TextAnchor.MiddleCenter, 20);

            var diffBackBtn = CreateButton("DiffBackBtn", "←", diffPanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            diffBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Theme Panel ---
            var themePanel = new GameObject("ThemePanel");
            themePanel.transform.SetParent(menuPanel.transform, false);
            var themePanelRect = themePanel.AddComponent<RectTransform>();
            themePanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            themePanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            themePanelRect.offsetMin = Vector2.zero;
            themePanelRect.offsetMax = Vector2.zero;

            var themeTitle = CreateUIText("ThemeTitle", "Wybierz typ kart", themePanel.transform,
                new Vector2(0f, 0.82f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 28);

            var carsBtn = CreateButton("CarsBtn", "🚗 Samochody", themePanel.transform,
                new Vector2(0.2f, 0.64f), new Vector2(0.8f, 0.8f));
            carsBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.8f, 1f);
            var princessBtn = CreateButton("PrincessBtn", "👸 Księżniczki", themePanel.transform,
                new Vector2(0.2f, 0.38f), new Vector2(0.8f, 0.54f));
            princessBtn.GetComponent<Image>().color = new Color(0.9f, 0.4f, 0.7f, 1f);
            var colorsBtn = CreateButton("ColorsBtn", "🎨 Kolory", themePanel.transform,
                new Vector2(0.2f, 0.12f), new Vector2(0.8f, 0.28f));

            var themeBackBtn = CreateButton("ThemeBackBtn", "←", themePanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            themeBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // --- Names Panel ---
            var namesPanel = new GameObject("NamesPanel");
            namesPanel.transform.SetParent(menuPanel.transform, false);
            var namesPanelRect = namesPanel.AddComponent<RectTransform>();
            namesPanelRect.anchorMin = new Vector2(0.1f, 0.15f);
            namesPanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            namesPanelRect.offsetMin = Vector2.zero;
            namesPanelRect.offsetMax = Vector2.zero;

            var p1Label = CreateUIText("P1Label", "Imię Gracza 1", namesPanel.transform,
                new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.96f), TextAnchor.MiddleLeft, 24);
            var p1Input = CreateInputField("P1Input", "Player 1", namesPanel.transform,
                new Vector2(0.05f, 0.66f), new Vector2(0.95f, 0.8f));

            var p2Label = CreateUIText("P2Label", "Imię Gracza 2", namesPanel.transform,
                new Vector2(0.05f, 0.48f), new Vector2(0.95f, 0.62f), TextAnchor.MiddleLeft, 24);
            var p2Input = CreateInputField("P2Input", "Player 2", namesPanel.transform,
                new Vector2(0.05f, 0.32f), new Vector2(0.95f, 0.46f));

            var startBtn = CreateButton("StartBtn", "Start", namesPanel.transform,
                new Vector2(0.2f, 0.06f), new Vector2(0.8f, 0.26f));
            startBtn.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.3f, 1f);

            var namesBackBtn = CreateButton("NamesBackBtn", "←", namesPanel.transform,
                new Vector2(0.3f, -0.18f), new Vector2(0.7f, 0.0f));
            namesBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            // Wire MainMenu
            var mainMenu = canvas.AddComponent<UI.MainMenu>();
            var mmSo = new SerializedObject(mainMenu);
            mmSo.FindProperty("menuPanel").objectReferenceValue = menuPanel;
            mmSo.FindProperty("startPanel").objectReferenceValue = startPanel;
            mmSo.FindProperty("optionsPanel").objectReferenceValue = optionsPanel;
            mmSo.FindProperty("languagePanel").objectReferenceValue = langPanel;
            mmSo.FindProperty("modePanel").objectReferenceValue = modePanel;
            mmSo.FindProperty("difficultyPanel").objectReferenceValue = diffPanel;
            mmSo.FindProperty("namesPanel").objectReferenceValue = namesPanel;
            mmSo.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
            mmSo.FindProperty("optionsButton").objectReferenceValue = optionsBtn.GetComponent<Button>();
            mmSo.FindProperty("quitButton").objectReferenceValue = quitBtn.GetComponent<Button>();
            mmSo.FindProperty("leaderboardButton").objectReferenceValue = leaderBtn.GetComponent<Button>();
            mmSo.FindProperty("leaderboardButtonText").objectReferenceValue = leaderBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("startLeaderboardPanel").objectReferenceValue = startLeaderPanel;
            mmSo.FindProperty("startLeaderboardText").objectReferenceValue = startLeaderText;
            mmSo.FindProperty("startLeaderboardBackButton").objectReferenceValue = startLeaderBackBtn.GetComponent<Button>();
            mmSo.FindProperty("creditsButton").objectReferenceValue = creditsBtn.GetComponent<Button>();
            mmSo.FindProperty("creditsPanel").objectReferenceValue = creditsPanel;
            mmSo.FindProperty("creditsBackButton").objectReferenceValue = creditsBackBtn.GetComponent<Button>();
            mmSo.FindProperty("playButtonText").objectReferenceValue = playBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("optionsButtonText").objectReferenceValue = optionsBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("optionsTitle").objectReferenceValue = optionsTitle;
            mmSo.FindProperty("languageButton").objectReferenceValue = langBtn.GetComponent<Button>();
            mmSo.FindProperty("languageButtonText").objectReferenceValue = langBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("optionsBackButton").objectReferenceValue = optionsBackBtn.GetComponent<Button>();
            mmSo.FindProperty("menuMusicToggle").objectReferenceValue = menuMusicBtn.GetComponent<Button>();
            mmSo.FindProperty("menuMusicToggleText").objectReferenceValue = menuMusicBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("gameMusicToggle").objectReferenceValue = gameMusicBtn.GetComponent<Button>();
            mmSo.FindProperty("gameMusicToggleText").objectReferenceValue = gameMusicBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("languageTitle").objectReferenceValue = langTitle;
            mmSo.FindProperty("polishButton").objectReferenceValue = plBtn.GetComponent<Button>();
            mmSo.FindProperty("englishButton").objectReferenceValue = enBtn.GetComponent<Button>();
            mmSo.FindProperty("languageBackButton").objectReferenceValue = langBackBtn.GetComponent<Button>();
            mmSo.FindProperty("gameTypePanel").objectReferenceValue = gameTypePanel;
            mmSo.FindProperty("gameTypeTitle").objectReferenceValue = gameTypeTitle;
            mmSo.FindProperty("arcadeButton").objectReferenceValue = arcadeBtn.GetComponent<Button>();
            mmSo.FindProperty("arcadeButtonText").objectReferenceValue = arcadeBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("challengeButton").objectReferenceValue = challengeBtn.GetComponent<Button>();
            mmSo.FindProperty("challengeButtonText").objectReferenceValue = challengeBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("timeAttackButton").objectReferenceValue = timeAttackBtn.GetComponent<Button>();
            mmSo.FindProperty("timeAttackButtonText").objectReferenceValue = timeAttackBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("gameTypeBackButton").objectReferenceValue = gameTypeBackBtn.GetComponent<Button>();
            mmSo.FindProperty("challengeNamesPanel").objectReferenceValue = challengeNamesPanel;
            mmSo.FindProperty("challengeNameLabel").objectReferenceValue = challengeNameLabel;
            mmSo.FindProperty("challengeNameInput").objectReferenceValue = challengeNameInput.GetComponent<InputField>();
            mmSo.FindProperty("challengeStartButton").objectReferenceValue = challengeStartBtn.GetComponent<Button>();
            mmSo.FindProperty("challengeStartText").objectReferenceValue = challengeStartBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("challengeNamesBackButton").objectReferenceValue = challengeNamesBackBtn.GetComponent<Button>();
            mmSo.FindProperty("challengeThemePanel").objectReferenceValue = challengeThemePanel;
            mmSo.FindProperty("challengeThemeTitle").objectReferenceValue = challengeThemeTitle;
            mmSo.FindProperty("challengeColorsButton").objectReferenceValue = chColorsBtn.GetComponent<Button>();
            mmSo.FindProperty("challengePrincessButton").objectReferenceValue = chPrincessBtn.GetComponent<Button>();
            mmSo.FindProperty("challengeCarsButton").objectReferenceValue = chCarsBtn.GetComponent<Button>();
            mmSo.FindProperty("challengeThemeBackButton").objectReferenceValue = challengeThemeBackBtn.GetComponent<Button>();
            mmSo.FindProperty("modeTitle").objectReferenceValue = modeTitle;
            mmSo.FindProperty("twoPlayersButton").objectReferenceValue = twoPlayersBtn.GetComponent<Button>();
            mmSo.FindProperty("singlePlayerButton").objectReferenceValue = singlePlayerBtn.GetComponent<Button>();
            mmSo.FindProperty("twoPlayersText").objectReferenceValue = twoPlayersBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("singlePlayerText").objectReferenceValue = singlePlayerBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("difficultyTitle").objectReferenceValue = diffTitle;
            mmSo.FindProperty("easyButton").objectReferenceValue = easyBtn.GetComponent<Button>();
            mmSo.FindProperty("mediumButton").objectReferenceValue = mediumBtn.GetComponent<Button>();
            mmSo.FindProperty("hardButton").objectReferenceValue = hardBtn.GetComponent<Button>();
            mmSo.FindProperty("difficultyBackButton").objectReferenceValue = diffBackBtn.GetComponent<Button>();
            mmSo.FindProperty("themePanel").objectReferenceValue = themePanel;
            mmSo.FindProperty("themeTitle").objectReferenceValue = themeTitle;
            mmSo.FindProperty("colorsThemeButton").objectReferenceValue = colorsBtn.GetComponent<Button>();
            mmSo.FindProperty("princessThemeButton").objectReferenceValue = princessBtn.GetComponent<Button>();
            mmSo.FindProperty("carsThemeButton").objectReferenceValue = carsBtn.GetComponent<Button>();
            mmSo.FindProperty("themeBackButton").objectReferenceValue = themeBackBtn.GetComponent<Button>();
            mmSo.FindProperty("modeBackButton").objectReferenceValue = modeBackBtn.GetComponent<Button>();
            mmSo.FindProperty("namesBackButton").objectReferenceValue = namesBackBtn.GetComponent<Button>();
            mmSo.FindProperty("player1Input").objectReferenceValue = p1Input.GetComponent<InputField>();
            mmSo.FindProperty("player2Input").objectReferenceValue = p2Input.GetComponent<InputField>();
            mmSo.FindProperty("player1Label").objectReferenceValue = p1Label;
            mmSo.FindProperty("player2Label").objectReferenceValue = p2Label;
            mmSo.FindProperty("startButton").objectReferenceValue = startBtn.GetComponent<Button>();
            mmSo.FindProperty("startButtonText").objectReferenceValue = startBtn.GetComponentInChildren<Text>();
            mmSo.ApplyModifiedProperties();
        }

        private static Font _cachedFont;
        private static Font GetGameFont()
        {
            if (_cachedFont == null)
                _cachedFont = Resources.Load<Font>("Fonts/FredokaOne-Regular");
            if (_cachedFont == null)
                _cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return _cachedFont;
        }

        private static Text CreateUIText(string name, string text, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, TextAnchor alignment, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = (fontSize + 6) * 2;
            txt.fontStyle = FontStyle.Normal;
            txt.alignment = alignment;
            txt.color = new Color(0.25f, 0.25f, 0.25f);
            txt.font = GetGameFont();
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Bevel effect: outline + shadow for 3D depth
            var outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.3f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(1f, 1f, 1f, 0.5f);
            shadow.effectDistance = new Vector2(-1f, 1f);

            return txt;
        }

        private static GameObject CreateButton(string name, string label, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);
            var btnImg = btn.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.5f, 0.9f, 1f);
            btnImg.sprite = UI.RoundedButtonHelper.GetRoundedSprite();
            btnImg.type = Image.Type.Sliced;
            var button = btn.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
            colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            var btnRect = btn.GetComponent<RectTransform>();
            btnRect.anchorMin = anchorMin;
            btnRect.anchorMax = anchorMax;
            btnRect.offsetMin = Vector2.zero;
            btnRect.offsetMax = Vector2.zero;

            // Shine overlay (white gradient reflection)
            var shine = new GameObject("Shine");
            shine.transform.SetParent(btn.transform, false);
            var shineImg = shine.AddComponent<Image>();
            shineImg.sprite = UI.RoundedButtonHelper.GetShineSprite();
            shineImg.type = Image.Type.Sliced;
            shineImg.color = Color.white;
            shineImg.raycastTarget = false;
            var shineRect = shine.GetComponent<RectTransform>();
            shineRect.anchorMin = Vector2.zero;
            shineRect.anchorMax = Vector2.one;
            shineRect.offsetMin = Vector2.zero;
            shineRect.offsetMax = Vector2.zero;

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btn.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = 76;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = GetGameFont();
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 28;
            txt.resizeTextMaxSize = 76;
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;
            txt.verticalOverflow = VerticalWrapMode.Truncate;
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(10f, 4f);
            txtRect.offsetMax = new Vector2(-10f, -4f);

            // Bevel: dark shadow below (depth) + light shadow above (highlight)
            var txtShadow = txtObj.AddComponent<Shadow>();
            txtShadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            txtShadow.effectDistance = new Vector2(1.5f, -2f);
            var txtHighlight = txtObj.AddComponent<Shadow>();
            txtHighlight.effectColor = new Color(1f, 1f, 1f, 0.35f);
            txtHighlight.effectDistance = new Vector2(-1f, 1.5f);

            return btn;
        }

        private static GameObject CreateInputField(string name, string placeholder, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.25f, 1f);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Text child
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(go.transform, false);
            var textComp = textObj.AddComponent<Text>();
            textComp.fontSize = 24;
            textComp.alignment = TextAnchor.MiddleLeft;
            textComp.color = Color.white;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.supportRichText = false;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.05f, 0f);
            textRect.anchorMax = new Vector2(0.95f, 1f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            // Placeholder child
            var phObj = new GameObject("Placeholder");
            phObj.transform.SetParent(go.transform, false);
            var phText = phObj.AddComponent<Text>();
            phText.text = placeholder;
            phText.fontSize = 24;
            phText.alignment = TextAnchor.MiddleLeft;
            phText.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
            phText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            phText.fontStyle = FontStyle.Italic;
            var phRect = phObj.GetComponent<RectTransform>();
            phRect.anchorMin = new Vector2(0.05f, 0f);
            phRect.anchorMax = new Vector2(0.95f, 1f);
            phRect.offsetMin = Vector2.zero;
            phRect.offsetMax = Vector2.zero;

            var input = go.AddComponent<InputField>();
            input.textComponent = textComp;
            input.placeholder = phText;

            return go;
        }
    }
}
