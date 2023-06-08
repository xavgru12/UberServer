﻿using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class MemberAuthenticationResultView
    {
        public string AuthToken { get; set; }
        public bool IsAccountComplete { get; set; }
        public LuckyDrawUnityView LuckyDraw { get; set; }
        public MemberAuthenticationResult MemberAuthenticationResult { get; set; }
        public MemberView MemberView { get; set; }
        public PlayerStatisticsView PlayerStatisticsView { get; set; }
        public DateTime ServerTime { get; set; }
        public string ServerGameVersion { get; set; }
        public int BanDuration { get; set; } = 0;
        public int MuteDuration { get; set; } = 0;
    }
}
