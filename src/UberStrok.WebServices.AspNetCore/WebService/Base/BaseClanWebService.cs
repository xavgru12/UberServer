using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Contracts;

namespace UberStrok.WebServices.AspNetCore.WebService.Base
{
    public abstract class BaseClanWebService : IClanAsyncWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseClanWebService));

        public abstract Task<ClanView> OnGetOwnClan(string authToken, int groupId);

        public abstract Task<ClanCreationReturnView> OnCreateClan(GroupCreationView creationView);

        public abstract Task<List<GroupInvitationView>> OnGetAllGroupInvitations(string authToken);

        public abstract Task<List<GroupInvitationView>> OnGetPendingGroupInvitations(int groupId, string authToken);

        public abstract int OnGetMyClanId(string authToken);

        public abstract Task<int> OnUpdateMemberPosition(MemberPositionUpdateView memberPositionUpdate);

        public abstract Task<int> OnTransferOwnership(int groupId, string authToken, int newLeaderCmid);

        public abstract Task<int> OnKick(int groupId, string authToken, int cmidToKick);

        public abstract Task<int> OnLeaveClan(int groupId, string authToken);

        public abstract Task<int> OnDisbandClan(int groupId, string authToken);

        public abstract Task<int> OnInviteMemberToJoinAGroup(int clanId, string authToken, int invitee, string message);

        public abstract Task<ClanRequestAcceptView> OnAcceptClanInvitation(int clanInvitationId, string authToken);

        public abstract Task<ClanRequestDeclineView> OnDeclineClanInvitation(int clanInvitationId, string authToken);

        public abstract Task<int> OnCancelInvite(int clanInvitationId, string authToken);

        public async Task<byte[]> GetOwnClan(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                int groupId = Int32Proxy.Deserialize(bytes);
                ClanView view = await OnGetOwnClan(authToken, groupId);
                using MemoryStream outBytes = new MemoryStream();
                ClanViewProxy.Serialize(outBytes, view);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetOwnClan request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> UpdateMemberPosition(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                MemberPositionUpdateView memberPositionUpdate = MemberPositionUpdateViewProxy.Deserialize(bytes);
                int result = await OnUpdateMemberPosition(memberPositionUpdate);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle UpdateMemberPosition request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> InviteMemberToJoinAGroup(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int clanId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int invitee = Int32Proxy.Deserialize(bytes);
                string message = StringProxy.Deserialize(bytes);
                int result = await OnInviteMemberToJoinAGroup(clanId, authToken, invitee, message);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle InviteMemberToJoinAGroup request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> AcceptClanInvitation(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int clanInvitationId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                ClanRequestAcceptView clanRequestAccept = await OnAcceptClanInvitation(clanInvitationId, authToken);
                using MemoryStream outBytes = new MemoryStream();
                ClanRequestAcceptViewProxy.Serialize(outBytes, clanRequestAccept);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle AcceptClanInvitation request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> DeclineClanInvitation(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int clanInvitationId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                ClanRequestDeclineView clanRequestDecline = await OnDeclineClanInvitation(clanInvitationId, authToken);
                using MemoryStream outBytes = new MemoryStream();
                ClanRequestDeclineViewProxy.Serialize(outBytes, clanRequestDecline);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle DeclineClanInvitation request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> KickMemberFromClan(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int cmidToKick = Int32Proxy.Deserialize(bytes);
                int id = await OnKick(groupId, authToken, cmidToKick);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, id);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle KickMemberFromClan request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> DisbandGroup(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int result = await OnDisbandClan(groupId, authToken);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LeaveAClan request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> LeaveAClan(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int result = await OnLeaveClan(groupId, authToken);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LeaveAClan request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMyClanId(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    int id = OnGetMyClanId(authToken);
                    using MemoryStream outBytes = new MemoryStream();
                    Int32Proxy.Serialize(outBytes, id);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMyClanId request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> CancelInvitation(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int groupInvitationId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int id = await OnCancelInvite(groupInvitationId, authToken);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, id);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CancelInvitation request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> GetAllGroupInvitations(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                List<GroupInvitationView> view = await OnGetAllGroupInvitations(authToken);
                using MemoryStream outBytes = new MemoryStream();
                ListProxy<GroupInvitationView>.Serialize(outBytes, view, GroupInvitationViewProxy.Serialize);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllGroupInvitations request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> GetPendingGroupInvitations(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                List<GroupInvitationView> view = await OnGetPendingGroupInvitations(groupId, authToken);
                using MemoryStream outBytes = new MemoryStream();
                ListProxy<GroupInvitationView>.Serialize(outBytes, view, GroupInvitationViewProxy.Serialize);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetPendingGroupInvitations request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> CreateClan(byte[] data)
        {
            _ = 2;
            try
            {
                MemoryStream bytes = new MemoryStream(data);
                byte[] result;
                try
                {
                    GroupCreationView clanView = GroupCreationViewProxy.Deserialize(bytes);
                    ClanCreationReturnView view = await OnCreateClan(clanView);
                    MemoryStream outBytes = new MemoryStream();
                    byte[] array;
                    try
                    {
                        ClanCreationReturnViewProxy.Serialize(outBytes, view);
                        array = outBytes.ToArray();
                    }
                    finally
                    {
                        if (outBytes != null)
                        {
                            await ((IAsyncDisposable)outBytes).DisposeAsync();
                        }
                    }
                    result = array;
                }
                finally
                {
                    if (bytes != null)
                    {
                        await ((IAsyncDisposable)bytes).DisposeAsync();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CreateClan request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> TransferOwnership(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int newLeaderCmid = Int32Proxy.Deserialize(bytes);
                int result = await OnTransferOwnership(groupId, authToken, newLeaderCmid);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle TransferOwnership request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> CanOwnAClan(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
