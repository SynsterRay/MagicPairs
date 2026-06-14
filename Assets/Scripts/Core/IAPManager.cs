using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System;

namespace MagicPairs.Core
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        public static IAPManager Instance { get; private set; }

        private IStoreController _store;
        private Action<bool> _pendingCallback;

        public bool IsInitialized => _store != null;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("com.magicpairs.coins100", ProductType.Consumable);
            builder.AddProduct("com.magicpairs.coins500", ProductType.Consumable);
            builder.AddProduct("com.magicpairs.coins1500", ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProduct(string productId, Action<bool> callback)
        {
            if (_store == null) { callback?.Invoke(false); return; }
            _pendingCallback = callback;
            _store.InitiatePurchase(productId);
        }

        public string GetLocalizedPrice(string productId)
        {
            if (_store == null) return null;
            var product = _store.products.WithID(productId);
            return product?.metadata?.localizedPriceString;
        }

        // IStoreListener
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _store = controller;
            Debug.Log("[IAP] Initialized");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogWarning($"[IAP] Init failed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogWarning($"[IAP] Init failed: {error} - {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string id = args.purchasedProduct.definition.id;
            int coins = id switch
            {
                "com.magicpairs.coins100" => 100,
                "com.magicpairs.coins500" => 500,
                "com.magicpairs.coins1500" => 1500,
                _ => 0
            };

            if (coins > 0)
                PlayerWallet.Add(coins);

            _pendingCallback?.Invoke(true);
            _pendingCallback = null;
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.LogWarning($"[IAP] Purchase failed: {product.definition.id} - {reason}");
            _pendingCallback?.Invoke(false);
            _pendingCallback = null;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription desc)
        {
            Debug.LogWarning($"[IAP] Purchase failed: {product.definition.id} - {desc.message}");
            _pendingCallback?.Invoke(false);
            _pendingCallback = null;
        }
    }
}
