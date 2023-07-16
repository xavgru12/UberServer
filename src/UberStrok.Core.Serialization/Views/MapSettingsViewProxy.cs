using System.IO;
using UberStrok.Core.Views;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization.Views
{
    public static class MapSettingsViewProxy
	{
    public static MapSettings Deserialize(Stream bytes)
    {
      int num = Int32Proxy.Deserialize(bytes);
      MapSettings mapSettings = (MapSettings) null;
      if (num != 0)
      {
        mapSettings = new MapSettings();
        mapSettings.KillsCurrent = Int32Proxy.Deserialize(bytes);
        mapSettings.KillsMax = Int32Proxy.Deserialize(bytes);
        mapSettings.KillsMin = Int32Proxy.Deserialize(bytes);
        mapSettings.PlayersCurrent = Int32Proxy.Deserialize(bytes);
        mapSettings.PlayersMax = Int32Proxy.Deserialize(bytes);
        mapSettings.PlayersMin = Int32Proxy.Deserialize(bytes);
        mapSettings.TimeCurrent = Int32Proxy.Deserialize(bytes);
        mapSettings.TimeMax = Int32Proxy.Deserialize(bytes);
        mapSettings.TimeMin = Int32Proxy.Deserialize(bytes);
      }
      return mapSettings;
    }

    public static void Serialize(Stream stream, MapSettings instance)
    {
      int num = 0;
      if (instance != null)
      {
        using (MemoryStream bytes = new MemoryStream())
        {
          Int32Proxy.Serialize((Stream) bytes, instance.KillsCurrent);
          Int32Proxy.Serialize((Stream) bytes, instance.KillsMax);
          Int32Proxy.Serialize((Stream) bytes, instance.KillsMin);
          Int32Proxy.Serialize((Stream) bytes, instance.PlayersCurrent);
          Int32Proxy.Serialize((Stream) bytes, instance.PlayersMax);
          Int32Proxy.Serialize((Stream) bytes, instance.PlayersMin);
          Int32Proxy.Serialize((Stream) bytes, instance.TimeCurrent);
          Int32Proxy.Serialize((Stream) bytes, instance.TimeMax);
          Int32Proxy.Serialize((Stream) bytes, instance.TimeMin);
          Int32Proxy.Serialize(stream, ~num);
          bytes.WriteTo(stream);
        }
      }
      else
        Int32Proxy.Serialize(stream, 0);
    }
	}
}
