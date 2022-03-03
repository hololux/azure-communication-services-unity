using AzureCallingSDK.Plugins.WSA;

namespace AzureCallingSDK
{
    public class AzureCallingUWP : IAzureCalling
    {
        private AzureCallingUWPPlugin _callingUWPPlugin;
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
    }
}
