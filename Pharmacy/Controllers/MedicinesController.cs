
namespace Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IDataRepo<Medicine> _medicineRepo;
        private readonly IDataRepo<Category> _categoryRepo;

        public MedicinesController(
            IDataRepo<Medicine> medicineRepo,
            IDataRepo<Category> categoryRepo
            )
        {
            _medicineRepo = medicineRepo;
            _categoryRepo = categoryRepo;
        }
        //private new List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        //private long _maxAllowedImageSize = 1024 * 1024 * 5; // 5 MB

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
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpPost]
        public async Task<IActionResult> AddMedicine( MedicineDto dto )
        {

            var isValidCategory = await _categoryRepo.GetByIdAsync(dto.CategoryId) != null;
            if ( !isValidCategory ) return BadRequest($"Invalid CategoryId , No Category with Id {dto.CategoryId}");


            # region handling image upload 
            /*
            // validate the image size
            if ( !_allowedExtensions.Contains(Path.GetExtension(dto.Image.FileName).ToLower()) )
                return BadRequest("Invalid Image Format, Only .jpg and .png are allowed");
            // validate the image size
            if ( dto.Image.Length > _maxAllowedImageSize )
                return BadRequest("Image size is too large, Maximum allowed size is 5MB");

            // to store the image in the database we need to convert it to byte array
            using var dataStream = new MemoryStream();
            await dto.Image.CopyToAsync(dataStream);
            */
            #endregion

            var medicine = new Medicine
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                //Image = dataStream.ToArray(),
                CategoryId = dto.CategoryId,
            };

            await _medicineRepo.AddAsync(medicine);
            await _medicineRepo.Save();
            return Ok(medicine);
        }

        #endregion

        #region UpdateMedicine api/Medicines/{id}
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine( int id,MedicineDto dto )
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);

            // validations
            // validate if the Medicine id is not found
            if ( medicine == null )
                return NotFound($"No Medicine Found with Id {id}");

            // validate the entered category id
            var isValidCategory = await _categoryRepo.GetByIdAsync(dto.CategoryId) != null;
            if ( !isValidCategory ) return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");


            #region  handling image upload
            /*
            if ( dto.Image != null )
            {
                // validate the image size
                if ( !_allowedExtensions.Contains(Path.GetExtension(dto.Image.FileName).ToLower()) )
                    return BadRequest("Invalid Image Format, Only .jpg and .png are allowed");

                // validate the image size
                if ( dto.Image.Length > _maxAllowedImageSize )
                    return BadRequest("Image size is too large, Maximum allowed size is 5MB");

                // to store the image in the database we need to convert it to byte array
                using var dataStream = new MemoryStream();
                await dto.Image.CopyToAsync(dataStream);

                medicine.Image = dataStream.ToArray();
            }
            */
            #endregion

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
        [Authorize(Roles = StaticUserRoles.ADMIN)]
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
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpPut("name/{name}")]
        public async Task<IActionResult> UpdateMedicineByName( string name, MedicineDto dto )
        {
            var medicine = await _medicineRepo.GetByNameAsync(name);
            if ( medicine == null ) return NotFound($"No Medicine Named {name}");

            // validate the entered category id
            var isValidCategory = await _categoryRepo.GetByIdAsync(dto.CategoryId) != null;
            if ( !isValidCategory ) return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");

            #region  handling image upload
            /*
            if ( dto.Image != null )
            {
                // validate the image size
                if ( !_allowedExtensions.Contains(Path.GetExtension(dto.Image.FileName).ToLower()) )
                    return BadRequest("Invalid Image Format, Only .jpg and .png are allowed");

                // validate the image size
                if ( dto.Image.Length > _maxAllowedImageSize )
                    return BadRequest("Image size is too large, Maximum allowed size is 5MB");

                // to store the image in the database we need to convert it to byte array
                using var dataStream = new MemoryStream();
                await dto.Image.CopyToAsync(dataStream);

                medicine.Image = dataStream.ToArray();
            }
            */
            #endregion

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
        [Authorize(Roles = StaticUserRoles.ADMIN)]
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

        #region Filtre Medicines => api/Medicines/Filter
        [HttpGet("Filter")]
        public async Task<IEnumerable<MedicineDto>> FilterMedicines( string keyword )
        {


            var medicines = await _medicineRepo.FilterMedicines(keyword);

            return medicines.Select(m => new MedicineDto
            {
                Name = m.Name,
                Description = m.Description,
                Price = m.Price
            });
        }
        #endregion

        #region Filter Medicines by Category => api/Medicines/FilterCategory
        [HttpGet("FilterCategory")]
        public async Task<IActionResult> FilterMedicinesByCategory( [FromQuery] string category )
        {
            var medicines = await _medicineRepo.FilterMedicinesByCategory(category);

            if ( medicines == null || !medicines.Any() )
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
