using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Contracts;

namespace UberStrok.WebServices.AspNetCore.WebService.Base
{
    public abstract class BaseRelationshipWebService : IRelationshipAsyncWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseRelationshipWebService));

        public abstract Task<List<ContactGroupView>> OnGetContactGroupView(string authToken);

        public abstract Task<List<ContactRequestView>> OnGetContactRequestsView(string authToken);

        public abstract Task<PublicProfileView> OnAcceptContactRequest(string authToken, int contactRequestId);

        public abstract Task<bool> OnDeclineContactRequest(string authToken, int contactRequestId);

        public abstract MemberOperationResult OnDeleteContact(string authToken, int contactCmid);

        public abstract Task OnSendContactRequest(string authToken, int receiverCmid, string message);

        public async Task<byte[]> AcceptContactRequest(byte[] data)
        {
            _ = 2;
            try
            {
                MemoryStream bytes = new MemoryStream(data);
                byte[] result;
                try
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    int contactRequestId = Int32Proxy.Deserialize(bytes);
                    PublicProfileView view = await OnAcceptContactRequest(authToken, contactRequestId);
                    MemoryStream outBytes = new MemoryStream();
                    byte[] array;
                    try
                    {
                        PublicProfileViewProxy.Serialize(outBytes, view);
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
                Log.Error("Unable to handle AcceptContactRequest request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> DeclineContactRequest(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                int contactRequestId = Int32Proxy.Deserialize(bytes);
                bool view = await OnDeclineContactRequest(authToken, contactRequestId);
                using MemoryStream outBytes = new MemoryStream();
                BooleanProxy.Serialize(outBytes, view);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SendContactRequest request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> DeleteContact(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    int contactCmid = Int32Proxy.Deserialize(bytes);
                    MemberOperationResult view = OnDeleteContact(authToken, contactCmid);
                    using MemoryStream outBytes = new MemoryStream();
                    EnumProxy<MemberOperationResult>.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle DeleteContact request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> GetContactRequests(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                List<ContactRequestView> view = await OnGetContactRequestsView(authToken);
                using MemoryStream outBytes = new MemoryStream();
                ListProxy<ContactRequestView>.Serialize(outBytes, view, ContactRequestViewProxy.Serialize);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SendContactRequest request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> GetContactsByGroups(byte[] data)
        {
            _ = 2;
            try
            {
                MemoryStream bytes = new MemoryStream(data);
                byte[] result;
                try
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    _ = BooleanProxy.Deserialize(bytes);
                    List<ContactGroupView> view = await OnGetContactGroupView(authToken);
                    MemoryStream outBytes = new MemoryStream();
                    byte[] array;
                    try
                    {
                        ListProxy<ContactGroupView>.Serialize(outBytes, view, ContactGroupViewProxy.Serialize);
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
                Log.Error("Unable to handle GetContactsByGroups request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> SendContactRequest(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    int receiverCmid = Int32Proxy.Deserialize(bytes);
                    string message = StringProxy.Deserialize(bytes);
                    _ = OnSendContactRequest(authToken, receiverCmid, message);
                    using MemoryStream outBytes = new MemoryStream();
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SendContactRequest request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
