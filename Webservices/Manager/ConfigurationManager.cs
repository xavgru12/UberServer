// Decompiled with JetBrains decompiler
// Type: Webservices.Manager.ConfigurationManager
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace Webservices.Manager
{
  public class ConfigurationManager
  {
    public static bool Initialize()
    {
      try
      {
        Configuration configuration = new Configuration();
        string str = (string) null;
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Main.json")))
          str = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Main.json"));
        else
          File.WriteAllText("Data/Main.json", JsonConvert.SerializeObject((object) configuration, Formatting.Indented));
        Program.Config = str == null ? configuration : JsonConvert.DeserializeObject<Configuration>(str);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return false;
      }
    }

    public static void ReInitialize()
    {
      while (true)
      {
        try
        {
          Configuration configuration = new Configuration();
          string str = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Main.json"));
          if (str != null)
          {
            Program.Config = JsonConvert.DeserializeObject<Configuration>(str);
            break;
          }
          Program.Config = configuration;
          break;
        }
        catch
        {
          Thread.Sleep(100);
        }
      }
    }
  }
}
