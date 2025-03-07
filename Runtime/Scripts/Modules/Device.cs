using System.Runtime.InteropServices;

namespace YaGamesSDK
{
    public class Device
    {
        private static string _deviceType;
        private static bool _isTouch;

        [DllImport("__Internal")]
        private static extern string GetDeviceInfoExtern();

        public static bool IsTouch
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                if (_deviceType == null)
                {
                    _deviceType = GetDeviceInfoExtern();
                    _isTouch = _deviceType == "mobile" || _deviceType == "tablet";
                }

                return _isTouch;
#endif
            }
        }
    }
}
