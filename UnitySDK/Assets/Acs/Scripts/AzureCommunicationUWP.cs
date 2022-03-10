using Hololux.Acs.Plugins.WSA;

namespace Hololux.Acs
{
    public class AzureCommunicationUWP : IAzureCommunication
    {
        #region private fields
        private AzureCommunicationUWPPlugin _communicationUWPPlugin;
        #endregion
        
        #region public methods
        public void Init(string userToken, string userName)
        {
            _communicationUWPPlugin ??= new AzureCommunicationUWPPlugin();
            
            _communicationUWPPlugin?.Init(userToken,userName);
        }
        
        public void JoinTeamsMeeting(string teamsLik)
        {
            _communicationUWPPlugin?.JoinTeamsCallAsync(teamsLik);
        }

        public void JoinGroupCall(string groupGuid)
        {
            _communicationUWPPlugin?.JoinGroupCallAsync(groupGuid);
        }

        public void LeaveMeeting()
        {
            _communicationUWPPlugin?.LeaveMeeting();
        }

        public void Mute()
        {
            _communicationUWPPlugin?.Mute();
        }

        public void Unmute()
        {
            _communicationUWPPlugin?.UnMute();
        }
        #endregion
    }
}
