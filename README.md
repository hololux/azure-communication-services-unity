# azure-communication-services-unity

Unity Plug-in for Azure Communication Services (ACS)

Supported Platforms: Android, UWP

Minimum Unity Version: 2020.3.20f1(not tested on lower versions)


## usage

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

```
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
 
