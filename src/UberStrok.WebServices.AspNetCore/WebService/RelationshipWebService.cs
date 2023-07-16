using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream;
using UberStrok.WebServices.AspNetCore.Core.Manager;
using UberStrok.WebServices.AspNetCore.Core.Session;
using UberStrok.WebServices.AspNetCore.WebService.Base;

namespace UberStrok.WebServices.AspNetCore.WebService
{
    public class RelationshipWebService : BaseRelationshipWebService
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(RelationshipWebService));
        private readonly GameSessionManager gameSessionManager;
        private readonly StreamManager streamManager;
        private readonly UserManager userManager;
        public RelationshipWebService(GameSessionManager gameSessionManager, StreamManager streamManager, UserManager userManager)
        {
            this.gameSessionManager = gameSessionManager;
            this.streamManager = streamManager;
            this.userManager = userManager;
        }

        public override async Task<List<ContactGroupView>> OnGetContactGroupView(string authToken)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                List<ContactGroupView> contactGroup = new List<ContactGroupView>();
                ContactGroupView defaultContact = new ContactGroupView();
                for (int i = 0; i < session.Document.Friends.Count; i++)
                {
                    int Id = session.Document.Friends[i];
                    PublicProfileView publicProfileView = gameSessionManager.TryGet(Id, out GameSession friendSession) ? friendSession.Document.Profile : (await userManager.GetUser(Id)).Profile;
                    PublicProfileView profile = publicProfileView;
                    if (profile != null)
                    {
                        defaultContact.Contacts.Add(profile);
                    }
                    else
                    {
                        Log.Error("Unable to obtain user profile!");
                    }
                }
                contactGroup.Add(defaultContact);
                return contactGroup;
            }
            return null;
        }

        public override async Task<List<ContactRequestView>> OnGetContactRequestsView(string authToken)
        {
            return gameSessionManager.TryGet(authToken, out GameSession session)
                ? (await streamManager.Get(session.Document.Streams)).Where((f) => f.StreamType == StreamType.ContactRequest && ((ContactRequestStream)f).ContactRequest.Status == ContactRequestStatus.Pending).Select((s) => ((ContactRequestStream)s).ContactRequest).ToList()
                : null;
        }

        public override async Task<PublicProfileView> OnAcceptContactRequest(string authToken, int contactRequestId)
        {
            if (!gameSessionManager.TryGet(authToken, out GameSession session))
            {
                Log.Error("Unable to accept contact request. An unidentified AuthToken was passed.");
                return null;
            }
            StreamDocument document = await streamManager.Get(contactRequestId);
            if (document != null && document.StreamType == StreamType.ContactRequest)
            {
                ContactRequestStream contactRequest = (ContactRequestStream)document;
                if (session.Document.FriendRequests.Remove(contactRequest.ContactRequest.InitiatorCmid))
                {
                    UserDocument userDocument = gameSessionManager.TryGet(contactRequest.ContactRequest.InitiatorCmid, out GameSession friendSession) ? friendSession.Document : await userManager.GetUser(contactRequest.ContactRequest.InitiatorCmid);
                    UserDocument user = userDocument;
                    if (user != null)
                    {
                        _ = session.Document.Streams.RemoveAll((T) => T == contactRequestId);
                        session.Document.Friends.Add(contactRequest.ContactRequest.InitiatorCmid);
                        user.Friends.Add(session.Document.UserId);
                        contactRequest.ContactRequest.Status = ContactRequestStatus.Accepted;
                        await userManager.Save(session.Document);
                        await userManager.Save(user);
                        await streamManager.Save(contactRequest);
                        return user.Profile;
                    }
                    Log.Error("Unable to accept contact request. User is null");
                }
                else
                {
                    Log.Error("Unable to accept contact request. Fail to remove id from friend request");
                }
            }
            else
            {
                Log.Error("Unable to accept contact request. An unidentified stream was passed.");
            }
            return null;
        }

        public override async Task<bool> OnDeclineContactRequest(string authToken, int contactRequestId)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                StreamDocument document = await streamManager.Get(contactRequestId);
                if (document != null && document.StreamType == StreamType.ContactRequest)
                {
                    ContactRequestStream contactRequest = (ContactRequestStream)document;
                    if (session.Document.FriendRequests.Remove(contactRequest.ContactRequest.InitiatorCmid))
                    {
                        _ = session.Document.Streams.RemoveAll((T) => T == contactRequestId);
                        contactRequest.ContactRequest.Status = ContactRequestStatus.Refused;
                        await userManager.Save(session.Document);
                        await streamManager.Save(contactRequest);
                        return true;
                    }
                }
            }
            return false;
        }

        public override MemberOperationResult OnDeleteContact(string authToken, int contactCmid)
        {
            if (gameSessionManager.TryGet(authToken, out GameSession session))
            {
                if (session.Document.Friends.Contains(contactCmid))
                {
                    UserDocument user = !gameSessionManager.TryGet(contactCmid, out GameSession friendSession) ? userManager.GetUser(contactCmid).Result : friendSession.Document;
                    if (user != null && user.Friends.Contains(session.Document.UserId))
                    {
                        _ = session.Document.Friends.Remove(contactCmid);
                        _ = user.Friends.Remove(session.Document.UserId);
                        _ = userManager.Save(user);
                        _ = userManager.Save(session.Document);
                        return MemberOperationResult.Ok;
                    }
                }
                return MemberOperationResult.MemberNotFound;
            }
            return MemberOperationResult.InvalidCmid;
        }

        public override async Task OnSendContactRequest(string authToken, int receiverCmid, string message)
        {
            if (!gameSessionManager.TryGet(authToken, out GameSession session))
            {
                return;
            }
            UserDocument userDocument = gameSessionManager.TryGet(receiverCmid, out GameSession friendSession) ? friendSession.Document : await userManager.GetUser(receiverCmid);
            UserDocument receiver = userDocument;
            if (receiver != null)
            {
                if (!receiver.FriendRequests.Contains(session.Document.UserId))
                {
                    UserDocument Initiator = session.Document;
                    int streamId = await streamManager.GetNextId();
                    ContactRequestStream contactRequest = new ContactRequestStream
                    {
                        StreamId = streamId,
                        ContactRequest = new ContactRequestView
                        {
                            InitiatorCmid = Initiator.UserId,
                            InitiatorName = Initiator.Profile.Name,
                            InitiatorMessage = message,
                            ReceiverCmid = receiverCmid,
                            SentDate = DateTime.Now,
                            RequestId = streamId
                        }
                    };
                    await streamManager.Create(contactRequest);
                    receiver.Streams.Add(streamId);
                    receiver.FriendRequests.Add(session.Document.UserId);
                    await userManager.Save(receiver);
                }
            }
            else
            {
                Log.Error("Unable to obtain user profile!");
            }
        }
    }
}
