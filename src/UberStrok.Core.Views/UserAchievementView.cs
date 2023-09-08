using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class UserAchievementView
    {
        public int AchievementID { get; set; }
        public int ProgressCount { get; set; }
        public bool IsClaimed { get; set; }
    }
}
