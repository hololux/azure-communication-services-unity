package com.hololux.azurecommunication;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;
import java.util.UUID;
import java.util.concurrent.ExecutionException;

import android.content.Context;
import android.util.Log;
import android.app.Activity;
import com.azure.android.communication.calling.Call;
import com.azure.android.communication.calling.CallAgent;
import com.azure.android.communication.calling.CallAgentOptions;
import com.azure.android.communication.calling.CallClient;
import com.azure.android.communication.calling.DeviceManager;
import com.azure.android.communication.calling.FrameConfirmation;
import com.azure.android.communication.calling.GroupCallLocator;
import com.azure.android.communication.calling.HangUpOptions;
import com.azure.android.communication.calling.JoinCallOptions;
import com.azure.android.communication.calling.LocalVideoStream;
import com.azure.android.communication.calling.SoftwareBasedVideoFrame;
import com.azure.android.communication.calling.VideoDeviceInfo;
import com.azure.android.communication.calling.VideoFormat;
import com.azure.android.communication.common.CommunicationTokenCredential;
import com.azure.android.communication.calling.TeamsMeetingLinkLocator;
import com.azure.android.communication.calling.MediaFrameKind;
import com.azure.android.communication.calling.PixelFormat;
import com.azure.android.communication.calling.VideoOptions;
import com.azure.android.communication.calling.VirtualDeviceIdentification;
import com.azure.android.communication.calling.MediaFrameSender;
import com.azure.android.communication.calling.OutboundVirtualVideoDevice;
import com.azure.android.communication.calling.OutboundVirtualVideoDeviceOptions;
import com.azure.android.communication.calling.VirtualDeviceRunningState;

public class AzureCommunicationPlugin
{
    private CallAgent callAgent;
    private Call call;
    private Activity activity;
    private DeviceManager deviceManager;
    private OutboundVirtualVideoDeviceOptions options;
    private MediaFrameSender mediaFrameSender;
    private OutboundVirtualVideoDevice outboundVirtualVideoDevice;
    private LocalVideoStream virtualVideoStream;
    private Thread frameSenderThread;

    private byte[] currentBuffer = null;
    private boolean newFrameArrived;
    private java.nio.ByteBuffer plane1 = null;
    private static final String LogTag = "Unity";

    public AzureCommunicationPlugin(){}

    public void init(Activity unityActivity, String userTokenToken, String userName)
    {
        activity = unityActivity;
        createAgent(userTokenToken,userName);
    }

    public void joinTeamsMeeting(String meetingLink)
    {
        if (meetingLink.isEmpty())
        {
            Log.i(LogTag, "Please enter Teams meeting link");
            return;
        }

        try
        {
            startCall(meetingLink);

        } catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public void joinGroupCall(String groupGuid)
    {
        if (groupGuid.isEmpty())
        {
            Log.i(LogTag, "Please enter group Guid");
            return;
        }

        UUID uuid = UUID.fromString(groupGuid);
        JoinCallOptions options = new JoinCallOptions();
        GroupCallLocator groupCallLocator = new GroupCallLocator(uuid);

        if(callAgent == null)
        {
            Log.i(LogTag, "agent is null");
        }

        Log.i(LogTag, "agent joinGroupCall");
        call = callAgent.join(
                getApplicationContext(),
                groupCallLocator,
                options);
    }

    public void leaveCall()
    {
        try
        {
            call.hangUp(new HangUpOptions());
        }
        catch (Exception e)
        {
            Log.e(LogTag, "error leaving meeting");
        }
    }

    public void sendFrame(byte[] input)
    {
        currentBuffer = input;
        newFrameArrived = true;
    }

    public void mute()
    {
        call.mute(getApplicationContext());
    }

    public void unMute()
    {
        call.unmute(getApplicationContext());
    }

    private void createAgent(String userToken, String userName)
    {
        try
        {
            // fix for link error CallAgentOptions callAgentOptions status is not found
            // this loads the native library :)
            CallClient nativeLibraryLoaded = new CallClient();

            CommunicationTokenCredential credential = new CommunicationTokenCredential(userToken);
            CallAgentOptions callAgentOptions = new CallAgentOptions();
            callAgentOptions.setDisplayName(userName);

            callAgent = new CallClient().createCallAgent(getApplicationContext(), credential, callAgentOptions).get();
            deviceManager = new CallClient().getDeviceManager(getApplicationContext()).get();
        }
        catch (Exception ex)
        {
            Log.i(LogTag, "Failed to create call agent.");
        }
    }

    private void startCall(String meetingLink) throws Exception
    {
        if(call != null)
        {
            joinTeamsCallInternal(meetingLink);
            return;
        }

        createOutboundVirtualVideoDevice();
        frameSenderThread = new Thread(new Runnable()
        {
            @Override
            public void run()
            {
                try {
                    java.nio.ByteBuffer sendBuffer = null;

                    while (outboundVirtualVideoDevice != null)
                    {
                        while (mediaFrameSender != null)
                        {
                            if (mediaFrameSender.getMediaFrameKind() == MediaFrameKind.VIDEO_SOFTWARE && newFrameArrived) {
                                SoftwareBasedVideoFrame sender = (SoftwareBasedVideoFrame) mediaFrameSender;
                                VideoFormat videoFormat = sender.getVideoFormat();

                                int timeStamp = sender.getTimestamp();

                                if (sendBuffer == null || videoFormat.getStride1() * videoFormat.getHeight() != sendBuffer.capacity()) {
                                    sendBuffer = ByteBuffer.allocateDirect(videoFormat.getStride1() * videoFormat.getHeight());
                                    sendBuffer.order(ByteOrder.nativeOrder());
                                }

                                sendBuffer.put(currentBuffer);

                                // Sends video frame to the other participants in the call.
                                FrameConfirmation fr = sender.sendFrame(sendBuffer, timeStamp).get();
                                sendBuffer.clear();

                                newFrameArrived = false;
                            }
                        }
                    }
                } catch (InterruptedException e)
                {
                    e.printStackTrace();
                } catch (ExecutionException e)
                {
                    e.printStackTrace();
                }
            }
        });

        frameSenderThread.start();

        while (virtualVideoStream == null)
        {
           ensureLocalVideoStreamWithVirtualCamera();
        }

        joinTeamsCallInternal(meetingLink);
    }

    private void joinTeamsCallInternal(String meetingLink)
    {
        JoinCallOptions joinCallOptions = new JoinCallOptions();
        joinCallOptions.setVideoOptions(new VideoOptions(new LocalVideoStream[]{
                virtualVideoStream
        }));

        TeamsMeetingLinkLocator teamsMeetingLinkLocator = new TeamsMeetingLinkLocator(meetingLink);

        call = callAgent.join(
                getApplicationContext(),
                teamsMeetingLinkLocator,
                joinCallOptions);
    }

    private void createOutboundVirtualVideoDevice() throws Exception
    {
        VirtualDeviceIdentification deviceId = new VirtualDeviceIdentification();
        deviceId.setId("VirtualDevice");
        deviceId.setName("Virtual Video Device");

        ArrayList<VideoFormat> videoFormats = new ArrayList<VideoFormat>();

        VideoFormat format = new VideoFormat();
        format.setWidth(1280);
        format.setHeight(720);
        format.setPixelFormat(PixelFormat.RGBA);
        format.setMediaFrameKind(MediaFrameKind.VIDEO_SOFTWARE);
        format.setFramesPerSecond(30);
        format.setStride1(1280 * 4);
        videoFormats.add(format);

        options = new OutboundVirtualVideoDeviceOptions();
        options.setDeviceIdentification(deviceId);
        options.setVideoFormats(videoFormats);

        options.addOnFlowChangedListener(virtualDeviceFlowControlArgs ->
        {
            if (virtualDeviceFlowControlArgs.getMediaFrameSender().getRunningState() == VirtualDeviceRunningState.STARTED)
            {
                mediaFrameSender = virtualDeviceFlowControlArgs.getMediaFrameSender();
            } else
            {
                mediaFrameSender = null;
            }
        });

        outboundVirtualVideoDevice = deviceManager.createOutboundVirtualVideoDevice(options).get();
    }

    private void ensureLocalVideoStreamWithVirtualCamera()
    {
        /*
         This can be used to get the front cam if it is supported
         List<VideoDeviceInfo> videoDeviceInfo =  deviceManager.getCameras();
         VideoDeviceInfo frontCam = videoDeviceInfo.get(0);
         */

        for (VideoDeviceInfo videoDeviceInfo : deviceManager.getCameras())
        {
            String deviceId = videoDeviceInfo.getId();

            if (deviceId.equalsIgnoreCase("VirtualDevice"))
            {
                virtualVideoStream = new LocalVideoStream(videoDeviceInfo, getApplicationContext());
            }
        }
    }

    private Context getApplicationContext()
    {
        if(activity == null)
        {
            Log.i(LogTag, "activity is null");
            return null;
        }

        return activity.getApplicationContext();
    }
}
