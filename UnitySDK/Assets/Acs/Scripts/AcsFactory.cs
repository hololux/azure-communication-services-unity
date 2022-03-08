using UnityEngine;

namespace Hololux.Acs
{
    public static class AcsFactory
    {
        #region public methods
        public static IAzureCommunication GetCommunicationInstance()
        {
            IAzureCommunication communicationInstance = Application.platform switch
            {
                RuntimePlatform.Android => new AzureCommunicationAndroid(),
                RuntimePlatform.WSAPlayerARM => new AzureCommunicationUWP(),
                RuntimePlatform.WSAPlayerX64 => new AzureCommunicationUWP(),
                RuntimePlatform.WSAPlayerX86 => new AzureCommunicationUWP(),
                _ => null
            };

            return communicationInstance;
        }
        #endregion
    }
}
