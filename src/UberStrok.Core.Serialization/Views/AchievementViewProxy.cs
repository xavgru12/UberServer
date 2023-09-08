using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class AchievementViewProxy
    {
        public static void Serialize(Stream stream, AchievementView Instance)
        {
            Int32Proxy.Serialize(stream, Instance.AchievementID);
            StringProxy.Serialize(stream, Instance.AchievementName);
            StringProxy.Serialize(stream, Instance.AchievementCondition);
            StringProxy.Serialize(stream, Instance.RewardDescription);
            Int32Proxy.Serialize(stream, Instance.MaxCount);
            BooleanProxy.Serialize(stream, Instance.IsAvailable);
        }

        public static AchievementView Deserialize(Stream stream)
        {
            return new AchievementView
            {
                AchievementID = Int32Proxy.Deserialize(stream),
                AchievementName = StringProxy.Deserialize(stream),
                AchievementCondition = StringProxy.Deserialize(stream),
                RewardDescription = StringProxy.Deserialize(stream),
                MaxCount = Int32Proxy.Deserialize(stream),
                IsAvailable = BooleanProxy.Deserialize(stream)
            };
        }
    }
}
