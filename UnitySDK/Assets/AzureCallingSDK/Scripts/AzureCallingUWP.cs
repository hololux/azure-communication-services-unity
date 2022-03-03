using AzureCallingSDK.Plugins.WSA;

namespace AzureCallingSDK
{
    public class AzureCallingUWP : IAzureCalling
    {
        #region private fields
        private AzureCallingUWPPlugin _callingUWPPlugin;
        #endregion
        
        #region public methods
        public void Init(string userToken, string userName)
        {
            _callingUWPPlugin ??= new AzureCallingUWPPlugin();
            
            _callingUWPPlugin?.Init(userToken,userName);
        }
        
        public void JoinTeamsMeeting(string teamsLik)
        {
            _callingUWPPlugin?.JoinAsync(teamsLik);
        }

        public void LeaveMeeting()
        {
            _callingUWPPlugin?.LeaveMeeting();
        }

        public void Mute()
        {
            throw new System.NotImplementedException();
        }

        public void Unmute()
        {
            throw new System.NotImplementedException();
        }

        public void StartVideo()
        {
            throw new System.NotImplementedException();
        }

        public void StopVideo()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
