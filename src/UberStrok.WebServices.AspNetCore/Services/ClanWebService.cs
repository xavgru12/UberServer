using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class ClanWebService : BaseClanWebService
    {
        private readonly IDbService _database;
        private readonly ISessionService _sessions;
        private readonly ILogger<ClanWebService> _logger;
        private readonly Regex ClanRegex = new Regex("[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?]", RegexOptions.Compiled);

        public ClanWebService(ILogger<ClanWebService> logger, IDbService database, ISessionService sessions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
        }
       
        public override async Task<ClanRequestAcceptView> OnAcceptClanInvitation(int clanInvitationId, string authToken)
        {
            try
            {
                var data = await _database.ClanInvitations.FindAsync(clanInvitationId);
                if (data != null)
                {
                    var clan = await _database.Clans.FindAsync(data.View.GroupId);
                    var member = await _sessions.GetMemberAsync(authToken);
                    clan.Members.Add(new ClanMember(member)
                    {
                        JoinDate = DateTime.Now
                    });
                    member.Clan = clan;
                    member.ClanId = clan.Id;
                    _ = _database.Clans.UpdateAsync(clan);
                    _ = _database.ClanInvitations.DeleteAsync(data);
                    _ = _database.Members.UpdateAsync(member);
                    return new ClanRequestAcceptView
                    {
                        ActionResult = 0,
                        ClanView = new ClanView()
                        {
                            MembersCount = clan.Members.Count,
                            OwnerCmid = clan.LeaderId,
                            Name = clan.Name,
                            Motto = clan.Motto,
                            GroupId = clan.Id,
                            Tag = clan.Tag,
                            Members = clan.Members.Select(x =>
                            {
                                var member = _database.Members.FindAsync(x.CmId).Result;
                                return new ClanMemberView
                                {
                                    Cmid = x.CmId,
                                    Name = member.Name,
                                    Lastlogin = member.LastLogin,
                                    JoiningDate = x.JoinDate
                                };
                            }).ToList()

                        }
                    };
                }
            }
            catch
            {

            }
            return new ClanRequestAcceptView
            {
                ActionResult = 69
            };
        }

        public override async Task<int> OnCancelInvite(int clanInvitationId, string authToken)
        {
            var data = await _database.ClanInvitations.FindAsync(clanInvitationId);
            if(data != null)
            {
                _ = _database.ClanInvitations.DeleteAsync(data);
            }
            return 0;
        }

        public override async Task<ClanCreationReturnView> OnCreateClan(GroupCreationView creationView)
        {
            var session = await _sessions.GetMemberAsync(creationView.AuthToken);
            if(session == null)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 69
                };
            }
            var clans = (await _database.Clans.Where(x => x.Name == creationView.Name || x.Tag == creationView.Tag )).ToList();
            if (!ValidName(creationView.Name) || creationView.Name.Length < 3 || creationView.Name.Length > 25)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 1
                };
            }
            if (session.ClanId != 0)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 2
                };
            }
            if(clans.Any(x => x.Name == creationView.Name))
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 3
                };
            }
            if(!ValidName(creationView.Tag) || creationView.Tag.Length < 2 || creationView.Tag.Length > 5)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 4
                };
            }
            if (clans.Any(x => x.Tag == creationView.Tag))
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 10
                };
            }
            if (!ValidName(creationView.Motto) || creationView.Motto.Length < 3 || creationView.Motto.Length > 25)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 8
                };
            }
            var clan = new Clan
            {
                LeaderId = session.Id,
                Name = creationView.Name,
                Tag = creationView.Tag,
                Motto = creationView.Motto,
                Members = new List<ClanMember>()
            };
            clan.Members.Add(new ClanMember(session)
            {
               JoinDate = DateTime.Now
            });
            session.ClanId = clan.Id;
            session.Clan = clan;
            _ = _database.Members.UpdateAsync(session);
            _ = _database.Clans.InsertAsync(clan);
            return new ClanCreationReturnView
            {
                ClanView = new ClanView
                {
                    MembersCount = clan.Members.Count,
                    OwnerCmid = clan.LeaderId,
                    Name = clan.Name,
                    Motto = clan.Motto,
                    GroupId = clan.Id,
                    Tag = clan.Tag,
                    Members = clan.Members.Select(x =>
                    {
                        var member = _database.Members.FindAsync(x.CmId).Result;
                        return new ClanMemberView
                        {
                            Cmid = x.CmId,
                            Name = member.Name,
                            Lastlogin = member.LastLogin,
                            JoiningDate = x.JoinDate
                        };
                    }).ToList(),
                }
            };
        }

        private bool ValidName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return ClanRegex.IsMatch(name);
            }
            return false;
        }

        public override async Task<ClanRequestDeclineView> OnDeclineClanInvitation(int clanInvitationId, string authToken)
        {
            await OnCancelInvite(clanInvitationId, authToken);
            return new ClanRequestDeclineView();
        }

        public override async Task<int> OnDisbandClan(int groupId, string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            if(member == null || member.ClanId == null || member.ClanId.Value != groupId)
            {
                return 69;
            }
            if(member.ClanId == groupId)
            {
                var clan = await _database.Clans.FindAsync(member.ClanId.Value);
                if(clan != null)
                {
                    foreach(var mem in clan.Members)
                    {
                        var m = await _database.Members.FindAsync(mem.CmId);
                        m.Clan = null;
                        m.ClanId = null;
                        _ = _database.Members.UpdateAsync(m);
                    }
                    _ = _database.Clans.DeleteAsync(clan);
                    return 0;
                }
            }
            return 69;
        }

        public override async Task<List<GroupInvitationView>> OnGetAllGroupInvitations(string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            return (await _database.ClanInvitations.Where(x => x.View.InviteeCmid == member.Id)).Select(x =>
            {
                x.View.GroupInvitationId = x.Id;
                return x.View;
            }).ToList();
        }

        public override async Task<int> OnGetMyClanId(string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            if(member == null || member.ClanId == null)
            {
                return 0;
            }
            return member.ClanId.Value;
        }

        public override async Task<ClanView> OnGetOwnClan(string authToken, int groupId)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            if (member == null || member.ClanId == null || member.ClanId.Value != groupId)
            {
                return null;
            }
            var clan = (await _database.Clans.Where(x => x.Id == member.ClanId)).FirstOrDefault();
            return new ClanView
            {
                MembersCount = clan.Members.Count,
                OwnerCmid = clan.LeaderId,
                Name = clan.Name,
                Motto = clan.Motto,
                GroupId = clan.Id,
                Tag = clan.Tag,
                Members = clan.Members.Select(x =>
                {
                    var member = _database.Members.FindAsync(x.CmId).Result;
                    return new ClanMemberView
                    {
                        Cmid = x.CmId,
                        Name = member.Name,
                        Lastlogin = member.LastLogin,
                        JoiningDate = x.JoinDate
                    };
                }).ToList(),
            };
        }

        public override async Task<List<GroupInvitationView>> OnGetPendingGroupInvitations(int groupId, string authToken)
        {
            return (await OnGetAllGroupInvitations(authToken)).Where(x => x.GroupId == groupId).ToList();
        }

        public override async Task<int> OnInviteMemberToJoinAGroup(int clanId, string authToken, int invitee, string message)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            if (member == null || member.ClanId == null)
            {
                return -1;
            }
            var clan = await _database.Clans.FindAsync(clanId);
            var invmem = await _database.Members.FindAsync(invitee);
            if(invmem == null || clan == null)
            {
                return -1;
            }
            if(!await _database.ClanInvitations.IsInvited(clanId, invitee))
            {
                await _database.ClanInvitations.InsertAsync(new GroupInvitation
                {
                    View = new GroupInvitationView { 
                        GroupId = clan.Id,
                        GroupName = clan.Name,
                        GroupTag = clan.Tag,
                        InviterCmid = member.Id,
                        InviteeName = member.Name,
                        Message = message,
                        InviteeCmid = invitee,
                        InviterName = invmem.Name
                    }
                });
                return 0;
            }
            return -1;
        }

        public override async Task<int> OnKick(int groupId, string authToken, int cmidToKick)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            var clan = await _database.Clans.FindAsync(groupId);
            if(member == null || clan == null || member.ClanId.Value != groupId)
            {
                return 69;
            }
            var clanMem = clan.Members.FirstOrDefault(x => x.CmId == member.Id);
            var kickMem = clan.Members.FirstOrDefault(x => x.CmId == cmidToKick);
            var kickable = false;
            switch (clanMem.Position)
            {
                case GroupPosition.Officer:
                    kickable = kickMem.Position == GroupPosition.Member;
                    break;
                case GroupPosition.Leader:
                    kickable = kickMem.Position != GroupPosition.Leader;
                    break;
            }
            if (kickable)
            {
                clan.Members.Remove(kickMem);
                var mem = await _database.Members.FindAsync(kickMem.CmId);
                mem.Clan = null;
                mem.ClanId = null;
                _ = _database.Members.UpdateAsync(mem);
                _ = _database.Clans.UpdateAsync(clan);
                return 0;
            }
            return 69;
        }

        public override async Task<int> OnLeaveClan(int groupId, string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            if(member == null || member.ClanId == null || member.ClanId.Value != groupId)
            {
                return 69;
            }
            var clan = await _database.Clans.FindAsync(groupId);
            var leaveMem = clan.Members.FirstOrDefault(x => x.CmId == member.Id);
            if(leaveMem == null)
            {
                return 69;
            }
            clan.Members.Remove(leaveMem);
            member.Clan = null;
            member.ClanId = null;
            _ = _database.Members.UpdateAsync(member);
            _ = _database.Clans.UpdateAsync(clan);
            return 0;
        }

        public override async Task<int> OnTransferOwnership(int groupId, string authToken, int newLeaderCmid)
        {
            //lazy implement this shit
            return 69;
        }

        public override async Task<int> OnUpdateMemberPosition(MemberPositionUpdateView memberPositionUpdate)
        {
            var member = await _sessions.GetMemberAsync(memberPositionUpdate.AuthToken);
            if(member == null || member.ClanId != memberPositionUpdate.GroupId)
            {
                return 69;
            }
            var clan = await _database.Clans.FindAsync(member.ClanId.Value);
            if(clan == null)
            {
                return -1;
            }
            var executor = clan.Members.FirstOrDefault(x => x.CmId == member.Id);
            var target = clan.Members.FirstOrDefault(x => x.CmId == memberPositionUpdate.MemberCmid);
            if(executor == null || target == null)
            {
                return 69;
            }
            var executable = false;
            switch (executor.Position)
            {
                case GroupPosition.Officer:
                    executable = target.Position == GroupPosition.Member;
                    break;
                case GroupPosition.Leader:
                    executable = target.Position != GroupPosition.Leader;
                    break;
            }
            if (executable)
            {
                target.Position = memberPositionUpdate.Position;
                _ = _database.Clans.UpdateAsync(clan);
                return 0;
            }
            return -1;
        }
    }
}
