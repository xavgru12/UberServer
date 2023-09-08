using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Game.Helper
{
    public static class XpPointsUtil
    {
        public static ApplicationConfigurationView Config { get; set; }

        public static int MaxPlayerLevel => Config.MaxLevel;

        public static void Init()
        {
            Config = new UserWebServiceClient(Application.Instance.Configuration.WebServices).GetAppConfig();
        }

        public static void GetXpRangeForLevel(int level, out int minXp, out int maxXp)
        {
            level = MathUtils.Clamp(level, 1, MaxPlayerLevel);
            minXp = 0;
            maxXp = 0;
            if (level < MaxPlayerLevel)
            {
                Config.XpRequiredPerLevel.TryGetValue(level, out minXp);
                Config.XpRequiredPerLevel.TryGetValue(level + 1, out maxXp);
            }
            else
            {
                Config.XpRequiredPerLevel.TryGetValue(MaxPlayerLevel, out minXp);
                maxXp = minXp + 1;
            }
        }

        public static int GetLevelForXp(int xp)
        {
            for (int num = MaxPlayerLevel; num > 0; num--)
            {
                if (Config.XpRequiredPerLevel.TryGetValue(num, out var value) && xp >= value)
                {
                    return num;
                }
            }
            return 1;
        }
    }

}
