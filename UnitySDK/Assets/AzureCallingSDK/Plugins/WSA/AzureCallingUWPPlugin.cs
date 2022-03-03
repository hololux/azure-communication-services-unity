#if WINDOWS_UWP
using Azure.Communication;
using Azure.Communication.Calling;
using System;
using System.Collections.Generic;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace AzureCallingSDK.Plugins.WSA
{
    internal class AzureCallingUWPPlugin 
    {
        #if WINDOWS_UWP
        private CallClient callClient;
        private CallAgent callAgent;
        private Call call;
        private DeviceManager deviceManager;
        private LocalVideoStream[] localVideoStream;
        #endif

        internal async Task Init(string token, string user)
        {
            #if WINDOWS_UWP
            await InitCallAgent(token, user);
            #endif
        }
        
        internal async Task JoinAsync(string teamsMeetingUrl)
        {
            #if WINDOWS_UWP
            GetCameraDevice();

            JoinCallOptions joinCallOptions = new JoinCallOptions();
            joinCallOptions.VideoOptions = new VideoOptions(localVideoStream);

            var teamsMeetingLinkLocator = new TeamsMeetingLinkLocator(teamsMeetingUrl);
            call = await callAgent.JoinAsync(teamsMeetingLinkLocator, joinCallOptions);
            #endif
        }
        
        internal async Task LeaveMeeting()
        {
            #if WINDOWS_UWP
            await call.HangUpAsync(new HangUpOptions());
            #endif
        }

        #if WINDOWS_UWP
        private async Task InitCallAgent(string user_token_, string user_name)
        {
            var token_credential = new Azure.WinRT.Communication.CommunicationTokenCredential(user_token_);

            callClient = new CallClient();
            deviceManager = await callClient.GetDeviceManager();
            localVideoStream = new LocalVideoStream[1];

            var callAgentOptions = new CallAgentOptions()
            {
                DisplayName = user_name
            };
            callAgent = await callClient.CreateCallAgent(token_credential, callAgentOptions);
            callAgent.OnCallsUpdated += CallAgent_OnCallsUpdated;
            callAgent.OnIncomingCall += CallAgent_OnIncomingCall;
        }

        private async void CallAgent_OnIncomingCall(object sender, IncomingCall incomingCall)
        {
            GetCameraDevice();

            AcceptCallOptions acceptCallOptions = new AcceptCallOptions();
            acceptCallOptions.VideoOptions = new VideoOptions(localVideoStream);
            call = await incomingCall.AcceptAsync(acceptCallOptions);
        }

        private async void CallAgent_OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
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
            if (deviceManager.Cameras.Count > 0)
            {
                var videoDeviceInfo = deviceManager.Cameras[0];
                localVideoStream[0] = new LocalVideoStream(videoDeviceInfo);
                var localUri = await localVideoStream[0].CreateBindingAsync();
            }
        }
        #endif
    }
}
