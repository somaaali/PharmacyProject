namespace Pharmacy.Models
{
	
		public enum RequestStatus
		{
			Pending,
			Approved,
			Rejected
		};

		public class Request
		{
			public int Id { get; set; }
			public RequestStatus Status { get; set; }

		// one to many with patient
		public string UserId { get; set; } // Foreign key to User/Identity
		public ApplicationUser User { get; set; } // Navigation property

		// many to many with medicine
		public List<Medicine>? Medicines { get; set; } //relation with medicine is many to many
			public Request()
			{
				Status = RequestStatus.Pending; // Set default value to RequestStatus.Pending enum value
			}
		}
}
