using UnityEngine;

namespace Hololux.Acs.Samples
{
    public class GroupCallingSample : AzureCommunicationSample
    {
        #region serialized feilds
        [SerializeField] private string groupGuid;
        #endregion

        #region public methods
        public void JoinCall()
        { 
            if (InCall) return;
            
            if (string.IsNullOrEmpty(groupGuid))
            {
                Debug.LogError("groupGuid is IsNullOrEmpty, please assign one");
                return;
            }
            
            InCall = true;
            AzureCommunication.JoinGroupCall(groupGuid);
        }
        #endregion
    }
}
