// Decompiled with JetBrains decompiler
// Type: Webservices.Database.Items.UserDocument
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;
using System.Collections.Generic;
using UberStrike.DataCenter.Common.Entities;

namespace Webservices.Database.Items
{
  public class UserDocument : MongoDocument
  {
    public int UserId { get; set; }

    public int ClanId { get; set; }

    public string UBBan { get; set; } = (string) null;

    public string UBMute { get; set; } = (string) null;

    public string Email { get; set; }

    public string Password { get; set; }

    public MemberView Member { get; set; }

    public bool isAccountComplete { get; set; } = false;

    public LoadoutView Loadout { get; set; }

    public List<ItemInventoryView> ItemInventory { get; set; }

    public PlayerStatisticsView Statistics { get; set; }

    public List<int> Friends { get; set; }

    public List<int> FriendRequests { get; set; }

    public List<int> ClanRequests { get; set; }

    public List<int> Streams { get; set; }

    public HashSet<string> HDD { get; set; }

    public HashSet<string> BIOS { get; set; }

    public HashSet<string> MOTHERBOARD { get; set; }

    public HashSet<string> MAC { get; set; }

    public HashSet<string> UNITY { get; set; }

    public HashSet<string> Names { get; set; }

    public int Kills { get; set; } = 0;

    public int Deaths { get; set; } = 0;

    public double Kdr { get; set; } = 0.0;

    public UserDocument()
    {
      this.Friends = new List<int>();
      this.FriendRequests = new List<int>();
      this.ClanRequests = new List<int>();
      this.Streams = new List<int>();
      this.Names = new HashSet<string>();
      this.HDD = new HashSet<string>();
      this.BIOS = new HashSet<string>();
      this.MOTHERBOARD = new HashSet<string>();
      this.MAC = new HashSet<string>();
      this.UNITY = new HashSet<string>();
      this.ItemInventory = new List<ItemInventoryView>();
    }

    public static UserDocument Default(int id, string email, string password)
    {
      LoadoutView loadoutView = new LoadoutView()
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
      MemberView memberView = new MemberView();
      memberView.PublicProfile.Cmid = id;
      memberView.PublicProfile.AccessLevel = MemberAccessLevel.Default;
      memberView.PublicProfile.IsChatDisabled = false;
      memberView.PublicProfile.GroupTag = "";
      memberView.PublicProfile.EmailAddressStatus = EmailAddressStatus.Verified;
      memberView.PublicProfile.Name = email;
      memberView.PublicProfile.LastLoginDate = DateTime.Now;
      memberView.MemberWallet.Cmid = id;
      memberView.MemberWallet.Credits = 1000000000;
      memberView.MemberWallet.CreditsExpiration = DateTime.MaxValue;
      memberView.MemberWallet.PointsExpiration = DateTime.MaxValue;
      memberView.MemberWallet.Points = 1000000000;
      memberView.MemberItems = new List<int>();
      PlayerStatisticsView playerStatisticsView = new PlayerStatisticsView()
      {
        Cmid = id,
        Headshots = 0,
        Hits = 0,
        Level = 1,
        Nutshots = 0,
        PersonalRecord = new PlayerPersonalRecordStatisticsView()
        {
          MostArmorPickedUp = 0,
          MostDamageDealt = 0,
          MostDamageReceived = 0,
          MostCannonSplats = 0,
          MostConsecutiveSnipes = 0,
          MostHandgunSplats = 0,
          MostHeadshots = 0,
          MostHealthPickedUp = 0,
          MostLauncherSplats = 0,
          MostMachinegunSplats = 0,
          MostMeleeSplats = 0,
          MostNutshots = 0,
          MostShotgunSplats = 0,
          MostSniperSplats = 0,
          MostSplats = 0,
          MostSplattergunSplats = 0,
          MostXPEarned = 0
        },
        Points = 0,
        Shots = 0,
        Splats = 0,
        Splatted = 0,
        TimeSpentInGame = 0,
        Xp = 0,
        WeaponStatistics = new PlayerWeaponStatisticsView()
        {
          CannonTotalDamageDone = 0,
          HandgunTotalDamageDone = 0,
          LauncherTotalDamageDone = 0,
          MachineGunTotalDamageDone = 0,
          MeleeTotalShotsFired = 0,
          MeleeTotalShotsHit = 0,
          MeleeTotalSplats = 0,
          ShotgunTotalDamageDone = 0,
          MeleeTotalDamageDone = 0,
          SniperTotalDamageDone = 0,
          SplattergunTotalDamageDone = 0,
          CannonTotalShotsFired = 0,
          CannonTotalShotsHit = 0,
          CannonTotalSplats = 0,
          HandgunTotalShotsFired = 0,
          HandgunTotalShotsHit = 0,
          HandgunTotalSplats = 0,
          LauncherTotalShotsFired = 0,
          LauncherTotalShotsHit = 0,
          LauncherTotalSplats = 0,
          MachineGunTotalShotsFired = 0,
          MachineGunTotalShotsHit = 0,
          MachineGunTotalSplats = 0,
          ShotgunTotalShotsFired = 0,
          ShotgunTotalShotsHit = 0,
          ShotgunTotalSplats = 0,
          SniperTotalShotsFired = 0,
          SniperTotalShotsHit = 0,
          SniperTotalSplats = 0,
          SplattergunTotalShotsFired = 0,
          SplattergunTotalShotsHit = 0,
          SplattergunTotalSplats = 0
        }
      };
      return new UserDocument()
      {
        UserId = id,
        Email = email,
        Password = password,
        Friends = new List<int>(),
        FriendRequests = new List<int>(),
        ClanRequests = new List<int>(),
        Streams = new List<int>(),
        Names = new HashSet<string>(),
        Member = memberView,
        Statistics = playerStatisticsView,
        ItemInventory = new List<ItemInventoryView>(),
        Loadout = loadoutView
      };
    }
  }
}
