using System;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Manager;

namespace UberStrok.WebServices.AspNetCore.Core.Session
{
    public class GameSession
    {
        public string SessionId
        {
            get;
            set;
        }

        public string IPAddress
        {
            get;
            set;
        }

        public string MachineId
        {
            get;
            set;
        }

        public MemberView Member
        {
            get;
            set;
        }

        public UserDocument Document
        {
            get;
            set;
        }

        private readonly ClanManager clanManager;
        public GameSession(string sessionId, UserDocument document, ClanManager clanManager)
        {
            SessionId = sessionId;
            Member = new MemberView(document.Profile, document.Wallet, document.Inventory.Select((ItemInventoryView t) => t.ItemId).ToList())
            {
                PublicProfile =
                {
                    LastLoginDate = DateTime.Now
                }
            };
            Document = document;
            this.clanManager = clanManager;
            RefreshClanLastLoginTime();
        }

        public void RefreshClanLastLoginTime()
        {
            if (Document.ClanId != 0)
            {
                _ = Task.Run(async delegate
                {
                    ClanDocument clan = await clanManager.Get(Document.ClanId);
                    if (clan != null)
                    {
                        ClanMemberView member = clan.Clan.Members.Find((ClanMemberView m) => m.Cmid == Document.UserId);
                        if (member != null)
                        {
                            member.Lastlogin = DateTime.UtcNow;
                            await clanManager.Save(clan);
                        }
                    }
                });
            }
        }
    }
}
