using System.ServiceModel;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RelationshipWebService : BaseWebService, IRelationshipWebServiceContract
    {
        //TODO: Implement BaseRelationshipWebService.

        private readonly static ILog Log = LogManager.GetLogger(typeof(RelationshipWebService).Name);

        public RelationshipWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public byte[] AcceptContactRequest(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] DeclineContactRequest(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] DeleteContact(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] GetContactRequests(byte[] data)
        {
            //throw new NotImplementedException();
            return null;
        }


        public byte[] GetContactsByGroups(byte[] data) {
            
				using (var bytes = new MemoryStream(data)) {
					var cmid = Int32Proxy.Deserialize(bytes);
					var applicationId = Int32Proxy.Deserialize(bytes);

					using (var outputStream = new MemoryStream()) {
						ListProxy<ContactGroupView>.Serialize(outputStream, new List<ContactGroupView>(), ContactGroupViewProxy.Serialize);

						return outputStream.ToArray();
					}
				}

		}

        public byte[] SendContactRequest(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
