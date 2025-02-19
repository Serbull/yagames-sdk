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
        private static extern void PurchaseExtern(string productId);

        [DllImport("__Internal")]
        private static extern void ConsumePurchaseExtern(string productId);

        [DllImport("__Internal")]
        private static extern void RestorePurchasesExtern();

        #endregion

        public static event Action OnPurchasesRestored;
        public static event Action<string> OnPurchaseSuccessful;

        private static readonly List<string> _restoredProducts = new();

        public static bool IsPurchasesRestored { get; private set; }
        public static string[] RestoredProducts => _restoredProducts.ToArray();

        public static string GetProductPrice(string productId)
        {
#if UNITY_EDITOR
            return "-";
#else
            return GetProductPriceExtern(productId);
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
            OnPurchaseSuccessful?.Invoke(productId);
#else
            PurchaseExtern(productId);
#endif
        }

        public static void ConsumePurchase(string productId)
        {
            if (_restoredProducts.Contains(productId))
            {
                _restoredProducts.Remove(productId);

#if !UNITY_EDITOR
                ConsumePurchaseExtern(productId);
#endif
            }
        }

        public void PurchaseSuccessful(string productId)
        {
            _restoredProducts.Add(productId);
            OnPurchaseSuccessful?.Invoke(productId);
        }

        public void PurchaseRestored(string productId)
        {
            _restoredProducts.Add(productId);
        }

        public void AllProductsRestored()
        {
            IsPurchasesRestored = true;
            OnPurchasesRestored?.Invoke();
        }
    }
}
