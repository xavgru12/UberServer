using log4net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Db;

namespace UberStrok.WebServices
{
    public class UserManager
    {
        public class Session
        {
            public string AuthToken { get; set; }
            public string Ip { get; set; }
            public string Hwd { get; set; }
            public MemberView Member { get; set; }
        }

        private readonly static ILog Log = LogManager.GetLogger(typeof(UserManager).Name);

        private int _nextCmid;
        private readonly Dictionary<string, Session> _sessions; // AuthToken -> Session

        private readonly WebServiceContext _ctx;
        public UserManager(WebServiceContext ctx)
        {
            _ctx = ctx;

            Db = new UserDb();
            Authed = new HashSet<string>();

            _sessions = new Dictionary<string, Session>();
            _nextCmid = Db.GetNextCmid();
            if (_nextCmid == -1)
            {
                _nextCmid = 0;
                Db.SetNextCmid(_nextCmid);
            }
        }


        public HashSet<string> Authed { get; }
        public UserDb Db { get; }

        public MemberView NewMember()
        {
            var cmid = Interlocked.Increment(ref _nextCmid);
            Random rnd = new Random();
            var name = GenerateName(rnd.Next(4, 10));
            var publicProfile = new PublicProfileView(
                cmid,
                name,
                MemberAccessLevel.Default,
                false,
                DateTime.UtcNow,
                EmailAddressStatus.Verified,
                "-1"
            );

            var memberWallet = new MemberWalletView(
                cmid,
                _ctx.Configuration.Wallet.StartingCredits,
                _ctx.Configuration.Wallet.StartingPoints,
                DateTime.MaxValue,
                DateTime.MaxValue
            );

            var memberInventories = new List<ItemInventoryView>();
            var shop = _ctx.Items.GetShop();
            foreach (var w in shop.WeaponItems)
            {
                memberInventories.Add(new ItemInventoryView(w.ID, null, -1, cmid));
            }
            foreach (var w in shop.GearItems)
            {
                memberInventories.Add(new ItemInventoryView(w.ID, null, -1, cmid));
            }
            foreach (var w in shop.QuickItems)
            {
                memberInventories.Add(new ItemInventoryView(w.ID, null, -1, cmid));
            }

            //TODO: Create helper function for conversion of this stuff.
            var memberItems = new List<int>();
            for (int i = 0; i < memberInventories.Count; i++)
                memberItems.Add(memberInventories[i].ItemId);

            var memberLoadout = new LoadoutView
            {
                Cmid = cmid,
                MeleeWeapon = 1,
                Weapon1 = 12
            };

            var member = new MemberView(
                publicProfile,
                memberWallet,
                memberItems
            );

            // Save the member.
            Db.Profiles.Save(publicProfile);
            Db.Wallets.Save(memberWallet);
            Db.Inventories.Save(cmid, memberInventories);
            Db.Loadouts.Save(memberLoadout);

            Db.SetNextCmid(_nextCmid);

            return member;
        }

        public MemberView GetMember(string authToken)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));

            lock (_sessions)
            {
                foreach (var s in _sessions)
                {
                    if (s.Key.Contains(authToken))
                    {
                        return s.Value.Member;
                    }
                }
            }
            return null;
        }

        public MemberView GetMember(int cmid)
        {
            Console.WriteLine("Received cmid " + cmid);
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            lock (_sessions)
            {
                foreach (var value in _sessions.Values)
                {
                    Console.WriteLine(value.Member.PublicProfile.Cmid);
                    if (value.Member.PublicProfile.Cmid == cmid)
                        return value.Member;
                }
            }
            return null;
        }

        public Session GetSession(int cmid)
        {
            if (cmid <= 0)
                throw new ArgumentException("CMID must be greater than 0.");

            lock (_sessions)
            {
                foreach (var value in _sessions.Values)
                {
                    if (value.Member.PublicProfile.Cmid == cmid)
                        return value;
                }
            }
            return null;
        }

        public Session LogInUser(MemberView member, string ip, string machineId)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            // Encode ServiceBase URL into the AuthToken so the realtime servers can figure out
            // where the user came from.
            var data = _ctx.ServiceBase + "#####" + DateTime.UtcNow.ToFileTime();
            var bytes = Encoding.UTF8.GetBytes(data);
            var authToken = Convert.ToBase64String(bytes);
            var session = default(Session);

            member.PublicProfile.LastLoginDate = DateTime.UtcNow;

            lock (_sessions)
            {
                foreach (var kv in _sessions)
                {
                    var value = kv.Value;
                    if (value.Member.PublicProfile.Cmid == member.PublicProfile.Cmid)
                    {
                        /* Replace players with same CMID, not the neatest of fixes, but it works. */
                        LogOutUser(value.Member);
                        Log.Info($"Kicking player with CMID {value.Member.PublicProfile.Cmid} cause of new login.");
                        break;
                    }
                }

                session = new Session
                {
                    AuthToken = authToken,
                    Member = member,
                    Ip = ip,
                    Hwd = machineId
                };
                Console.WriteLine("Login with authToken " + authToken);
                _sessions.Add(authToken, session);
            }

            // Save only profile since we only modified the profile.
            Db.Profiles.Save(member.PublicProfile);
            return session;
        }

        public bool LogOutUser(MemberView member)
        {
            foreach (var kv in _sessions)
            {
                var value = kv.Value;
                if (value.Member.PublicProfile.Cmid == member.PublicProfile.Cmid)
                {
                    /* Replace players with same CMID, not the neatest of fixes, but it works. */
                    _sessions.Remove(kv.Key);
                    Log.Info($"Player with CMID {value.Member.PublicProfile.Cmid} logged out.");
                    return true;
                }
            }

            return false;
        }

        public bool IsDumbAss(string ip, string machineId)
        {
            if (ip == null || machineId == null)
            {
                return true;
            }
            bool checkIp = true;
            if (IPAddress.IsLoopback(IPAddress.Parse(ip)))
            {
                checkIp = false;
            }
            int weight = 0;
            foreach (var kv in _sessions)
            {
                if (checkIp)
                {
                    if (kv.Value.Ip == ip)
                    {
                        weight++;
                    }
                }
                if (kv.Value.Hwd == machineId)
                {
                    weight++;
                }
            }
            return weight > 1;
        }
        public string GenerateName(int len)
        {
            Random r = new Random();
            char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'l', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x' };
            char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
            StringBuilder Name = new StringBuilder();
            int b = 0; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name.Append(consonants[r.Next(consonants.Length)]);
                b++;
                Name.Append(vowels[r.Next(vowels.Length)]);
                b++;
            }
            return Name.ToString();
        }
    }
}
