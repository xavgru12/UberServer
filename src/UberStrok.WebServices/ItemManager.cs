using log4net;
using System;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices
{
    public class ItemManager
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(ItemManager).Name);
        public ItemManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            var items = Utils.DeserializeJsonAt<UberStrikeItemShopClientView>("configs/game/items.json");
            if (items == null)
                throw new FileNotFoundException("configs/game/items.json file not found.");
            _hotReload = new FileSystemWatcher("configs\\game");
            _hotReload.Filter = "items.json";
            _hotReload.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite;
            _hotReload.Changed += _hotReload_Changed;
            _hotReload.IncludeSubdirectories = true;
            _hotReload.EnableRaisingEvents = true;
            _items = items;
        }

        private void _hotReload_Changed(object sender, FileSystemEventArgs e)
        {
            Log.Info("Refreshing Items list");
            var items = Utils.DeserializeJsonAt<UberStrikeItemShopClientView>("configs/game/items.json");
            _items = items;
        }

        private readonly FileSystemWatcher _hotReload;
        private UberStrikeItemShopClientView _items;
        private readonly WebServiceContext _ctx;

        public UberStrikeItemShopClientView GetShop()
        {
            return _items;
        }
    }
}
