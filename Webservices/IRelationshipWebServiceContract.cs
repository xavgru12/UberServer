// Decompiled with JetBrains decompiler
// Type: Webservices.IRelationshipWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  public interface IRelationshipWebServiceContract
  {
    [OperationContract]
    byte[] SendContactRequest(byte[] data);

    [OperationContract]
    byte[] GetContactRequests(byte[] data);

    [OperationContract]
    byte[] AcceptContactRequest(byte[] data);

    [OperationContract]
    byte[] DeclineContactRequest(byte[] data);

    [OperationContract]
    byte[] DeleteContact(byte[] data);

    [OperationContract]
    byte[] GetContactsByGroups(byte[] data);

    [OperationContract]
    byte[] MoveContactToGroup(byte[] data);
  }
}
