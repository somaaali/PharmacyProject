namespace Pharmacy.Dtos
{
	public class RegisterDto
	{
	
			[Required(ErrorMessage = "FirstName is required")]
			public string FirstName { get; set; }

			[Required(ErrorMessage = "LastName is required")]
			public string LastName { get; set; }

			[Required(ErrorMessage = "UserName is required")]
			public string UserName { get; set; }

			[EmailAddress]
			[Required(ErrorMessage = "Email is required")]
			public string Email { get; set; }

			[Required(ErrorMessage = "Password is required")]
			public string Password { get; set; }

			[Required(ErrorMessage = "Address is required")]
			public string Address { get; set; }

			[Phone]
			[Required(ErrorMessage = "Phone is required")]
			public string Phone { get; set; }
	}
}
