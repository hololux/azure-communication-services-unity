namespace AzureCallingSDK
{
    public interface IAzureCalling
    { 
        void Init(string userToken, string userName);
        void JoinTeamsMeeting(string teamsLik);
        void LeaveMeeting();
        void Mute();
        void Unmute();
        void StartVideo();
        void StopVideo();
    }
}
