using log4net;
using System;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Contracts;

namespace UberStrok.WebServices.AspNetCore.WebService.Base
{
    public abstract class BaseAuthenticationWebService : IAuthenticationAsyncWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseAuthenticationWebService));

        public abstract Task<MemberAuthenticationResultView> OnLoginSteam(string clientVersion, string steamId, string authToken, string machineId, string hwid, bool isMac);

        public abstract Task<AccountCompletionResultView> OnCompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId);

        public async Task<byte[]> CompleteAccount(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                int cmid = Int32Proxy.Deserialize(bytes);
                string name = StringProxy.Deserialize(bytes);
                ChannelType channelType = EnumProxy<ChannelType>.Deserialize(bytes);
                string locale = StringProxy.Deserialize(bytes);
                string machineId = StringProxy.Deserialize(bytes);
                AccountCompletionResultView view = await OnCompleteAccount(cmid, name, channelType, locale, machineId);
                using MemoryStream outBytes = new MemoryStream();
                AccountCompletionResultViewProxy.Serialize(outBytes, view);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CompleteAccount request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> CreateUser(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CreateUser request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> LinkSteamMember(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LinkSteamMember request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> LoginMemberEmail(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberEmail request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> LoginMemberFacebookUnitySdk(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberFacebookUnitySdk request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> LoginMemberPortal(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberPortal request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> LoginSteam(byte[] data)
        {
            _ = 2;
            try
            {
                MemoryStream bytes = new MemoryStream(data);
                byte[] result;
                try
                {
                    string clientVersion = StringProxy.Deserialize(bytes);
                    string steamId = StringProxy.Deserialize(bytes);
                    string authToken = StringProxy.Deserialize(bytes);
                    string machineId = StringProxy.Deserialize(bytes);
                    string hwid = StringProxy.Deserialize(bytes);
                    bool isMac = BooleanProxy.Deserialize(bytes);
                    MemberAuthenticationResultView view = await OnLoginSteam(clientVersion, steamId, authToken, machineId, hwid, isMac);
                    MemoryStream outBytes = new MemoryStream();
                    byte[] array;
                    try
                    {
                        MemberAuthenticationResultViewProxy.Serialize(outBytes, view);
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
                Log.Error("Unable to handle LoginSteam request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
