namespace Pharmacy.Models
{
	public class SearchHistory
	{
		public int Id { get; set; }

		
		public string UserId { get; set; }
		public ApplicationUser User { get; set; } // Navigation property

		public string SearchQuery { get; set; }
	}
}
