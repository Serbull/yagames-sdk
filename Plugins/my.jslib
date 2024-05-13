mergeInto(LibraryManager.library, {
    
    ShowInterstitialAdExtern: function ()    {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onOpen: () => {
                    console.log('Interstitial ad open.');
                    myGameInstance.SendMessage('YandexSDK', 'InterstitialAdOpened');
                },
                onClose: function(wasShown) {
		    console.log('Interstitial ad shown.');
                    myGameInstance.SendMessage('YandexSDK', 'InterstitialAdClosed');
                },
                onError: function(error) {
                    console.log('Error while open interstitial ad:', error);
                }
            }
        })
    },

    ShowRewardedAdExtern: function () {
        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('Video ad open.');
                    myGameInstance.SendMessage('YandexSDK', 'RewardedAdOpened');
                },
                onRewarded: () => {
                    console.log('Rewarded!');
                    myGameInstance.SendMessage('YandexSDK', 'RewardedAdGranded');
                },
                onClose: () => {
                    console.log('Video ad closed.');
                    myGameInstance.SendMessage('YandexSDK', 'RewardedAdClosed');
                }, 
                onError: (e) => {
                    console.log('Error while open video ad:', e);
                    myGameInstance.SendMessage('YandexSDK', 'RewardedAdNotReady');
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

    SetLeaderboardScoreExtern: function (name, score) {
        var nameString = UTF8ToString(name);
        ysdk.isAvailableMethod('leaderboards.setLeaderboardScore')
        .then(function (isAvailable) {
            if (isAvailable) {
                lb.setLeaderboardScore(nameString, score);
                console.log('Successful add score to leaderboard');
            } else {
                console.log('Cannot set leaderboard score!');
            }
        })
        .catch(function (error) {
            console.error('Get available leaderboard return error: ', error);
        })
    },

    GetLanguageExtern: function () {
        var lang = ysdk.environment.i18n.lang;
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    },

    CheckCanReviewExtern: function () {
        ysdk.feedback.canReview()
        .then(({ value, reason }) => {
            if (value) {
                console.log('Review is available');
                myGameInstance.SendMessage('YandexSDK', 'ReviewAvailable');
            } else {
                console.log('Review not available: ', reason);
                myGameInstance.SendMessage('YandexSDK', 'ReviewNotAvailable', reason);
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
                        if(feedbackSent) myGameInstance.SendMessage('YandexSDK', 'ReviewFinishSuccessful');
                        else myGameInstance.SendMessage('YandexSDK', 'ReviewFinishCancel');
                    })
            } else {
                console.log('Review cancelled: ', reason);
                myGameInstance.SendMessage('YandexSDK', 'ReviewFinishCancel');
            }
        })
    },

    RestorePurchasesExtern: function () {
        console.log('Restore purchases');
        payments.getPurchases()
        .then(purchases => {
            purchases.forEach((purchase) => {
                console.log('Restore: ', purchase.productID);
                myGameInstance.SendMessage('YandexSDK', 'PurchaseRestored', purchase.productID);
            });
            console.log('Purchases restored successful');
            myGameInstance.SendMessage('YandexSDK', 'PurchasesRestored');
        }).catch(err => {
            console.log('Purchases restored: ', err);
            myGameInstance.SendMessage('YandexSDK', 'PurchasesRestored');
            // Выбрасывает исключение USER_NOT_AUTHORIZED для неавторизованных пользователей.
        })
    },

    PurchaseConsumableExtern: function (productId) {
        console.log('Error: method not exist');
    },

    PurchaseNonConsumableExtern: function (productId) {
        var productIdString = UTF8ToString(productId);
        console.log('Purchase: ', productIdString);
        payments.purchase({ id: productIdString })
        .then(purchase => {
            console.log('Purchase successful: ', productIdString);
            myGameInstance.SendMessage('YandexSDK', 'PurchaseSuccessful', productIdString);
            // Покупка успешно совершена!
        }).catch(err => {
            console.log('Purchase failed: ', productIdString);
            myGameInstance.SendMessage('YandexSDK', 'PurchaseFailed', productIdString);
            // Покупка не удалась: в консоли разработчика не добавлен товар с таким id,
            // пользователь не авторизовался, передумал и закрыл окно оплаты,
            // истекло отведенное на покупку время, не хватило денег и т. д.
        })
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
    
    SaveGameExtern: function (data) {
        var dataString = UTF8ToString(data);
        var myobj = JSON.parse(dataString);
        player.setData(myobj);
    },

    LoadGameExtern: function () {
        if(player == null) {
            console.log("Player is null: return null save data");
            myGameInstance.SendMessage('YandexSDK', 'UserDataLoaded', null);
            return;
        }
        player.getData().then(_data => {
            const myJSON = JSON.stringify(_data);
            console.log('Player data: ', myJSON);
            myGameInstance.SendMessage('YandexSDK', 'UserDataLoaded', myJSON);
        });
    },

    LoadFlagsExtern: function () {
        console.log('[MyJslib] Request flags');
        ysdk.getFlags().then(flags => {
            const myJson = JSON.stringify(flags);
            console.log('[MyJslib] Flags loaded:', myJson);
            myGameInstance.SendMessage('YandexSDK', 'FlagsLoaded', myJson);
        });
    },
});