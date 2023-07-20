// Decompiled with JetBrains decompiler
// Type: Webservices.IApplicationWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  public interface IApplicationWebServiceContract
  {
    [OperationContract]
    byte[] AuthenticateApplication(byte[] data);

    [OperationContract]
    byte[] PreAuthenticate(byte[] data);

    [OperationContract]
    byte[] GetMaps(byte[] data);

    [OperationContract]
    byte[] GetLiveFeed(byte[] data);

    [OperationContract]
    byte[] GetItemAssetBundles(byte[] data);

    [OperationContract]
    byte[] GetMyIP(byte[] data);

    [OperationContract]
    byte[] GetPhotonServerName(byte[] data);

    [OperationContract]
    byte[] GetPhotonServers(byte[] data);

    [OperationContract]
    byte[] IsAlive(byte[] data);

    [OperationContract]
    byte[] ReportBug(byte[] data);

    [OperationContract]
    byte[] RecordException(byte[] data);

    [OperationContract]
    byte[] RecordExceptionUnencrypted(byte[] data);

    [OperationContract]
    byte[] RecordTutorialStep(byte[] data);

    [OperationContract]
    byte[] RegisterClientApplication(byte[] data);

    [OperationContract]
    byte[] SetLevelVersion(byte[] data);
  }
}
