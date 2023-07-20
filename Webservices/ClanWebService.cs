// Decompiled with JetBrains decompiler
// Type: Webservices.ClanWebService
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using UberStrike.Core.Serialization;

namespace Webservices
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  internal class ClanWebService : IClanWebServiceContract
  {
    public ClanWebService() => Console.Write("Initializing ClanWebService...\t\t\t");

    public byte[] GetClan(byte[] data) => new byte[0];

    public byte[] UpdateMemberPosition(byte[] data) => new MemoryStream().ToArray();

    public byte[] InviteMemberToJoinAGroup(byte[] data) => new MemoryStream().ToArray();

    public byte[] AcceptClanInvitation(byte[] data) => new MemoryStream().ToArray();

    public byte[] DeclineClanInvitation(byte[] data) => new MemoryStream().ToArray();

    public byte[] KickMemberFromClan(byte[] data) => new MemoryStream().ToArray();

    public byte[] DisbandGroup(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        Int32Proxy.Serialize((Stream) bytes, 0);
        return bytes.ToArray();
      }
    }

    public byte[] LeaveAClan(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetMyClanId(byte[] data) => new byte[0];

    public byte[] CancelInvitation(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetAllGroupInvitations(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetPendingGroupInvitations(byte[] data) => new MemoryStream().ToArray();

    public byte[] CreateClan(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream(data))
      {
        GroupCreationView groupCreationView = GroupCreationViewProxy.Deserialize((Stream) bytes);
        ClanView clanView = new ClanView(3333, 420, groupCreationView.Description, groupCreationView.Name, groupCreationView.Motto, "adresa", DateTime.Today, string.Empty, GroupType.Clan, DateTime.Today, groupCreationView.Tag, 1000, GroupColor.Red, GroupFontStyle.Bold, groupCreationView.ApplicationId, 5903926, "Isus Krist", new List<ClanMemberView>()
        {
          new ClanMemberView("Isus Krist", 5903926, GroupPosition.Leader, DateTime.Today, DateTime.Today)
        });
        using (MemoryStream memoryStream = new MemoryStream())
        {
          ClanCreationReturnViewProxy.Serialize((Stream) memoryStream, new ClanCreationReturnView()
          {
            ClanView = clanView,
            ResultCode = 0
          });
          return bytes.ToArray();
        }
      }
    }

    public byte[] TransferOwnership(byte[] data) => new MemoryStream().ToArray();

    public byte[] CanOwnAClan(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        BooleanProxy.Serialize((Stream) bytes, true);
        return bytes.ToArray();
      }
    }

    public byte[] IsMemberPartOfAnyGroup(byte[] data) => new byte[0];

    public byte[] IsMemberPartOfGroup(byte[] data) => new byte[0];
  }
}
