using System;
using System.Text;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class PlayerStatisticsView
    {
        public PlayerStatisticsView()
        {
            PersonalRecord = new PlayerPersonalRecordStatisticsView();
            WeaponStatistics = new PlayerWeaponStatisticsView();
        }

        public PlayerStatisticsView(int cmid, int splats, int splatted, long shots, long hits, int headshots, int nutshots, PlayerPersonalRecordStatisticsView personalRecord, PlayerWeaponStatisticsView weaponStatistics)
        {
            Cmid = cmid;
            Hits = hits;
            Level = 0;
            Shots = shots;
            Splats = splats;
            Splatted = splatted;
            Headshots = headshots;
            Nutshots = nutshots;
            Xp = 0;
            PersonalRecord = personalRecord;
            WeaponStatistics = weaponStatistics;
        }

        public PlayerStatisticsView(int cmid, int splats, int splatted, long shots, long hits, int headshots, int nutshots, int xp, int level, PlayerPersonalRecordStatisticsView personalRecord, PlayerWeaponStatisticsView weaponStatistics)
        {
            Cmid = cmid;
            Hits = hits;
            Level = level;
            Shots = shots;
            Splats = splats;
            Splatted = splatted;
            Headshots = headshots;
            Nutshots = nutshots;
            Xp = xp;
            PersonalRecord = personalRecord;
            WeaponStatistics = weaponStatistics;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            _ = builder.Append("[PlayerStatisticsView: ");
            _ = builder.Append("[Cmid: ");
            _ = builder.Append(Cmid);
            _ = builder.Append("][Hits: ");
            _ = builder.Append(Hits);
            _ = builder.Append("][Level: ");
            _ = builder.Append(Level);
            _ = builder.Append("][Shots: ");
            _ = builder.Append(Shots);
            _ = builder.Append("][Splats: ");
            _ = builder.Append(Splats);
            _ = builder.Append("][Splatted: ");
            _ = builder.Append(Splatted);
            _ = builder.Append("][Headshots: ");
            _ = builder.Append(Headshots);
            _ = builder.Append("][Nutshots: ");
            _ = builder.Append(Nutshots);
            _ = builder.Append("][Xp: ");
            _ = builder.Append(Xp);
            _ = builder.Append("]");
            _ = builder.Append(PersonalRecord);
            _ = builder.Append(WeaponStatistics);
            _ = builder.Append("]");
            return builder.ToString();
        }

        public int Cmid { get; set; }
        public int Headshots { get; set; }
        public long Hits { get; set; }
        public int Level { get; set; }
        public int Nutshots { get; set; }
        public PlayerPersonalRecordStatisticsView PersonalRecord { get; set; }
        public long Shots { get; set; }
        public int Splats { get; set; }
        public int Splatted { get; set; }
        public int TimeSpentInGame { get; set; }
        public PlayerWeaponStatisticsView WeaponStatistics { get; set; }
        public int Xp { get; set; }
    }
}
