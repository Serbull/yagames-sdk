using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace YaGamesSDK
{
    public class Purchasing
    {
        #region INTERNAL

        [DllImport("__Internal")]
        private static extern string GetProductPriceExtern(string productId);

        [DllImport("__Internal")]
        private static extern string GetPriceCurrencyImageExtern(string productId);

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

        public static bool IsPurchasesRestored { get; private set; }
        public static string[] PurchasedProducts => _purchasedProducts.ToArray();

        public static string GetProductPrice(string productId)
        {
#if UNITY_EDITOR
            return "-";
#else
            return GetProductPriceExtern(productId);
#endif
        }

        public static string GetPriceCurrencyImage(string productId)
        {
#if UNITY_EDITOR
            return "-";
#else
            return GetPriceCurrencyImageExtern(productId);
#endif
        }

        public static void RestorePurchases()
        {
            if (IsPurchasesRestored) return;

#if !UNITY_EDITOR
            RestorePurchasesExtern();
#endif
        }

        public static void Purchase(string productId)
        {
#if UNITY_EDITOR
            new Purchasing().PurchaseSuccessful(productId);
#else
            PurchaseExtern(productId);
#endif
        }

        public static void ConsumePurchase(string productId)
        {
            if (_purchasedProducts.Contains(productId))
            {
                _purchasedProducts.Remove(productId);

#if !UNITY_EDITOR
                ConsumePurchaseExtern(productId);
#endif
            }
        }

        public void PurchaseSuccessful(string productId)
        {
            YaGames.Log("Purchase successful: " + productId);
            _purchasedProducts.Add(productId);
            OnPurchaseSuccessful?.Invoke(productId);
        }

        public void PurchaseFailed(string productId)
        {
            YaGames.Log("Purchase failed: " + productId);
            OnPurchaseFailed?.Invoke(productId);
        }

        public void PurchaseRestored(string productId)
        {
            YaGames.Log("Product restored: " + productId);
            _purchasedProducts.Add(productId);
        }

        public void AllProductsRestored()
        {
            YaGames.Log("All products restored");
            IsPurchasesRestored = true;
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
    }
}
