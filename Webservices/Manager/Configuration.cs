// Decompiled with JetBrains decompiler
// Type: Webservices.Manager.Configuration
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

namespace Webservices.Manager
{
  public class Configuration
  {
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 27017;

    public string Username { get; set; } = "root";

    public string Password { get; set; } = string.Empty;

    public string DbName { get; set; } = "UberStrike";

    public string Hash { get; set; } = (string) null;
  }
}
