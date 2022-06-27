using UnityEngine;

namespace Hololux.Acs
{
    public static class AcsFactory
    {
        private static IAzureCommunication _azureCommunication;
        
        #region public methods
        public static IAzureCommunication GetCommunicationInstance()
        {
            if (_azureCommunication != null) return _azureCommunication;
            
            IAzureCommunication communicationInstance = Application.platform switch
            {
                RuntimePlatform.Android => new AzureCommunicationAndroid(),
                RuntimePlatform.WSAPlayerARM => new AzureCommunicationUWP(),
                RuntimePlatform.WSAPlayerX64 => new AzureCommunicationUWP(),
                RuntimePlatform.WSAPlayerX86 => new AzureCommunicationUWP(),
                _ => null
            };

            _azureCommunication = communicationInstance;
            return communicationInstance;
        }
        #endregion
    }
}
