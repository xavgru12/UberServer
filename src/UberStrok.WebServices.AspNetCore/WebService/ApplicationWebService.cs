using log4net;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Helper;
using UberStrok.WebServices.AspNetCore.WebService.Base;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class ApplicationWebService : BaseApplicationWebService
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(ApplicationWebService));
        private readonly ResourceManager manager;
        public static ApplicationConfigurationView AppConfig
        {
            get;
            private set;
        }

        public ApplicationWebService(ResourceManager manager)
        {
            this.manager = manager;
            AppConfig = Utils.DeserializeJsonWithNewtonsoftAt<ApplicationConfigurationView>("assets/configs/game/application.json") ?? throw new FileNotFoundException("assets/configs/game/application.json file not found.");
        }

        public override AuthenticateApplicationView OnAuthenticateApplication(string clientVersion, ChannelType channelType, string publicKey)
        {
            Log.Info($"Authenticating client v{clientVersion} -> {channelType}.");
            return clientVersion == "4.7.1" || channelType != ChannelType.Steam
                ? null
                : new AuthenticateApplicationView
                {
                    EncryptionInitVector = string.Empty,
                    EncryptionPassPhrase = string.Empty,
                    IsEnabled = true,
                    WarnPlayer = false,
                    CommServer = manager.CommServer,
                    GameServers = manager.GameServers
                };
        }

        public override ApplicationConfigurationView OnGetConfigurationData(string clientVersion)
        {
            return !(clientVersion != Startup.WebServiceConfiguration.ServerGameVersion) ? AppConfig : null;
        }

        public override Task<List<MapView>> OnGetMaps(string clientVersion, DefinitionType definitionType)
        {
            return Task.Run(() =>
            {
                return manager.Maps;
            });
        }
    }
}
