using UnityEngine;
using UnityEngine.Android;

namespace Hololux.Acs.Samples
{
    public class PluginTest : MonoBehaviour
    {
        #region serialized feilds
        [SerializeField] private string userName;
        [SerializeField] private string userToken;
        [SerializeField] private string teamsLink;
        #endregion

        #region private feilds
        private IAzureCommunication _azureCommunication;
        #endregion

        #region unity methods
        private void Start()
        {
            #if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
            #endif

            _azureCommunication = AcsFactory.GetCommunicationInstance();
            Init();
        }
        
        void Update()
        {
            transform.Rotate(10.0f *Time.deltaTime, 10.0f*Time.deltaTime, -7.0f*Time.deltaTime);
        }
        #endregion
    
        public void JoinTeamsMeeting()
        {
            if (teamsLink == null)
            {
                Debug.LogError("teamsLink is null, please assign one");
                return;
            }
        
            _azureCommunication.JoinTeamsMeeting(teamsLink);
        }

        public void LeaveMeeting()
        {
            _azureCommunication.LeaveMeeting();  
        }

        public void Mute(bool mute)
        {
            Debug.Log(mute);
            
            if (mute)
            {
                _azureCommunication.Mute();
            }
            else
            {
                _azureCommunication.Unmute();
            }
        }

        #region private methods
        private void Init()
        {
            if (userToken == null)
            {
                Debug.LogError("User token is null, please assign one");
                return;
            }

            _azureCommunication.Init(userToken,userName);
        }
        #endregion
    }
}
