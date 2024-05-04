namespace Pharmacy.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		[EmailAddress]
		public string Address { get; set; }
		[Phone]
		public string Phone { get; set; }
	}
}
