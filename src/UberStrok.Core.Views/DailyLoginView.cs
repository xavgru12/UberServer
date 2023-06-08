using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class DailyLoginDocument
    {
        public DailyLoginDocument(int bonusID)
        {
            BonusID = bonusID;
            LastClaimDate = DateTime.Now;
            Streak = 0;
        }
        public int BonusID { get; set; }
        public DateTime LastClaimDate { get; set; }
        public int Streak { get; set; }
    }
}
