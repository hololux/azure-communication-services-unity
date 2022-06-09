using System;
using UnityEngine;

namespace Hololux.Acs
{
    public class AzureCommunicationAndroid : IAzureCommunication
    {
        #region private fields
        private AndroidJavaClass _pluginClass;
        private AndroidJavaObject _pluginInstance;
        private IntPtr _sendFrameMethodPtr;
        private IntPtr _pluginInstancePtr;
        #endregion

        #region properties
        private AndroidJavaClass PluginClass => _pluginClass ??= new AndroidJavaClass(PluginName);
        private AndroidJavaObject PluginInstance; // => _pluginInstance ??= PluginClass.CallStatic<AndroidJavaObject>("getInstance");
        #endregion

        #region constants
        private const string PluginName = "com.holospaces.azurecommunication.AzureCommunicationPlugin";
        #endregion

        #region public methods
        public void Init(string userToken, string userName)
        {
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");   
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

            PluginInstance = new AndroidJavaObject(PluginName);
            PluginInstance.Call("init", unityActivity,userToken,userName);
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

        public void SendFrame(byte[] frameData)
        {
            sbyte[] signedFrameData = new sbyte[frameData.Length]; // android byte[] is signed 
            Buffer.BlockCopy( frameData, 0, signedFrameData, 0, frameData.Length );
            PluginInstance.Call("sendFrame", signedFrameData);
        }
        #endregion
    }
}
