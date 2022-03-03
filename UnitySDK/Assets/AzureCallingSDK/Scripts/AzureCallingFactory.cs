using UnityEngine;

namespace AzureCallingSDK
{
    public static class AzureCallingFactory
    {
        #region public methods
        public static IAzureCalling GetCallingInstance()
        {
            IAzureCalling callingInstance = Application.platform switch
            {
                RuntimePlatform.Android => new AzureCallingAndroid(),
                RuntimePlatform.WSAPlayerARM => new AzureCallingUWP(),
                RuntimePlatform.WSAPlayerX64 => new AzureCallingUWP(),
                RuntimePlatform.WSAPlayerX86 => new AzureCallingUWP(),
                _ => null
            };

            return callingInstance;
        }
        #endregion
    }
}
