// Decompiled with JetBrains decompiler
// Type: Webservices.IUserWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  public interface IUserWebServiceContract
  {
    [OperationContract]
    byte[] ChangeMemberName(byte[] data);

    [OperationContract]
    byte[] IsDuplicateMemberName(byte[] data);

    [OperationContract]
    byte[] FindMembers(byte[] data);

    [OperationContract]
    byte[] GenerateNonDuplicateMemberNames(byte[] data);

    [OperationContract]
    byte[] GetMemberWallet(byte[] data);

    [OperationContract]
    byte[] GetInventory(byte[] data);

    [OperationContract]
    byte[] GetCurrencyDeposits(byte[] data);

    [OperationContract]
    byte[] GetItemTransactions(byte[] data);

    [OperationContract]
    byte[] GetPointsDeposits(byte[] data);

    [OperationContract]
    byte[] GetLoadout(byte[] data);

    [OperationContract]
    byte[] SetLoadout(byte[] data);

    [OperationContract]
    byte[] GetMember(byte[] data);

    [OperationContract]
    byte[] GetMemberSessionData(byte[] data);

    [OperationContract]
    byte[] GetMemberListSessionData(byte[] data);

    [OperationContract]
    byte[] GetLevelCapsView(byte[] data);

    [OperationContract]
    byte[] GetPublicProfile(byte[] data);

    [OperationContract]
    byte[] GetStatistics(byte[] data);

    [OperationContract]
    byte[] GetRealTimeStatistics(byte[] data);

    [OperationContract]
    byte[] GetUserAndTopStats(byte[] data);

    [OperationContract]
    byte[] GetXPEventsView(byte[] data);

    [OperationContract]
    byte[] SetScore(byte[] data);
  }
}
