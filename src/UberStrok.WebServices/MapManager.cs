using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices
{
    public class OldMapManager
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(OldMapManager).Name);
        public OldMapManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            var maps = Utils.DeserializeJsonAt<List<MapView>>("configs/game/maps.json");
            if (maps == null)
                throw new FileNotFoundException("configs/game/maps.json file not found.");
            _hotReload = new FileSystemWatcher("configs\\game");
            _hotReload.Filter = "maps.json";
            _hotReload.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite;
            _hotReload.Changed += _hotReload_Changed;
            _hotReload.IncludeSubdirectories = true;
            _hotReload.EnableRaisingEvents = true;
            _maps = maps;
        }

        private void _hotReload_Changed(object sender, FileSystemEventArgs e)
        {
            Log.Info("Refreshing Maps");
            do
            {
                try
                {
                    var maps = Utils.DeserializeJsonAt<List<MapView>>("configs/game/maps.json");
                    _maps = maps;
                    break;
                }
                catch
                {
                    //having error, keep repeats
                }
            }
            while (true);


        }

        private readonly FileSystemWatcher _hotReload;
        private List<MapView> _maps;
        private readonly WebServiceContext _ctx;

        public List<MapView> GetAll()
        {
            return _maps;
        }

        public MapView Get(string sceneName)
        {
            if (sceneName == null)
                throw new ArgumentNullException(nameof(sceneName));

            for (int i = 0; i < _maps.Count; i++)
            {
                var map = _maps[i];
                if (map.SceneName == sceneName)
                    return map;
            }

            return null;
        }
    }
}
