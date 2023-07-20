// Decompiled with JetBrains decompiler
// Type: Webservices.UserWebService
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using UberStrike.Core.Serialization;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Common.Entities;
using Webservices.Database.Items;
using Webservices.Manager;

namespace Webservices
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class UserWebService : IUserWebServiceContract
  {
    public UserWebService() => Console.Write("Initializing UserWebService...\t\t\t");

    public byte[] ChangeMemberName(byte[] data) => new MemoryStream().ToArray();

    public byte[] IsDuplicateMemberName(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        BooleanProxy.Serialize((Stream) bytes, false);
        return bytes.ToArray();
      }
    }

    public byte[] GenerateNonDuplicateMemberNames(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetMemberWallet(byte[] data)
    {
      int id = -1;
      using (MemoryStream bytes = new MemoryStream(data))
        id = Int32Proxy.Deserialize((Stream) bytes);
      MemberWalletView memberWallet = UserManager.GetUser(id).GetAwaiter().GetResult().Member.MemberWallet;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MemberWalletViewProxy.Serialize((Stream) memoryStream, memberWallet);
        return memoryStream.ToArray();
      }
    }

    public byte[] GetInventory(byte[] data)
    {
      MemoryStream bytes1 = new MemoryStream(data);
      int id = Int32Proxy.Deserialize((Stream) bytes1);
      bytes1.Close();
      MemoryStream bytes2 = new MemoryStream();
      UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
      if (result != null)
      {
        List<ItemInventoryView> instance = result.ItemInventory ?? new List<ItemInventoryView>();
        ListProxy<ItemInventoryView>.Serialize((Stream) bytes2, (ICollection<ItemInventoryView>) instance, new ListProxy<ItemInventoryView>.Serializer<ItemInventoryView>(ItemInventoryViewProxy.Serialize));
      }
      else
        ListProxy<ItemInventoryView>.Serialize((Stream) bytes2, (ICollection<ItemInventoryView>) new List<ItemInventoryView>(), new ListProxy<ItemInventoryView>.Serializer<ItemInventoryView>(ItemInventoryViewProxy.Serialize));
      return bytes2.ToArray();
    }

    public byte[] GetCurrentDeposits(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetItemTransactions(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetPointsDeposits(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetLoadout(byte[] data)
    {
      MemoryStream bytes = new MemoryStream(data);
      int id = Int32Proxy.Deserialize((Stream) bytes);
      bytes.Close();
      UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        if (result == null || result.Loadout == null)
        {
          LoadoutView instance = new LoadoutView()
          {
            Cmid = id,
            LoadoutId = id,
            Gloves = 1086,
            Head = 1084,
            LowerBody = 1088,
            UpperBody = 1087,
            Face = 1085,
            Boots = 1089,
            SkinColor = "f0f5f5"
          };
          LoadoutViewProxy.Serialize((Stream) memoryStream, instance);
        }
        else
          LoadoutViewProxy.Serialize((Stream) memoryStream, result.Loadout);
        return memoryStream.ToArray();
      }
    }

    public byte[] SetLoadout(byte[] data)
    {
      MemoryStream bytes1 = new MemoryStream(data);
      LoadoutView loadoutView = LoadoutViewProxy.Deserialize((Stream) bytes1);
      bytes1.Close();
      using (MemoryStream bytes2 = new MemoryStream())
      {
        UserDocument result = UserManager.GetUser(loadoutView.Cmid).GetAwaiter().GetResult();
        if (result != null)
        {
          if (result.Loadout != null && result.Loadout != loadoutView)
          {
            result.Loadout = loadoutView;
            UserManager.Save(result).GetAwaiter().GetResult();
          }
          EnumProxy<MemberOperationResult>.Serialize((Stream) bytes2, MemberOperationResult.Ok);
        }
        return bytes2.ToArray();
      }
    }

    public byte[] GetMember(byte[] data)
    {
      MemoryStream bytes = new MemoryStream(data);
      UserDocument result = UserManager.GetUser(Int32Proxy.Deserialize((Stream) bytes)).GetAwaiter().GetResult();
      bytes.Close();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        UberstrikeUserViewModel instance = new UberstrikeUserViewModel()
        {
          CmuneMemberView = result.Member,
          UberstrikeMemberView = new UberstrikeMemberView()
        };
        UberstrikeUserViewModelProxy.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public byte[] GetMemberSessionData(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetMemberListSessionData(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetCurrencyDeposits(byte[] data) => new byte[0];

    public byte[] GetLevelCapsView(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MemoryStream bytes = memoryStream;
        List<PlayerLevelCapView> instance = new List<PlayerLevelCapView>();
        instance.Add(new PlayerLevelCapView()
        {
          PlayerLevelCapId = 0,
          Level = 0,
          XPRequired = 0
        });
        instance.Add(new PlayerLevelCapView()
        {
          PlayerLevelCapId = 1,
          Level = 1,
          XPRequired = 68
        });
        instance.Add(new PlayerLevelCapView()
        {
          PlayerLevelCapId = 2,
          Level = 2,
          XPRequired = 500
        });
        instance.Add(new PlayerLevelCapView()
        {
          PlayerLevelCapId = 3,
          Level = 3,
          XPRequired = 1000
        });
        ListProxy<PlayerLevelCapView>.Serializer<PlayerLevelCapView> serialization = new ListProxy<PlayerLevelCapView>.Serializer<PlayerLevelCapView>(PlayerLevelCapViewProxy.Serialize);
        ListProxy<PlayerLevelCapView>.Serialize((Stream) bytes, (ICollection<PlayerLevelCapView>) instance, serialization);
        return memoryStream.ToArray();
      }
    }

    public byte[] GetXPEventsView(byte[] data) => new byte[0];

    public byte[] SetScore(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream(data))
      {
        MatchView matchView = MatchViewProxy.Deserialize((Stream) bytes);
        if (matchView != null)
        {
          foreach (PlayerStatisticsView playerStatisticsView1 in matchView.PlayersCompleted)
          {
            UserDocument result = UserManager.GetUser(playerStatisticsView1.Cmid).GetAwaiter().GetResult();
            if (result != null)
            {
              result.Member.MemberWallet.Points += playerStatisticsView1.Points;
              TaskAwaiter awaiter;
              if (result.Statistics != null)
              {
                PlayerStatisticsView statistics = result.Statistics;
                statistics.Headshots += playerStatisticsView1.Headshots;
                statistics.Hits += playerStatisticsView1.Hits;
                statistics.Level = playerStatisticsView1.Level;
                statistics.Nutshots += playerStatisticsView1.Nutshots;
                statistics.PersonalRecord = playerStatisticsView1.PersonalRecord;
                statistics.Points += playerStatisticsView1.Points;
                statistics.Shots += playerStatisticsView1.Shots;
                statistics.Splats += playerStatisticsView1.Splats;
                statistics.Splatted += playerStatisticsView1.Splatted;
                statistics.TimeSpentInGame += playerStatisticsView1.TimeSpentInGame;
                statistics.WeaponStatistics = playerStatisticsView1.WeaponStatistics;
                statistics.Xp += playerStatisticsView1.Xp;
                result.Statistics = statistics;
                awaiter = UserManager.Save(result).GetAwaiter();
                awaiter.GetResult();
              }
              else
              {
                PlayerStatisticsView playerStatisticsView2 = new PlayerStatisticsView();
                playerStatisticsView2.Cmid = playerStatisticsView1.Cmid;
                playerStatisticsView2.Headshots += playerStatisticsView1.Headshots;
                playerStatisticsView2.Hits += playerStatisticsView1.Hits;
                playerStatisticsView2.Level = playerStatisticsView1.Level;
                playerStatisticsView2.Nutshots += playerStatisticsView1.Nutshots;
                playerStatisticsView2.PersonalRecord = playerStatisticsView1.PersonalRecord;
                playerStatisticsView2.Points += playerStatisticsView1.Points;
                playerStatisticsView2.Shots += playerStatisticsView1.Shots;
                playerStatisticsView2.Splats += playerStatisticsView1.Splats;
                playerStatisticsView2.Splatted += playerStatisticsView1.Splatted;
                playerStatisticsView2.TimeSpentInGame += playerStatisticsView1.TimeSpentInGame;
                playerStatisticsView2.WeaponStatistics = playerStatisticsView1.WeaponStatistics;
                playerStatisticsView2.Xp += playerStatisticsView1.Xp;
                result.Statistics = playerStatisticsView2;
                awaiter = UserManager.Save(result).GetAwaiter();
                awaiter.GetResult();
              }
            }
          }
        }
      }
      return new byte[0];
    }

    public byte[] FindMembers(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MemoryStream bytes = memoryStream;
        Dictionary<int, string> instance = new Dictionary<int, string>();
        instance.Add(10, "Bot Ivan");
        DictionaryProxy<int, string>.Serializer<int> keySerialization = new DictionaryProxy<int, string>.Serializer<int>(Int32Proxy.Serialize);
        DictionaryProxy<int, string>.Serializer<string> valueSerialization = new DictionaryProxy<int, string>.Serializer<string>(StringProxy.Serialize);
        DictionaryProxy<int, string>.Serialize((Stream) bytes, instance, keySerialization, valueSerialization);
        return memoryStream.ToArray();
      }
    }

    public byte[] GetPublicProfile(byte[] data)
    {
      MemoryStream bytes = new MemoryStream(data);
      int id = Int32Proxy.Deserialize((Stream) bytes);
      bytes.Close();
      UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        if (result != null)
          PublicProfileViewProxy.Serialize((Stream) memoryStream, result.Member.PublicProfile);
        else
          PublicProfileViewProxy.Serialize((Stream) memoryStream, (PublicProfileView) null);
        return memoryStream.ToArray();
      }
    }

    public byte[] GetStatistics(byte[] data)
    {
      int id;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        bytes.Seek(0L, SeekOrigin.Begin);
        id = Int32Proxy.Deserialize((Stream) bytes);
      }
      UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        if (result != null)
          PlayerCardViewProxy.Serialize((Stream) memoryStream, new PlayerCardView()
          {
            Cmid = id,
            Name = result.Member.PublicProfile.Name,
            Hits = result.Statistics.Hits,
            Precision = "100%",
            Shots = result.Statistics.Shots,
            Splats = result.Statistics.Splats,
            Splatted = result.Statistics.Splatted,
            Ranking = 1,
            TagName = result.Member.PublicProfile.GroupTag
          });
        return memoryStream.ToArray();
      }
    }

    public byte[] GetRealTimeStatistics(byte[] data)
    {
      int id;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        bytes.Seek(0L, SeekOrigin.Begin);
        id = Int32Proxy.Deserialize((Stream) bytes);
      }
      UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        if (result != null)
          PlayerCardViewProxy.Serialize((Stream) memoryStream, new PlayerCardView()
          {
            Cmid = id,
            Name = result.Member.PublicProfile.Name,
            Hits = result.Statistics.Hits,
            Precision = "100%",
            Shots = result.Statistics.Shots,
            Splats = result.Statistics.Splats,
            Splatted = result.Statistics.Splatted,
            Ranking = 1,
            TagName = result.Member.PublicProfile.GroupTag
          });
        return memoryStream.ToArray();
      }
    }

    public byte[] GetUserAndTopStats(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
        return memoryStream.ToArray();
    }
  }
}
