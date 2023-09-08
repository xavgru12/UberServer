using System;
using System.Collections.Generic;
namespace UberStrok.Core.Views
{
    [Serializable]
    public class DailyBonusView
    {
        public int Streak { get; set; }
        public int RewardClass { get; set; } //-1 for points, -2 for credits, >0 for items
        public string Label { get; set; }
        public int Amount { get; set; } // >0 for coins, credits and consumables. -1 for weapons and gears
        public int Duration { get; set; } //0 for points, credits. >0 for temp items, -1 for permanent
    }

    public class DailyBonuses
    {
        public int ID { get; set; }
        public List<DailyBonusView> BonusView { get; set; }
    }
}
