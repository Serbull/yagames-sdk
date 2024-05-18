using System.Collections;
using UnityEngine;
//using TMPro;
using System;

namespace YandexGames
{
    public class InterstitialAdTimerPopup : MonoBehaviour
    {
        [SerializeField] private GameObject _previewPanel;
        [SerializeField] private GameObject _finishPanel;
        //[SerializeField] private TextMeshProUGUI _timerText;

        private bool _adClosed;
        private Action _closeAction;

        public void Initialize(Action closeAction, int time = 3)
        {
            _closeAction = closeAction;
            _previewPanel.SetActive(true);
            _finishPanel.SetActive(false);
            StartCoroutine(TimerAnim(time));
        }

        public void AdClosed()
        {
            _adClosed = true;
            _previewPanel.SetActive(false);
            _finishPanel.SetActive(true);
        }

        private IEnumerator TimerAnim(int time)
        {
            var prefix = "Через ";
            while (time > 0)
            {
                //_timerText.text = prefix + time;
                yield return new WaitForSecondsRealtime(1);
                time--;
            }
        }

        public void ClickOnPanel()
        {
            if (_adClosed)
            {
                _closeAction?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}