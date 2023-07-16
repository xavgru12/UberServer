using UberStrok.Core.Common;
using UberStrok.WebServices.AspNetCore.WebService;

namespace UberStrok.WebServices.AspNetCore.Helper
{
    public static class XpPointsUtil
    {
        public static int MaxPlayerLevel => ApplicationWebService.AppConfig.MaxLevel;

        public static void GetXpRangeForLevel(int level, out int minXp, out int maxXp)
        {
            level = MathUtils.Clamp(level, 1, MaxPlayerLevel);
            if (level < MaxPlayerLevel)
            {
                _ = ApplicationWebService.AppConfig.XpRequiredPerLevel.TryGetValue(level, out minXp);
                _ = ApplicationWebService.AppConfig.XpRequiredPerLevel.TryGetValue(level + 1, out maxXp);
            }
            else
            {
                _ = ApplicationWebService.AppConfig.XpRequiredPerLevel.TryGetValue(MaxPlayerLevel, out minXp);
                maxXp = minXp + 1;
            }
        }

        public static int GetLevelForXp(int xp)
        {
            for (int i = MaxPlayerLevel; i > 0; i--)
            {
                if (ApplicationWebService.AppConfig.XpRequiredPerLevel.TryGetValue(i, out int num) && xp >= num)
                {
                    return i;
                }
            }
            return 1;
        }
    }
}
