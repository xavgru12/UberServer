using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream
{
    public class GroupInvitationStream : StreamDocument
    {
        public GroupInvitationView GroupInvitation
        {
            get;
            set;
        }

        public override StreamType StreamType => StreamType.GroupInvitation;
    }
}
