using log4net;
using System;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.WebServices.AspNetCore.Contracts;

namespace UberStrok.WebServices.AspNetCore.WebService.Base
{
    public abstract class BaseModerationWebService : IModerationWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseModerationWebService));

        public abstract int OnBan(string serviceAuth, int cmid);

        public abstract int OnBanIp(string authToken, string ip);

        public abstract int OnBanCmid(string authToken, int cmid);

        public abstract int OnBanHwd(string authToken, string hwd);

        public abstract int OnUnbanCmid(string authToken, int cmid);

        public byte[] Ban(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string serviceAuth = StringProxy.Deserialize(bytes);
                int cmid = Int32Proxy.Deserialize(bytes);
                int result = OnBan(serviceAuth, cmid);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle Ban", ex);
                return null;
            }
        }

        public byte[] UnbanCmid(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                int cmid = Int32Proxy.Deserialize(bytes);
                int result = OnUnbanCmid(authToken, cmid);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle UnbanCmid", ex);
                return null;
            }
        }

        public byte[] BanCmid(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                int cmid = Int32Proxy.Deserialize(bytes);
                int result = OnBanCmid(authToken, cmid);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BanCmid", ex);
                return null;
            }
        }

        public byte[] BanIp(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                string ip = StringProxy.Deserialize(bytes);
                int result = OnBanIp(authToken, ip);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BanHwd", ex);
                return null;
            }
        }

        public byte[] BanHwd(byte[] data)
        {
            try
            {
                using MemoryStream bytes = new MemoryStream(data);
                string authToken = StringProxy.Deserialize(bytes);
                string hwd = StringProxy.Deserialize(bytes);
                int result = OnBanHwd(authToken, hwd);
                using MemoryStream outBytes = new MemoryStream();
                Int32Proxy.Serialize(outBytes, result);
                return outBytes.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BanHwd", ex);
                return null;
            }
        }
    }
}
