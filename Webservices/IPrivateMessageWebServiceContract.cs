// Decompiled with JetBrains decompiler
// Type: Webservices.IPrivateMessageWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  internal interface IPrivateMessageWebServiceContract
  {
    [OperationContract]
    byte[] GetAllMessageThreadsForUser(byte[] data);

    [OperationContract]
    byte[] GetThreadMessages(byte[] data);

    [OperationContract]
    byte[] SendMessage(byte[] data);

    [OperationContract]
    byte[] GetMessageWithId(byte[] data);

    [OperationContract]
    byte[] MarkThreadAsRead(byte[] data);

    [OperationContract]
    byte[] DeleteThread(byte[] data);
  }
}
