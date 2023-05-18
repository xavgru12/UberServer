using System;
using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberTransactions
    {
        public Guid Id { get; set; }
        public List<ItemTransaction> Items { get; set; }
        public List<PointsDeposit> Points { get; set; }
    }
}
