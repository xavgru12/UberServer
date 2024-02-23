using System;
using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ContactGroupView
    {
        public ContactGroupView()
        {
            Contacts = new List<PublicProfileView>(0);
            GroupName = string.Empty;
        }

        public ContactGroupView(int groupID, string groupName, List<PublicProfileView> contacts)
        {
            GroupId = groupID;
            GroupName = groupName;
            Contacts = contacts;
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<PublicProfileView> Contacts { get; set; }

        public override string ToString()
        {
            string text = string.Concat(new object[]
            {
                "[Contact group: [Group ID: ",
                GroupId,
                "][Group Name :",
                GroupName,
                "][Contacts: "
            });
            foreach (PublicProfileView publicProfileView in Contacts)
            {
                text = text + "[Contact: " + publicProfileView.ToString() + "]";
            }
            text += "]]";
            return text;
        }
    }
}
