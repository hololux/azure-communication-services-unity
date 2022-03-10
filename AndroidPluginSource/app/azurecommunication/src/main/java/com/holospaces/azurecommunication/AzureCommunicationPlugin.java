package com.holospaces.azurecommunication;
import android.content.Context;
import android.util.Log;
import android.app.Activity;

import com.azure.android.communication.calling.Call;
import com.azure.android.communication.calling.CallAgent;
import com.azure.android.communication.calling.CallClient;
import com.azure.android.communication.calling.HangUpOptions;
import com.azure.android.communication.calling.JoinCallOptions;
import com.azure.android.communication.common.CommunicationTokenCredential;
import com.azure.android.communication.calling.TeamsMeetingLinkLocator;

import java.util.concurrent.ExecutionException;

public class AzureCommunicationPlugin
{
    public static final AzureCommunicationPlugin Instance = new AzureCommunicationPlugin();
    public static final String LOGTAG = "Plugin";

    private CallAgent agent;
    private Call call;
    private Activity activity;

    public AzureCommunicationPlugin()
    {
    }

    public static AzureCommunicationPlugin getInstance()
    {
        return Instance;
    }

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

        JoinCallOptions options = new JoinCallOptions();
        TeamsMeetingLinkLocator teamsMeetingLinkLocator = new TeamsMeetingLinkLocator(meetingLink);

        if(agent == null)
        {
            Log.i(LOGTAG, "agent is null");
        }

        Log.i(LOGTAG, "agent join");
        call = agent.join(
                getApplicationContext(),
                teamsMeetingLinkLocator,
                options);
    }

    public void leaveMeeting()
    {
        try
        {
            call.hangUp(new HangUpOptions()).get();
        }
        catch (ExecutionException | InterruptedException e)
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
            agent = new CallClient().createCallAgent(getApplicationContext(), credential).get();
        }
        catch (Exception ex)
        {
            Log.i(LOGTAG, "Failed to create call agent.");
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
