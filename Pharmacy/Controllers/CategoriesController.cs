

namespace Pharmacy.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = StaticUserRoles.ADMIN)]

	public class CategoriesController : ControllerBase
	{
		
			private readonly PharmacyDbContext _context;

			public CategoriesController(PharmacyDbContext context)
			{
				_context = context;
			}
			#region ENDPOINTS

			#region ViewCategories => api/Categories
			[HttpGet]
			public async Task<IActionResult> GetAllAsync()
			{
				var Categories = await _context.Categories.Select(c => new CategoryDto
				{
					Name = c.Name,
				}).ToListAsync();
				return Ok(Categories);
			}
			#endregion

			#region ViewCategoriesByName => api/Categories/{name}
			[HttpGet("{name}")]
			public async Task<IActionResult> GetByName(string name)
			{
				var Category = await _context.Categories.Where(c => c.Name == name).FirstOrDefaultAsync();
            if (Category == null)
					return NotFound();
				var dto = new CategoryDto
				{
					Name = Category.Name,

				};
				return Ok(dto);
			}	
		#endregion

			#region AddCategory api/Categories
			[HttpPost]
			public async Task<IActionResult> AddCategory(CategoryDto dto)
			{
				
				var category = new Category
				{
					Name = dto.Name,
				};

				await _context.Categories.AddAsync(category);
				await _context.SaveChangesAsync();

				var responseData = new
				{
					CategoryId = category.CategoryId,
					Name = category.Name,
				};

				return Ok(responseData);
			}
		#endregion

			#region DeleteCategory api/Categories/{name}
			[HttpDelete("{name}")]
			public async Task<IActionResult> DeleteAsync(string name)
			{
				var category = await _context.Categories.FindAsync(name);
				if (category == null)

					return NotFound($"No Category Was Found With ID {name}");
				_context.Remove(category);
				_context.SaveChanges();
				return Ok(category);
			}
		#endregion

			#region UpdateCategory api/Categories/{name}
			[HttpPut("{name}")]
			public async Task<IActionResult> UpdateAsync(string name, CategoryDto dto)
			{


				var category = await _context.Categories.FindAsync(name);
				if (category == null)

					return NotFound($"No Category Was Found With ID {name}");
				category.Name = dto.Name;
				_context.SaveChanges();
				return Ok(category);
				
			}
		#endregion

		#endregion
	}
}
