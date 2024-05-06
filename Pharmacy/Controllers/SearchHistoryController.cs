
namespace Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SearchHistoryController : ControllerBase
    {
        private readonly IDataRepo<SearchHistory> _searchHistoryRepo;
        private readonly IDataRepo<Category> _categoryRepo;

        public SearchHistoryController(
            IDataRepo<SearchHistory> searchHistoryRepo,
            IDataRepo<Category> categoryRepo
            )
        {
            _searchHistoryRepo = searchHistoryRepo;
            _categoryRepo = categoryRepo;
        }


        [HttpPost]
        public async Task<IActionResult> CaptureSearchHistory( string searchQuery )
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if ( string.IsNullOrEmpty(userId) )
            {
                return BadRequest("User ID not found in claims.");
            }

            var searchHistoryRecord = new SearchHistory
            {
                UserId = userId,
                SearchQuery = searchQuery,
            };

            await _searchHistoryRepo.AddAsync(searchHistoryRecord);
            await _searchHistoryRepo.Save();

            return Ok();
        }

        // Retrieve current user's search history
        [HttpGet]
        public async Task<IActionResult> GetUserSearchHistory()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var searchHistory = await _searchHistoryRepo.GetSearchHistoryByUserId(userId);
            var searchHistoryDto = searchHistory.Select(s => new SearchHistoryDto
            {
                SearchQuery = s.SearchQuery
            });

            return Ok(searchHistoryDto);
        }

    }
}
