using System;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Helper
{
    public static class ClanHelper
    {
        public static ClanView GetClanView(GroupCreationView groupCreation, int groupId, PublicProfileView profileView)
        {
            return new ClanView
            {
                GroupId = groupId,
                Name = groupCreation.Name,
                Tag = groupCreation.Tag.Replace(" ", ""),
                Motto = groupCreation.Motto,
                Description = groupCreation.Description,
                MembersLimit = 12,
                ApplicationId = 1,
                FoundingDate = DateTime.Now,
                OwnerCmid = profileView.Cmid,
                OwnerName = profileView.Name
            };
        }

        public static bool IsTagLocked(this List<LockedClanTags> lockedClanTags, int cmid, string tag)
        {
            foreach (LockedClanTags locked in lockedClanTags)
            {
                if (tag.Replace(" ", "") == locked.Tag)
                {
                    return cmid != locked.Cmid;
                }
            }
            return false;
        }
    }

    public class LockedClanTags
    {
        public string Tag { get; set; }
        public int Cmid { get; set; }
    }
}
