#if WINDOWS_UWP
using Azure.Communication;
using Azure.Communication.Calling;
using System;
using System.Collections.Generic;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace Hololux.Acs.Plugins.WSA
{
    internal class AzureCommunicationUWPPlugin 
    {
        #if WINDOWS_UWP
        private CallClient _callClient;
        private CallAgent _callAgent;
        private Call _call;
        private DeviceManager _deviceManager;
        private LocalVideoStream[] _localVideoStream;
        #endif

        internal async Task Init(string token, string userName)
        {
            #if WINDOWS_UWP
            await InitCallAgent(token, userName);
            #endif
        }
        
        internal async Task JoinAsync(string teamsMeetingUrl)
        {
            #if WINDOWS_UWP
            GetCameraDevice();

            JoinCallOptions joinCallOptions = new JoinCallOptions();
            joinCallOptions.VideoOptions = new VideoOptions(_localVideoStream);

            var teamsMeetingLinkLocator = new TeamsMeetingLinkLocator(teamsMeetingUrl);
            _call = await _callAgent.JoinAsync(teamsMeetingLinkLocator, joinCallOptions);
            #endif
        }
        
        internal async Task LeaveMeeting()
        {
            #if WINDOWS_UWP
            await _call.HangUpAsync(new HangUpOptions());
            #endif
        }
        
        internal async void Mute()
        {
            #if WINDOWS_UWP
            await _call.Mute();
            #endif
        }
        
        internal async void UnMute()
        {
            #if WINDOWS_UWP
            await _call.Unmute();
            #endif
        }
        
        #if WINDOWS_UWP
        private async Task InitCallAgent(string userToken, string userName)
        {
            var tokenCredential = new Azure.WinRT.Communication.CommunicationTokenCredential(userToken);

            _callClient = new CallClient();
            _deviceManager = await _callClient.GetDeviceManager();
            _localVideoStream = new LocalVideoStream[1];

            var callAgentOptions = new CallAgentOptions()
            {
                DisplayName = userName
            };
            
            _callAgent = await _callClient.CreateCallAgent(tokenCredential, callAgentOptions);
            _callAgent.OnCallsUpdated += OnCallAgentCallsUpdated;
            _callAgent.OnIncomingCall += OnCallAgentIncomingCall;
        }

        private async void OnCallAgentIncomingCall(object sender, IncomingCall incomingCall)
        {
            GetCameraDevice();

            AcceptCallOptions acceptCallOptions = new AcceptCallOptions();
            acceptCallOptions.VideoOptions = new VideoOptions(_localVideoStream);
            _call = await incomingCall.AcceptAsync(acceptCallOptions);
        }

        private async void OnCallAgentCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
            foreach (var call in args.AddedCalls)
            {
                foreach (var remoteParticipant in call.RemoteParticipants)
                {
                    await AddVideoStreams(remoteParticipant.VideoStreams);
                    remoteParticipant.OnVideoStreamsUpdated += async (s, a) => await AddVideoStreams(a.AddedRemoteVideoStreams);
                }
                call.OnRemoteParticipantsUpdated += Call_OnRemoteParticipantsUpdated; ;
                call.OnStateChanged += Call_OnStateChanged;
            }
        }

        private async void Call_OnStateChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (((Call)sender).State)
            {
                case CallState.Disconnected:
                    break;
                default:
                    break;
            }
        }

        private async void Call_OnRemoteParticipantsUpdated(object sender, ParticipantsUpdatedEventArgs args)
        {
            foreach (var remoteParticipant in args.AddedParticipants)
            {
                await AddVideoStreams(remoteParticipant.VideoStreams);
                remoteParticipant.OnVideoStreamsUpdated += async (s, a) => await AddVideoStreams(a.AddedRemoteVideoStreams);
            }
        }

        private async Task AddVideoStreams(IReadOnlyList<RemoteVideoStream> streams)
        {
            foreach (var remoteVideoStream in streams)
            {
                var remoteUri = await remoteVideoStream.CreateBindingAsync();
                remoteVideoStream.Start();
            }
        }

        private async void GetCameraDevice()
        {
            if (_deviceManager.Cameras.Count > 0)
            {
                var videoDeviceInfo = _deviceManager.Cameras[0];
                _localVideoStream[0] = new LocalVideoStream(videoDeviceInfo);
                var localUri = await _localVideoStream[0].CreateBindingAsync();
            }
        }
        #endif
    }
}
