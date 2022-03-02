using AzureCallingSDK;
using UnityEngine;
using UnityEngine.Android;

namespace AzureCallingSDK.Samples
{
    public class PluginTest : MonoBehaviour
    {
        #region serialized feilds
        [SerializeField] private string userName;
        [SerializeField] private string userToken;
        [SerializeField] private string teamsLink;
        #endregion

        #region private feilds
        private IAzureCalling _azureCalling;
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

            _azureCalling = AzureCallingFactory.GetCallingInstance();
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
        
            _azureCalling.JoinTeamsMeeting(teamsLink);
        }

        public void LeaveMeeting()
        {
            _azureCalling.LeaveMeeting();  
        }

        #region private methods
        private void Init()
        {
            if (userToken == null)
            {
                Debug.LogError("User token is null, please assign one");
                return;
            }

            _azureCalling.Init(userToken,userName);
        }
        #endregion
    }
}
