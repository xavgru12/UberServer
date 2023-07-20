// Decompiled with JetBrains decompiler
// Type: Webservices.IShopWebServiceContract
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System.ServiceModel;

namespace Webservices
{
  [ServiceContract]
  public interface IShopWebServiceContract
  {
    [OperationContract]
    byte[] BuyMasBundle(byte[] data);

    [OperationContract]
    byte[] BuyiPadBundle(byte[] data);

    [OperationContract]
    byte[] BuyiPhoneBundle(byte[] data);

    [OperationContract]
    byte[] BuyItem(byte[] data);

    [OperationContract]
    byte[] BuyPack(byte[] data);

    [OperationContract]
    byte[] GetAllLuckyDraws_1(byte[] data);

    [OperationContract]
    byte[] GetAllLuckyDraws_2(byte[] data);

    [OperationContract]
    byte[] GetAllMysteryBoxs_1(byte[] data);

    [OperationContract]
    byte[] GetAllMysteryBoxs_2(byte[] data);

    [OperationContract]
    byte[] GetBundles(byte[] data);

    [OperationContract]
    byte[] GetLuckyDraw(byte[] data);

    [OperationContract]
    byte[] GetMysteryBox(byte[] data);

    [OperationContract]
    byte[] GetShop(byte[] data);

    [OperationContract]
    byte[] RollLuckyDraw(byte[] data);

    [OperationContract]
    byte[] RollMysteryBox(byte[] data);

    [OperationContract]
    byte[] UseConsumableItem(byte[] data);
  }
}
