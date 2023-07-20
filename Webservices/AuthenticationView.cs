// Decompiled with JetBrains decompiler
// Type: Webservices.AuthenticationView
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System;

namespace Webservices
{
  public class AuthenticationView
  {
    public string authToken { get; set; }

    public DateTime loginTime { get; set; }

    public bool isAuth { get; set; }
  }
}
