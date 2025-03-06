using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace YaGamesSDK.Components
{
    [RequireComponent(typeof(Image))]
    public class WebImage : MonoBehaviour
    {
        [SerializeField] private bool _disableImageIfNotLoaded;

        private Sprite _defaultSprite;

        private Image _targetImage;
        private Texture2D _currentTexture;
        private Sprite _currentSprite;
        private string _lastUrl;

        private void Start()
        {
            if (_targetImage == null)
            {
                Initiliaze();
            }
        }

        private void Initiliaze()
        {
            _targetImage = GetComponent<Image>();
            _defaultSprite = _targetImage.sprite;
            if (_disableImageIfNotLoaded)
            {
                _targetImage.enabled = false;
            }
        }

        public void LoadImageFromURL(string url)
        {
            if (_targetImage == null)
            {
                Initiliaze();
            }

            if (url == _lastUrl && _currentSprite != null)
            {
                return;
            }

            StartCoroutine(DownloadImage(url));
        }

        private IEnumerator DownloadImage(string url)
        {
            if (_disableImageIfNotLoaded)
            {
                _targetImage.enabled = false;
            }
            else
            {
                _targetImage.sprite = _defaultSprite;
            }

            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (_currentSprite != null)
                {
                    Destroy(_currentSprite);
                }

                if (_currentTexture != null)
                {
                    Destroy(_currentTexture);
                }

                _currentTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                _currentSprite = Sprite.Create(_currentTexture, new Rect(0, 0, _currentTexture.width, _currentTexture.height), new Vector2(0.5f, 0.5f));
                _lastUrl = url;

                _targetImage.enabled = true;
                _targetImage.sprite = _currentSprite;
            }
            else
            {
                Debug.LogError("Load Error: " + request.error);
            }
        }
    }
}
