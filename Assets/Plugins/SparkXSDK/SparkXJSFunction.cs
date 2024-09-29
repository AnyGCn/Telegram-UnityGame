using System.Runtime.InteropServices;

namespace SparkX
{
    internal static class SparkXJSFunction
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        internal static extern string GetLoginToken();
    
        [DllImport("__Internal")]
        internal static extern string GetUserId();
    
        [DllImport("__Internal")]
        internal static extern string GetUserName();

        [DllImport("__Internal")]
        internal static extern string GetTelegramWebAppInitData();
    
        [DllImport("__Internal")]
        internal static extern string GetWebUrl();
        
        [DllImport("__Internal")]
        internal static extern void ShareViaTelegram(string shareUrl, string shareComment);
#else
        internal static string GetLoginToken()
        {
            return "Not Supported";
        }
    
        internal static string GetUserId()
        {
            return "Not Supported";
        }
        
        internal static string GetUserName()
        {
            return "Not Supported";
        }
        
        internal static string GetTelegramWebAppInitData()
        {
            return "Not Supported";
        }
        
        internal static string GetWebUrl()
        {
            return "Not Supported";
        }
        
        internal static void ShareViaTelegram(string shareUrl, string shareComment)
        {
            
        }
#endif
    }
}
