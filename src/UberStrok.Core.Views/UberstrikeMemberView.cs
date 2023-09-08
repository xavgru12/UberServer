using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class UberstrikeMemberView
    {
        public UberstrikeMemberView()
        {
            // Space
        }

        public UberstrikeMemberView(PlayerCardView playerCardView, PlayerStatisticsView playerStatisticsView)
        {
            PlayerCardView = playerCardView;
            PlayerStatisticsView = playerStatisticsView;
        }

        public override string ToString()
        {
            string str = "[Uberstrike member view: ";
            if (PlayerCardView != null)
            {
                str += PlayerCardView.ToString();
            }
            else
            {
                str += "null";
            }

            if (PlayerStatisticsView != null)
            {
                str += PlayerStatisticsView.ToString();
            }
            else
            {
                str += "null";
            }

            return str + "]";
        }

        public PlayerCardView PlayerCardView { get; set; }
        public PlayerStatisticsView PlayerStatisticsView { get; set; }
    }
}
