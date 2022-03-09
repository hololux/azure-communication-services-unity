# azure-communication-services-unity

Unity Plug-in for Azure Communication Services (ACS)

Supported Platforms: Android, UWP

Minimum Unity Version: 2020.3.20f1(not tested on lower versions)


## Usage

Please download the latest release from 
https://github.com/hololux/azure-communication-services-unity/releases
or checkout this repository

Initialize the plugin instance by calling AcsFactory.GetCommunicationInstance();

```
private IAzureCommunication _azureCommunication;
  
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
```

After getting the calling inistance, initialize the the instance with user acces token
by calling  _azureCommunication.Init(userToken,userName);

To generate userToken quickly, please use azure communication resource.
https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/identity/quick-create-identity

Alternatively you can achieve this using 'CommunicationIdentityClient' to generate token from the code
https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/access-tokens?pivots=programming-language-csharp

```
[SerializeField] private string userName;
[SerializeField] private string userToken;

private void Init()
{
   if (userToken == null)
   {
      Debug.LogError("User token is null, please assign one");
      return;
   }

    _azureCommunication.Init(userToken,userName);
}
 ```
 
In order to make a group call, call the method _azureCommunication.JoinTeamsMeeting(teamsLink)
with valid Team meeting link.
 
```
[SerializeField] private string teamsLink;

public void JoinTeamsMeeting()
{
    if (teamsLink == null)
    {
      Debug.LogError("teamsLink is null, please assign one");
      return;
    }  
    
  _azureCommunication.JoinTeamsMeeting(teamsLink);         
}
 ```
 
To leave the call, call the method _azureCommunication.LeaveMeeting()
 
```
public void LeaveMeeting()
{
  _azureCommunication.LeaveMeeting();  
}
 ```
 ## Permissions
 
 ### Android
 
 following persmitions need to be included inside the android manifest
 ```
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    <uses-permission android:name="android.permission.RECORD_AUDIO" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.READ_PHONE_STATE" />
 ```
 Also, you need to include the following line. Currenly it is included in the plugins manifest file. 
 
 ```
 <uses-library android:name="org.apache.http.legacy" android:required="false"/>
 
 ```
 Please refer this page for more details
 https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/voice-video-calling/getting-started-with-calling?pivots=platform-android
 
  ### Uwp
  
  Please enable the following capabilities in the player settings
  
  1. Internet (Client & Server) 
  2. Microphone
 
  https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/voice-video-calling/getting-started-with-calling?pivots=platform-windows
 
 
 
 
