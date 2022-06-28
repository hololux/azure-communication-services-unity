
namespace Hololux.Acs
{
    public interface IAzureCommunication
    { 
        void Init(string userToken, string userName);
        void JoinTeamsMeeting(string teamsLik);
        void JoinGroupCall(string groupGuid);
        void LeaveCall();
        void Mute();
        void Unmute();
        void SendFrame(byte[] frameData);
    }
}
