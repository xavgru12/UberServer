

// UberStrok.Realtime.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// UberStrok.Realtime.Server.Application
using System;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Util;
using Newtonsoft.Json;
using Photon.SocketServer;
using UberStrok.Realtime.Server;

public abstract class Application : ApplicationBase
{
    public static Application Instance => (Application)(object)ApplicationBase.Instance;

    protected ILog Log { get; }

    public ApplicationConfiguration Configuration { get; private set; }

    private PeerConfiguration PeerConfiguration { get; set; }

    protected Application()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        Log = LogManager.GetLogger(((object)this).GetType().Name);
    }

    protected abstract void OnSetup();

    protected abstract void OnTearDown();

    protected abstract Peer OnCreatePeer(InitRequest initRequest);

    private void SetupLog4net()
    {
        ((ContextPropertiesBase)GlobalContext.Properties)["Photon:ApplicationLogPath"] = Path.Combine(((ApplicationBase)this).ApplicationPath, "log");
        FileInfo fileInfo = new FileInfo(Path.Combine(((ApplicationBase)this).BinaryPath, "log4net.config"));
        if (fileInfo.Exists)
        {
            XmlConfigurator.ConfigureAndWatch(fileInfo);
        }
    }

    private void SetupConfigs()
    {
        string text = Path.Combine(((ApplicationBase)this).BinaryPath, "uberstrok.realtime.server.json");
        Log.Info((object)("Loading configuration at " + text));
        if (!File.Exists(text))
        {
            Configuration = ApplicationConfiguration.Default;
            Log.Info((object)"uberstrok.realtime.server.json not found, using default configuration.");
        }
        else
        {
            try
            {
                string text2 = File.ReadAllText(text);
                Configuration = JsonConvert.DeserializeObject<ApplicationConfiguration>(text2);
                Configuration.Check();
                Log.Info((object)"uberstrok.realtime.server.json loaded ->");
                Log.Info((object)$"\tCompositeHashes({Configuration.CompositeHashBytes.Count})");
                Log.Info((object)$"\tJunkHashes({Configuration.JunkHashBytes.Count})");
                Log.Info((object)$"\tHeartbeatTimeout = {Configuration.HeartbeatTimeout}");
                Log.Info((object)$"\tHeartbeatInterval = {Configuration.HeartbeatInterval}");
            }
            catch (Exception ex)
            {
                Log.Fatal((object)"Failed to load or parse uberstrok.realtime.server.json", ex);
                throw;
            }
        }
        PeerConfiguration = new PeerConfiguration(Configuration.WebServices, Configuration.WebServicesAuth, Configuration.ServerGameVersion, Configuration.HeartbeatTimeout, Configuration.HeartbeatInterval, Configuration.CompositeHashBytes, Configuration.JunkHashBytes);
    }

    protected sealed override void Setup()
    {
        SetupLog4net();
        SetupConfigs();
        OnSetup();
        Log.Info((object)("Setup " + ((object)this).GetType().Name + "... Complete"));
    }

    protected sealed override void TearDown()
    {
        OnTearDown();
        Log.Info((object)("TearDown " + ((object)this).GetType().Name + "... Complete"));
    }

    protected sealed override PeerBase CreatePeer(InitRequest initRequest)
    {
        Log.Info((object)$"Accepted new connection at {initRequest.RemoteIP}:{initRequest.RemotePort}.");
        initRequest.UserData = PeerConfiguration;
        return (PeerBase)(object)OnCreatePeer(initRequest);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = e.ExceptionObject as Exception;
        if (e.IsTerminating)
        {
            Log.Fatal((object)"Unhandled exception", ex);
        }
        else
        {
            Log.Error((object)"Unhandled exception", ex);
        }
    }
}
