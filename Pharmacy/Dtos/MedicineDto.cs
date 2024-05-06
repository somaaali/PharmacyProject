namespace Pharmacy.Dtos
{
	public class MedicineDto
	{
			public string Name { get; set; }
			public string Description { get; set; }
			public decimal Price { get; set; }
		//    public IFormFile Image { get; set; } // to store Media

		   public int CategoryId { get; set; } // FK
		}
}
