using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MapViewProxy
    {
        public static MapView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            MapView val = new MapView();
            if (((uint)num & (true ? 1u : 0u)) != 0)
            {
                val.Description = StringProxy.Deserialize(bytes);
            }
            if (((uint)num & 2u) != 0)
            {
                val.DisplayName = StringProxy.Deserialize(bytes);
            }
            val.BoxType = EnumProxy<GameBoxType>.Deserialize(bytes);
            val.MapId = Int32Proxy.Deserialize(bytes);
            val.MaxPlayers = Int32Proxy.Deserialize(bytes);
            val.RecommendedItemId = Int32Proxy.Deserialize(bytes);
            if (((uint)num & 4u) != 0)
            {
                val.SceneName = StringProxy.Deserialize(bytes);
            }
            if (((uint)num & 8u) != 0)
            {
                val.Settings = DictionaryProxy<GameModeType, MapSettingsView>.Deserialize(bytes, EnumProxy<GameModeType>.Deserialize, MapSettingsViewProxy.Deserialize);
            }
            val.SupportedGameModes = Int32Proxy.Deserialize(bytes);
            val.SupportedItemClass = Int32Proxy.Deserialize(bytes);
            return val;
        }

        public static void Serialize(Stream stream, MapView instance)
        {
            int num = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (instance.Description != null)
                {
                    StringProxy.Serialize(memoryStream, instance.Description);
                }
                else
                {
                    num |= 1;
                }
                if (instance.DisplayName != null)
                {
                    StringProxy.Serialize(memoryStream, instance.DisplayName);
                }
                else
                {
                    num |= 2;
                }
                EnumProxy<GameBoxType>.Serialize(memoryStream, instance.BoxType);
                Int32Proxy.Serialize(memoryStream, instance.MapId);
                Int32Proxy.Serialize(memoryStream, instance.MaxPlayers);
                Int32Proxy.Serialize(memoryStream, instance.RecommendedItemId);
                if (instance.SceneName != null)
                {
                    StringProxy.Serialize(memoryStream, instance.SceneName);
                }
                else
                {
                    num |= 4;
                }
                if (instance.Settings != null)
                {
                    DictionaryProxy<GameModeType, MapSettingsView>.Serialize(memoryStream, instance.Settings, EnumProxy<GameModeType>.Serialize, MapSettingsViewProxy.Serialize);
                }
                else
                {
                    num |= 8;
                }
                Int32Proxy.Serialize(memoryStream, instance.SupportedGameModes);
                Int32Proxy.Serialize(memoryStream, instance.SupportedItemClass);
                Int32Proxy.Serialize(stream, ~num);
                memoryStream.WriteTo(stream);
            }
        }
    }
}
