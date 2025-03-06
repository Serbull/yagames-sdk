using UnityEngine;

namespace YaGamesSDK.Components
{
    [RequireComponent(typeof(WebImage))]
    public class WebImageLoaderOnStart : MonoBehaviour
    {
        [SerializeField] private string _url;

        private void Start()
        {
            GetComponent<WebImage>().LoadImageFromURL(_url);
        }
    }
}
