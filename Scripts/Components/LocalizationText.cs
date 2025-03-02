using UnityEngine;
using TMPro;

namespace YaGamesSDK.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationText : MonoBehaviour
    {
        [SerializeField] private string _textEn;
        [SerializeField] private string _textRu;

        private TextMeshProUGUI _text;
        private string _arg0;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        protected void UpdateText()
        {
            var text = string.Format(GetText(), _arg0);

            if (_text != null && text != null)
            {
                _text.text = text;
            }
        }

        public void SetArgument(object arg0 = null)
        {
            _arg0 = arg0?.ToString();
            UpdateText();
        }

        private string GetText()
        {
            var lang = YaGames.GetLanguage("en");
            if (lang == "ru")
            {
                return _textRu;
            }
            else
            {
                return _textEn;
            }
        }
    }
}
