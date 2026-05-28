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
            gm.AddComponent<Players.ScoreTracker>();
            gm.AddComponent<Input.TouchInputHandler>();

            var cardGrid = gm.AddComponent<Cards.CardGrid>();

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
            gopSo.ApplyModifiedProperties();
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
    }
}
