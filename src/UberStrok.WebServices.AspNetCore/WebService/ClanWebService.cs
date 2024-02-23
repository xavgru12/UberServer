using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Core.Session;
using UberStrok.WebServices.AspNetCore.Helper;
using UberStrok.WebServices.AspNetCore.WebService.Base;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class ClanWebService : BaseClanWebService
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(ClanWebService));

        private readonly Regex ClanRegex = new Regex("[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?]", RegexOptions.Compiled);

        private readonly GameSessionManager gameSessionManager;
        private readonly ClanManager clanManager;
        private readonly UserManager userManager;
        private readonly StreamManager streamManager;
        private readonly ResourceManager resourceManager;
        public ClanWebService(GameSessionManager gameSessionManager, ClanManager clanManager, UserManager userManager, StreamManager streamManager, ResourceManager resourceManager)
        {
            this.clanManager = clanManager;
            this.gameSessionManager = gameSessionManager;
            this.userManager = userManager;
            this.streamManager = streamManager;
            this.resourceManager = resourceManager;
        }

        public override async Task<ClanView> OnGetOwnClan(string authToken, int groupId)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.ClanId == groupId)
                {
                    ClanDocument clan = await clanManager.Get(groupId);
                    if (clan != null)
                    {
                        return clan.Clan;
                    }
                    Log.Error("ClanManager.Get returned null");
                }
                else
                {
                    Log.Error("Mismatch between groupId and session.Document.ClanId.");
                }
            }
            else
            {
                Log.Error("An unidentified AuthToken was passed.");
            }
            return null;
        }

        public override async Task<ClanCreationReturnView> OnCreateClan(GroupCreationView groupCreation)
        {
            if (!gameSessionManager.TryGet(groupCreation.AuthToken, out GameSession session))
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 69
                };
            }
            if (!ValidString(groupCreation.Name) || groupCreation.Name.Length < 3 || groupCreation.Name.Length > 25)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 1
                };
            }
            if (resourceManager.LockedTags.IsTagLocked(session.Document.UserId, groupCreation.Tag))
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 690
                };
            }
            if (session.Document.ClanId != 0)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 2
                };
            }
            if (await clanManager.IsClanNameUsed(groupCreation.Name))
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 3
                };
            }
            if (!ValidString(groupCreation.Tag) || groupCreation.Tag.Length < 2 || groupCreation.Tag.Length > 5)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 4
                };
            }
            if (await clanManager.IsClanTagUsed(groupCreation.Tag))
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 10
                };
            }
            if (!ValidString(groupCreation.Motto) || groupCreation.Motto.Length < 3 || groupCreation.Motto.Length > 25)
            {
                return new ClanCreationReturnView
                {
                    ResultCode = 8
                };
            }
            ClanDocument document = await clanManager.Create(groupCreation, session.Document.Profile);
            if (document.Clan.AddMemberToClan(ClanMemberHelper.GetClanMemberView(session.Document, GroupPosition.Leader)))
            {
                session.Document.ClanId = document.ClanId;
                session.Document.Profile.GroupTag = document.Clan.Tag;
                await userManager.Save(session.Document);
                await clanManager.Save(document);
                return new ClanCreationReturnView
                {
                    ClanView = document.Clan
                };
            }
            return new ClanCreationReturnView
            {
                ResultCode = 12345
            };
        }

        public override async Task<List<GroupInvitationView>> OnGetAllGroupInvitations(string authToken)
        {
            return gameSessionManager.TryGet(authToken, out GameSession session)
                ? (await streamManager.Get(session.Document.Streams)).Where((f) => f.StreamType == StreamType.GroupInvitation && ((GroupInvitationStream)f).GroupInvitation.InviteeCmid == session.Document.UserId).Select((s) => ((GroupInvitationStream)s).GroupInvitation).ToList()
                : null;
        }

        public override async Task<List<GroupInvitationView>> OnGetPendingGroupInvitations(int groupId, string authToken)
        {
            return gameSessionManager.TryGet(authToken, out GameSession session)
                ? (await streamManager.Get(session.Document.Streams)).Where((f) => f.StreamType == StreamType.GroupInvitation && ((GroupInvitationStream)f).GroupInvitation.InviteeCmid != session.Document.UserId).Select((s) => ((GroupInvitationStream)s).GroupInvitation).ToList()
                : null;
        }

        public override async Task<int> OnUpdateMemberPosition(MemberPositionUpdateView memberPositionUpdate)
        {
            if (!gameSessionManager.TryGet(memberPositionUpdate.AuthToken, out GameSession session))
            {
                Log.Error("An unidentified AuthToken was passed.");
                return -1;
            }
            if (session.Document.ClanId != memberPositionUpdate.GroupId)
            {
                Log.Error("Mismatch between groupId and session.Document.ClanId.");
                return -1;
            }
            ClanDocument document = await clanManager.Get(memberPositionUpdate.GroupId);
            if (document != null)
            {
                ClanMemberView executorMemberView = document.Clan.GetMember(session.Document.UserId);
                ClanMemberView targetMemberView = document.Clan.GetMember(memberPositionUpdate.MemberCmid);
                if (executorMemberView != null && targetMemberView != null)
                {
                    if (executorMemberView.HasHigherPermissionThan(targetMemberView.Position))
                    {
                        targetMemberView.Position = memberPositionUpdate.Position;
                        await clanManager.Save(document);
                        return 0;
                    }
                    Log.Error("executorMemberView.HasHigherPermissionThan returned false");
                }
                else
                {
                    Log.Error("executorMemberView or targetMemberView is null");
                }
            }
            else
            {
                Log.Error("ClanManager.Get returned false");
            }
            return -1;
        }

        public override async Task<int> OnTransferOwnership(int groupId, string authToken, int newLeaderCmid)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.ClanId == groupId)
                {
                    ClanDocument document = await clanManager.Get(groupId);
                    if (document != null)
                    {
                        ClanMemberView executorMemberView = document.Clan.GetMember(session.Document.UserId);
                        ClanMemberView newLeaderMemberView = document.Clan.GetMember(newLeaderCmid);
                        if (executorMemberView != null && newLeaderMemberView != null)
                        {
                            newLeaderMemberView.Position = GroupPosition.Leader;
                            executorMemberView.Position = GroupPosition.Member;
                            document.Clan.OwnerName = newLeaderMemberView.Name;
                            document.Clan.OwnerCmid = newLeaderMemberView.Cmid;
                            _ = clanManager.Save(document);
                            return 0;
                        }
                        Log.Error("executorMemberView or newLeaderMemberView is null");
                    }
                    else
                    {
                        Log.Error("ClanManager.Get returned false");
                    }
                }
                else
                {
                    Log.Error("Mismatch between groupId and session.Document.ClanId.");
                }
            }
            else
            {
                Log.Error("An unidentified AuthToken was passed.");
            }
            return 69;
        }

        public override async Task<int> OnKick(int groupId, string authToken, int cmidToKick)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.ClanId == groupId)
                {
                    ClanDocument document = await clanManager.Get(groupId);
                    if (document != null)
                    {
                        ClanMemberView executorMemberView = document.Clan.GetMember(session.Document.UserId);
                        ClanMemberView userMemberView = document.Clan.GetMember(cmidToKick);
                        if (executorMemberView != null && userMemberView != null)
                        {
                            if (executorMemberView.HasHigherPermissionThan(userMemberView))
                            {
                                if (document.Clan.RemoveMemberFromClan(userMemberView))
                                {
                                    UserDocument userDocument = gameSessionManager.TryGet(userMemberView.Cmid, out GameSession userSession) ? userSession.Document : userManager.GetUser(userMemberView.Cmid).Result;
                                    userDocument.ClanId = 0;
                                    userDocument.Profile.GroupTag = string.Empty;
                                    _ = userManager.Save(userDocument);
                                    _ = clanManager.Save(document);
                                    return 0;
                                }
                            }
                            else
                            {
                                Log.Error("executorMemberView.HasHigherPermissionThan returned false");
                            }
                        }
                        else
                        {
                            Log.Error("executorMemberView or userMemberView is null");
                        }
                    }
                    else
                    {
                        Log.Error("ClanManager.Get returned false");
                    }
                }
                else
                {
                    Log.Error("Mismatch between groupId and session.Document.ClanId.");
                }
            }
            else
            {
                Log.Error("An unidentified AuthToken was passed.");
            }
            return 69;
        }

        public override async Task<int> OnLeaveClan(int groupId, string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.ClanId == groupId)
                {
                    ClanDocument document = await clanManager.Get(groupId);
                    if (document != null)
                    {
                        ClanMemberView userMemberView = document.Clan.GetMember(session.Document.UserId);
                        if (document.Clan.RemoveMemberFromClan(userMemberView))
                        {
                            if (gameSessionManager.TryGet(userMemberView.Cmid, out GameSession userSession))
                            {
                                userSession.Document.ClanId = 0;
                                userSession.Document.Profile.GroupTag = string.Empty;
                                await userManager.Save(userSession.Document);
                                await clanManager.Save(document);
                                return 0;
                            }
                            Log.Error("Received leave clan while user doesn't have any active session");
                            return -1;
                        }
                        Log.Error("document.Clan.RemoveMemberFromClan returned false");
                    }
                    else
                    {
                        Log.Error("ClanManager.Get returned false");
                    }
                }
                else
                {
                    Log.Error("Mismatch between groupId and session.Document.ClanId.");
                }
            }
            else
            {
                Log.Error("An unidentified AuthToken was passed.");
            }
            return 69;
        }

        public override async Task<int> OnDisbandClan(int groupId, string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.ClanId == groupId)
                {
                    ClanDocument document = await clanManager.Get(groupId);
                    if (document != null)
                    {
                        for (int i = 0; i < document.Clan.Members.Count; i++)
                        {
                            ClanMemberView userMemberView = document.Clan.Members[i];
                            UserDocument userDocument = gameSessionManager.TryGet(userMemberView.Cmid, out GameSession userSession) ? userSession.Document : userManager.GetUser(userMemberView.Cmid).Result;
                            userDocument.ClanId = 0;
                            userDocument.Profile.GroupTag = string.Empty;
                            _ = userManager.Save(userDocument);
                        }
                        await clanManager.Remove(document);
                        return 0;
                    }
                    Log.Error("ClanManager.Get returned false");
                }
                else
                {
                    Log.Error("Mismatch between groupId and session.Document.ClanId.");
                }
            }
            else
            {
                Log.Error("An unidentified AuthToken was passed.");
            }
            return 69;
        }

        public override async Task<int> OnInviteMemberToJoinAGroup(int clanId, string authToken, int invitee, string message)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                ClanDocument clanDocument = await clanManager.Get(clanId);
                if (clanDocument != null)
                {
                    UserDocument userDocument = session.Document;
                    if (!userDocument.ClanRequests.Contains(clanId))
                    {
                        int streamId = await streamManager.GetNextId();
                        UserDocument inviteeDocument = gameSessionManager.TryGet(invitee, out GameSession userSession) ? userSession.Document : userManager.GetUser(invitee).Result;
                        GroupInvitationStream groupInvitation = new GroupInvitationStream
                        {
                            StreamId = streamId,
                            GroupInvitation = new GroupInvitationView
                            {
                                GroupInvitationId = streamId,
                                GroupId = clanId,
                                GroupName = clanDocument.Clan.Name,
                                GroupTag = clanDocument.Clan.Tag,
                                InviterCmid = userDocument.UserId,
                                InviterName = userDocument.Profile.Name,
                                InviteeCmid = inviteeDocument.UserId,
                                InviteeName = inviteeDocument.Profile.Name,
                                Message = message
                            }
                        };
                        await streamManager.Create(groupInvitation);
                        userDocument.Streams.Add(streamId);
                        inviteeDocument.Streams.Add(streamId);
                        await userManager.Save(userDocument);
                        await userManager.Save(inviteeDocument);
                        return 0;
                    }
                }
            }
            return -1;
        }

        public override async Task<ClanRequestAcceptView> OnAcceptClanInvitation(int clanInvitationId, string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.ClanId == 0)
                {
                    StreamDocument document = await streamManager.Get(clanInvitationId);
                    if (document != null && document.StreamType == StreamType.GroupInvitation)
                    {
                        GroupInvitationView groupInvitation = ((GroupInvitationStream)document).GroupInvitation;
                        ClanDocument clanDocument = await clanManager.Get(groupInvitation.GroupId);
                        if (clanDocument != null)
                        {
                            if (clanDocument.Clan.AddMemberToClan(ClanMemberHelper.GetClanMemberView(session.Document)))
                            {
                                UserDocument user = !gameSessionManager.TryGet(groupInvitation.InviterCmid, out GameSession friendSession) ? userManager.GetUser(groupInvitation.InviterCmid).Result : friendSession.Document;
                                session.Document.ClanId = clanDocument.ClanId;
                                session.Document.Profile.GroupTag = clanDocument.Clan.Tag;
                                _ = session.Document.Streams.Remove(groupInvitation.GroupInvitationId);
                                _ = user.Streams.Remove(groupInvitation.GroupInvitationId);
                                _ = await Task.WhenAny(userManager.Save(user), userManager.Save(session.Document), clanManager.Save(clanDocument));
                                return new ClanRequestAcceptView
                                {
                                    ActionResult = 0,
                                    ClanView = clanDocument.Clan
                                };
                            }
                            Log.Error("Unable to accept clan invite. Unable to add user to clan");
                        }
                        else
                        {
                            Log.Error("Unable to accept clan invite. Unable to get clan stream");
                        }
                    }
                    else
                    {
                        Log.Error("Unable to accept clan invite. Unable to verify stream");
                    }
                }
                else
                {
                    Log.Error("Unable to accept clan invite. Clan id is not zero");
                }
            }
            else
            {
                Log.Error("Unable to accept clan invite. Game session doesn't exist");
            }
            return new ClanRequestAcceptView
            {
                ActionResult = 69
            };
        }

        public override async Task<ClanRequestDeclineView> OnDeclineClanInvitation(int clanInvitationId, string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                StreamDocument document = await streamManager.Get(clanInvitationId);
                if (document != null && document.StreamType == StreamType.GroupInvitation)
                {
                    GroupInvitationView groupInvitation = ((GroupInvitationStream)document).GroupInvitation;
                    UserDocument inviterDocument = gameSessionManager.TryGet(groupInvitation.InviterCmid, out GameSession userSession) ? userSession.Document : userManager.GetUser(groupInvitation.InviterCmid).Result;
                    _ = inviterDocument.ClanRequests.RemoveAll((T) => T == groupInvitation.GroupId);
                    _ = inviterDocument.Streams.RemoveAll((T) => T == groupInvitation.GroupInvitationId);
                    _ = session.Document.Streams.RemoveAll((T) => T == groupInvitation.GroupInvitationId);
                    _ = userManager.Save(inviterDocument);
                    _ = userManager.Save(session.Document);
                }
            }
            return new ClanRequestDeclineView();
        }

        public override async Task<int> OnCancelInvite(int clanInvitationId, string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                StreamDocument document = await streamManager.Get(clanInvitationId);
                if (document != null && document.StreamType == StreamType.GroupInvitation)
                {
                    GroupInvitationView groupInvitation = ((GroupInvitationStream)document).GroupInvitation;
                    UserDocument inviteeDocument = gameSessionManager.TryGet(groupInvitation.InviteeCmid, out GameSession userSession) ? userSession.Document : userManager.GetUser(groupInvitation.InviteeCmid).Result;
                    _ = inviteeDocument.ClanRequests.RemoveAll((T) => T == groupInvitation.GroupId);
                    _ = inviteeDocument.Streams.RemoveAll((T) => T == groupInvitation.GroupInvitationId);
                    _ = session.Document.Streams.RemoveAll((T) => T == groupInvitation.GroupInvitationId);
                    await userManager.Save(inviteeDocument);
                    await userManager.Save(session.Document);
                }
            }
            return 0;
        }

        public override int OnGetMyClanId(string authToken)
        {
            return !gameSessionManager.TryGet(authToken, out GameSession session) ? 0 : session.Document.ClanId;
        }

        private bool ValidString(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && ClanRegex.IsMatch(name);
        }
    }
}
