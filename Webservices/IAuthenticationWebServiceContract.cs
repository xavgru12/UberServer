// Decompiled with JetBrains decompiler
// Type: Webservices.IAuthenticationWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  public interface IAuthenticationWebServiceContract
  {
    [OperationContract]
    byte[] CreateUser(byte[] data);

    [OperationContract]
    byte[] CompleteAccount(byte[] data);

    [OperationContract]
    byte[] LoginMemberEmail(byte[] data);

    [OperationContract]
    byte[] FacebookSingleSignOn(byte[] data);

    [OperationContract]
    byte[] LoginMemberFacebook(byte[] data);

    [OperationContract]
    byte[] LoginMemberCookie(byte[] data);

    [OperationContract]
    byte[] UncompleteAccount(byte[] data);
  }
}
