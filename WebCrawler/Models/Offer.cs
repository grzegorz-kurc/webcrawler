namespace WebCrawler.Models
{
	public class Offer
	{
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public string Url { get; set; }
		public string? Price { get; set; }
		public string? PromoPrice { get; set; }
		public string? OldPrice { get; set; }
		public string? Description { get; set; }
	}
}
