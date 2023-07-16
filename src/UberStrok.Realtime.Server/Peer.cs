using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using log4net;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime;
using UberStrok.Realtime.Server;
using UberStrok.WebServices.Client;

public abstract class Peer : ClientPeer
{
    private enum HeartbeatState
    {
        Ok,
        Waiting,
        Failed
    }

    private static readonly Random _random = new Random();

    private string _heartbeat;

    private DateTime _heartbeatNextTime;

    private DateTime _heartbeatExpireTime;

    private HeartbeatState _heartbeatState;

    public bool IsMac { get; set; }

    protected ILog Log { get; }

    protected UberstrikeUserView UserView { get; set; }

    public int HeartbeatTimeout { get; set; }

    public int HeartbeatInterval { get; set; }

    public bool HasError { get; protected set; }

    public string AuthToken { get; protected set; }

    public OperationHandlerCollection Handlers { get; }

    protected PeerConfiguration Configuration { get; }

    public Peer(InitRequest request)
        : base(request)
    {
        if (!(request.UserData is PeerConfiguration peerConfiguration))
        {
            throw new ArgumentException("InitRequest.UserData was not a PeerConfiguration instance", "request");
        }
        if (request.ApplicationId != RealtimeVersion.Current)
        {
            throw new ArgumentException("InitRequest had an invalid application ID", "request");
        }
        Configuration = peerConfiguration;
        HeartbeatTimeout = peerConfiguration.HeartbeatTimeout;
        HeartbeatInterval = peerConfiguration.HeartbeatInterval;
        Log = LogManager.GetLogger(((object)this).GetType().Name);
        Handlers = new OperationHandlerCollection();
        if (Configuration.JunkHashes.Count > 0)
        {
            _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
        }
    }

    public virtual void Tick()
    {
        //IL_0070: Unknown result type (might be due to invalid IL or missing references)
        //IL_0077: Invalid comparison between Unknown and I4
        switch (_heartbeatState)
        {
            case HeartbeatState.Ok:
                if (Configuration.JunkHashes.Count > 0 && DateTime.UtcNow >= _heartbeatNextTime)
                {
                    Heartbeat();
                }
                break;
            case HeartbeatState.Waiting:
                if (DateTime.UtcNow >= _heartbeatExpireTime)
                {
                    ((PeerBase)this).Disconnect();
                }
                break;
            case HeartbeatState.Failed:
                if ((int)GetUser(retrieve: true).CmuneMemberView.PublicProfile.AccessLevel != 10)
                {
                    SendError();
                }
                break;
        }
    }

    public bool Authenticate(string authToken, string magicHash)
    {
        //IL_0078: Unknown result type (might be due to invalid IL or missing references)
        //IL_007f: Invalid comparison between Unknown and I4
        AuthToken = authToken ?? throw new ArgumentNullException("authToken");
        if (magicHash == null)
        {
            throw new ArgumentNullException("magicHash");
        }
        Log.Info((object)$"Authenticating {authToken}:{magicHash} at {((PeerBase)this).RemoteIP}:{((PeerBase)this).RemotePort}");
        UberstrikeUserView user = GetUser(retrieve: true);
        OnAuthenticate(user);
        bool flag = (int)user.CmuneMemberView.PublicProfile.AccessLevel != 10;
        if (Configuration.CompositeHashes.Count > 0 && flag)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(authToken);
            for (int i = 0; i < Configuration.CompositeHashes.Count; i++)
            {
                string text = HashBytes(Configuration.CompositeHashes[i], bytes);
                if (text == magicHash)
                {
                    Log.Debug((object)("MagicHash: " + text + " == " + magicHash));
                    return true;
                }
                Log.Error((object)("MagicHash: " + text + " != " + magicHash));
            }
            return false;
        }
        return true;
    }

    public void Heartbeat()
    {
        if (Configuration.JunkHashes.Count != 0)
        {
            _heartbeat = GenerateHeartbeat();
            _heartbeatExpireTime = DateTime.UtcNow.AddSeconds(HeartbeatTimeout);
            _heartbeatState = HeartbeatState.Waiting;
            SendHeartbeat(_heartbeat);
        }
    }

    public bool HeartbeatCheck(string responseHash)
    {
        if (responseHash == null)
        {
            throw new ArgumentNullException("responseHash");
        }
        if (_heartbeat == null)
        {
            Log.Error((object)"Heartbeat was null while checking.");
            return false;
        }
        for (int i = 0; i < Configuration.JunkHashes.Count; i++)
        {
            byte[] a = Configuration.JunkHashes[i];
            byte[] bytes = Encoding.ASCII.GetBytes(_heartbeat);
            string text = HashBytes(a, bytes);
            if (text == responseHash)
            {
                _heartbeat = null;
                _heartbeatNextTime = DateTime.UtcNow.AddSeconds(HeartbeatInterval);
                _heartbeatState = HeartbeatState.Ok;
                return true;
            }
            Log.Error((object)("Heartbeat: " + text + " != " + responseHash));
        }
        _heartbeat = null;
        _heartbeatState = HeartbeatState.Failed;
        return false;
    }

    public abstract void SendHeartbeat(string hash);

    public virtual void SendError(string message = "An error occured that forced UberStrike to halt.")
    {
        HasError = true;
    }

    public int Ban()
    {
        if (UserView == null)
        {
            return 1;
        }
        return new ModerationWebServiceClient(Configuration.WebServices).Ban(Configuration.WebServicesAuth, UserView.CmuneMemberView.PublicProfile.Cmid);
    }

    protected virtual void OnAuthenticate(UberstrikeUserView userView)
    {
    }

    protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
    {
        foreach (OperationHandler handler in Handlers)
        {
            try
            {
                handler.OnDisconnect(this, reasonCode, reasonDetail);
            }
            catch (Exception ex)
            {
                Log.Error((object)("Error while handling disconnection of peer on " + handler.GetType().Name), ex);
            }
        }
    }

    protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
    {
        if (operationRequest.Parameters.Count < 1)
        {
            Log.Warn((object)$"Client at {((PeerBase)this).RemoteIPAddress}:{((PeerBase)this).RemotePort} did not send enough parameters. Disconnecting.");
            ((PeerBase)this).Disconnect();
            return;
        }
        byte b = operationRequest.Parameters.Keys.First();
        OperationHandler operationHandler = Handlers[b];
        if (operationHandler == null)
        {
            Log.Warn((object)$"Client {((PeerBase)this).RemoteIPAddress}:{((PeerBase)this).RemotePort} sent an operation request on a handler which is not registered.");
            return;
        }
        if (!(operationRequest.Parameters[b] is byte[] buffer))
        {
            Log.Warn((object)$"Client {((PeerBase)this).RemoteIPAddress} sent an operation request but the data type was not byte[]. Disconnecting.");
            ((PeerBase)this).Disconnect();
            return;
        }
        using (MemoryStream bytes = new MemoryStream(buffer))
        {
            try
            {
                operationHandler.OnOperationRequest(this, operationRequest.OperationCode, bytes);
            }
            catch (Exception ex)
            {
                Log.Error((object)$"Error while handling request on {operationHandler.GetType().Name} -> :{operationRequest.OperationCode}", ex);
            }
        }
    }

    private static string HashBytes(byte[] a, byte[] b)
    {
        byte[] array = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, array, 0, a.Length);
        Buffer.BlockCopy(b, 0, array, a.Length, b.Length);
        byte[] bytes = null;
        using (SHA256 sHA = SHA256.Create())
        {
            bytes = sHA.ComputeHash(array);
        }
        return BytesToHexString(bytes);
    }

    private static string BytesToHexString(byte[] bytes)
    {
        StringBuilder stringBuilder = new StringBuilder(64);
        for (int i = 0; i < bytes.Length; i++)
        {
            stringBuilder.Append(bytes[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }

    private static string GenerateHeartbeat()
    {
        byte[] array = new byte[32];
        _random.NextBytes(array);
        return BytesToHexString(array);
    }

    public UberstrikeUserView GetUser(bool retrieve)
    {
        if (retrieve || UserView == null)
        {
            Log.Debug((object)("Retrieving User from " + Configuration.WebServices));
            UserView = new UserWebServiceClient(Configuration.WebServices).GetMember(AuthToken);
        }
        return UserView;
    }

    public void SendEndGame(StatsCollectionView totaStatsCollectionView, StatsCollectionView bestStatsCollectionView)
    {
        new UserWebServiceClient(Configuration.WebServices).SendEndGame(AuthToken, totaStatsCollectionView, bestStatsCollectionView);
    }

    public MemberOperationResult SetLoadout(string authToken, LoadoutView loadout)
    {
        try
        {
            return new UserWebServiceClient(Configuration.WebServices).SetLoadout(Configuration.WebServicesAuth, authToken, loadout);
        }
        catch (Exception e)
        {
            Log.Error("Failed to setloadout", e);
        }
        return MemberOperationResult.InvalidHandle;
    }

    protected ApplicationConfigurationView GetAppConfig()
    {
        Log.Debug((object)("Retrieving AppConfig from " + Configuration.WebServices));
        return new UserWebServiceClient(Configuration.WebServices).GetAppConfig();
    }
}
