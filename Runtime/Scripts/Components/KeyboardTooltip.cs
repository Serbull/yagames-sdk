using UnityEngine;
using TMPro;

namespace YaGamesSDK.Components
{
    public class KeyboardTooltip : MonoBehaviour
    {
        [SerializeField] private string _key = "A";

        private void Start()
        {
            if (Device.IsTouch)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnValidate()
        {
            GetComponentInChildren<TextMeshProUGUI>().text = _key;
        }
    }
}
