using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomNameGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Helper;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class ResourceManager
    {
        private readonly HashSet<string> m_badWords;

        private readonly Regex NameRegex = new Regex("^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?]{3,18}$", RegexOptions.Compiled);

        private readonly Regex MottoRegex = new Regex("^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?]{3,18}$", RegexOptions.Compiled);

        public List<MapView> Maps
        {
            get;
            private set;
        }

        public List<AchievementView> AllAchievements { get; private set; }

        public PhotonView CommServer
        {
            get;
            private set;
        }

        public List<BundleView> Bundles
        {
            get;
            private set;
        }

        public List<PhotonView> GameServers
        {
            get;
            private set;
        }

        public UberStrikeItemShopClientView Items
        {
            get;
            private set;
        }

        public PersonNameGenerator NameGenerator
        {
            get;
            private set;
        }

        public List<LockedClanTags> LockedTags { get; private set; }

        public ResourceManager()
        {
            Maps = Utils.DeserializeJsonWithNewtonsoftAt<List<MapView>>("assets/configs/game/maps.json") ?? throw new FileNotFoundException("assets/configs/game/maps.json file not found.");
            Bundles = Utils.DeserializeJsonWithNewtonsoftAt<List<BundleView>>("assets/configs/game/bundles.json") ?? throw new FileNotFoundException("assets/configs/game/bundles.json file not found.");
            Items = Utils.DeserializeJsonWithNewtonsoftAt<UberStrikeItemShopClientView>("assets/configs/game/items.json") ?? throw new FileNotFoundException("assets/configs/game/items.json file not found.");
            NameGenerator = new PersonNameGenerator();
            //AllAchievements = (Utils.DeserializeJsonWithNewtonsoftAt<List<AchievementView>>("assets/configs/game/achievements.json") ?? throw new FileNotFoundException("assets/configs/game/achievements.json file not found."));

            LockedTags = Utils.DeserializeJsonWithNewtonsoftAt<List<LockedClanTags>>("assets/configs/game/lockedclantags.json") ?? throw new FileNotFoundException("assets/configs/game/lockedclantags.json file not found.");
            Console.WriteLine(JsonConvert.SerializeObject(LockedTags));

            int id2 = 0;
            JObject servers = Utils.DeserializeJsonWithNewtonsoftAt<JObject>("assets/configs/game/servers.json");
            if (servers == null)
            {
                throw new FileNotFoundException("assets/configs/game/servers.json file not found.");
            }
            JToken commServer = servers["CommServer"];
            JToken gameServers = servers["GameServers"];
            PhotonView photonView = new PhotonView();
            id2 = photonView.PhotonId = id2 + 1;
            photonView.IP = GetIPAddress((string?)commServer["IP"]);
            photonView.Port = (int)commServer["Port"];
            photonView.Region = RegionType.UsEast;
            photonView.UsageType = PhotonUsageType.CommServer;
            photonView.Name = "UbzStuff.Realtime.CommServer";
            photonView.MinLatency = 0;
            CommServer = photonView;
            GameServers = new List<PhotonView>();
            foreach (JToken token in (IEnumerable<JToken>)gameServers)
            {
                PhotonView photonView2 = new PhotonView();
                id2 = photonView2.PhotonId = id2 + 1;
                photonView2.IP = GetIPAddress((string?)token["IP"]);
                photonView2.Port = (int)token["Port"];
                photonView2.Region = (RegionType)(int)token["Region"];
                photonView2.UsageType = PhotonUsageType.All;
                photonView2.Name = (string?)token["Name"];
                photonView2.MinLatency = (int)token["MinLatency"];
                PhotonView server = photonView2;
                GameServers.Add(server);
            }
            m_badWords = new HashSet<string>();
            string[] files = Directory.GetFiles("assets/services/badwords/", "*.*", SearchOption.TopDirectoryOnly);
            foreach (string path in files)
            {
                string[] file = File.ReadAllLines(path);
                for (int i = 0; i < file.Length; i++)
                {
                    string word = file[i].Trim().ToLowerInvariant();
                    if (!m_badWords.Contains(word))
                    {
                        _ = m_badWords.Add(word);
                    }
                }
            }
        }

        public string GetIPAddress(string hostname)
        {
            if (IPAddress.TryParse(hostname, out IPAddress _))
            {
                return hostname;
            }
            IPHostEntry host = Dns.GetHostEntry(hostname);
            IPAddress[] addressList = host.AddressList;
            foreach (IPAddress ip in addressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Unable to get host entry at " + hostname);
        }

        public MapView Get(string sceneName)
        {
            if (sceneName == null)
            {
                throw new ArgumentNullException(sceneName);
            }
            foreach (MapView map in Maps)
            {
                if (map.SceneName == sceneName)
                {
                    return map;
                }
            }
            return null;
        }

        internal bool IsNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && NameRegex.IsMatch(name);
        }

        internal bool IsNameOffensive(string name)
        {
            return m_badWords.Contains(name.ToLower());
        }
    }
}
