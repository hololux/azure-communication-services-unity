using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace Hololux.Acs.Samples
{
    public abstract class AzureCommunicationSample : MonoBehaviour
    {
        #region serialized feilds
        [SerializeField] private string userName;
        [SerializeField] private string userToken;
        #endregion

        #region private feilds
        protected IAzureCommunication AzureCommunication;
        #endregion

        #region unity methods
        private void Start()
        {
            #if PLATFORM_ANDROID

            List<string> permissions = new List<string>();
            
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                permissions.Add(Permission.Microphone);
            }
            
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                permissions.Add(Permission.Camera);
            }
            
            Permission.RequestUserPermissions(permissions.ToArray());
            #endif

            AzureCommunication = AcsFactory.GetCommunicationInstance();
            Init();
        }
        
        private void Update()
        {
            transform.Rotate(10.0f *Time.deltaTime, 10.0f*Time.deltaTime, -7.0f*Time.deltaTime);
        }
        #endregion

        #region public methods
        public void LeaveMeeting()
        {
            AzureCommunication.LeaveCall();  
        }

        public void Mute(bool mute)
        {
            Debug.Log(mute);
            
            if (mute)
            {
                AzureCommunication.Mute();
            }
            else
            {
                AzureCommunication.Unmute();
            }
        }
        #endregion

        #region private methods
        private void Init()
        {
            if (userToken == null)
            {
                Debug.LogError("User token is null, please assign one");
                return;
            }

            AzureCommunication.Init(userToken,userName);
        }
        #endregion
    }
}
