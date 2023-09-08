using System;
using System.Linq;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;

namespace UberStrok.WebServices.AspNetCore.Helper
{
    internal static class ClanMemberHelper
    {
        internal static ClanMemberView GetClanMemberView(UserDocument document, GroupPosition position = GroupPosition.Member)
        {
            return new ClanMemberView
            {
                Cmid = document.UserId,
                Name = document.Profile.Name,
                Position = position,
                JoiningDate = DateTime.Now,
                Lastlogin = document.Profile.LastLoginDate
            };
        }

        internal static bool AddMemberToClan(this ClanView clan, ClanMemberView clanMember)
        {
            if (clan.MembersCount < clan.MembersLimit)
            {
                clan.Members.Add(clanMember);
                clan.MembersCount++;
                return true;
            }
            return false;
        }

        internal static bool RemoveMemberFromClan(this ClanView clan, ClanMemberView clanMember)
        {
            if (clan.Members.Remove(clanMember))
            {
                clan.MembersCount--;
                return true;
            }
            return false;
        }

        internal static ClanMemberView GetMember(this ClanView clan, int cmid)
        {
            return clan.Members.Single((t) => t.Cmid == cmid);
        }

        internal static bool HasHigherPermissionThan(this ClanMemberView memberView, GroupPosition position)
        {
            return memberView.Position switch
            {
                GroupPosition.Officer => position == GroupPosition.Member,
                GroupPosition.Leader => position != GroupPosition.Leader,
                _ => false,
            };
        }

        internal static bool HasHigherPermissionThan(this ClanMemberView memberView, ClanMemberView memberView2)
        {
            GroupPosition rankInClan = memberView.Position;
            GroupPosition position = memberView2.Position;
            return rankInClan switch
            {
                GroupPosition.Officer => position == GroupPosition.Member,
                GroupPosition.Leader => position != GroupPosition.Leader,
                _ => false,
            };
        }
    }
}
