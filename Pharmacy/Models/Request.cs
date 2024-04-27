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
			

			// many to many with medicine
			public List<Medicine> Medicines { get; set; } //relation with medicine is many to many


			public Request()
			{
				Status = RequestStatus.Pending; // Set default value to RequestStatus.Pending enum value
			}
		}
}
