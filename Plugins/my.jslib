mergeInto(LibraryManager.library, {

    SendGameReadyExtern: function () {
        console.log('[YaGamesLib] Game Ready');
        ysdk.features.LoadingAPI.ready();
    },

    ShowInterstitialAdExtern: function () {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onOpen: () => {
                    console.log('[YaGamesLib] Interstitial ad open.');
                    myGameInstance.SendMessage('YaGames', 'InterstitialAdOpened');
                },
                onClose: function (wasShown) {
                    console.log('[YaGamesLib] Interstitial ad shown.');
                    myGameInstance.SendMessage('YaGames', 'InterstitialAdClosed');
                },
                onError: function (error) {
                    console.log('[YaGamesLib] Error while open interstitial ad:', error);
                }
            }
        })
    },

    ShowRewardedAdExtern: function () {
        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('[YaGamesLib] Video ad open.');
                    myGameInstance.SendMessage('YaGames', 'RewardedAdOpened');
                },
                onRewarded: () => {
                    console.log('[YaGamesLib] Rewarded!');
                    myGameInstance.SendMessage('YaGames', 'RewardedAdGranded');
                },
                onClose: () => {
                    console.log('[YaGamesLib] Video ad closed.');
                    myGameInstance.SendMessage('YaGames', 'RewardedAdClosed');
                },
                onError: (e) => {
                    console.log('[YaGamesLib] Error while open video ad:', e);
                    myGameInstance.SendMessage('YaGames', 'RewardedAdNotReady');
                }
            }
        })
    },

    ShowBannerAdExtern: function () {
        ysdk.adv.showBannerAdv();
    },

    HideBannerAdExtern: function () {
        ysdk.adv.hideBannerAdv();
    },

    SetLeaderboardScoreExtern: function (name, score, extraData) {
        var nameString = UTF8ToString(name);
        var extraDataString = UTF8ToString(extraData);
        ysdk.isAvailableMethod('leaderboards.setLeaderboardScore')
            .then(function (isAvailable) {
                if (isAvailable) {
                    lb.setLeaderboardScore(nameString, score, extraDataString);
                    console.log('Successful add score to leaderboard');
                } else {
                    console.log('Cannot set leaderboard score!');
                }
            })
            .catch(function (error) {
                console.error('Get available leaderboard return error: ', error);
            })
    },

    LoadLeaderboardExtern: function (name, includeUser, quantityAround, quantityTop) {
        var nameString = UTF8ToString(name);
        ysdk.getLeaderboards()
            .then(lb => {
                lb.getLeaderboardEntries(nameString, { quantityTop: quantityTop, includeUser: includeUser, quantityAround: quantityAround })
                    .then(res => {

                        res.entries = res.entries.map(entry => ({
                            ...entry,
                            avatarUrl: entry.player.getAvatarSrc('small')
                        }));

                        const myJson = JSON.stringify(res);
                        console.log('[YaGamesLib] Leaderboard loaded:', myJson);
                        myGameInstance.SendMessage('YaGames', 'LeaderboardLoaded', myJson);
                    });
            });
    },

    GetLanguageExtern: function () {
        var lang = ysdk.environment.i18n.lang;
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    },

    GetDeviceInfoExtern: function () {
        console.log('[YaGamesLib] Device type:', ysdk.deviceInfo.type);
        var data = ysdk.deviceInfo.type;
        var bufferSize = lengthBytesUTF8(data) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(data, buffer, bufferSize);
        return buffer;
    },

    CheckCanReviewExtern: function () {
        ysdk.feedback.canReview()
            .then(({ value, reason }) => {
                if (value) {
                    console.log('Review is available');
                    myGameInstance.SendMessage('YaGames', 'ReviewAvailable');
                } else {
                    console.log('Review not available: ', reason);
                    myGameInstance.SendMessage('YaGames', 'ReviewNotAvailable', reason);
                }
            })
    },

    ShowReviewExtern: function () {
        ysdk.feedback.canReview()
            .then(({ value, reason }) => {
                if (value) {
                    ysdk.feedback.requestReview()
                        .then(({ feedbackSent }) => {
                            console.log(feedbackSent);
                            if (feedbackSent) myGameInstance.SendMessage('YaGames', 'ReviewFinishSuccessful');
                            else myGameInstance.SendMessage('YaGames', 'ReviewFinishCancel');
                        })
                } else {
                    console.log('Review cancelled: ', reason);
                    myGameInstance.SendMessage('YaGames', 'ReviewFinishCancel');
                }
            })
    },

    RestorePurchasesExtern: function () {
        console.log('Restore purchases');
        payments.getPurchases()
            .then(purchases => {
                purchases.forEach((purchase) => {
                    console.log('Restore: ', purchase.productID);
                    myGameInstance.SendMessage('YaGames', 'PurchasingProductRestored', purchase.productID);
                });
                console.log('Purchases restored successful');
                myGameInstance.SendMessage('YaGames', 'PurchasingAllProductsRestored');
            }).catch(err => {
                console.log('Purchases restored with error: ', err);
                myGameInstance.SendMessage('YaGames', 'PurchasingAllProductsRestored');
                // Выбрасывает исключение USER_NOT_AUTHORIZED для неавторизованных пользователей.
            })
    },

    PurchaseExtern: function (productId) {
        var productIdString = UTF8ToString(productId);
        console.log('Purchase: ', productIdString);
        payments.purchase({ id: productIdString })
            .then(purchase => {
                console.log('Purchase successful: ', productIdString);
                myGameInstance.SendMessage('YaGames', 'PurchasingPurchaseSuccessful', productIdString);
                // Покупка успешно совершена!
            }).catch(err => {
                console.log('Purchase failed: ', productIdString);
                myGameInstance.SendMessage('YaGames', 'PurchasingPurchaseFailed', productIdString);
                // Покупка не удалась: в консоли разработчика не добавлен товар с таким id,
                // пользователь не авторизовался, передумал и закрыл окно оплаты,
                // истекло отведенное на покупку время, не хватило денег и т. д.
            })
    },

    ConsumePurchaseExtern: function (productId) {
        var productIdString = UTF8ToString(productId);
        console.log('Consume purchase: ', productIdString);
        payments.getPurchases().then(purchases => {
            let purchase = purchases.find(p => p.productID === productIdString);
            if (purchase) {
                payments.consumePurchase(purchase.purchaseToken);
            }
        });
    },

    GetProductPriceExtern: function (productId) {
        var productIdString = UTF8ToString(productId);
        var result;
        var product = products.find(p => p.id === productIdString);
        if (product) {
            console.log('product price: ', productIdString, product.priceValue);
            result = product.priceValue;
        } else {
            console.log('product not found: ', productIdString);
            result = '-';
        }
        var bufferSize = lengthBytesUTF8(result) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(product.priceValue, buffer, bufferSize);
        return buffer;
    },

    GetCurrencyImageExtern: function (productId) {
        var productIdString = UTF8ToString(productId);
        var product = products.find(p => p.id === productIdString);
        if (product) {
            var result = product.getPriceCurrencyImage("medium");
            var bufferSize = lengthBytesUTF8(result) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(result, buffer, bufferSize);
            return buffer;
        } else {
            console.log('product not found: ', productIdString);
            return 0;
        }
    },

    SaveGameExtern: function (data) {
        var dataString = UTF8ToString(data);
        var myobj = JSON.parse(dataString);
        player.setData(myobj);
    },

    LoadGameExtern: function () {
        if (player == null) {
            console.log('[YaGamesLib] Player is null: return null save data');
            myGameInstance.SendMessage('YaGames', 'CloudSavesLoaded', null);
            return;
        }
        player.getData().then(_data => {
            const myJSON = JSON.stringify(_data);
            console.log('[YaGamesLib] Player data: ', myJSON);
            myGameInstance.SendMessage('YaGames', 'CloudSavesLoaded', myJSON);
        });
    },

    LoadFlagsExtern: function () {
        console.log('[YaGamesLib] Request flags');
        ysdk.getFlags().then(flags => {
            const myJson = JSON.stringify(flags);
            console.log('[YaGamesLib] Flags loaded:', myJson);
            myGameInstance.SendMessage('YaGames', 'FlagsLoaded', myJson);
        });
    },

    GetPlayerInfoExtern: function () {
        const name = player ? player.getName() : null;
        const id = player ? player.getUniqueID() : null;

        return JSON.stringify({
            name: name,
            id: id
        });
    },
});