// Decompiled with JetBrains decompiler
// Type: Webservices.Helper.ClanMemberHelper
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;
using System.Linq;
using Webservices.Database.Items;

namespace Webservices.Helper
{
  internal static class ClanMemberHelper
  {
    internal static ClanMemberView GetClanMemberView(UserDocument document, GroupPosition position = GroupPosition.Member) => new ClanMemberView()
    {
      Cmid = document.UserId,
      Name = document.Member.PublicProfile.Name,
      Position = position,
      JoiningDate = DateTime.Now,
      Lastlogin = document.Member.PublicProfile.LastLoginDate
    };

    internal static bool AddMemberToClan(this ClanView clan, ClanMemberView clanMember)
    {
      if (clan.MembersCount >= clan.MembersLimit)
        return false;
      clan.Members.Add(clanMember);
      ++clan.MembersCount;
      return true;
    }

    internal static bool RemoveMemberFromClan(this ClanView clan, ClanMemberView clanMember)
    {
      if (!clan.Members.Remove(clanMember))
        return false;
      --clan.MembersCount;
      return true;
    }

    internal static ClanMemberView GetMember(this ClanView clan, int cmid) => clan.Members.Single<ClanMemberView>((Func<ClanMemberView, bool>) (t => t.Cmid == cmid));

    internal static bool HasHigherPermissionThan(
      this ClanMemberView memberView,
      GroupPosition position)
    {
      GroupPosition position1 = memberView.Position;
      return position1 != 0 ? position1 == GroupPosition.Officer && position == GroupPosition.Member : position != 0;
    }

    internal static bool HasHigherPermissionThan(
      this ClanMemberView memberView,
      ClanMemberView memberView2)
    {
      GroupPosition position1 = memberView.Position;
      GroupPosition position2 = memberView2.Position;
      return position1 != 0 ? position1 == GroupPosition.Officer && position2 == GroupPosition.Member : position2 != 0;
    }
  }
}
