using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace YaGamesSDK.Components
{
    public class IAPListener : MonoBehaviour
    {
        [SerializeField, InAppId] private string[] _listenProducts;
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

            foreach (var productId in _listenProducts)
            {
                var product = GetProduct(productId);
                if (product.Type == Purchasing.ProductType.Consumable && Purchasing.IsBought(product.Id))
                {
                    if (callbackPurchase)
                    {
                        Purchasing_OnPurchaseSuccessful(product.Id);
                    }
                    else
                    {
                        YaGames.LogError($"Use Consume for product: {product.Id}");
                    }
                }
            }
        }

        private void Purchasing_OnPurchaseSuccessful(string productId)
        {
            foreach (var product in _listenProducts)
            {
                if (product == productId)
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
                if (product == productId)
                {
                    _onPurchaseFailed?.Invoke(productId);
                    break;
                }
            }
        }

        private Purchasing.Product GetProduct(string id)
        {
            return Core.YaGamesSettings.Instance.Products.FirstOrDefault((product) => product.Id == id);
        }
    }
}
