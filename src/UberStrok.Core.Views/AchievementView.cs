using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class AchievementView
    {
        public int AchievementID { get; set; }
        public string AchievementName { get; set; }
        public string AchievementCondition { get; set; }
        public string RewardDescription { get; set; }
        public int MaxCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
