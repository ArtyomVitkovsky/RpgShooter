using UnityEngine.Device;

namespace _Project.Scripts.Utils
{
    public static class TabletDetectorUtil
    {
        public static bool IsTablet()
        {
            var aspect = (float)Screen.width / Screen.height;
            return aspect > 0.6f;
        }
    }
}