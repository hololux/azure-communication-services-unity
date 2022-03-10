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
            if (string.IsNullOrEmpty(groupGuid))
            {
                Debug.LogError("groupGuid is IsNullOrEmpty, please assign one");
                return;
            }
            
            AzureCommunication.JoinGroupCall(groupGuid);
        }
        #endregion
    }
}
