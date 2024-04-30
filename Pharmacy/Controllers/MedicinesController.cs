using Microsoft.EntityFrameworkCore;
using Pharmacy.Repositories;

namespace Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StaticUserRoles.ADMIN)]
    public class MedicinesController : ControllerBase
    {
        private readonly IDataRepo<Medicine> _medicineRepo;
        private readonly IDataRepo<Category> _categoryRepo;

        public MedicinesController(
            IDataRepo<Medicine> medicineRepo ,
            IDataRepo<Category> categoryRepo
            )
        {
            _medicineRepo = medicineRepo;
            _categoryRepo = categoryRepo;
        }

        #region ENDPOINTS

        #region Crud

        #region ViewMedicines => api/Medicines
        [HttpGet]
        public async Task<IActionResult> ViewMedicines()
        {
            var medicines = await _medicineRepo.GetAllAsync();
            return Ok(medicines);
        }
        #endregion

        #region ViewMedicineById => api/Medicines/{id}
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetMedicineById( int id )
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);

            // validate if the Medicine id is not found
            if ( medicine == null )
                return NotFound($"No Medicine Found with Id {id}");

            return Ok(medicine);
        }
        #endregion

        #region AddMedicine api/Medicines
        [HttpPost]
        public async Task<IActionResult> AddMedicine( MedicineDto dto )
        {
            // validate the entered category id (if necessary)
            // var isValidCategory = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            //if (!isValidCategory) return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");

            var isValidCategory = await _categoryRepo.GetByIdAsync(dto.CategoryId) != null;
            if ( !isValidCategory ) return BadRequest($"Invalid CategoryId , No Category with Id {dto.CategoryId}");


            var medicine = new Medicine
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
            };

            await _medicineRepo.AddAsync(medicine);
            await _medicineRepo.Save();
            return Ok(medicine);
        }

        #endregion
 
        #region UpdateMedicine api/Medicines/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine( int id, MedicineDto dto )
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);

            // validate if the Medicine id is not found
            if ( medicine == null )
                return NotFound($"No Medicine Found with Id {id}");

            // validate the entered category id
            var isValidCategory = await _categoryRepo.GetByIdAsync(dto.CategoryId) != null;
            if ( !isValidCategory ) return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");


            medicine.Name = dto.Name;
            medicine.Description = dto.Description;
            medicine.Price = dto.Price;
            medicine.CategoryId = dto.CategoryId;

            await _medicineRepo.UpdateAsync(medicine);
            await _medicineRepo.Save();
            return Ok(medicine);
        }
        #endregion

        #region DeleteMedicine api/Medicines/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine( int id )
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);

            // validate if the Medicine id is not found
            if ( medicine == null )
                return NotFound($"No Medicine Found with Id {id}");

            await _medicineRepo.DeleteAsync(medicine);
            await _medicineRepo.Save(); 
            return Ok(medicine);
        }
        #endregion

        #endregion

        #region ByName 

        #region ViewMedicineByName => api/Medicines/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName( string name )
        {
            var medicine = await _medicineRepo.GetByNameAsync(name);
            if ( medicine == null )
                return NotFound($"No Medicine is Named {name}");

            return Ok(medicine);
        }
        #endregion

        #region UpdateMedicineByName => PUT api/Medicines/name/{name}
        [HttpPut("name/{name}")]
        public async Task<IActionResult> UpdateMedicineByName( string name, MedicineDto dto )
        {
            var medicine = await _medicineRepo.GetByNameAsync(name);
            if ( medicine == null )
                return NotFound($"No Medicine Named {name}");

            // validate the entered category id 
            var isValidCategory = await _categoryRepo.GetByIdAsync(dto.CategoryId) != null;
            if ( !isValidCategory )
                return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");

            medicine.Name = dto.Name;
            medicine.Description = dto.Description;
            medicine.Price = dto.Price;
            medicine.CategoryId = dto.CategoryId;

            await _medicineRepo.UpdateAsync(medicine);
            await _medicineRepo.Save();

            return Ok(medicine);
        }
        #endregion

        #region DeleteMedicineByName => DELETE api/Medicines/name/{name}
        [HttpDelete("name/{name}")]
        public async Task<IActionResult> DeleteMedicineByName( string name )
        {
            var medicine = await _medicineRepo.GetByNameAsync(name);
            if ( medicine == null )
                return NotFound($"No Medicine Found with Name {name}");

            await _medicineRepo.DeleteAsync(medicine);
            await _medicineRepo.Save();

            return Ok(medicine);
        }
		#endregion


		#endregion

		#region Search Medicines => api/Medicines/search
		[HttpGet("search")]
		public async Task<IEnumerable<MedicineDto>> SearchMedicines(string keyword)
		{
			

			var medicines = await _medicineRepo.SearchMedicines(keyword);

			return medicines.Select(m => new MedicineDto
			{
				Name = m.Name,
				Description = m.Description,
				Price = m.Price
			});
		}
		#endregion

		#region Filter Medicines by Category => api/Medicines/filter
		[HttpGet("filter")]
		public async Task<IActionResult> FilterMedicinesByCategory([FromQuery] string category)
		{
			var medicines = await _medicineRepo.FilterMedicinesByCategory(category);

			if (medicines == null || !medicines.Any())
			{
				return NotFound("No medicines found for the specified category.");
			}

			var medicineDtos = medicines.Select(m => new MedicineDto
			{
				Name = m.Name,
				Description = m.Description,
				Price = m.Price
			});

			return Ok(medicineDtos);
		}
		#endregion
		#endregion
	}
}
