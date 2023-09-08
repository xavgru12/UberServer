using System;
using System.Text;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class PlayerPersonalRecordStatisticsView
    {
        public PlayerPersonalRecordStatisticsView()
        {
            // Space
        }

        public PlayerPersonalRecordStatisticsView(int mostHeadshots, int mostNutshots, int mostConsecutiveSnipes, int mostXPEarned, int mostSplats, int mostDamageDealt, int mostDamageReceived, int mostArmorPickedUp, int mostHealthPickedUp, int mostMeleeSplats, int mostMachinegunSplats, int mostShotgunSplats, int mostSniperSplats, int mostSplattergunSplats, int mostCannonSplats, int mostLauncherSplats)
        {
            MostArmorPickedUp = mostArmorPickedUp;
            MostCannonSplats = mostCannonSplats;
            MostConsecutiveSnipes = mostConsecutiveSnipes;
            MostDamageDealt = mostDamageDealt;
            MostDamageReceived = mostDamageReceived;
            MostHeadshots = mostHeadshots;
            MostHealthPickedUp = mostHealthPickedUp;
            MostLauncherSplats = mostLauncherSplats;
            MostMachinegunSplats = mostMachinegunSplats;
            MostMeleeSplats = mostMeleeSplats;
            MostNutshots = mostNutshots;
            MostShotgunSplats = mostShotgunSplats;
            MostSniperSplats = mostSniperSplats;
            MostSplats = mostSplats;
            MostSplattergunSplats = mostSplattergunSplats;
            MostXPEarned = mostXPEarned;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            _ = builder.Append("[PlayerPersonalRecordStatisticsView: ");
            _ = builder.Append("[MostArmorPickedUp: ");
            _ = builder.Append(MostArmorPickedUp);
            _ = builder.Append("][MostCannonSplats: ");
            _ = builder.Append(MostCannonSplats);
            _ = builder.Append("][MostConsecutiveSnipes: ");
            _ = builder.Append(MostConsecutiveSnipes);
            _ = builder.Append("][MostDamageDealt: ");
            _ = builder.Append(MostDamageDealt);
            _ = builder.Append("][MostDamageReceived: ");
            _ = builder.Append(MostDamageReceived);
            _ = builder.Append("][MostHeadshots: ");
            _ = builder.Append(MostHeadshots);
            _ = builder.Append("][MostHealthPickedUp: ");
            _ = builder.Append(MostHealthPickedUp);
            _ = builder.Append("][MostLauncherSplats: ");
            _ = builder.Append(MostLauncherSplats);
            _ = builder.Append("][MostMachinegunSplats: ");
            _ = builder.Append(MostMachinegunSplats);
            _ = builder.Append("][MostMeleeSplats: ");
            _ = builder.Append(MostMeleeSplats);
            _ = builder.Append("][MostNutshots: ");
            _ = builder.Append(MostNutshots);
            _ = builder.Append("][MostShotgunSplats: ");
            _ = builder.Append(MostShotgunSplats);
            _ = builder.Append("][MostSniperSplats: ");
            _ = builder.Append(MostSniperSplats);
            _ = builder.Append("][MostSplats: ");
            _ = builder.Append(MostSplats);
            _ = builder.Append("][MostSplattergunSplats: ");
            _ = builder.Append(MostSplattergunSplats);
            _ = builder.Append("][MostXPEarned: ");
            _ = builder.Append(MostXPEarned);
            _ = builder.Append("]]");
            return builder.ToString();
        }

        public int MostArmorPickedUp { get; set; }
        public int MostCannonSplats { get; set; }
        public int MostConsecutiveSnipes { get; set; }
        public int MostDamageDealt { get; set; }
        public int MostDamageReceived { get; set; }
        public int MostHeadshots { get; set; }
        public int MostHealthPickedUp { get; set; }
        public int MostLauncherSplats { get; set; }
        public int MostMachinegunSplats { get; set; }
        public int MostMeleeSplats { get; set; }
        public int MostNutshots { get; set; }
        public int MostShotgunSplats { get; set; }
        public int MostSniperSplats { get; set; }
        public int MostSplats { get; set; }
        public int MostSplattergunSplats { get; set; }
        public int MostXPEarned { get; set; }
    }
}
