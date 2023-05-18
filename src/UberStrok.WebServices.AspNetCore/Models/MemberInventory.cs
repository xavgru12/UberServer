using System;
using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberInventory
    {
        public Guid Id { get; set; }
        public Dictionary<int, InventoryItem> Items { get; set; }
    }
}
