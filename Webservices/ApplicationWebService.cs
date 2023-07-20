// Decompiled with JetBrains decompiler
// Type: Webservices.ApplicationWebService
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Serialization;
using UberStrike.DataCenter.Common.Entities;

namespace Webservices
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class ApplicationWebService : IApplicationWebServiceContract
  {
    private List<MapView> mapData;
    private const string IP_ADDRESS_EU = "163.172.110.23";
    private Random random = new Random();

    public ApplicationWebService()
    {
      Console.Write("Initializing ApplicationWebService...\t\t");
      this.mapData = JsonConvert.DeserializeObject<List<MapView>>(File.ReadAllText("Data/Maps.json"));
    }

    public byte[] PreAuthenticate(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        string randomString = this.RandomString;
        AuthenticationView authenticationView = new AuthenticationView()
        {
          authToken = randomString,
          isAuth = false,
          loginTime = DateTime.Now
        };
        lock (AuthenticationWebService.Sync)
          AuthenticationWebService.authenticationView.Add(authenticationView);
        StringProxy.Serialize((Stream) bytes, this.Encrypt(randomString.ToString()));
        return bytes.ToArray();
      }
    }

    public byte[] AuthenticateApplication(byte[] data)
    {
      string hashint;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        bytes.Seek(0L, SeekOrigin.Begin);
        StringProxy.Deserialize((Stream) bytes);
        EnumProxy<ChannelType>.Deserialize((Stream) bytes);
        StringProxy.Deserialize((Stream) bytes);
        hashint = StringProxy.Deserialize((Stream) bytes);
      }
      string instance1 = this.GetAuthToken(hashint) ?? this.RandomString;
      using (MemoryStream bytes = new MemoryStream())
      {
        AuthenticateApplicationView instance2 = new AuthenticateApplicationView()
        {
          GameServers = new List<PhotonView>()
          {
            new PhotonView()
            {
              IP = "163.172.110.23",
              MinLatency = 0,
              Name = "[FR] Paris",
              Port = 5058,
              PhotonId = 2,
              Region = RegionType.EuWest,
              UsageType = PhotonUsageType.All
            }
          },
          CommServer = new PhotonView()
          {
            IP = "163.172.110.23",
            MinLatency = 0,
            Name = "CommServer",
            Port = 5057,
            PhotonId = 1,
            Region = RegionType.EuWest,
            UsageType = PhotonUsageType.CommServer
          },
          IsEnabled = true,
          WarnPlayer = false,
          EncryptionInitVector = "",
          EncryptionPassPhrase = ""
        };
        AuthenticateApplicationViewProxy.Serialize((Stream) bytes, instance2);
        StringProxy.Serialize((Stream) bytes, instance1);
        return bytes.ToArray();
      }
    }

    private string GetAuthToken(string hashint)
    {
      string empty = string.Empty;
      if (string.IsNullOrEmpty(hashint) || string.IsNullOrEmpty(Program.Config.Hash))
        return (string) null;
      string authToken = this.Decrypt(hashint, Program.Config.Hash);
      lock (AuthenticationWebService.Sync)
      {
        for (int index = 0; index < AuthenticationWebService.authenticationView.Count; ++index)
        {
          if (AuthenticationWebService.authenticationView[index].authToken == authToken)
            AuthenticationWebService.authenticationView[index].isAuth = true;
        }
        AuthenticationWebService.authenticationView.RemoveAll((Predicate<AuthenticationView>) (x => (x.loginTime - DateTime.Now).TotalMinutes > 30.0));
      }
      return authToken;
    }

    public string RandomString => new string(Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10).Select<string, char>((Func<string, char>) (s => s[this.random.Next(s.Length)])).ToArray<char>());

    private string Decrypt(string hashint, string key)
    {
      string[] strArray = hashint.Split('.');
      string str1 = "";
      char ch;
      foreach (string s in strArray)
      {
        string str2 = str1;
        ch = Convert.ToChar(int.Parse(s));
        string str3 = ch.ToString();
        str1 = str2 + str3;
      }
      string str4 = "";
      for (int index = 0; index < str1.Length; ++index)
      {
        string str5 = str4;
        ch = (char) ((uint) str1[index] ^ (uint) key[index % key.Length]);
        string str6 = ch.ToString();
        str4 = str5 + str6;
      }
      return str4;
    }

    private string Encrypt(string message)
    {
      Random random = new Random();
      List<string> values = new List<string>();
      string str = random.Next(1000, 9999).ToString();
      for (int index = 0; index < message.Length; ++index)
      {
        char ch = (char) ((uint) message[index] ^ (uint) str[index % str.Length]);
        values.Add(((int) ch).ToString());
      }
      values.Add(str.ToString());
      return string.Join(".", (IEnumerable<string>) values);
    }

    public byte[] GetMaps(byte[] data)
    {
      MemoryStream bytes = new MemoryStream();
      ListProxy<MapView>.Serialize((Stream) bytes, (ICollection<MapView>) this.mapData, new ListProxy<MapView>.Serializer<MapView>(MapViewProxy.Serialize));
      return bytes.ToArray();
    }

    public byte[] GetItemAssetBundles(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetLiveFeed(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MemoryStream bytes = memoryStream;
        List<LiveFeedView> instance = new List<LiveFeedView>();
        instance.Add(new LiveFeedView(DateTime.Now, 2, "Welcome to UberKill.", string.Empty, 1));
        ListProxy<LiveFeedView>.Serializer<LiveFeedView> serialization = new ListProxy<LiveFeedView>.Serializer<LiveFeedView>(LiveFeedViewProxy.Serialize);
        ListProxy<LiveFeedView>.Serialize((Stream) bytes, (ICollection<LiveFeedView>) instance, serialization);
        return memoryStream.ToArray();
      }
    }

    public byte[] GetMyIP(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        StringProxy.Serialize((Stream) bytes, "163.172.110.23");
        return bytes.ToArray();
      }
    }

    public byte[] GetPhotonServerName(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        StringProxy.Serialize((Stream) bytes, "Test server");
        return bytes.ToArray();
      }
    }

    public byte[] GetPhotonServers(byte[] data)
    {
      PhotonView photonView1 = new PhotonView()
      {
        IP = "163.172.110.23",
        MinLatency = 0,
        Name = "CommServer",
        Port = 5055,
        PhotonId = 1,
        Region = RegionType.EuWest,
        UsageType = PhotonUsageType.CommServer
      };
      PhotonView photonView2 = new PhotonView()
      {
        IP = "163.172.110.23",
        MinLatency = 0,
        Name = "[EU] Croatia",
        Port = 5056,
        PhotonId = 2,
        Region = RegionType.EuWest,
        UsageType = PhotonUsageType.All
      };
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MemoryStream bytes = memoryStream;
        List<PhotonView> instance = new List<PhotonView>();
        instance.Add(photonView1);
        instance.Add(photonView2);
        ListProxy<PhotonView>.Serializer<PhotonView> serialization = new ListProxy<PhotonView>.Serializer<PhotonView>(PhotonViewProxy.Serialize);
        ListProxy<PhotonView>.Serialize((Stream) bytes, (ICollection<PhotonView>) instance, serialization);
        return memoryStream.ToArray();
      }
    }

    public byte[] IsAlive(byte[] data) => new MemoryStream().ToArray();

    public byte[] ReportBug(byte[] data) => new MemoryStream().ToArray();

    public byte[] SetLevelVersion(byte[] data) => new MemoryStream().ToArray();

    public byte[] RecordException(byte[] data) => new byte[0];

    public byte[] RecordExceptionUnencrypted(byte[] data) => new byte[0];

    public byte[] RecordTutorialStep(byte[] data) => new byte[0];

    [Obsolete("Not used anymore I guess.")]
    public byte[] RegisterClientApplication(byte[] data) => new byte[0];
  }
}
