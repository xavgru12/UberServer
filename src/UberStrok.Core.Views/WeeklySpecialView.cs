
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberStrok.Core.Views {
	[Serializable]
	public class WeeklySpecialView {
		public int Id { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public string ImageUrl { get; set; }

		public int ItemId { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public WeeklySpecialView() {
		}

		public WeeklySpecialView(string title, string text, string imageUrl, int itemId) {
			this.Title = title;
			this.Text = text;
			this.ImageUrl = imageUrl;
			this.ItemId = itemId;
		}

		public WeeklySpecialView(string title, string text, string imageUrl, int itemId, int id, DateTime startDate, DateTime? endDate) : this(title, text, imageUrl, itemId) {
			this.Id = id;
			this.StartDate = startDate;
			this.EndDate = endDate;
		}
	}
}