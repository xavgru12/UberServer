﻿using System;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices
{
    public class ItemManager
    {
        public ItemManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            var items = Utils.DeserializeJsonAt<UberStrikeItemShopClientView>("configs/game/items.json");
            if (items == null)
                throw new FileNotFoundException("configs/game/items.json file not found.");

            _items = items;
        }

        private readonly UberStrikeItemShopClientView _items;
        private readonly WebServiceContext _ctx;

        public UberStrikeItemShopClientView GetShop()
        {
            return _items;
        }
    }
}
