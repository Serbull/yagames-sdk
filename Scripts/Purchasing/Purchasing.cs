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
        private static extern void PurchaseConsumableExtern(string productId);

        [DllImport("__Internal")]
        private static extern void PurchaseNonConsumableExtern(string productId);

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

        public static void Purchase(string productId, bool consumable)
        {
#if UNITY_EDITOR
            OnPurchaseSuccessful?.Invoke(productId);
#else
            if (consumable) PurchaseConsumableExtern(productId);
            else PurchaseNonConsumableExtern(productId);
#endif
        }

        public void PurchaseSuccessful(string productId)
        {
            OnPurchaseSuccessful?.Invoke(productId);
        }

        public void PurchaseRestored(string productId)
        {
            if (!_restoredProducts.Contains(productId))
            {
                _restoredProducts.Add(productId);
            }
        }

        public void AllProductsRestored()
        {
            IsPurchasesRestored = true;
            OnPurchasesRestored?.Invoke();
        }
    }
}
