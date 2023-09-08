using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class WebServiceConfiguration
    {
        public class WalletConfiguration
        {
            [JsonPropertyName("starting_points")]
            public int StartingPoints
            {
                get;
                set;
            }

            [JsonPropertyName("starting_credits")]
            public int StartingCredits
            {
                get;
                set;
            }
        }

        public class LoadoutConfiguration
        {
            [JsonPropertyName("starting_items")]
            public List<int> StartingItems
            {
                get;
                set;
            }
        }

        public struct MongoDatabaseInfo
        {
            [JsonPropertyName("host")]
            public string Host
            {
                get;
                set;
            }

            [JsonPropertyName("username")]
            public string Username
            {
                get;
                set;
            }

            [JsonPropertyName("password")]
            public string Password
            {
                get;
                set;
            }

            [JsonPropertyName("database")]
            public string Database
            {
                get;
                set;
            }
        }

        public static readonly WebServiceConfiguration Default = new WebServiceConfiguration
        {
            Locked = false,
            ServiceAuth = "HelloWorld",
            ServiceBase = "http://localhost/2.0",
            ServerGameVersion = "4.7.2",
            Wallet = new WalletConfiguration
            {
                StartingCredits = 10000,
                StartingPoints = 10000
            },
            Loadout = new LoadoutConfiguration
            {
                StartingItems = new List<int>
                {
                    1,
                    12
                }
            },
            ServiceDatabase = new MongoDatabaseInfo
            {
                Host = "localhost",
                Database = "Uberkill",
                Username = "root",
                Password = ""
            }
        };

        [JsonPropertyName("locked")]
        public bool Locked
        {
            get;
            set;
        }

        [JsonPropertyName("service_auth")]
        public string ServiceAuth
        {
            get;
            set;
        }

        [JsonPropertyName("service_base")]
        public string ServiceBase
        {
            get;
            set;
        }

        [JsonPropertyName("service_socket")]
        public string ServiceSocket
        {
            get;
            set;
        }

        [JsonPropertyName("server_game_version")]
        public string ServerGameVersion
        {
            get;
            set;
        }

        [JsonPropertyName("wallet")]
        public WalletConfiguration Wallet
        {
            get;
            set;
        }

        [JsonPropertyName("loadout")]
        public LoadoutConfiguration Loadout
        {
            get;
            set;
        }

        [JsonPropertyName("service_database")]
        public MongoDatabaseInfo ServiceDatabase
        {
            get;
            set;
        }
    }
}
