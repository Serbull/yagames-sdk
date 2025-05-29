using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace YaGamesSDK
{
    public class Purchasing
    {
        public enum ProductType
        {
            Consumable,
            NonConsumable
        }

        [Serializable]
        public class Product
        {
            public string Id;
            public ProductType Type;
        }

        #region INTERNAL

        [DllImport("__Internal")]
        private static extern string GetProductPriceExtern(string productId);

        [DllImport("__Internal")]
        private static extern string GetCurrencyImageExtern(string productId);

        [DllImport("__Internal")]
        private static extern void PurchaseExtern(string productId);

        [DllImport("__Internal")]
        private static extern void ConsumePurchaseExtern(string productId);

        [DllImport("__Internal")]
        private static extern void RestorePurchasesExtern();

        #endregion

        public static event Action OnPurchasesRestored;
        public static event Action<string> OnPurchaseSuccessful;
        public static event Action<string> OnPurchaseFailed;

        private static readonly List<string> _purchasedProducts = new();

        public static bool IsInitialized { get; private set; }
        public static string[] PurchasedProducts => _purchasedProducts.ToArray();

        public static string GetProductPrice(string productId)
        {
#if UNITY_EDITOR
            return "-";
#else
            BaseProductToAB(ref productId);
            return GetProductPriceExtern(productId);
#endif
        }

        public static string GetCurrencyImage(string productId)
        {
#if UNITY_EDITOR
            return "https://yastatic.net/s3/games-static/static-data/images/payments/sdk/currency-icon-s@2x.png";
#else
            BaseProductToAB(ref productId);
            return GetCurrencyImageExtern(productId);
#endif
        }

        public static void Initialize()
        {
            if (IsInitialized) return;

#if !UNITY_EDITOR
            RestorePurchasesExtern();
#endif
        }

        public static void Purchase(string productId)
        {
#if UNITY_EDITOR
            new Purchasing().PurchaseSuccessful(productId);
#else
            BaseProductToAB(ref productId);
            PurchaseExtern(productId);
#endif
        }

        public static void ConsumePurchase(string productId)
        {
            if (_purchasedProducts.Contains(productId))
            {
                _purchasedProducts.Remove(productId);

#if !UNITY_EDITOR
                BaseProductToAB(ref productId);
                ConsumePurchaseExtern(productId);
#endif
            }
        }

        public void PurchaseSuccessful(string productId)
        {
            ABToBaseProduct(ref productId);
            YaGames.Log("Purchase successful: " + productId);
            _purchasedProducts.Add(productId);
            OnPurchaseSuccessful?.Invoke(productId);
        }

        public void PurchaseFailed(string productId)
        {
            ABToBaseProduct(ref productId);
            YaGames.Log("Purchase failed: " + productId);
            OnPurchaseFailed?.Invoke(productId);
        }

        public void PurchaseRestored(string productId)
        {
            ABToBaseProduct(ref productId);
            YaGames.Log("Product restored: " + productId);
            _purchasedProducts.Add(productId);
        }

        public void AllProductsRestored()
        {
            YaGames.Log("Purchasing initialized");
            IsInitialized = true;
            OnPurchasesRestored?.Invoke();
        }

        public static bool IsBought(string productId)
        {
            foreach (var product in _purchasedProducts)
            {
                if (product == productId)
                {
                    return true;
                }
            }

            return false;
        }

        private static void BaseProductToAB(ref string productId)
        {
            var suffix = Flags.GetFlag("inAppProducts", "-");
            if (!string.IsNullOrEmpty(suffix) && suffix != "-")
            {
                productId += suffix;
            }
        }

        private static void ABToBaseProduct(ref string productId)
        {
            var suffix = Flags.GetFlag("inAppProducts", "-");
            if (!string.IsNullOrEmpty(suffix) && suffix != "-" && productId.EndsWith(suffix))
            {
                productId = productId[..^suffix.Length];
            }
        }
    }
}
