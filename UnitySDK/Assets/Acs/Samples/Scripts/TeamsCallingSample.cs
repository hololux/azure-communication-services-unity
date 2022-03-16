using UnityEngine;

namespace Hololux.Acs.Samples
{
    public class TeamsCallingSample : AzureCommunicationSample
    {
        #region serialized feilds
        [SerializeField] private string teamsLink;
        #endregion
        
        #region public methods
        public void JoinCall()
        {
            if (InCall) return;
            
            if (string.IsNullOrEmpty(teamsLink))
            {
                Debug.LogError("teamsLink is IsNullOrEmpty ");
                return;
            }

            InCall = true;
            AzureCommunication.JoinTeamsMeeting(teamsLink);
        }
        #endregion
    }
}
