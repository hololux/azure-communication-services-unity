# azure-communication-services-unity

Unity Plug-in for Azure Communication Services (ACS)

Supported Platforms: Android, UWP

Not supported on Editor

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
    Permission.RequestUserPermissions({Permission.Microphone,Permission.Camera});
    #endif
    
    _azureCommunication = AcsFactory.GetCommunicationInstance();         
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
    _azureCommunication.Init(userToken,userName);
}
 ```

#### Teams calling
In order to make a Teams call, call the method _azureCommunication.JoinTeamsMeeting(teamsLink)
with valid Team meeting link.
 
```
[SerializeField] private string teamsLink;

public void JoinTeamsMeeting()
{
  _azureCommunication.JoinTeamsMeeting(teamsLink);         
}
 ```
 
#### Group calling
In order to make a group call, call the method _azureCommunication.JoinGroupCall(groupGuid)
with a GUID. 
 
```
[SerializeField] private string groupGuid;

public void JoinGroupCall()
{ 
  _azureCommunication.JoinGroupCall(groupGuid);         
}
 ```
 
To leave the call, call the method _azureCommunication.LeaveCall()
 
```
public void LeaveCall()
{
  _azureCommunication.LeaveCall();  
}
 ```
 
#### Sending video frames on Android devices
By default HoloLens sends mixed reality capture frames, for android you have to manualy send the frames.

To send video frames from android devices, call the method _azureCommunication.SendFrame(byetes[] bytes). 
Please have look into AcsAndroidFrameSender.cs implementations.

 ```
_azureCommunication.SendFrame(imageBytes);
 ```
 
 ## Permissions
 
 ### Android
 
 Following persmitions need to be included inside the android manifest. Currenly it is included in the plugins manifest file.
 ```
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    <uses-permission android:name="android.permission.RECORD_AUDIO" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.READ_PHONE_STATE" />
 ```
 Also, you need to include the following line. 
 
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
 
  ## Buiding Project
  
  ### Android
  
  Android manifest must be linked in the player settings and chosse IL2CPP(ARM64) as scripring background.
  
  ### Uwp
  
  Build the project as VisualStudio project, and for HoloLens choose ARM64 architecture. For standalone UWP app, please choose x64.
   
  ## Samples
  Package contains sample scenes under Assets\Acs\Samples folder. Please use this for checking the functinalities.
  
  
 
   
  

 
 
