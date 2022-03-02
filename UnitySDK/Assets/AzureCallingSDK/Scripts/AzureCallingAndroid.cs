using UnityEngine;

namespace AzureCallingSDK
{
    public class AzureCallingAndroid : IAzureCalling
    {
        private AndroidJavaClass _pluginClass;
        private AndroidJavaObject _pluginInstance;
        private AndroidJavaClass PluginClass => _pluginClass ??= new AndroidJavaClass(PluginName);
        private AndroidJavaObject PluginInstance => _pluginInstance ??= PluginClass.CallStatic<AndroidJavaObject>("getInstance");
        
        private const string PluginName = "com.holospaces.azurecommunication.AzureCommunicationPlugin";
        
        public void Init(string userToken, string userName)
        {
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");   
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity"); 
            
            PluginInstance.Call("init", unityActivity, userToken);   
        }
        
        public void JoinTeamsMeeting(string teamsLik)
        {
            PluginInstance.Call("joinTeamsMeeting",teamsLik); 
        }
    
        public void LeaveMeeting()
        {
            PluginInstance.Call("leaveMeeting");  
        }
    }
}
