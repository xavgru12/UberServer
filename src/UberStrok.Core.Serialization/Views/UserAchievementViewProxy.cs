using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class UserAchievementViewProxy
    {
        public static void Serialize(Stream stream, UserAchievementView Instance)
        {
            Int32Proxy.Serialize(stream, Instance.AchievementID);
            Int32Proxy.Serialize(stream, Instance.ProgressCount);
            BooleanProxy.Serialize(stream, Instance.IsClaimed);
        }

        public static UserAchievementView Deserialize(Stream stream)
        {
            return new UserAchievementView
            {
                AchievementID = Int32Proxy.Deserialize(stream),
                ProgressCount = Int32Proxy.Deserialize(stream),
                IsClaimed = BooleanProxy.Deserialize(stream)
            };
        }
    }
}
