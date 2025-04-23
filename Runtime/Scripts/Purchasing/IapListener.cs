using UnityEngine;
using UnityEngine.Events;

namespace YaGamesSDK.Components
{
    public class IapListener : MonoBehaviour
    {
        public enum Type
        {
            Consumable,
            NonConsumable
        }

        [SerializeField] private string _productId;
        [SerializeField] private Type _type;
        [Space]
        [SerializeField] private UnityEvent _onPurchaseSuccessful;
        [SerializeField] private UnityEvent _onPurchaseFailed;

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

        public void AddListener(UnityAction<bool> onPurchaseCompleted)
        {
            _onPurchaseSuccessful.AddListener(() => onPurchaseCompleted(true));
            _onPurchaseFailed.AddListener(() => onPurchaseCompleted(false));
            CheckConsumableProduct(true);
        }

        public void RemoveListener(UnityAction<bool> onPurchaseCompleted)
        {
            _onPurchaseSuccessful.RemoveListener(() => onPurchaseCompleted(true));
            _onPurchaseFailed.RemoveListener(() => onPurchaseCompleted(false));
        }

        private void CheckConsumableProduct(bool callbackPurchase)
        {
            if (_type == Type.Consumable && IsBought())
            {
                if (callbackPurchase)
                {
                    Purchasing_OnPurchaseSuccessful(_productId);
                }
                else
                {
                    YaGames.LogError($"Use Consume for product: {_productId}");
                }
            }
        }

        private void Purchasing_OnPurchaseSuccessful(string productId)
        {
            if (productId == _productId)
            {
                _onPurchaseSuccessful?.Invoke();
                CheckConsumableProduct(false);
            }
        }

        private void Purchasing_OnPurchaseFailed(string productId)
        {
            if (productId == _productId)
            {
                _onPurchaseFailed?.Invoke();
            }
        }

        public void ConsumePurchase()
        {
            if (_type == Type.Consumable)
            {
                Purchasing.ConsumePurchase(_productId);
            }
        }

        public bool IsBought()
        {
            return Purchasing.IsBought(_productId);
        }
    }
}
