using UberStrok.Core.Common;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Core.Session;
using UberStrok.WebServices.AspNetCore.WebService.Base;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class ModerationWebService : BaseModerationWebService
    {
        private readonly SecurityManager securityManager;
        private readonly GameSessionManager gameSessionManager;
        public ModerationWebService(SecurityManager securityManager, GameSessionManager gameSessionManager)
        {
            this.securityManager = securityManager;
            this.gameSessionManager = gameSessionManager;
        }
        public override int OnBan(string serviceAuth, int cmid)
        {
            securityManager.BanCmid(cmid);
            if (gameSessionManager.TryGet(cmid, out GameSession session))
            {
                if (session.IPAddress != null)
                {
                    securityManager.BanIp(session.IPAddress);
                }
                if (session.MachineId != null)
                {
                    securityManager.BanHwd(session.MachineId);
                }
            }
            return 0;
        }

        public override int OnBanIp(string authToken, string ip)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Member.PublicProfile.AccessLevel < MemberAccessLevel.SeniorModerator)
                {
                    return 1;
                }
                securityManager.BanIp(ip);
            }
            return 0;
        }

        public override int OnBanCmid(string authToken, int cmid)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Member.PublicProfile.AccessLevel < MemberAccessLevel.SeniorQA)
                {
                    return 1;
                }
                securityManager.BanCmid(cmid);
            }
            return 0;
        }

        public override int OnBanHwd(string authToken, string hwd)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Member.PublicProfile.AccessLevel < MemberAccessLevel.SeniorModerator)
                {
                    return 1;
                }
                securityManager.BanHwd(hwd);
            }
            return 0;
        }

        public override int OnUnbanCmid(string authToken, int cmid)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Member.PublicProfile.AccessLevel < MemberAccessLevel.SeniorQA)
                {
                    return 1;
                }
                securityManager.UnbanCmid(cmid);
            }
            return 0;
        }
    }
}
