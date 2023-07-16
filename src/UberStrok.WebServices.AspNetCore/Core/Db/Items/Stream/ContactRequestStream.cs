using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream
{
    public class ContactRequestStream : StreamDocument
    {
        public ContactRequestView ContactRequest
        {
            get;
            set;
        }

        public override StreamType StreamType => StreamType.ContactRequest;
    }
}
