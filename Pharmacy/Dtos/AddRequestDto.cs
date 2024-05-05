namespace Pharmacy.Dtos
{
	public class AddRequestDto
	{
		public string PatientName { get; set; }
		public List<string> MedicinesNames { get; set; }
	}
}
