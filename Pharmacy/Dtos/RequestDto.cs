namespace Pharmacy.Dtos
{
	public class RequestDto
	{
		public string PatientName { get; set; }
		public List<string> MedicinesNames { get; set; }

		public RequestStatus Status { get; set; }

	}
}
