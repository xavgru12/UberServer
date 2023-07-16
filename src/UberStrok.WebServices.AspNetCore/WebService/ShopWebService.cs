using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Core.Session;
using UberStrok.WebServices.AspNetCore.Helper;
using UberStrok.WebServices.AspNetCore.WebService.Base;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class ShopWebService : BaseShopWebService
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(ShopWebService));

        private readonly ResourceManager resourceManager;
        private readonly GameSessionManager gameSessionManager;
        private readonly UserManager userManager;
        public ShopWebService(ResourceManager resourceManager, GameSessionManager gameSessionManager, UserManager userManager)
        {
            this.resourceManager = resourceManager;
            this.gameSessionManager = gameSessionManager;
            this.userManager = userManager;
        }

        public override UberStrikeItemShopClientView OnGetShop()
        {
            return resourceManager.Items;
        }

        public override List<BundleView> OnGetBundles(ChannelType channel)
        {
            return resourceManager.Bundles;
        }

        public override BuyItemResult OnBuyItem(int itemId, string authToken, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberStrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                BaseUberStrikeItemView itemView = OnGetShop().GetItem(itemId, itemType);
                if (itemView != null)
                {
                    if (itemView.IsForSale)
                    {
                        if (itemView.LevelLock > 0 && session.Document.Statistics.Level < itemView.LevelLock)
                        {
                            return BuyItemResult.InvalidLevel;
                        }
                        ItemPriceView price = itemView.GetPrice(currencyType, durationType);
                        if (price == null)
                        {
                            return BuyItemResult.InvalidData;
                        }
                        MemberWalletView userWallet = session.Document.Wallet;
                        switch (currencyType)
                        {
                            case UberStrikeCurrencyType.Credits:
                                if (userWallet.Credits >= price.Price)
                                {
                                    userWallet.Credits -= price.Price;
                                    break;
                                }
                                return BuyItemResult.NotEnoughCurrency;
                            case UberStrikeCurrencyType.Points:
                                if (userWallet.Points >= price.Price)
                                {
                                    userWallet.Points -= price.Price;
                                    break;
                                }
                                return BuyItemResult.NotEnoughCurrency;
                        }
                        int days;
                        switch ((int)durationType)
                        {
                            case 1:
                                days = 1;
                                break;
                            case 2:
                                days = 7;
                                break;
                            case 5:
                                days = 0;
                                break;
                            default:
                                return BuyItemResult.InvalidExpirationDate; //if some incorrect date is passed
                        }
                        foreach (ItemInventoryView item in session.Document.Inventory)
                        {
                            if (item.ItemId == itemId) //if item exists in inventory
                            {
                                if (item.ExpirationDate != null) //if item isnt permanent
                                {
                                    if (days == 0)
                                    {
                                        item.ExpirationDate = null;
                                    }
                                    else if (days == 1 || days == 7)
                                    {
                                        if (item.ExpirationDate.Value > DateTime.Now) //if item hasnt expired yet
                                        {
                                            DateTime tempdate = item.ExpirationDate.Value;
                                            _ = tempdate.AddDays(days);
                                            item.ExpirationDate = tempdate;
                                        }
                                        else
                                        {
                                            item.ExpirationDate = DateTime.UtcNow.AddDays(days); //if item has expired
                                        }
                                    }
                                    _ = userManager.Save(session.Document);
                                    return BuyItemResult.OK;
                                }
                                else
                                {
                                    return BuyItemResult.AlreadyInInventory;
                                }
                            }
                        }
                        DateTime? date = null;
                        if (days == 1 || days == 7)
                        {
                            date = DateTime.Now.AddDays(days);
                        }
                        session.Document.Inventory.Add(new ItemInventoryView(itemId, date, -1, session.Document.UserId));
                        _ = userManager.Save(session.Document);
                        return BuyItemResult.OK;
                    }
                    return BuyItemResult.IsNotForSale;
                }
                return BuyItemResult.InvalidData;
            }
            Log.Error("An unidentified AuthToken was passed.");
            return BuyItemResult.InvalidMember;
        }
    }
}
