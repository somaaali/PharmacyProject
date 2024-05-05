namespace Pharmacy.Models
{
	public class Medicine
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }

		[FileExtensions(Extensions = "jpg,jpeg,png")]
		[DataType(DataType.ImageUrl)]
		public string MedicineImagePath { get; set; }

		// one to many with category
		public int CategoryId { get; set; } // FK
		public Category Category { get; set; } // Navigation property

		// many to many with request
		public List<Request> Requests { get; set; }
	}
}
