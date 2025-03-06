using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YaGamesSDK
{
    public class RateUsForRewardPopup : MonoBehaviour
    {
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private Button _rateButton;
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private Image _currencyImage;

        private bool _rateClicked;

        private System.Action _callback;
        private bool _grandReward;

        public void Initialize(System.Action callback, int currencyCount, Sprite currencyIcon)
        {
            _mainPanel.SetActive(true);
            _errorPanel.SetActive(false);
            _rateButton.onClick.AddListener(Rate_BtnClick);
            _currencyText.text = currencyCount.ToString();
            _currencyImage.sprite = currencyIcon;

            _callback = callback;
            YaGames.OnReviewFinish += YandexSDK_OnReviewFinish;
        }

        private void OnDestroy()
        {
            YaGames.OnReviewFinish -= YandexSDK_OnReviewFinish;
        }

        private void Update()
        {
            if (_grandReward)
            {
                _grandReward = false;
                _callback?.Invoke();
                ClosePanel();
            }
        }

        private void YandexSDK_OnReviewFinish(bool sent)
        {
            if (sent)
            {
                _grandReward = true;
            }
            else
            {
                _mainPanel.SetActive(false);
                _errorPanel.SetActive(true);
            }
        }

        private void Rate_BtnClick()
        {
            if (!_rateClicked)
            {
                _rateClicked = true;
                YaGames.ShowReview();
            }
        }

        public void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}