using log4net;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Core.Session;
using UberStrok.WebServices.AspNetCore.Helper;
using UberStrok.WebServices.AspNetCore.WebService.Base;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class UserWebService : BaseUserWebService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserWebService));

        private readonly GameSessionManager gameSessionManager;
        private readonly UserManager userManager;
        private readonly ResourceManager resourceManager;
        private readonly ClanManager clanManager;
        public UserWebService(GameSessionManager gameSessionManager, UserManager userManager, ResourceManager resourceManager, ClanManager clanManager)
        {
            this.gameSessionManager = gameSessionManager;
            this.userManager = userManager;
            this.resourceManager = resourceManager;
            this.clanManager = clanManager;
        }

        public override LoadoutView OnGetLoadout(string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                HashSet<string> authed = gameSessionManager.Authed;
                lock (authed)
                {
                    if (!gameSessionManager.Authed.Add(authToken))
                    {
                        return null;
                    }
                }
                return session.Document.Loadout;
            }
            return null;
        }

        public override UberstrikeUserView OnGetMember(string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                return new UberstrikeUserView
                {
                    CmuneMemberView = session.Member,
                    UberstrikeMemberView = new UberstrikeMemberView
                    {
                        PlayerStatisticsView = session.Document.Statistics
                    }
                };
            }
            Log.Error("An unidentified AuthToken was passed.");
            return null;
        }

        public override MemberWalletView OnGetMemberWallet(string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                return session.Document.Wallet;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return null;
        }

        public override PlayerStatisticsView OnGetPlayerStats(string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                return session.Document.Statistics;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return null;
        }

        public override List<ItemInventoryView> OnGetInventory(string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                return session.Document.Inventory;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return null;
        }

        public override async Task<List<string>> OnGenerateNonDuplicatedMemberNames(string username)
        {
            List<string> usernames = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                string generateUsername = username + Utils.Random.Next(0, 10000).ToString();
                if (!await userManager.IsNameUsed(generateUsername))
                {
                    usernames.Add(generateUsername);
                }
            }
            return usernames;
        }

        public override LoadoutView OnGetLoadoutServer(string serviceAuth, string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (Startup.WebServiceConfiguration.ServiceAuth == serviceAuth)
                {
                    return session.Document.Loadout;
                }
                Log.Error("An invalid service auth was passed.");
            }
            else
            {
                Log.Error("An unidentified AuthToken was passed.");
            }
            return null;
        }

        public override async Task<MemberOperationResult> OnSetWallet(string authToken, MemberWalletView walletView)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                session.Member.MemberWallet = walletView;
                await userManager.Save(session.Document);
                return MemberOperationResult.Ok;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return MemberOperationResult.InvalidData;
        }

        public override async Task<MemberOperationResult> OnSetLoadout(string authToken, LoadoutView loadoutView)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (!CheckLoadoutAndInventory(session.Document.Loadout, session.Document.Inventory))
                {
                    Log.Error("Inventory check not passed.");
                    return MemberOperationResult.InvalidData;
                }
                session.Document.Loadout = loadoutView;
                await userManager.Save(session.Document);
                return MemberOperationResult.Ok;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return MemberOperationResult.InvalidData;
        }

        public override async Task OnEndOfMatch(string authToken, StatsCollectionView totalStats, StatsCollectionView bestStats)
        {
            Log.Info("End of match triggered, sending rewards");
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                MemberWalletView wallet = session.Document.Wallet;
                PlayerStatisticsView statistics = session.Document.Statistics;
                PlayerWeaponStatisticsView weaponStatistics = statistics.WeaponStatistics;
                PlayerPersonalRecordStatisticsView personalRecord = statistics.PersonalRecord;
                weaponStatistics.MachineGunTotalDamageDone += totalStats.MachineGunDamageDone;
                weaponStatistics.MachineGunTotalSplats += totalStats.MachineGunKills;
                weaponStatistics.MachineGunTotalShotsFired += totalStats.MachineGunShotsFired;
                weaponStatistics.MachineGunTotalShotsHit += totalStats.MachineGunShotsHit;
                weaponStatistics.ShotgunTotalDamageDone += totalStats.ShotgunDamageDone;
                weaponStatistics.ShotgunTotalSplats += totalStats.ShotgunSplats;
                weaponStatistics.ShotgunTotalShotsFired += totalStats.ShotgunShotsFired;
                weaponStatistics.ShotgunTotalShotsHit += totalStats.ShotgunShotsHit;
                weaponStatistics.SplattergunTotalDamageDone += totalStats.SplattergunDamageDone;
                weaponStatistics.SplattergunTotalSplats += totalStats.SplattergunKills;
                weaponStatistics.SplattergunTotalShotsFired += totalStats.SplattergunShotsFired;
                weaponStatistics.SplattergunTotalShotsHit += totalStats.SplattergunShotsHit;
                weaponStatistics.SniperTotalDamageDone += totalStats.SniperDamageDone;
                weaponStatistics.SniperTotalSplats += totalStats.SniperKills;
                weaponStatistics.SniperTotalShotsFired += totalStats.SniperShotsFired;
                weaponStatistics.SniperTotalShotsHit += totalStats.SniperShotsHit;
                weaponStatistics.MeleeTotalDamageDone += totalStats.MeleeDamageDone;
                weaponStatistics.MeleeTotalSplats += totalStats.MeleeKills;
                weaponStatistics.MeleeTotalShotsFired += totalStats.MeleeShotsFired;
                weaponStatistics.MeleeTotalShotsHit += totalStats.MeleeShotsHit;
                weaponStatistics.CannonTotalDamageDone += totalStats.CannonDamageDone;
                weaponStatistics.CannonTotalSplats += totalStats.CannonKills;
                weaponStatistics.CannonTotalShotsFired += totalStats.CannonShotsFired;
                weaponStatistics.CannonTotalShotsHit += totalStats.CannonShotsHit;
                weaponStatistics.LauncherTotalDamageDone += totalStats.LauncherDamageDone;
                weaponStatistics.LauncherTotalSplats += totalStats.LauncherKills;
                weaponStatistics.LauncherTotalShotsFired += totalStats.LauncherShotsFired;
                weaponStatistics.LauncherTotalShotsHit += totalStats.LauncherShotsHit;
                if (personalRecord.MostArmorPickedUp < bestStats.ArmorPickedUp)
                {
                    personalRecord.MostArmorPickedUp = bestStats.ArmorPickedUp;
                }
                if (personalRecord.MostHealthPickedUp < bestStats.HealthPickedUp)
                {
                    personalRecord.MostHealthPickedUp = bestStats.HealthPickedUp;
                }
                if (personalRecord.MostMachinegunSplats < bestStats.MachineGunKills)
                {
                    personalRecord.MostMachinegunSplats = bestStats.MachineGunKills;
                }
                if (personalRecord.MostShotgunSplats < bestStats.ShotgunSplats)
                {
                    personalRecord.MostShotgunSplats = bestStats.ShotgunSplats;
                }
                if (personalRecord.MostSplattergunSplats < bestStats.SplattergunKills)
                {
                    personalRecord.MostSplattergunSplats = bestStats.SplattergunKills;
                }
                if (personalRecord.MostSniperSplats < bestStats.SniperKills)
                {
                    personalRecord.MostSniperSplats = bestStats.SniperKills;
                }
                if (personalRecord.MostMeleeSplats < bestStats.MeleeKills)
                {
                    personalRecord.MostMeleeSplats = bestStats.MeleeKills;
                }
                if (personalRecord.MostCannonSplats < bestStats.CannonKills)
                {
                    personalRecord.MostCannonSplats = bestStats.CannonKills;
                }
                if (personalRecord.MostLauncherSplats < bestStats.LauncherKills)
                {
                    personalRecord.MostLauncherSplats = bestStats.LauncherKills;
                }
                if (personalRecord.MostDamageDealt < bestStats.GetDamageDealt())
                {
                    personalRecord.MostDamageDealt = bestStats.GetDamageDealt();
                }
                if (personalRecord.MostDamageReceived < bestStats.DamageReceived)
                {
                    personalRecord.MostDamageReceived = bestStats.DamageReceived;
                }
                if (personalRecord.MostHeadshots < bestStats.Headshots)
                {
                    personalRecord.MostHeadshots = bestStats.Headshots;
                }
                if (personalRecord.MostNutshots < bestStats.Nutshots)
                {
                    personalRecord.MostNutshots = bestStats.Nutshots;
                }
                if (personalRecord.MostSplats < bestStats.GetKills())
                {
                    personalRecord.MostSplats = bestStats.GetKills();
                }
                if (personalRecord.MostXPEarned < bestStats.Xp)
                {
                    personalRecord.MostXPEarned = bestStats.Xp;
                }
                if (personalRecord.MostConsecutiveSnipes < bestStats.ConsecutiveSnipes)
                {
                    personalRecord.MostConsecutiveSnipes = bestStats.ConsecutiveSnipes;
                }
                statistics.Xp += totalStats.Xp;
                wallet.Points += totalStats.Points;
                session.Document.Kills += totalStats.GetKills();
                session.Document.Deaths += totalStats.Deaths;
                Log.Info(JsonConvert.SerializeObject(wallet));
                statistics.Level = XpPointsUtil.GetLevelForXp(statistics.Xp);
                await userManager.Save(session.Document);
                return;
            }
            Log.Error("An unidentified AuthToken was passed.");
        }

        public override async Task<MemberOperationResult> OnChangeMemberName(string authToken, string username, string local, string machineId)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (resourceManager.IsNameValid(username))
                {
                    if (!resourceManager.IsNameOffensive(username))
                    {
                        if (await userManager.IsNameUsed(username))
                        {
                            ItemInventoryView itemInventory = session.Document.Inventory.SingleOrDefault((t) => t.ItemId == 1294);
                            if (itemInventory != null)
                            {
                                try
                                {
                                    await userManager.ChangeName(session.Document.UserId, username);
                                }
                                catch (MongoWriteException)
                                {
                                    return MemberOperationResult.DuplicateName;
                                }
                                PublicProfileView profile = session.Document.Profile;
                                _ = session.Document.Names.Add(profile.Name);
                                profile.Name = username;
                                _ = session.Document.Inventory.Remove(itemInventory);
                                if (session.Document.ClanId != 0)
                                {
                                    ClanDocument document = await clanManager.Get(session.Document.ClanId);
                                    if (document != null)
                                    {
                                        ClanMemberView clanMember = document.Clan.GetMember(session.Document.UserId);
                                        clanMember.Name = username;
                                        if (clanMember.Position == GroupPosition.Leader)
                                        {
                                            document.Clan.OwnerName = username;
                                        }
                                        await clanManager.Save(document);
                                    }
                                }
                                await userManager.Save(session.Document);
                                return MemberOperationResult.Ok;
                            }
                            return MemberOperationResult.InvalidData;
                        }
                        return MemberOperationResult.DuplicateName;
                    }
                    return MemberOperationResult.OffensiveName;
                }
                return MemberOperationResult.InvalidName;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return MemberOperationResult.InvalidData;
        }

        public override ApplicationConfigurationView OnGetAppConfig()
        {
            return ApplicationWebService.AppConfig;
        }

        public override async Task<bool> OnIsDuplicateMemberName(string username)
        {
            try
            {
                return await userManager.IsNameUsed(username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        private bool CheckLoadoutAndInventory(LoadoutView loadout, List<ItemInventoryView> inventory)
        {
            if (!inventory.Any(x => x.ItemId == loadout.Boots) && loadout.Boots != 0 && loadout.Boots != 1089)
            {
                return false;
            }

            if (!inventory.Any(x => x.ItemId == loadout.Face) && loadout.Face != 0)
            {
                return false;
            }

            if (!inventory.Any(x => x.ItemId == loadout.FunctionalItem1) && loadout.FunctionalItem1 != 0)
            {
                return false;
            }

            if (!inventory.Any(x => x.ItemId == loadout.FunctionalItem2) && loadout.FunctionalItem2 != 0)
            {
                return false;
            }

            if (!inventory.Any(x => x.ItemId == loadout.FunctionalItem3) && loadout.FunctionalItem3 != 0)
            {
                return false;
            }

            if (!inventory.Any(x => x.ItemId == loadout.Gloves) && loadout.Gloves != 0 && loadout.Gloves != 1086)
            {
                return false;
            }

            if (!inventory.Any(x => x.ItemId == loadout.Head) && loadout.Head != 0 && loadout.Head != 1084)
            {
                return false;
            }

            return (inventory.Any(x => x.ItemId == loadout.LowerBody) || loadout.LowerBody == 0 || loadout.LowerBody == 1088)
&& (inventory.Any(x => x.ItemId == loadout.MeleeWeapon) || loadout.MeleeWeapon == 0)
&& (inventory.Any(x => x.ItemId == loadout.QuickItem1) || loadout.QuickItem1 == 0)
&& (inventory.Any(x => x.ItemId == loadout.QuickItem2) || loadout.QuickItem2 == 0)
&& (inventory.Any(x => x.ItemId == loadout.QuickItem3) || loadout.QuickItem3 == 0)
&& (inventory.Any(x => x.ItemId == loadout.UpperBody) || loadout.UpperBody == 0 || loadout.UpperBody == 1087)
&& (inventory.Any(x => x.ItemId == loadout.Weapon1) || loadout.Weapon1 == 0)
&& (inventory.Any(x => x.ItemId == loadout.Weapon2) || loadout.Weapon2 == 0)
&& (inventory.Any(x => x.ItemId == loadout.Weapon3) || loadout.Weapon3 == 0);
        }
    }
}
