using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseClanWebService : IClanAsyncWebServiceContract
    {
        public abstract Task<ClanView> OnGetOwnClan(string authToken, int groupId);

        public abstract Task<ClanCreationReturnView> OnCreateClan(GroupCreationView creationView);

        public abstract Task<List<GroupInvitationView>> OnGetAllGroupInvitations(string authToken);

        public abstract Task<List<GroupInvitationView>> OnGetPendingGroupInvitations(int groupId, string authToken);

        public abstract Task<int> OnGetMyClanId(string authToken);

        public abstract Task<int> OnUpdateMemberPosition(MemberPositionUpdateView memberPositionUpdate);

        public abstract Task<int> OnTransferOwnership(int groupId, string authToken, int newLeaderCmid);

        public abstract Task<int> OnKick(int groupId, string authToken, int cmidToKick);

        public abstract Task<int> OnLeaveClan(int groupId, string authToken);

        public abstract Task<int> OnDisbandClan(int groupId, string authToken);

        public abstract Task<int> OnInviteMemberToJoinAGroup(int clanId, string authToken, int invitee, string message);

        public abstract Task<ClanRequestAcceptView> OnAcceptClanInvitation(int clanInvitationId, string authToken);

        public abstract Task<ClanRequestDeclineView> OnDeclineClanInvitation(int clanInvitationId, string authToken);

        public abstract Task<int> OnCancelInvite(int clanInvitationId, string authToken);
        async Task<byte[]> IClanAsyncWebServiceContract.AcceptClanInvitation(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int clanInvitationId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                ClanRequestAcceptView clanRequestAccept = await OnAcceptClanInvitation(clanInvitationId, authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    ClanRequestAcceptViewProxy.Serialize(outBytes, clanRequestAccept);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.CancelInvitation(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int groupInvitationId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int id = await OnCancelInvite(groupInvitationId, authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, id);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.CanOwnAClan(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                string authToken = StringProxy.Deserialize(bytes);
                int groupId = Int32Proxy.Deserialize(bytes);
                ClanView view = await OnGetOwnClan(authToken, groupId);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    ClanViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.CreateClan(byte[] data)
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
                        await((IAsyncDisposable)outBytes).DisposeAsync();
                    }
                }
                result = array;
            }
            finally
            {
                if (bytes != null)
                {
                    await((IAsyncDisposable)bytes).DisposeAsync();
                }
            }
            return result;
        }

        async Task<byte[]> IClanAsyncWebServiceContract.DeclineClanInvitation(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int clanInvitationId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                ClanRequestDeclineView clanRequestDecline = await OnDeclineClanInvitation(clanInvitationId, authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    ClanRequestDeclineViewProxy.Serialize(outBytes, clanRequestDecline);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.DisbandGroup(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int result = await OnDisbandClan(groupId, authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, result);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.GetAllGroupInvitations(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                string authToken = StringProxy.Deserialize(bytes);
                List<GroupInvitationView> view = await OnGetAllGroupInvitations(authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    ListProxy<GroupInvitationView>.Serialize(outBytes, view, GroupInvitationViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.GetMyClanId(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                string authToken = StringProxy.Deserialize(bytes);
                int id = await OnGetMyClanId(authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, id);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.GetOwnClan(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                string authToken = StringProxy.Deserialize(bytes);
                int groupId = Int32Proxy.Deserialize(bytes);
                ClanView view = await OnGetOwnClan(authToken, groupId);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    ClanViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.GetPendingGroupInvitations(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                List<GroupInvitationView> view = await OnGetPendingGroupInvitations(groupId, authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    ListProxy<GroupInvitationView>.Serialize(outBytes, view, GroupInvitationViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.InviteMemberToJoinAGroup(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int clanId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int invitee = Int32Proxy.Deserialize(bytes);
                string message = StringProxy.Deserialize(bytes);
                int result = await OnInviteMemberToJoinAGroup(clanId, authToken, invitee, message);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, result);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.KickMemberFromClan(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int cmidToKick = Int32Proxy.Deserialize(bytes);
                int id = await OnKick(groupId, authToken, cmidToKick);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, id);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.LeaveAClan(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int result = await OnLeaveClan(groupId, authToken);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, result);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.TransferOwnership(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                int groupId = Int32Proxy.Deserialize(bytes);
                string authToken = StringProxy.Deserialize(bytes);
                int newLeaderCmid = Int32Proxy.Deserialize(bytes);
                int result = await OnTransferOwnership(groupId, authToken, newLeaderCmid);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, result);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IClanAsyncWebServiceContract.UpdateMemberPosition(byte[] data)
        {
            using (MemoryStream bytes = new MemoryStream(data))
            {
                MemberPositionUpdateView memberPositionUpdate = MemberPositionUpdateViewProxy.Deserialize(bytes);
                int result = await OnUpdateMemberPosition(memberPositionUpdate);
                using (MemoryStream outBytes = new MemoryStream())
                {
                    Int32Proxy.Serialize(outBytes, result);
                    return outBytes.ToArray();
                }
            }
        }
    }
}
