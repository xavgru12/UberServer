using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
  public class MemberAuthenticationResultView
  {
    public MemberAuthenticationResult MemberAuthenticationResult { get; set; }

    public MemberView MemberView { get; set; }

    public PlayerStatisticsView PlayerStatisticsView { get; set; }

    public DateTime ServerTime { get; set; }

    public bool IsAccountComplete { get; set; }

    public bool IsTutorialComplete { get; set; }

    public WeeklySpecialView WeeklySpecial { get; set; }

    public LuckyDrawUnityView LuckyDraw { get; set; }
  }
}
