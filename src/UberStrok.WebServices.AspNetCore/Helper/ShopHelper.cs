using System.Collections.Generic;
using System.Linq;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Helper
{
    public static class ShopHelper
    {
        internal static UberStrikeItemGearView GetItem(this List<UberStrikeItemGearView> list, int itemId)
        {
            return list.Single((t) => t.ID == itemId);
        }

        internal static UberStrikeItemQuickView GetItem(this List<UberStrikeItemQuickView> list, int itemId)
        {
            return list.Single((t) => t.ID == itemId);
        }

        internal static UberStrikeItemWeaponView GetItem(this List<UberStrikeItemWeaponView> list, int itemId)
        {
            return list.Single((t) => t.ID == itemId);
        }

        internal static UberStrikeItemFunctionalView GetItem(this List<UberStrikeItemFunctionalView> list, int itemId)
        {
            return list.Single((t) => t.ID == itemId);
        }

        internal static bool IsOwned(this List<ItemInventoryView> list, int itemId)
        {
            return list.Find((t) => t.ItemId == itemId) != null;
        }

        internal static ItemPriceView GetPrice(this BaseUberStrikeItemView item, UberStrikeCurrencyType currency, BuyingDurationType duration)
        {
            if (item?.Prices != null)
            {
                foreach (ItemPriceView itemPrice2 in item.Prices)
                {
                    if (itemPrice2.Currency == currency && itemPrice2.Duration == duration)
                    {
                        return itemPrice2;
                    }
                }
            }
            return null;
        }

        internal static BaseUberStrikeItemView GetItem(this UberStrikeItemShopClientView uberStrikeItem, int itemId, UberStrikeItemType itemType)
        {
            return itemType switch
            {
                UberStrikeItemType.Functional => uberStrikeItem.FunctionalItems.GetItem(itemId),
                UberStrikeItemType.Weapon => uberStrikeItem.WeaponItems.GetItem(itemId),
                UberStrikeItemType.QuickUse => uberStrikeItem.QuickItems.GetItem(itemId),
                UberStrikeItemType.Gear => uberStrikeItem.GearItems.GetItem(itemId),
                _ => null,
            };
        }
    }
}
