﻿using Pharmacy.Repositories;

namespace Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StaticUserRoles.ADMIN)]
    public class MedicinesController : ControllerBase
    {
        private readonly IDataRepo<Medicine> _medicineRepo;

        public MedicinesController( IDataRepo<Medicine> medicineRepo )
        {
            _medicineRepo = medicineRepo;
        }

        #region ENDPOINTS

        #region ViewMedicines => api/Medicines
        [HttpGet]
        public async Task<IActionResult> ViewMedicines()
        {
            var medicines = await _medicineRepo.GetAllAsync();
            return Ok(medicines);
        }
        #endregion

        #region ViewMedicineById => api/Medicines/{id}
        [HttpGet("{id}")]
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
            // if (!isValidCategory) return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");

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

            // validate the entered category id (if necessary)
            // var isValidCategory = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            // if (!isValidCategory) return BadRequest($"Invalid Category No Category with Id {dto.CategoryId}");

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
    }
}
