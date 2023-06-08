using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Items
{
    public class UserDocument : MongoDocument
    {
        public int UserId
        {
            get;
            set;
        }

        public int ClanId
        {
            get;
            set;
        }

        public string SteamId
        {
            get;
            set;
        }

        public string UBBan
        {
            get;
            set;
        }

        public string UBMute
        {
            get;
            set;
        }

        public PublicProfileView Profile
        {
            get;
            set;
        }

        public MemberWalletView Wallet
        {
            get;
            set;
        }

        public PlayerStatisticsView Statistics
        {
            get;
            set;
        }

        public LoadoutView Loadout
        {
            get;
            set;
        }

        public List<ItemInventoryView> Inventory
        {
            get;
            set;
        }

        public List<int> Friends
        {
            get;
            set;
        }

        public List<int> FriendRequests
        {
            get;
            set;
        }

        public List<int> ClanRequests
        {
            get;
            set;
        }

        public List<int> Streams
        {
            get;
            set;
        }

        public HashSet<string> HDD
        {
            get;
            set;
        }

        public HashSet<string> BIOS
        {
            get;
            set;
        }

        public HashSet<string> MOTHERBOARD
        {
            get;
            set;
        }

        public HashSet<string> MAC
        {
            get;
            set;
        }

        public HashSet<string> UNITY
        {
            get;
            set;
        }

        public HashSet<string> Names
        {
            get;
            set;
        }

        public int Kills
        {
            get;
            set;
        }

        public int Deaths
        {
            get;
            set;
        }

        public UserDocument()
        {
            Friends = new List<int>();
            FriendRequests = new List<int>();
            ClanRequests = new List<int>();
            Streams = new List<int>();
            Names = new HashSet<string>();
            HDD = new HashSet<string>();
            BIOS = new HashSet<string>();
            MOTHERBOARD = new HashSet<string>();
            MAC = new HashSet<string>();
            UNITY = new HashSet<string>();
        }
    }
}
