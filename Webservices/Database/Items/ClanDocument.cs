// Decompiled with JetBrains decompiler
// Type: Webservices.Database.Items.ClanDocument
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;

namespace Webservices.Database.Items
{
  public class ClanDocument : MongoDocument
  {
    public int ClanId { get; set; }

    public ClanView Clan { get; set; }
  }
}
