﻿using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class GroupCreationView
    {
        public GroupCreationView()
        {
        }

        public GroupCreationView(string name, string description, string motto, string address, bool hasPicture, int applicationId, string authToken, string tag, string locale)
        {
            Name = name;
            Description = description;
            Motto = motto;
            Address = address;
            HasPicture = hasPicture;
            ApplicationId = applicationId;
            AuthToken = authToken;
            Tag = tag;
            Locale = locale;
        }

        public GroupCreationView(string name, string motto, int applicationId, string authToken, string tag, string locale)
        {
            Name = name;
            Description = string.Empty;
            Motto = motto;
            Address = string.Empty;
            HasPicture = false;
            ApplicationId = applicationId;
            AuthToken = authToken;
            Tag = tag;
            Locale = locale;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Motto { get; set; }

        public string Address { get; set; }

        public bool HasPicture { get; set; }

        public int ApplicationId { get; set; }

        public string AuthToken { get; set; }

        public string Tag { get; set; }

        public string Locale { get; set; }

        public override string ToString()
        {
            string text = string.Concat(new string[]
            {
                "[GroupCreationView: [name:",
                Name,
                "][description:",
                Description,
                "][Motto:",
                Motto,
                "]"
            });
            string text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "[Address:",
                Address,
                "][Has picture:",
                HasPicture,
                "][Application Id:",
                ApplicationId,
                "][AuthToken:",
                AuthToken,
                "]"
            });
            text2 = text;
            return string.Concat(new string[]
            {
                text2,
                "[Tag:",
                Tag,
                "][Locale:",
                Locale,
                "]"
            });
        }
    }
}
