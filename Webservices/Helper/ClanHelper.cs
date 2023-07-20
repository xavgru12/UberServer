// Decompiled with JetBrains decompiler
// Type: Webservices.Helper.ClanHelper
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;

namespace Webservices.Helper
{
  public static class ClanHelper
  {
    public static ClanView GetClanView(
      GroupCreationView groupCreation,
      int groupId,
      PublicProfileView profileView)
    {
      ClanView clanView = new ClanView();
      clanView.GroupId = groupId;
      clanView.Name = groupCreation.Name;
      clanView.Tag = groupCreation.Tag;
      clanView.Motto = groupCreation.Motto;
      clanView.Description = groupCreation.Description;
      clanView.MembersLimit = 12;
      clanView.ApplicationId = 1;
      clanView.FoundingDate = DateTime.Now;
      clanView.OwnerCmid = profileView.Cmid;
      clanView.OwnerName = profileView.Name;
      return clanView;
    }
  }
}
