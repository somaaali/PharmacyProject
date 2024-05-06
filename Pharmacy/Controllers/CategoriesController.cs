
namespace Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.PATIENT)]
    public class CategoriesController : ControllerBase
    {
        private readonly IDataRepo<Category> _categoryRepo;

        public CategoriesController( IDataRepo<Category> categoryRepo )
        {
            _categoryRepo = categoryRepo;
        }

        #region ENDPOINTS

        #region ViewCategories => api/Categories
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            var categoryDtos = categories.Select(c => new ViewCategoryDto
            {
                CategoryId=c.CategoryId,
				Name = c.Name,
            }).ToList();
            return Ok(categoryDtos);
        }
        #endregion

        #region ViewCategoryById => api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ViewCategoryById( int id )
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            // validate if the Category id is not found
            if ( category == null )
                return NotFound($"No Category Found with Id {id}");

            return Ok(category);
        }

        #endregion

        #region AddCategory api/Categories
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpPost]
        public async Task<IActionResult> AddCategory( CategoryDto dto )
        {
            var category = new Category
            {
                Name = dto.Name,
            };

            await _categoryRepo.AddAsync(category);
            await _categoryRepo.Save();
            return Ok(new { category.CategoryId, category.Name });
        }
        #endregion

        #region UpdateCategoryById api/Categories/{id}
        [Authorize(Roles = StaticUserRoles.ADMIN)]

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryById( int id, CategoryDto dto )
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            // validate if the Category id is not found
            if ( category == null )
                return NotFound($"No Category Found with Id {id}");

            category.Name = dto.Name;
            

            await _categoryRepo.UpdateAsync(category);
            await _categoryRepo.Save();

            return Ok(category);
        }

        #endregion

        #region DeleteCategoryById api/Categories/{id}
        [Authorize(Roles = StaticUserRoles.ADMIN)]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryById( int id )
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            // validate if the Category id is not found
            if ( category == null )
                return NotFound($"No Category Found with Id {id}");

            await _categoryRepo.DeleteAsync(category);
            await _categoryRepo.Save();

            return Ok(category);
        }

        #endregion

        #region ByName 

        #region ViewCategoriesByName => api/Categories/name/{name}
        [HttpGet("name/{name}")]

        public async Task<IActionResult> GetByName( string name )
        {
            var category = await _categoryRepo.GetByNameAsync(name);
            if ( category == null )
                return NotFound();

            var dto = new CategoryDto
            {
                Name = category.Name,
            };
            return Ok(dto);
        }
        #endregion

        #region UpdateCategoryByName api/Categories/name/{name}
        [Authorize(Roles = StaticUserRoles.ADMIN)]

        [HttpPut("name/{name}")]
        public async Task<IActionResult> UpdateAsync( string name, CategoryDto dto )
        {
            var category = await _categoryRepo.GetByNameAsync(name);
            if ( category == null )
                return NotFound($"No Category Was Found Named {name}");

            category.Name = dto.Name;
            await _categoryRepo.UpdateAsync(category);
            await _categoryRepo.Save();
            return Ok(category);
        }
        #endregion

        #region DeleteCategoryByName api/Categories/name/{name}
        [Authorize(Roles = StaticUserRoles.ADMIN)]

        [HttpDelete("name/{name}")]
        public async Task<IActionResult> DeleteAsync( string name )
        {
            var category = await _categoryRepo.GetByNameAsync(name);
            if ( category == null )
                return NotFound($"No Category Was Found Named {name}");

            await _categoryRepo.DeleteAsync(category);
            await _categoryRepo.Save();
            return Ok(category);
        }
        #endregion

        #endregion


        #endregion
    }
}
