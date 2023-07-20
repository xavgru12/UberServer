// Decompiled with JetBrains decompiler
// Type: Webservices.IClanWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  internal interface IClanWebServiceContract
  {
    [OperationContract]
    byte[] GetClan(byte[] data);

    [OperationContract]
    byte[] UpdateMemberPosition(byte[] data);

    [OperationContract]
    byte[] InviteMemberToJoinAGroup(byte[] data);

    [OperationContract]
    byte[] AcceptClanInvitation(byte[] data);

    [OperationContract]
    byte[] DeclineClanInvitation(byte[] data);

    [OperationContract]
    byte[] KickMemberFromClan(byte[] data);

    [OperationContract]
    byte[] DisbandGroup(byte[] data);

    [OperationContract]
    byte[] LeaveAClan(byte[] data);

    [OperationContract]
    byte[] GetMyClanId(byte[] data);

    [OperationContract]
    byte[] CancelInvitation(byte[] data);

    [OperationContract]
    byte[] GetAllGroupInvitations(byte[] data);

    [OperationContract]
    byte[] GetPendingGroupInvitations(byte[] data);

    [OperationContract]
    byte[] CreateClan(byte[] data);

    [OperationContract]
    byte[] TransferOwnership(byte[] data);

    [OperationContract]
    byte[] CanOwnAClan(byte[] data);

    [OperationContract]
    byte[] IsMemberPartOfAnyGroup(byte[] data);

    [OperationContract]
    byte[] IsMemberPartOfGroup(byte[] data);
  }
}
