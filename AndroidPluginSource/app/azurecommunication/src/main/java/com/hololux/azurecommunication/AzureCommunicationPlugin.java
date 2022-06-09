package com.hololux.azurecommunication;

import android.content.Context;
import android.util.Log;
import android.app.Activity;
import com.azure.android.communication.calling.Call;
import com.azure.android.communication.calling.CallAgent;
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

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;
import java.util.Random;
import java.util.UUID;
import java.util.concurrent.ExecutionException;

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
    private static final String LOGTAG = "Unity";

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
            Log.i(LOGTAG, "Please enter Teams meeting link");
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

    public void sendFrame(byte[] input)
    {
        currentBuffer = input;
        newFrameArrived = true;
    }

    public void joinGroupCall(String groupGuid)
    {
        if (groupGuid.isEmpty())
        {
            Log.i(LOGTAG, "Please enter group Guid");
            return;
        }

        UUID uuid = UUID.fromString(groupGuid);
        JoinCallOptions options = new JoinCallOptions();
        GroupCallLocator groupCallLocator = new GroupCallLocator(uuid);

        if(callAgent == null)
        {
            Log.i(LOGTAG, "agent is null");
        }

        Log.i(LOGTAG, "agent joinGroupCall");
        call = callAgent.join(
                getApplicationContext(),
                groupCallLocator,
                options);
    }

    public void leaveMeeting()
    {
        try
        {
            call.hangUp(new HangUpOptions());
        }
        catch (Exception e)
        {
            Log.e(LOGTAG, "error leaving meeting");
        }
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
            CommunicationTokenCredential credential = new CommunicationTokenCredential(userToken);
            callAgent = new CallClient().createCallAgent(getApplicationContext(), credential).get();

            deviceManager = new CallClient().getDeviceManager(getApplicationContext()).get();
        }
        catch (Exception ex)
        {
            Log.i(LOGTAG, "Failed to create call agent.");
        }
    }

    private void startCall(String meetingLink) throws Exception
    {
        // See section on starting the call
        createOutboundVirtualVideoDevice();

        frameSenderThread = new Thread(new Runnable()
        {
            @Override
            public void run()
            {
                try {
                    java.nio.ByteBuffer plane1 = null;
                    Random rand = new Random();

                    while (outboundVirtualVideoDevice != null)
                    {
                        while (mediaFrameSender != null)
                        {
                            if (mediaFrameSender.getMediaFrameKind() == MediaFrameKind.VIDEO_SOFTWARE && newFrameArrived)
                            {
                                SoftwareBasedVideoFrame sender = (SoftwareBasedVideoFrame) mediaFrameSender;
                                VideoFormat videoFormat = sender.getVideoFormat();

                                // Gets the timestamp for when the video frame has been created.
                                // This allows better synchronization with audio.
                                int timeStamp = sender.getTimestamp();

                                // Adjusts frame dimensions to the video format that network conditions can manage.
                                if (plane1 == null || videoFormat.getStride1() * videoFormat.getHeight() != plane1.capacity()) {
                                    plane1 = ByteBuffer.allocateDirect(videoFormat.getStride1() * videoFormat.getHeight());
                                    plane1.order(ByteOrder.nativeOrder());
                                }

                                plane1.put(currentBuffer);

/*
                                // Generates random gray scaled bands as video frame.
                                int bandsCount = rand.nextInt(15) + 1;
                                int bandBegin = 0;
                                int bandThickness = videoFormat.getHeight() * videoFormat.getStride1() / bandsCount;

                                for (int i = 0; i < bandsCount; ++i) {
                                    byte greyValue = (byte) rand.nextInt(254);
                                    java.util.Arrays.fill(plane1.array(), bandBegin, bandBegin + bandThickness, greyValue);
                                    bandBegin += bandThickness;
                                }

 */

                                // Sends video frame to the other participants in the call.
                                FrameConfirmation fr = sender.sendFrame(plane1, timeStamp).get();
                                plane1.clear();

                                newFrameArrived = false;
                                // Waits before generating the next video frame.
                                // Video format defines how many frames per second app must generate.
                                //Thread.sleep((long) (1000.0f / videoFormat.getFramesPerSecond()));
                            }
                        }

                        // Virtual camera hasn't been created yet.
                        // Let's wait a little bit before checking again.
                        // This is for demo only purpose.
                        // Please use a better synchronization mechanism.
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
        JoinCallOptions joinCallOptions = new JoinCallOptions();
        try
        {
            Thread.sleep(2000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        ensureLocalVideoStreamWithVirtualCamera();

        joinCallOptions.setVideoOptions(new VideoOptions(new LocalVideoStream[]{
                virtualVideoStream
        }));

        TeamsMeetingLinkLocator teamsMeetingLinkLocator = new TeamsMeetingLinkLocator(meetingLink);

        callAgent.join(
                getApplicationContext(),
                teamsMeetingLinkLocator,
                joinCallOptions);
    }

    private void InitCall(String meetingLink) throws Exception
    {
        createOutboundVirtualVideoDevice();
        JoinCallOptions joinCallOptions = new JoinCallOptions();

        ensureLocalVideoStreamWithVirtualCamera();

        joinCallOptions.setVideoOptions(new VideoOptions(new LocalVideoStream[]{
                virtualVideoStream
        }));

        TeamsMeetingLinkLocator teamsMeetingLinkLocator = new TeamsMeetingLinkLocator(meetingLink);

        callAgent.join(
                getApplicationContext(),
                teamsMeetingLinkLocator,
                joinCallOptions);

    }

    private void createOutboundVirtualVideoDevice() throws Exception
    {
        VirtualDeviceIdentification deviceId = new VirtualDeviceIdentification();
        deviceId.setId("QuickStartVirtualVideoDevice");
        deviceId.setName("My First Virtual Video Device");

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
        for (VideoDeviceInfo videoDeviceInfo : deviceManager.getCameras())
        {
            String deviceId = videoDeviceInfo.getId();
            if (deviceId.equalsIgnoreCase("QuickStartVirtualVideoDevice"))
            {
                virtualVideoStream = new LocalVideoStream(videoDeviceInfo, getApplicationContext());
            }
        }
    }

    private Context getApplicationContext()
    {
        if(activity == null)
        {
            Log.i(LOGTAG, "activity is null");
            return null;
        }

        return activity.getApplicationContext();
    }
}
