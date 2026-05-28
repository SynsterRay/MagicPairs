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
            card.transform.localScale = new Vector3(0.7f, 1.0f, 1f);

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
            gm.AddComponent<Players.ScoreTracker>();
            gm.AddComponent<Input.TouchInputHandler>();

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

            // Score Display
            var scorePanel = new GameObject("ScorePanel");
            scorePanel.transform.SetParent(canvas.transform, false);
            var scoreRect = scorePanel.AddComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0f, 0.92f);
            scoreRect.anchorMax = new Vector2(1f, 1f);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;

            var p1Score = CreateUIText("Player1Score", "Gracz 1: 0", scorePanel.transform,
                new Vector2(0f, 0f), new Vector2(0.5f, 1f), TextAnchor.MiddleCenter, 32);
            var p2Score = CreateUIText("Player2Score", "Gracz 2: 0", scorePanel.transform,
                new Vector2(0.5f, 0f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 32);

            var scoreDisplay = canvas.AddComponent<UI.ScoreDisplay>();
            var sdSo = new SerializedObject(scoreDisplay);
            sdSo.FindProperty("player1ScoreText").objectReferenceValue = p1Score;
            sdSo.FindProperty("player2ScoreText").objectReferenceValue = p2Score;
            sdSo.ApplyModifiedProperties();

            // Turn Indicator
            var turnText = CreateUIText("TurnIndicator", "Tura: Gracz 1", canvas.transform,
                new Vector2(0.2f, 0.87f), new Vector2(0.8f, 0.92f), TextAnchor.MiddleCenter, 28);

            var turnIndicator = canvas.AddComponent<UI.TurnIndicator>();
            var tiSo = new SerializedObject(turnIndicator);
            tiSo.FindProperty("turnText").objectReferenceValue = turnText;
            tiSo.ApplyModifiedProperties();

            // Game Over Panel
            var goPanel = new GameObject("GameOverPanel");
            goPanel.transform.SetParent(canvas.transform, false);
            var goPanelImg = goPanel.AddComponent<Image>();
            goPanelImg.color = new Color(0f, 0f, 0f, 0.85f);
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

            // Main Menu Panel
            var menuPanel = new GameObject("MenuPanel");
            menuPanel.transform.SetParent(canvas.transform, false);
            var menuImg = menuPanel.AddComponent<Image>();
            menuImg.color = new Color(0.08f, 0.08f, 0.15f, 0.95f);
            var menuRect = menuPanel.GetComponent<RectTransform>();
            menuRect.anchorMin = Vector2.zero;
            menuRect.anchorMax = Vector2.one;
            menuRect.offsetMin = Vector2.zero;
            menuRect.offsetMax = Vector2.zero;

            // Title
            var title = CreateUIText("Title", "Magic Pairs", menuPanel.transform,
                new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.95f), TextAnchor.MiddleCenter, 56);

            // --- Language Panel ---
            var langPanel = new GameObject("LanguagePanel");
            langPanel.transform.SetParent(menuPanel.transform, false);
            var langPanelRect = langPanel.AddComponent<RectTransform>();
            langPanelRect.anchorMin = new Vector2(0.1f, 0.2f);
            langPanelRect.anchorMax = new Vector2(0.9f, 0.8f);
            langPanelRect.offsetMin = Vector2.zero;
            langPanelRect.offsetMax = Vector2.zero;

            var langTitle = CreateUIText("LangTitle", "Choose Language / Wybierz język", langPanel.transform,
                new Vector2(0f, 0.7f), new Vector2(1f, 0.95f), TextAnchor.MiddleCenter, 30);

            var plBtn = CreateButton("PolishBtn", "Polski", langPanel.transform,
                new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.55f));
            var enBtn = CreateButton("EnglishBtn", "English", langPanel.transform,
                new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.3f));

            var quitBtn = CreateButton("QuitBtn", "✕", langPanel.transform,
                new Vector2(0.35f, -0.15f), new Vector2(0.65f, -0.02f));
            quitBtn.GetComponent<Image>().color = new Color(0.6f, 0.15f, 0.15f, 1f);
            quitBtn.GetComponentInChildren<Text>().fontSize = 32;

            // --- Mode Panel ---
            var modePanel = new GameObject("ModePanel");
            modePanel.transform.SetParent(menuPanel.transform, false);
            var modePanelRect = modePanel.AddComponent<RectTransform>();
            modePanelRect.anchorMin = new Vector2(0.1f, 0.2f);
            modePanelRect.anchorMax = new Vector2(0.9f, 0.8f);
            modePanelRect.offsetMin = Vector2.zero;
            modePanelRect.offsetMax = Vector2.zero;

            var modeTitle = CreateUIText("ModeTitle", "Wybierz tryb", modePanel.transform,
                new Vector2(0f, 0.7f), new Vector2(1f, 0.95f), TextAnchor.MiddleCenter, 30);

            var twoPlayersBtn = CreateButton("TwoPlayersBtn", "2 Graczy", modePanel.transform,
                new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.55f));
            var singlePlayerBtn = CreateButton("SinglePlayerBtn", "1 Gracz (vs AI)", modePanel.transform,
                new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.3f));
            singlePlayerBtn.GetComponent<Image>().color = new Color(0.6f, 0.3f, 0.8f, 1f);

            var modeBackBtn = CreateButton("ModeBackBtn", "←", modePanel.transform,
                new Vector2(0f, 0.0f), new Vector2(0.15f, 0.1f));
            modeBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            modeBackBtn.GetComponentInChildren<Text>().fontSize = 32;

            // --- Difficulty Panel ---
            var diffPanel = new GameObject("DifficultyPanel");
            diffPanel.transform.SetParent(menuPanel.transform, false);
            var diffPanelRect = diffPanel.AddComponent<RectTransform>();
            diffPanelRect.anchorMin = new Vector2(0.1f, 0.2f);
            diffPanelRect.anchorMax = new Vector2(0.9f, 0.8f);
            diffPanelRect.offsetMin = Vector2.zero;
            diffPanelRect.offsetMax = Vector2.zero;

            var diffTitle = CreateUIText("DiffTitle", "Wybierz poziom trudności", diffPanel.transform,
                new Vector2(0f, 0.75f), new Vector2(1f, 0.95f), TextAnchor.MiddleCenter, 28);

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
                new Vector2(0.05f, 0.15f), new Vector2(0.3f, 0.3f), TextAnchor.MiddleCenter, 20);
            CreateUIText("MediumLabel", "4x5", diffPanel.transform,
                new Vector2(0.35f, 0.15f), new Vector2(0.65f, 0.3f), TextAnchor.MiddleCenter, 20);
            CreateUIText("HardLabel", "5x6", diffPanel.transform,
                new Vector2(0.7f, 0.15f), new Vector2(0.95f, 0.3f), TextAnchor.MiddleCenter, 20);

            var diffBackBtn = CreateButton("DiffBackBtn", "←", diffPanel.transform,
                new Vector2(0f, 0.0f), new Vector2(0.15f, 0.1f));
            diffBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            diffBackBtn.GetComponentInChildren<Text>().fontSize = 32;

            // --- Names Panel ---
            var namesPanel = new GameObject("NamesPanel");
            namesPanel.transform.SetParent(menuPanel.transform, false);
            var namesPanelRect = namesPanel.AddComponent<RectTransform>();
            namesPanelRect.anchorMin = new Vector2(0.1f, 0.1f);
            namesPanelRect.anchorMax = new Vector2(0.9f, 0.8f);
            namesPanelRect.offsetMin = Vector2.zero;
            namesPanelRect.offsetMax = Vector2.zero;

            var p1Label = CreateUIText("P1Label", "Imię Gracza 1", namesPanel.transform,
                new Vector2(0f, 0.75f), new Vector2(1f, 0.85f), TextAnchor.MiddleLeft, 24);
            var p1Input = CreateInputField("P1Input", "Gracz 1", namesPanel.transform,
                new Vector2(0f, 0.6f), new Vector2(1f, 0.74f));

            var p2Label = CreateUIText("P2Label", "Imię Gracza 2", namesPanel.transform,
                new Vector2(0f, 0.45f), new Vector2(1f, 0.55f), TextAnchor.MiddleLeft, 24);
            var p2Input = CreateInputField("P2Input", "Gracz 2", namesPanel.transform,
                new Vector2(0f, 0.3f), new Vector2(1f, 0.44f));

            var startBtn = CreateButton("StartBtn", "Start", namesPanel.transform,
                new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.2f));
            startBtn.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.3f, 1f);

            var namesBackBtn = CreateButton("NamesBackBtn", "←", namesPanel.transform,
                new Vector2(0f, 0.87f), new Vector2(0.15f, 0.99f));
            namesBackBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            namesBackBtn.GetComponentInChildren<Text>().fontSize = 32;

            // Wire MainMenu
            var mainMenu = canvas.AddComponent<UI.MainMenu>();
            var mmSo = new SerializedObject(mainMenu);
            mmSo.FindProperty("menuPanel").objectReferenceValue = menuPanel;
            mmSo.FindProperty("languagePanel").objectReferenceValue = langPanel;
            mmSo.FindProperty("modePanel").objectReferenceValue = modePanel;
            mmSo.FindProperty("difficultyPanel").objectReferenceValue = diffPanel;
            mmSo.FindProperty("namesPanel").objectReferenceValue = namesPanel;
            mmSo.FindProperty("languageTitle").objectReferenceValue = langTitle;
            mmSo.FindProperty("polishButton").objectReferenceValue = plBtn.GetComponent<Button>();
            mmSo.FindProperty("englishButton").objectReferenceValue = enBtn.GetComponent<Button>();
            mmSo.FindProperty("quitButton").objectReferenceValue = quitBtn.GetComponent<Button>();
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
            mmSo.FindProperty("modeBackButton").objectReferenceValue = modeBackBtn.GetComponent<Button>();
            mmSo.FindProperty("namesBackButton").objectReferenceValue = namesBackBtn.GetComponent<Button>();
            mmSo.FindProperty("player1Input").objectReferenceValue = p1Input.GetComponent<InputField>();
            mmSo.FindProperty("player2Input").objectReferenceValue = p2Input.GetComponent<InputField>();
            mmSo.FindProperty("player1Label").objectReferenceValue = p1Label;
            mmSo.FindProperty("player2Label").objectReferenceValue = p2Label;
            mmSo.FindProperty("startButton").objectReferenceValue = startBtn.GetComponent<Button>();
            mmSo.FindProperty("startButtonText").objectReferenceValue = startBtn.GetComponentInChildren<Text>();
            mmSo.FindProperty("titleText").objectReferenceValue = title;
            mmSo.ApplyModifiedProperties();
        }

        private static Text CreateUIText(string name, string text, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, TextAnchor alignment, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.alignment = alignment;
            txt.color = Color.white;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return txt;
        }

        private static GameObject CreateButton(string name, string label, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);
            var btnImg = btn.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.5f, 0.9f, 1f);
            btn.AddComponent<Button>();
            var btnRect = btn.GetComponent<RectTransform>();
            btnRect.anchorMin = anchorMin;
            btnRect.anchorMax = anchorMax;
            btnRect.offsetMin = Vector2.zero;
            btnRect.offsetMax = Vector2.zero;

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btn.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = 28;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

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
