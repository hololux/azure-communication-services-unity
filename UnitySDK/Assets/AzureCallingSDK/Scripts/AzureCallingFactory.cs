using UnityEngine;

namespace AzureCallingSDK
{
    public static class AzureCallingFactory
    {
        public static IAzureCalling GetCallingInstance()
        {
            IAzureCalling callingInstance = Application.platform switch
            {
                RuntimePlatform.Android => new AzureCallingAndroid(),
                RuntimePlatform.WSAPlayerARM => new AzureCallingUWP(),
                _ => null
            };

            return callingInstance;
        }
    }
}
