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
    public abstract class BaseUserWebService : IUserAsyncWebServiceContract
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(BaseUserWebService));

        public abstract LoadoutView OnGetLoadout(string authToken);

        public abstract ApplicationConfigurationView OnGetAppConfig();

        public abstract Task<bool> OnIsDuplicateMemberName(string username);

        public abstract UberstrikeUserView OnGetMember(string authToken);

        public abstract MemberWalletView OnGetMemberWallet(string authToken);

        public abstract PlayerStatisticsView OnGetPlayerStats(string authToken);

        public abstract List<ItemInventoryView> OnGetInventory(string authToken);

        public abstract Task<List<string>> OnGenerateNonDuplicatedMemberNames(string username);

        public abstract LoadoutView OnGetLoadoutServer(string serviceAuth, string authToken);

        public abstract Task<MemberOperationResult> OnSetLoadout(string authToken, LoadoutView loadoutView);

        public abstract Task<MemberOperationResult> OnSetWallet(string authToken, MemberWalletView walletView);

        public abstract Task OnEndOfMatch(string authToken, StatsCollectionView totalStats, StatsCollectionView bestStats);

        public abstract Task<MemberOperationResult> OnChangeMemberName(string authToken, string username, string local, string machineId);

        public async Task<byte[]> ChangeMemberName(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                string username = StringProxy.Deserialize(bytes);
                string locale = StringProxy.Deserialize(bytes);
                string machineId = StringProxy.Deserialize(bytes);
                MemberOperationResult result = await OnChangeMemberName(authToken, username, locale, machineId);
                using MemoryStream outBytes = new MemoryStream();
                EnumProxy<MemberOperationResult>.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle ChangeMemberName request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> IsDuplicateMemberName(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string username = StringProxy.Deserialize(bytes);
                bool result = await OnIsDuplicateMemberName(username);
                using MemoryStream outBytes = new MemoryStream();
                BooleanProxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle IsDuplicateMemberName request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> GenerateNonDuplicatedMemberNames(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string username = StringProxy.Deserialize(bytes);
                List<string> usernames = await OnGenerateNonDuplicatedMemberNames(username);
                using MemoryStream outBytes = new MemoryStream();
                ListProxy<string>.Serialize(outBytes, usernames, StringProxy.Serialize);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GenerateNonDuplicateMemberNames request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMemberWallet(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    MemberWalletView view = OnGetMemberWallet(authToken);
                    using MemoryStream outBytes = new MemoryStream();
                    MemberWalletViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberWallet request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetInventory(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    List<ItemInventoryView> view = OnGetInventory(authToken);
                    using MemoryStream outBytes = new MemoryStream();
                    ListProxy<ItemInventoryView>.Serialize(outBytes, view, ItemInventoryViewProxy.Serialize);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetInventory request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetCurrencyDeposits(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetCurrencyDeposits request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetItemTransactions(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetItemTransactions request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetPointsDeposits(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetPointsDeposits request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetLoadout(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    LoadoutView view = OnGetLoadout(authToken);
                    using MemoryStream outBytes = new MemoryStream();
                    LoadoutViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLoadout request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetLoadoutServer(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string serviceAuth = StringProxy.Deserialize(bytes);
                    string authToken = StringProxy.Deserialize(bytes);
                    LoadoutView view = OnGetLoadoutServer(serviceAuth, authToken);
                    using MemoryStream outBytes = new MemoryStream();
                    LoadoutViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLoadoutServer request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> SetLoadout(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                MemberOperationResult result;
                string serviceAuth = StringProxy.Deserialize(bytes);
                if (serviceAuth != Startup.WebServiceConfiguration.ServiceAuth)
                {
                    result = MemberOperationResult.InvalidHandle;
                }
                else
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    LoadoutView loadoutView = LoadoutViewProxy.Deserialize(bytes);
                    result = await OnSetLoadout(authToken, loadoutView);
                }
                using MemoryStream outBytes = new MemoryStream();
                EnumProxy<MemberOperationResult>.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SetLoadout request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task EndOfMatch(byte[] data)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    string authToken = StringProxy.Deserialize(memoryStream);
                    StatsCollectionView totalStats = StatsCollectionViewProxy.Deserialize(memoryStream);
                    StatsCollectionView bestStats = StatsCollectionViewProxy.Deserialize(memoryStream);
                    await OnEndOfMatch(authToken, totalStats, bestStats);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle EndOfMatch request:");
                Log.Error(ex);
            }
        }

        public Task<byte[]> SetWallet(byte[] data)
        {
            try
            {
                return Task.Run(async () =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    MemberWalletView walletView = MemberWalletViewProxy.Deserialize(bytes);
                    MemberOperationResult result = await OnSetWallet(authToken, walletView);
                    using MemoryStream outBytes = new MemoryStream();
                    EnumProxy<MemberOperationResult>.Serialize(outBytes, result);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SetWallet request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMember(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    UberstrikeUserView view = OnGetMember(authToken);
                    using MemoryStream outBytes = new MemoryStream();
                    UberstrikeUserViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMember request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMemberSessionData(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberSessionData request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMemberListSessionData(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMemberListSessionData request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetAppConfig()
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream outBytes = new MemoryStream();
                    ApplicationConfigurationView view = OnGetAppConfig();
                    ApplicationConfigurationViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAppConfig request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
