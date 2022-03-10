using UnityEngine;

namespace Hololux.Acs
{
    public class AzureCommunicationAndroid : IAzureCommunication
    {
        #region private fields
        private AndroidJavaClass _pluginClass;
        private AndroidJavaObject _pluginInstance;
        #endregion

        #region properties
        private AndroidJavaClass PluginClass => _pluginClass ??= new AndroidJavaClass(PluginName);
        private AndroidJavaObject PluginInstance => _pluginInstance ??= PluginClass.CallStatic<AndroidJavaObject>("getInstance");
        #endregion

        #region constants
        private const string PluginName = "com.holospaces.azurecommunication.AzureCommunicationPlugin";
        #endregion

        #region public methods
        public void Init(string userToken, string userName)
        {
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");   
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity"); 
            
            PluginInstance.Call("init", unityActivity,userToken);   
        }
        
        public void JoinTeamsMeeting(string teamsLik)
        {
            PluginInstance.Call("joinTeamsMeeting",teamsLik); 
        }

        public void JoinGroupCall(string groupGuid)
        {
            PluginInstance.Call("joinGroupCall",groupGuid); 
        }

        public void LeaveMeeting()
        {
            PluginInstance.Call("leaveMeeting");  
        }

        public void Mute()
        {
            PluginInstance.Call("mute");  
        }

        public void Unmute()
        {
            PluginInstance.Call("unMute");
        }
        #endregion
    }
}
