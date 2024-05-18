### Установка:
- С помощью UPM добавить пакет по ссылке на git `https://github.com/Serbull/yagames-sdk.git`

<img width="460" alt="image" src="https://github.com/Serbull/yagames-sdk/assets/54623966/9de8ed05-904c-416e-a2c8-b6fd23636db4">

### Инициализация:
- Добавить объект `YaGames.prefab` на загрузочную сцену.
### Настройка:

 <img width="460" alt="image" src="https://github.com/Serbull/yagames-sdk/assets/54623966/e87697b3-6184-4847-857e-7ddbd6a67653">
 
- `Show Interstitial On Repeat` показывать интерстишл рекламу на повторе.
- `Interstitial Repeat Timer` интервал показа интерстишл рекламы.

### Методы:
#### Реклама:
- `YaGames.ShowBannerAd();` показать баннер.
- `YaGames.HideBannerAd();` скрыть баннер.
- `YaGames.ShowInterstitialAd();` показать интерстишл рекламы.
- `YaGames.ShowInterstitialAdWithTimer(int time = 3)` показать интерстишл рекламу через время вместе с попапом таймером.
- `YaGames.ShowRewardedAd(Action callback);` показать видео с вознаграждением, `callback` - на награду.
#### Языки:
- `YaGames.GetLanguage();` возвращает `string` с кодом языка (напр: ‘ru’ , ‘en’ , ‘de’)
#### Флаги:
- `YaGames.IsFlagsLoaded` свойство на проверку загружены ли флаги с сервера.
- `YaGames.OnFlagsLoaded` событие на загрузку флагов с сервера.
- `YaGames.GetFlag(string flag, int defaultValue);` получить флаг из Яндекс консоли, `flag` - айдишник флага, `defaultValue` - базовое значение для флага - используется в эдиторе и в случае если флаги не загрузились с сервера.
#### Таблица лидеров:
- `SetLeaderboadScore(string leaderboardName, int score);`
#### Оценка игры:
- `IsReviewAvailable` возвращает возможно ли оценить игру 
- `ShowReview();` показать окно с оценкой игры
- `ShowReviewForReward(Action callback);` показать попап с наградой за оценку игры, `callback` - на выдачу награды.
