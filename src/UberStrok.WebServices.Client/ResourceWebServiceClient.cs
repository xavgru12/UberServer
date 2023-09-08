using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Serialization;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{
    public class ResourceWebServiceClient : BaseWebServiceClient<IApplicationWebServiceContract>
    {
        public ResourceWebServiceClient(string endPoint) : base(endPoint, "ApplicationWebService")
        {
        }

        public MapView GetMap(string authToken)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);

                var data = Channel.GetMaps(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return MapViewProxy.Deserialize(inBytes);
            }
        }
    }
}
