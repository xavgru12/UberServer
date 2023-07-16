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
    public abstract class BaseApplicationWebService : IApplicationAsyncWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseApplicationWebService));

        public abstract ApplicationConfigurationView OnGetConfigurationData(string clientVersion);

        public abstract Task<List<MapView>> OnGetMaps(string clientVersion, DefinitionType definitionType);

        public abstract AuthenticateApplicationView OnAuthenticateApplication(string clientVersion, ChannelType channelType, string publicKey);

        public Task<byte[]> AuthenticateApplication(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string version = StringProxy.Deserialize(bytes);
                    ChannelType channelType = EnumProxy<ChannelType>.Deserialize(bytes);
                    string publicKey = StringProxy.Deserialize(bytes);
                    AuthenticateApplicationView view = OnAuthenticateApplication(version, channelType, publicKey);
                    using MemoryStream outBytes = new MemoryStream();
                    AuthenticateApplicationViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle AuthenticateApplication request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetConfigurationData(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string version = StringProxy.Deserialize(bytes);
                    ApplicationConfigurationView view = OnGetConfigurationData(version);
                    using MemoryStream outBytes = new MemoryStream();
                    ApplicationConfigurationViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetConfigurationData request:");
                Log.Error(ex);
                return null;
            }
        }

        public async Task<byte[]> GetMaps(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string version = StringProxy.Deserialize(bytes);
                DefinitionType definitionType = EnumProxy<DefinitionType>.Deserialize(bytes);
                List<MapView> view = await OnGetMaps(version, definitionType);
                using MemoryStream outBytes = new MemoryStream();
                ListProxy<MapView>.Serialize(outBytes, view, MapViewProxy.Serialize);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMaps request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> SetMatchScore(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle SetMatchScore request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
