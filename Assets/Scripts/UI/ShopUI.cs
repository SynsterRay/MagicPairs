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
            if (_panel != null) { Destroy(_panel); _panel = null; _content = null; }
            CreatePanel(onBack);
            if (_panel == null) return;
            _panel.SetActive(true);
            // Update title to reflect current language
            var titleObj = _panel.transform.Find("ShopTitle");
            if (titleObj != null) titleObj.GetComponent<Text>().text = Localization.Get("shop");
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
                new Vector2(0.1f, 0.88f), new Vector2(0.5f, 0.96f), TextAnchor.MiddleLeft, 48);
            title.color = new Color(0.3f, 0.1f, 0.5f);

            // Coins display
            _coinsText = UIFactory.CreateText("CoinsDisplay", $"{PlayerWallet.Coins}", _panel.transform,
                new Vector2(0.55f, 0.88f), new Vector2(0.85f, 0.96f), TextAnchor.MiddleLeft, 40);
            _coinsText.color = new Color(0.9f, 0.75f, 0.1f);

            // Coin icon
            var coinIcon = new GameObject("CoinIcon");
            coinIcon.transform.SetParent(_panel.transform, false);
            var ciImg = coinIcon.AddComponent<Image>();
            ciImg.sprite = UIIcons.Get("coin_icon");
            ciImg.preserveAspect = true;
            ciImg.raycastTarget = false;
            var ciRect = coinIcon.GetComponent<RectTransform>();
            ciRect.anchorMin = new Vector2(0.46f, 0.88f);
            ciRect.anchorMax = new Vector2(0.55f, 0.96f);
            ciRect.offsetMin = Vector2.zero;
            ciRect.offsetMax = Vector2.zero;

            // Scroll content area
            var scrollArea = new GameObject("ScrollArea", typeof(RectTransform));
            scrollArea.transform.SetParent(_panel.transform, false);
            var scrollAreaRect = scrollArea.GetComponent<RectTransform>();
            scrollAreaRect.anchorMin = new Vector2(0.03f, 0.21f);
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
            var backGo = new GameObject("ShopBackBtn");
            backGo.transform.SetParent(_panel.transform, false);
            var backImg = backGo.AddComponent<Image>();
            backImg.sprite = UIIcons.Get("back");
            backImg.preserveAspect = true;
            backImg.color = Color.white;
            var backBtn = backGo.AddComponent<Button>();
            backBtn.transition = Selectable.Transition.None;
            var backRect = backGo.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.30f, 0.12f);
            backRect.anchorMax = new Vector2(0.70f, 0.20f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;
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
            var themes = new System.Collections.Generic.List<ShopItem>();
            foreach (var item in ShopCatalog.Items)
                if (item.type == ShopItemType.CardTheme) themes.Add(item);
            CreateIconRow(themes);

            // Section: Power-ups
            CreateSectionHeader("Power-ups");
            var powerups = new System.Collections.Generic.List<ShopItem>();
            foreach (var item in ShopCatalog.Items)
                if (item.type == ShopItemType.PowerUpPeek || item.type == ShopItemType.PowerUpShuffle || item.type == ShopItemType.PowerUpFreeze)
                    powerups.Add(item);
            CreateIconRow(powerups);

            // Section: Coin Packs
            CreateSectionHeader($"🪙 {Localization.Get("coins")}");
            var coins = new System.Collections.Generic.List<ShopItem>();
            foreach (var item in ShopCatalog.Items)
                if (item.type == ShopItemType.CoinPack) coins.Add(item);
            CreateIconRow(coins);
        }

        private void CreateSectionHeader(string text)
        {
            var go = new GameObject("Header");
            go.transform.SetParent(_content, false);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 70f;
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = 42;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.3f, 0.1f, 0.5f);
            txt.font = UIFactory.GetFont();
        }

        private void CreateIconRow(System.Collections.Generic.List<ShopItem> items)
        {
            var row = new GameObject("Row");
            row.transform.SetParent(_content, false);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 300f;

            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                var item = items[i];
                float xMin = (float)i / count + 0.01f;
                float xMax = (float)(i + 1) / count - 0.01f;

                // Icon as button (top 70%)
                string iconName = GetItemIcon(item);
                var iconObj = new GameObject("Btn_" + i);
                iconObj.transform.SetParent(row.transform, false);
                var iconImg = iconObj.AddComponent<Image>();
                iconImg.sprite = UIIcons.Get(iconName ?? "coins");
                iconImg.preserveAspect = true;
                iconImg.color = Color.white;
                var btn = iconObj.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
                var iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(xMin + 0.02f, 0.35f);
                iconRect.anchorMax = new Vector2(xMax - 0.02f, 1.0f);
                iconRect.offsetMin = Vector2.zero;
                iconRect.offsetMax = Vector2.zero;

                // Wire button action
                var capturedItem = item;
                bool owned = item.type == ShopItemType.CardTheme && item.theme.HasValue && ShopCatalog.IsThemeUnlocked(item.theme.Value);
                if (item.type == ShopItemType.CoinPack)
                    btn.onClick.AddListener(() => OnBuyCoinPack(capturedItem));
                else if (!owned && item.type != ShopItemType.CardTheme)
                    btn.onClick.AddListener(() => OnBuy(capturedItem));

                // Name label
                string displayName = GetItemName(item);
                var nameObj = new GameObject("Name_" + i);
                nameObj.transform.SetParent(row.transform, false);
                var nameTxt = nameObj.AddComponent<Text>();
                nameTxt.text = displayName;
                nameTxt.fontSize = 24;
                nameTxt.fontStyle = FontStyle.Bold;
                nameTxt.alignment = TextAnchor.MiddleCenter;
                nameTxt.color = new Color(0.2f, 0.2f, 0.2f);
                nameTxt.font = UIFactory.GetFont();
                nameTxt.resizeTextForBestFit = true;
                nameTxt.resizeTextMinSize = 16;
                nameTxt.resizeTextMaxSize = 24;
                var nameRect = nameObj.GetComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(xMin, 0.15f);
                nameRect.anchorMax = new Vector2(xMax, 0.35f);
                nameRect.offsetMin = Vector2.zero;
                nameRect.offsetMax = Vector2.zero;

                // Price tag
                string priceText = GetPriceLabel(item);
                var priceObj = new GameObject("Price_" + i);
                priceObj.transform.SetParent(row.transform, false);
                var priceBg = priceObj.AddComponent<Image>();
                priceBg.color = new Color(0.1f, 0.7f, 0.3f, 1f);
                priceBg.sprite = RoundedButtonHelper.GetRoundedSprite();
                priceBg.type = Image.Type.Sliced;
                var priceRect = priceObj.GetComponent<RectTransform>();
                priceRect.anchorMin = new Vector2(xMin + 0.01f, 0.0f);
                priceRect.anchorMax = new Vector2(xMax - 0.01f, 0.18f);
                priceRect.offsetMin = Vector2.zero;
                priceRect.offsetMax = Vector2.zero;

                var priceTxtObj = new GameObject("PriceText");
                priceTxtObj.transform.SetParent(priceObj.transform, false);
                var priceTxt = priceTxtObj.AddComponent<Text>();
                priceTxt.text = priceText;
                priceTxt.fontSize = 20;
                priceTxt.fontStyle = FontStyle.Bold;
                priceTxt.alignment = TextAnchor.MiddleCenter;
                priceTxt.color = Color.white;
                priceTxt.font = UIFactory.GetFont();
                priceTxt.resizeTextForBestFit = true;
                priceTxt.resizeTextMinSize = 12;
                priceTxt.resizeTextMaxSize = 20;
                var ptr = priceTxtObj.GetComponent<RectTransform>();
                ptr.anchorMin = Vector2.zero;
                ptr.anchorMax = Vector2.one;
                ptr.offsetMin = new Vector2(4f, 2f);
                ptr.offsetMax = new Vector2(-4f, -2f);
            }
        }

        private string GetPriceLabel(ShopItem item)
        {
            bool owned = item.type == ShopItemType.CardTheme && item.theme.HasValue && ShopCatalog.IsThemeUnlocked(item.theme.Value);
            if (owned) return "✓";
            if (item.type == ShopItemType.CardTheme) return "🔒";
            if (item.type == ShopItemType.CoinPack)
            {
                string price = GetCoinPackPrice(item);
                string discount = item.quantity switch
                {
                    500 => " • 20% OFF",
                    1500 => " • 40% OFF",
                    _ => ""
                };
                return price + discount;
            }
            return $"🪙 {item.coinPrice}";
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
            if (!PlayerWallet.CanAfford(item.coinPrice))
            {
                if (_coinsText != null)
                {
                    _coinsText.text = Localization.Get("notEnough");
                    _coinsText.color = Color.red;
                    Invoke(nameof(RestoreCoinsText), 1.5f);
                }
                return;
            }

            ShowConfirmDialog(item.coinPrice, () =>
            {
                if (ShopCatalog.TryPurchase(item))
                {
                    UpdateCoinsDisplay(PlayerWallet.Coins);
                    PopulateItems();
                }
            });
        }

        private GameObject _confirmDialog;

        private void ShowConfirmDialog(int cost, System.Action onConfirm)
        {
            if (_confirmDialog != null) Destroy(_confirmDialog);

            _confirmDialog = new GameObject("ConfirmDialog");
            _confirmDialog.transform.SetParent(_panel.transform, false);
            var bgImg = _confirmDialog.AddComponent<Image>();
            bgImg.color = new Color(0f, 0f, 0f, 0.7f);
            var bgRect = _confirmDialog.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var box = new GameObject("Box");
            box.transform.SetParent(_confirmDialog.transform, false);
            var boxImg = box.AddComponent<Image>();
            boxImg.color = Color.white;
            boxImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            boxImg.type = Image.Type.Sliced;
            var boxRect = box.GetComponent<RectTransform>();
            boxRect.anchorMin = new Vector2(0.15f, 0.35f);
            boxRect.anchorMax = new Vector2(0.85f, 0.65f);
            boxRect.offsetMin = Vector2.zero;
            boxRect.offsetMax = Vector2.zero;

            string msg = Localization.CurrentLanguage == Language.Polish
                ? $"Kupić za {cost} monet?" : $"Buy for {cost} coins?";
            UIFactory.CreateText("Msg", msg, box.transform,
                new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, 28);

            var yesBtn = UIFactory.CreateButton("YesBtn", Localization.Get("yes"), box.transform,
                new Vector2(0.1f, 0.1f), new Vector2(0.45f, 0.45f), new Color(0.1f, 0.7f, 0.3f, 1f));
            yesBtn.onClick.AddListener(() => { Destroy(_confirmDialog); onConfirm(); });

            var noBtn = UIFactory.CreateButton("NoBtn", Localization.Get("no"), box.transform,
                new Vector2(0.55f, 0.1f), new Vector2(0.9f, 0.45f), new Color(0.7f, 0.2f, 0.2f, 1f));
            noBtn.onClick.AddListener(() => Destroy(_confirmDialog));
        }

        private void OnBuyCoinPack(ShopItem item)
        {
            var iap = Core.IAPManager.Instance;
            if (iap == null || !iap.IsInitialized || string.IsNullOrEmpty(item.iapProductId))
            {
                Debug.LogWarning("[Shop] IAP not ready");
                return;
            }

            iap.BuyProduct(item.iapProductId, success =>
            {
                if (success)
                {
                    UpdateCoinsDisplay(PlayerWallet.Coins);
                    PopulateItems();
                }
            });
        }

        private void RestoreCoinsText()
        {
            UpdateCoinsDisplay(PlayerWallet.Coins);
        }

        private void UpdateCoinsDisplay(int coins)
        {
            if (_coinsText != null)
            {
                _coinsText.text = $"{coins}";
                _coinsText.color = new Color(0.9f, 0.75f, 0.1f);
            }
        }

        private string GetItemIcon(ShopItem item)
        {
            return item.type switch
            {
                ShopItemType.CardTheme => item.id switch
                {
                    "theme_animals" => "animals",
                    "theme_water_world" => "water world",
                    "theme_space" => "space",
                    _ => null
                },
                ShopItemType.PowerUpPeek => "peek",
                ShopItemType.PowerUpShuffle => "shuffle",
                ShopItemType.PowerUpFreeze => "freeze",
                _ => null
            };
        }

        private string GetItemName(ShopItem item)
        {
            return item.type switch
            {
                ShopItemType.CardTheme => item.id switch
                {
                    "theme_animals" => Localization.CurrentLanguage == Language.Polish ? "Zwierzęta" : "Animals",
                    "theme_water_world" => Localization.CurrentLanguage == Language.Polish ? "Wodny Świat" : "Water World",
                    "theme_space" => Localization.CurrentLanguage == Language.Polish ? "Kosmos" : "Space",
                    _ => item.id
                },
                ShopItemType.PowerUpPeek => $"{Localization.Get("peek")} x{item.quantity}",
                ShopItemType.PowerUpShuffle => $"{Localization.Get("shuffle")} x{item.quantity}",
                ShopItemType.PowerUpFreeze => $"{Localization.Get("freeze")} x{item.quantity}",
                ShopItemType.CoinPack => $"{Localization.Get("coins")} {item.quantity}",
                _ => item.id
            };
        }

        private string GetCoinPackPrice(ShopItem item)
        {
            var iap = Core.IAPManager.Instance;
            if (iap != null && iap.IsInitialized)
            {
                var price = iap.GetLocalizedPrice(item.iapProductId);
                if (!string.IsNullOrEmpty(price)) return price;
            }
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
