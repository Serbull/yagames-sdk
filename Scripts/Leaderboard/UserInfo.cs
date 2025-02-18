using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YaGamesSDK
{
    public class UserInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _placeText;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
    }
}