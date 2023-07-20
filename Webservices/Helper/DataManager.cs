// Decompiled with JetBrains decompiler
// Type: Webservices.Helper.DataManager
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using System;
using System.IO;
using Webservices.Manager;

namespace Webservices.Helper
{
  internal class DataManager
  {
    public static void StartWatcher()
    {
      FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
      fileSystemWatcher.Path = Path.Combine(Directory.GetCurrentDirectory(), "Data");
      fileSystemWatcher.Changed += new FileSystemEventHandler(DataManager.OnChanged);
      fileSystemWatcher.EnableRaisingEvents = true;
      fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security;
      fileSystemWatcher.Filter = "*.*";
    }

    private static void OnChanged(object source, FileSystemEventArgs e)
    {
      if (e.Name == "Shop.json")
      {
        ShopWebService.Initialize();
        Console.WriteLine("Shop Reloaded\nTime:" + DateTime.Now.ToString() + "\n");
      }
      if (!(e.Name == "Main.json"))
        return;
      ConfigurationManager.ReInitialize();
      Console.WriteLine("Main Config Reloaded\nTime:" + DateTime.Now.ToString() + "\n");
    }

    private static void MainManager() => new FileSystemWatcher()
    {
      Path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Main.json")
    }.Changed += new FileSystemEventHandler(DataManager.OnMainChanged);

    private static void OnMainChanged(object source, FileSystemEventArgs e)
    {
    }
  }
}
