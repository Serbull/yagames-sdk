using UnityEngine;
using UnityEngine.Events;
using System;

namespace YaGamesSDK.Components
{
    public class IapListener : MonoBehaviour
    {
        public enum Type
        {
            Consumable,
            NonConsumable
        }

        [Serializable]
        public class Product
        {
            public string ProductId;
            public Type Type;
        }

        [SerializeField] private Product[] _listenProducts;
        [Space]
        [SerializeField] private UnityEvent<string> _onPurchaseSuccessful;
        [SerializeField] private UnityEvent<string> _onPurchaseFailed;

        private void OnEnable()
        {
            Purchasing.OnPurchaseSuccessful += Purchasing_OnPurchaseSuccessful;
            Purchasing.OnPurchaseFailed += Purchasing_OnPurchaseFailed;
        }

        private void OnDisable()
        {
            Purchasing.OnPurchaseSuccessful -= Purchasing_OnPurchaseSuccessful;
            Purchasing.OnPurchaseFailed -= Purchasing_OnPurchaseFailed;
        }

        public void AddListener(UnityAction<string, bool> onPurchaseCompleted)
        {
            _onPurchaseSuccessful.AddListener((string productId) => onPurchaseCompleted(productId, true));
            _onPurchaseFailed.AddListener((string productId) => onPurchaseCompleted(productId, false));
            CheckConsumableProduct(true);
        }

        public void RemoveListener(UnityAction<string, bool> onPurchaseCompleted)
        {
            _onPurchaseSuccessful.RemoveListener((string productId) => onPurchaseCompleted(productId, true));
            _onPurchaseFailed.RemoveListener((string productId) => onPurchaseCompleted(productId, true));
        }

        private void CheckConsumableProduct(bool callbackPurchase)
        {
            if (_listenProducts == null) return;

            foreach (var product in _listenProducts)
            {
                if (product.Type == Type.Consumable && Purchasing.IsBought(product.ProductId))
                {
                    if (callbackPurchase)
                    {
                        Purchasing_OnPurchaseSuccessful(product.ProductId);
                    }
                    else
                    {
                        YaGames.LogError($"Use Consume for product: {product.ProductId}");
                    }
                }
            }
        }

        private void Purchasing_OnPurchaseSuccessful(string productId)
        {
            foreach (var product in _listenProducts)
            {
                if (product.ProductId == productId)
                {
                    _onPurchaseSuccessful?.Invoke(productId);
                    CheckConsumableProduct(false);
                    break;
                }
            }
        }

        private void Purchasing_OnPurchaseFailed(string productId)
        {
            foreach (var product in _listenProducts)
            {
                if (product.ProductId == productId)
                {
                    _onPurchaseFailed?.Invoke(productId);
                    break;
                }
            }
        }
    }
}
