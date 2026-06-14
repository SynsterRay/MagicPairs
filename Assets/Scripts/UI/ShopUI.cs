using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class ShopUI : MonoBehaviour
    {
        private GameObject _panel;
        private Text _coinsText;
        private Transform _content;
        private Button _backButton;

        private void Start()
        {
            PlayerWallet.OnCoinsChanged += UpdateCoinsDisplay;
        }

        private void OnDestroy()
        {
            PlayerWallet.OnCoinsChanged -= UpdateCoinsDisplay;
        }

        public void Show(System.Action onBack)
        {
            if (!_panel) { _panel = null; _content = null; }
            if (_panel == null) CreatePanel(onBack);
            if (_panel == null) return;
            _panel.SetActive(true);
            UpdateCoinsDisplay(PlayerWallet.Coins);
            PopulateItems();
        }

        public void Hide()
        {
            if (_panel) _panel.SetActive(false);
        }

        private void CreatePanel(System.Action onBack)
        {
            var canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            if (canvas == null) return;

            _panel = new GameObject("ShopPanel");
            _panel.transform.SetParent(canvas.transform, false);
            var panelImg = _panel.AddComponent<Image>();
            panelImg.color = new Color(1f, 1f, 1f, 1f);
            var panelRect = _panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Title
            var title = UIFactory.CreateText("ShopTitle", Localization.Get("shop"), _panel.transform,
                new Vector2(0.1f, 0.88f), new Vector2(0.9f, 0.96f), TextAnchor.MiddleCenter, 36);
            title.color = new Color(0.2f, 0.2f, 0.2f);

            // Coins display
            _coinsText = UIFactory.CreateText("CoinsDisplay", $"🪙 {PlayerWallet.Coins}", _panel.transform,
                new Vector2(0.6f, 0.88f), new Vector2(0.95f, 0.96f), TextAnchor.MiddleRight, 28);
            _coinsText.color = new Color(0.8f, 0.6f, 0.1f);

            // Scroll content area
            var scrollArea = new GameObject("ScrollArea", typeof(RectTransform));
            scrollArea.transform.SetParent(_panel.transform, false);
            var scrollAreaRect = scrollArea.GetComponent<RectTransform>();
            scrollAreaRect.anchorMin = new Vector2(0.03f, 0.12f);
            scrollAreaRect.anchorMax = new Vector2(0.97f, 0.87f);
            scrollAreaRect.offsetMin = Vector2.zero;
            scrollAreaRect.offsetMax = Vector2.zero;
            scrollArea.AddComponent<RectMask2D>();

            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(scrollArea.transform, false);
            _content = contentGo.transform;
            var contentRect = contentGo.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0f, 0f);
            var layout = contentGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12f;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var fitter = contentGo.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = scrollArea.AddComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.vertical = true;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Elastic;

            // Back button
            var backBtn = UIFactory.CreateButton("ShopBackBtn", "←", _panel.transform,
                new Vector2(0.3f, 0.02f), new Vector2(0.7f, 0.1f), new Color(0.4f, 0.4f, 0.4f, 1f));
            backBtn.onClick.AddListener(() => { Hide(); onBack?.Invoke(); });
            _backButton = backBtn;
        }

        private void PopulateItems()
        {
            // Clear old items
            var toDestroy = new System.Collections.Generic.List<GameObject>();
            foreach (Transform child in _content)
                toDestroy.Add(child.gameObject);
            foreach (var go in toDestroy)
                Destroy(go);

            // Section: Card Themes
            CreateSectionHeader(Localization.Get("chooseTheme"));
            foreach (var item in ShopCatalog.Items)
            {
                if (item.type == ShopItemType.CardTheme)
                    CreateShopRow(item);
            }

            // Section: Power-ups
            CreateSectionHeader("Power-ups");
            foreach (var item in ShopCatalog.Items)
            {
                if (item.type == ShopItemType.PowerUpPeek || item.type == ShopItemType.PowerUpShuffle || item.type == ShopItemType.PowerUpFreeze)
                    CreateShopRow(item);
            }

            // Section: Coin Packs
            CreateSectionHeader($"🪙 {Localization.Get("coins")}");
            foreach (var item in ShopCatalog.Items)
            {
                if (item.type == ShopItemType.CoinPack)
                    CreateCoinPackRow(item);
            }
        }

        private void CreateSectionHeader(string text)
        {
            var go = new GameObject("Header");
            go.transform.SetParent(_content, false);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 50f;
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = 28;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleLeft;
            txt.color = new Color(0.3f, 0.3f, 0.3f);
            txt.font = UIFactory.GetFont();
        }

        private void CreateShopRow(ShopItem item)
        {
            var row = new GameObject(item.id);
            row.transform.SetParent(_content, false);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 80f;
            var rowImg = row.AddComponent<Image>();
            rowImg.color = new Color(0.95f, 0.95f, 0.97f);
            rowImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            rowImg.type = Image.Type.Sliced;

            // Name
            string displayName = GetItemName(item);
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(row.transform, false);
            var nameTxt = nameObj.AddComponent<Text>();
            nameTxt.text = displayName;
            nameTxt.fontSize = 24;
            nameTxt.fontStyle = FontStyle.Bold;
            nameTxt.alignment = TextAnchor.MiddleLeft;
            nameTxt.color = new Color(0.2f, 0.2f, 0.2f);
            nameTxt.font = UIFactory.GetFont();
            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0f);
            nameRect.anchorMax = new Vector2(0.55f, 1f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // Status / Buy button
            bool owned = item.type == ShopItemType.CardTheme && item.theme.HasValue && ShopCatalog.IsThemeUnlocked(item.theme.Value);

            if (owned)
            {
                var statusObj = new GameObject("Status");
                statusObj.transform.SetParent(row.transform, false);
                var statusTxt = statusObj.AddComponent<Text>();
                statusTxt.text = "✓";
                statusTxt.fontSize = 32;
                statusTxt.alignment = TextAnchor.MiddleCenter;
                statusTxt.color = new Color(0.2f, 0.7f, 0.2f);
                statusTxt.font = UIFactory.GetFont();
                var statusRect = statusObj.GetComponent<RectTransform>();
                statusRect.anchorMin = new Vector2(0.7f, 0.15f);
                statusRect.anchorMax = new Vector2(0.95f, 0.85f);
                statusRect.offsetMin = Vector2.zero;
                statusRect.offsetMax = Vector2.zero;
            }
            else
            {
                var buyBtn = CreateBuyButton(row.transform, $"🪙 {item.coinPrice}");
                var capturedItem = item;
                buyBtn.onClick.AddListener(() => OnBuy(capturedItem));
            }
        }

        private void CreateCoinPackRow(ShopItem item)
        {
            var row = new GameObject(item.id);
            row.transform.SetParent(_content, false);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 80f;
            var rowImg = row.AddComponent<Image>();
            rowImg.color = new Color(1f, 0.97f, 0.88f);
            rowImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            rowImg.type = Image.Type.Sliced;

            // Coin graphic (spinning gold disc)
            var coinObj = new GameObject("CoinIcon");
            coinObj.transform.SetParent(row.transform, false);
            var coinImg = coinObj.AddComponent<Image>();
            coinImg.color = new Color(1f, 0.82f, 0.1f);
            coinImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            coinImg.type = Image.Type.Sliced;
            var coinRect = coinObj.GetComponent<RectTransform>();
            coinRect.anchorMin = new Vector2(0.03f, 0.15f);
            coinRect.anchorMax = new Vector2(0.12f, 0.85f);
            coinRect.offsetMin = Vector2.zero;
            coinRect.offsetMax = Vector2.zero;

            // Inner coin detail
            var coinInner = new GameObject("Inner");
            coinInner.transform.SetParent(coinObj.transform, false);
            var innerRect = coinInner.AddComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.2f, 0.2f);
            innerRect.anchorMax = new Vector2(0.8f, 0.8f);
            innerRect.offsetMin = Vector2.zero;
            innerRect.offsetMax = Vector2.zero;
            var innerImg = coinInner.AddComponent<Image>();
            innerImg.color = new Color(0.85f, 0.65f, 0f);
            innerImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            innerImg.type = Image.Type.Sliced;
            innerImg.raycastTarget = false;

            // Name
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(row.transform, false);
            var nameTxt = nameObj.AddComponent<Text>();
            nameTxt.text = $"{item.quantity} {Localization.Get("coins")}";
            nameTxt.fontSize = 24;
            nameTxt.fontStyle = FontStyle.Bold;
            nameTxt.alignment = TextAnchor.MiddleLeft;
            nameTxt.color = new Color(0.2f, 0.2f, 0.2f);
            nameTxt.font = UIFactory.GetFont();
            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.14f, 0f);
            nameRect.anchorMax = new Vector2(0.55f, 1f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // IAP button
            var buyBtn = CreateBuyButton(row.transform, GetCoinPackPrice(item));
            buyBtn.onClick.AddListener(() => OnBuyCoinPack(item));
        }

        private Button CreateBuyButton(Transform parent, string label)
        {
            var go = new GameObject("BuyBtn");
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.1f, 0.7f, 0.3f, 1f);
            img.sprite = RoundedButtonHelper.GetRoundedSprite();
            img.type = Image.Type.Sliced;
            var btn = go.AddComponent<Button>();
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.6f, 0.15f);
            rect.anchorMax = new Vector2(0.95f, 0.85f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = 22;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = UIFactory.GetFont();
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 16;
            txt.resizeTextMaxSize = 22;
            var tr = txtObj.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(4f, 2f);
            tr.offsetMax = new Vector2(-4f, -2f);

            return btn;
        }

        private void OnBuy(ShopItem item)
        {
            if (ShopCatalog.TryPurchase(item))
            {
                UpdateCoinsDisplay(PlayerWallet.Coins);
                PopulateItems(); // Refresh UI
            }
            else
            {
                // Flash "not enough" feedback
                if (_coinsText != null)
                {
                    _coinsText.text = Localization.Get("notEnough");
                    _coinsText.color = Color.red;
                    Invoke(nameof(RestoreCoinsText), 1.5f);
                }
            }
        }

        private void OnBuyCoinPack(ShopItem item)
        {
            // TODO: Integrate with Unity IAP
            // For now, show placeholder message
            Debug.Log($"[Shop] IAP purchase: {item.iapProductId} for {item.quantity} coins");
        }

        private void RestoreCoinsText()
        {
            UpdateCoinsDisplay(PlayerWallet.Coins);
        }

        private void UpdateCoinsDisplay(int coins)
        {
            if (_coinsText != null)
            {
                _coinsText.text = $"🪙 {coins}";
                _coinsText.color = new Color(0.8f, 0.6f, 0.1f);
            }
        }

        private string GetItemName(ShopItem item)
        {
            return item.type switch
            {
                ShopItemType.CardTheme => item.id switch
                {
                    "theme_animals" => "🐾 Animals",
                    "theme_dinos" => "🦕 Dinosaurs",
                    "theme_space" => "🚀 Space",
                    _ => item.id
                },
                ShopItemType.PowerUpPeek => $"🔍 {Localization.Get("peek")} x{item.quantity}",
                ShopItemType.PowerUpShuffle => $"🔄 {Localization.Get("shuffle")} x{item.quantity}",
                ShopItemType.PowerUpFreeze => $"❄️ {Localization.Get("freeze")} x{item.quantity}",
                _ => item.id
            };
        }

        private string GetCoinPackPrice(ShopItem item)
        {
            return item.quantity switch
            {
                100 => "$0.99",
                500 => "$3.99",
                1500 => "$8.99",
                _ => Localization.Get("buy")
            };
        }
    }
}
