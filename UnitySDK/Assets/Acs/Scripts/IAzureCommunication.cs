namespace Hololux.Acs
{
    public interface IAzureCommunication
    { 
        void Init(string userToken, string userName);
        void JoinTeamsMeeting(string teamsLik);
        void LeaveMeeting();
        void Mute();
        void Unmute();
    }
}