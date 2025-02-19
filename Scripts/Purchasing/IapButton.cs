using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YaGamesSDK.Components
{
    [RequireComponent(typeof(Button))]
    public class IapButton : MonoBehaviour
    {
        public enum Type
        {
            Cunsumable,
            NonConsumable
        }

        [SerializeField] private string _productId;
        [SerializeField] private Type _type;
        [Space]
        [SerializeField] private TextMeshProUGUI _priceText;
        [Space]
        public UnityEvent onPurchaseSuccessful;

        private bool _isBought;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Purchase);
        }

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
                       YaGames.LogError($"Consume product: {product}");
                    }
                }
            }
        }

        private void Purchase()
        {
            Purchasing.Purchase(_productId);
        }

        private void Purchasing_OnPurchaseSuccessful(string productId)
        {
            if (productId == _productId)
            {
                onPurchaseSuccessful?.Invoke();
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
                YaGames.LogError("'IsBought' works only for NonConsumables");
                return false;
            }
        }
    }
}
