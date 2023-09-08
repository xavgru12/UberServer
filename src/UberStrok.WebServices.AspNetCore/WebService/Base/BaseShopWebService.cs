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
    public abstract class BaseShopWebService : IShopAsyncWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseShopWebService));

        public abstract UberStrikeItemShopClientView OnGetShop();

        public abstract List<BundleView> OnGetBundles(ChannelType channel);

        public abstract BuyItemResult OnBuyItem(int itemId, string authToken, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberStrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType);

        public Task<byte[]> GetShop(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    UberStrikeItemShopClientView view = OnGetShop();
                    using MemoryStream outBytes = new MemoryStream();
                    UberStrikeItemShopClientViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetShop request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> BuyItem(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    int itemId = Int32Proxy.Deserialize(bytes);
                    string authToken = StringProxy.Deserialize(bytes);
                    UberStrikeCurrencyType currencyType = EnumProxy<UberStrikeCurrencyType>.Deserialize(bytes);
                    BuyingDurationType durationType = EnumProxy<BuyingDurationType>.Deserialize(bytes);
                    UberStrikeItemType itemType = EnumProxy<UberStrikeItemType>.Deserialize(bytes);
                    BuyingLocationType marketLocation = EnumProxy<BuyingLocationType>.Deserialize(bytes);
                    BuyingRecommendationType recommendationType = EnumProxy<BuyingRecommendationType>.Deserialize(bytes);
                    BuyItemResult result = OnBuyItem(itemId, authToken, currencyType, durationType, itemType, marketLocation, recommendationType);
                    using MemoryStream outBytes = new MemoryStream();
                    EnumProxy<BuyItemResult>.Serialize(outBytes, result);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyItem request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> BuyPack(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyPack request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetBundles(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    ChannelType channel = EnumProxy<ChannelType>.Deserialize(bytes);
                    List<BundleView> bundles = OnGetBundles(channel);
                    using MemoryStream outBytes = new MemoryStream();
                    ListProxy<BundleView>.Serialize(outBytes, bundles, BundleViewProxy.Serialize);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetBundles request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> BuyBundle(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyBundle request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> BuyBundleSteam(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyBundleSteam request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> FinishBuyBundleSteam(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle FinishBuyBundleSteam request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> VerifyReceipt(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle VerifyReceipt request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> UseConsumableItem(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle UseConsumableItem request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetAllMysteryBoxs_1(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllMysteryBoxs_1 request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetAllMysteryBoxs_2(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllMysteryBoxs_2 request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMysteryBox(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMysteryBox request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> RollMysteryBox(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle RollMysteryBox request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetAllLuckyDraws_1(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllLuckyDraws_1 request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetAllLuckyDraws_2(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllLuckyDraws_2 request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetLuckyDraw(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLuckyDraw request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> RollLuckyDraw(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle RollLuckyDraw request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
