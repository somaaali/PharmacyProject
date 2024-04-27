﻿namespace Pharmacy.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }

		public string Phone { get; set; }
		// public string Role { get; set; }
	}
}