using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MapViewProxy
  {
    public static void Serialize(Stream stream, MapView instance)
    {
      int num = 0;
      if (instance != null)
      {
        using (MemoryStream bytes = new MemoryStream())
        {
          if (instance.Description != null)
            StringProxy.Serialize((Stream) bytes, instance.Description);
          else
            num |= 1;
          if (instance.DisplayName != null)
            StringProxy.Serialize((Stream) bytes, instance.DisplayName);
          else
            num |= 2;
          if (instance.FileName != null)
            StringProxy.Serialize((Stream) bytes, instance.FileName);
          else
            num |= 4;
          BooleanProxy.Serialize((Stream) bytes, instance.IsBlueBox);
          Int32Proxy.Serialize((Stream) bytes, instance.MapId);
          Int32Proxy.Serialize((Stream) bytes, instance.MaxPlayers);
          Int32Proxy.Serialize((Stream) bytes, instance.RecommendedItemId);
          if (instance.SceneName != null)
            StringProxy.Serialize((Stream) bytes, instance.SceneName);
          else
            num |= 8;
          if (instance.Settings != null)
            DictionaryProxy<GameModeType, MapSettings>.Serialize((Stream) bytes, instance.Settings, new DictionaryProxy<GameModeType, MapSettings>.Serializer<GameModeType>(EnumProxy<GameModeType>.Serialize), new DictionaryProxy<GameModeType, MapSettings>.Serializer<MapSettings>(MapSettingsViewProxy.Serialize));
          else
            num |= 16;
          Int32Proxy.Serialize((Stream) bytes, instance.SupportedGameModes);
          Int32Proxy.Serialize(stream, ~num);
          bytes.WriteTo(stream);
        }
      }
      else
        Int32Proxy.Serialize(stream, 0);
    }

    public static MapView Deserialize(Stream bytes)
    {
      int num = Int32Proxy.Deserialize(bytes);
      MapView mapView = (MapView) null;
      if (num != 0)
      {
        mapView = new MapView();
        if ((num & 1) != 0)
          mapView.Description = StringProxy.Deserialize(bytes);
        if ((num & 2) != 0)
          mapView.DisplayName = StringProxy.Deserialize(bytes);
        if ((num & 4) != 0)
          mapView.FileName = StringProxy.Deserialize(bytes);
        mapView.IsBlueBox = BooleanProxy.Deserialize(bytes);
        mapView.MapId = Int32Proxy.Deserialize(bytes);
        mapView.MaxPlayers = Int32Proxy.Deserialize(bytes);
        mapView.RecommendedItemId = Int32Proxy.Deserialize(bytes);
        if ((num & 8) != 0)
          mapView.SceneName = StringProxy.Deserialize(bytes);
        if ((num & 16) != 0)
          mapView.Settings = DictionaryProxy<GameModeType, MapSettings>.Deserialize(bytes, new DictionaryProxy<GameModeType, MapSettings>.Deserializer<GameModeType>(EnumProxy<GameModeType>.Deserialize), new DictionaryProxy<GameModeType, MapSettings>.Deserializer<MapSettings>(MapSettingsViewProxy.Deserialize));
        mapView.SupportedGameModes = Int32Proxy.Deserialize(bytes);
      }
      return mapView;
    }
  }
}
