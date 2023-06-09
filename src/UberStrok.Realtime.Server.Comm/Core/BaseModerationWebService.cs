﻿using log4net;
using System;
using System.IO;
using System.ServiceModel;
using UberStrok.Core.Serialization;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public abstract class BaseModerationWebService : BaseWebService, IModerationWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseAuthenticationWebService).Name);

        protected BaseModerationWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public abstract int OnBan(string serviceAuth, int cmid);
        public abstract int OnUnbanCmid(string authToken, int cmid);
        public abstract int OnBanCmid(string authToken, int cmid);
        public abstract int OnBanIp(string authToken, string ip);
        public abstract int OnBanHwd(string authToken, string hwd);

        byte[] IModerationWebServiceContract.Ban(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    string serviceAuth = StringProxy.Deserialize(bytes);
                    int cmid = Int32Proxy.Deserialize(bytes);

                    var result = OnBan(serviceAuth, cmid);
                    using (var outBytes = new MemoryStream())
                    {
                        Int32Proxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle Ban", ex);
                return null;
            }
        }

        byte[] IModerationWebServiceContract.UnbanCmid(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    int cmid = Int32Proxy.Deserialize(bytes);

                    var result = OnUnbanCmid(authToken, cmid);
                    using (var outBytes = new MemoryStream())
                    {
                        Int32Proxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle UnbanCmid", ex);
                return null;
            }
        }

        byte[] IModerationWebServiceContract.BanCmid(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    int cmid = Int32Proxy.Deserialize(bytes);

                    var result = OnBanCmid(authToken, cmid);
                    using (var outBytes = new MemoryStream())
                    {
                        Int32Proxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BanCmid", ex);
                return null;
            }
        }

        byte[] IModerationWebServiceContract.BanHwd(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    string hwd = StringProxy.Deserialize(bytes);

                    var result = OnBanHwd(authToken, hwd);
                    using (var outBytes = new MemoryStream())
                    {
                        Int32Proxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BanHwd", ex);
                return null;
            }
        }

        byte[] IModerationWebServiceContract.BanIp(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    string authToken = StringProxy.Deserialize(bytes);
                    string ip = StringProxy.Deserialize(bytes);

                    var result = OnBanIp(authToken, ip);
                    using (var outBytes = new MemoryStream())
                    {
                        Int32Proxy.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BanHwd", ex);
                return null;
            }
        }
    }
}
