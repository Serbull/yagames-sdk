using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace YaGamesSDK
{
    public class LeaderboardPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private UserInfo _baseUserInfo;

        private void Start()
        {
            _baseUserInfo.gameObject.SetActive(false);
        }

        public void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}
