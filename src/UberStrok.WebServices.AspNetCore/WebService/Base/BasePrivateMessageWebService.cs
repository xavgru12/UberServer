using log4net;
using System;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Serialization;
using UberStrok.WebServices.AspNetCore.Contracts;

namespace UberStrok.WebServices.AspNetCore.WebService.Base
{
    public class BasePrivateMessageWebService : IPrivateMessageAsyncWebServiceContract
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BasePrivateMessageWebService));

        public Task<byte[]> DeleteThread(byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetAllMessageThreadsForUser(byte[] data)
        {
            try
            {
                return Task.Run(() =>
                {
                    using MemoryStream bytes = new MemoryStream(data);
                    string authToken = StringProxy.Deserialize(bytes);
                    int page = Int32Proxy.Deserialize(bytes);
                    using MemoryStream outBytes = new MemoryStream();
                    return outBytes.ToArray();
                });
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllMessageThreadsForUser request:");
                Log.Error(ex);
                return null;
            }
        }

        public Task<byte[]> GetMessageWithIdForCmid(byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetThreadMessages(byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> MarkThreadAsRead(byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> SendMessage(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
