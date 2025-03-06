using System.Runtime.InteropServices;

namespace YaGamesSDK
{
    public class DeviceInfo
    {
        private static string _deviceType;
        private static bool _isDeviceTouchable;

        [DllImport("__Internal")]
        private static extern string GetDeviceInfoExtern();

        public static bool IsDeviceTouchable
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                if (_deviceType == null)
                {
                    _deviceType = GetDeviceInfoExtern();
                    _isDeviceTouchable = _deviceType == "mobile" || _deviceType == "tablet";
                }

                return _isDeviceTouchable;
#endif
            }
        }
    }
}
