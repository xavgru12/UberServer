// Decompiled with JetBrains decompiler
// Type: Webservices.Program
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Webservices.Helper;
using Webservices.Manager;

namespace Webservices
{
  internal class Program
  {
    public static readonly string ServiceBaseUrl = "http://localhost:5100/2.0";
    public static readonly string AssetBaseUrl = "http://www.uberkill.cc/assets";
    private static ApplicationWebService applicationWebService;
    private static AuthenticationWebService authenticationWebService;
    private static RelationshipWebService relationshipWebService;
    private static ShopWebService shopWebService;
    private static UserWebService userWebService;
    private static ClanWebService clanWebService;
    private static PrivateMessageWebService privateMessageWebService;

    public static Configuration Config { get; set; } = new Configuration();

    private static void Main(string[] args)
    {
      try
      {
        Console.WriteLine("Uberkill Webservice 2020\nWebservice for Uberstrike 4.3.9\n");
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
        basicHttpBinding.Security.Mode = BasicHttpSecurityMode.None;
        ServicePointManager.ServerCertificateValidationCallback += (RemoteCertificateValidationCallback) ((sender, cert, chain, errors) => true);
        Program.applicationWebService = new ApplicationWebService();
        EndpointAddress endpointAddress1 = new EndpointAddress(string.Format("{0}/ApplicationWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost1 = new ServiceHost((object) Program.applicationWebService, Array.Empty<Uri>());
        serviceHost1.AddServiceEndpoint(typeof (IApplicationWebServiceContract), (Binding) basicHttpBinding, endpointAddress1.Uri);
        serviceHost1.Open();
        Console.WriteLine("OK");
        Program.authenticationWebService = new AuthenticationWebService();
        EndpointAddress endpointAddress2 = new EndpointAddress(string.Format("{0}/AuthenticationWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost2 = new ServiceHost((object) Program.authenticationWebService, Array.Empty<Uri>());
        serviceHost2.AddServiceEndpoint(typeof (IAuthenticationWebServiceContract), (Binding) basicHttpBinding, endpointAddress2.Uri);
        serviceHost2.Open();
        Console.WriteLine("OK");
        Program.relationshipWebService = new RelationshipWebService();
        EndpointAddress endpointAddress3 = new EndpointAddress(string.Format("{0}/RelationshipWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost3 = new ServiceHost((object) Program.relationshipWebService, Array.Empty<Uri>());
        serviceHost3.AddServiceEndpoint(typeof (IRelationshipWebServiceContract), (Binding) basicHttpBinding, endpointAddress3.Uri);
        serviceHost3.Open();
        Console.WriteLine("OK");
        Program.shopWebService = new ShopWebService(Program.authenticationWebService);
        EndpointAddress endpointAddress4 = new EndpointAddress(string.Format("{0}/ShopWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost4 = new ServiceHost((object) Program.shopWebService, Array.Empty<Uri>());
        serviceHost4.AddServiceEndpoint(typeof (IShopWebServiceContract), (Binding) basicHttpBinding, endpointAddress4.Uri);
        serviceHost4.Open();
        Console.WriteLine("OK");
        Program.userWebService = new UserWebService();
        EndpointAddress endpointAddress5 = new EndpointAddress(string.Format("{0}/UserWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost5 = new ServiceHost((object) Program.userWebService, Array.Empty<Uri>());
        serviceHost5.AddServiceEndpoint(typeof (IUserWebServiceContract), (Binding) basicHttpBinding, endpointAddress5.Uri);
        serviceHost5.Open();
        Console.WriteLine("OK");
        Program.clanWebService = new ClanWebService();
        EndpointAddress endpointAddress6 = new EndpointAddress(string.Format("{0}/ClanWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost6 = new ServiceHost((object) Program.clanWebService, Array.Empty<Uri>());
        serviceHost6.AddServiceEndpoint(typeof (IClanWebServiceContract), (Binding) basicHttpBinding, endpointAddress6.Uri);
        serviceHost6.Open();
        Console.WriteLine("OK");
        Program.privateMessageWebService = new PrivateMessageWebService();
        EndpointAddress endpointAddress7 = new EndpointAddress(string.Format("{0}/PrivateMessageWebService", (object) Program.ServiceBaseUrl));
        ServiceHost serviceHost7 = new ServiceHost((object) Program.privateMessageWebService, Array.Empty<Uri>());
        serviceHost7.AddServiceEndpoint(typeof (IPrivateMessageWebServiceContract), (Binding) basicHttpBinding, endpointAddress7.Uri);
        serviceHost7.Open();
        Console.WriteLine("OK");
        Console.Write("Initializing connection to MongoDB...\t\t");
        if (ConfigurationManager.Initialize() && UserManager.Init())
        {
          Console.WriteLine("OK");
        }
        else
        {
          Console.Write("\tFAIL\nPlease Restart\n");
          Program.Wait();
        }
        DataManager.StartWatcher();
        Console.WriteLine("All services running.");
        Program.Wait();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        Program.Wait();
      }
    }

    public static void Wait() => new TaskCompletionSource<bool>().Task.Wait();

    public static string LoadEmbeddedJson(string dataPath) => new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(dataPath)).ReadToEnd();

    public static double GetProcessUptime() => (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()).TotalSeconds;
  }
}
