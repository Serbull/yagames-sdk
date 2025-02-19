using System;
using UnityEngine;
using TMPro;

namespace YaGamesSDK.Components
{
    public class IapButton : MonoBehaviour
    {
        public enum Type
        {
            Cunsumable,
            NonConsumable
        }

        public event Action OnPurchaseSuccessful;

        [SerializeField] private string _productId;
        [SerializeField] private Type _type;
        [Space]
        [SerializeField] private TextMeshProUGUI _priceText;

        private bool _isBought;

        private void OnEnable()
        {
            Purchasing.OnPurchaseSuccessful += Purchasing_OnPurchaseSuccessful;
            FetchProduct();
        }

        private void Start()
        {
            if (_priceText != null)
            {
                _priceText.text = Purchasing.GetProductPrice(_productId);
            }
        }

        private void OnDisable()
        {
            Purchasing.OnPurchaseSuccessful -= Purchasing_OnPurchaseSuccessful;
        }

        private void FetchProduct()
        {
            _isBought = false;

            foreach (var product in Purchasing.RestoredProducts)
            {
                if (product == _productId)
                {
                    if (_type == Type.NonConsumable)
                    {
                        _isBought = true;
                    }
                    else
                    {
                        Debug.LogError($"Consume product: {product}");
                    }
                }
            }
        }

        private void Purchasing_OnPurchaseSuccessful(string productId)
        {
            if (productId == _productId)
            {
                OnPurchaseSuccessful?.Invoke();
                FetchProduct();
            }
        }

        public void ConsumePurchase()
        {
            if (_type == Type.Cunsumable)
            {
                Purchasing.ConsumePurchase(_productId);
            }
        }

        public bool IsBought()
        {
            if (_type == Type.NonConsumable)
            {
                FetchProduct();
                return _isBought;
            }
            else
            {
                Debug.LogError("'IsBought' works only for Non Consumables");
                return false;
            }
        }
    }
}
