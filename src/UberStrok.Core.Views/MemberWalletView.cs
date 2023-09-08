using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class MemberWalletView
    {
        public MemberWalletView()
        {
            CreditsExpiration = DateTime.MaxValue;
            PointsExpiration = DateTime.MaxValue;
        }

        public MemberWalletView(int cmid, int? credits, int? points, DateTime? creditsExpiration, DateTime? pointsExpiration)
        {
            if (!credits.HasValue)
            {
                credits = new int?(0);
            }

            if (!points.HasValue)
            {
                points = new int?(0);
            }

            if (!creditsExpiration.HasValue)
            {
                creditsExpiration = new DateTime?(DateTime.MaxValue);
            }

            if (!pointsExpiration.HasValue)
            {
                pointsExpiration = new DateTime?(DateTime.MaxValue);
            }

            SetMemberWallet(cmid, credits.Value, points.Value, creditsExpiration.Value, pointsExpiration.Value);
        }

        public MemberWalletView(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
        {
            SetMemberWallet(cmid, credits, points, creditsExpiration, pointsExpiration);
        }

        private void SetMemberWallet(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
        {
            Cmid = cmid;
            Credits = credits;
            Points = points;
            CreditsExpiration = creditsExpiration;
            PointsExpiration = pointsExpiration;
        }

        public override string ToString()
        {
            string text = "[Wallet: ";
            string text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "[CMID:",
                Cmid,
                "][Credits:",
                Credits,
                "][Credits Expiration:",
                CreditsExpiration,
                "][Points:",
                Points,
                "][Points Expiration:",
                PointsExpiration,
                "]"
            });
            return text + "]";
        }

        public int Cmid { get; set; }
        public int Credits { get; set; }
        public DateTime CreditsExpiration { get; set; }
        public int Points { get; set; }
        public DateTime PointsExpiration { get; set; }
    }
}
