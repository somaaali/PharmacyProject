
namespace Pharmacy.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RequestController : ControllerBase
	{

		private readonly PharmacyDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;




		public RequestController( PharmacyDbContext context, UserManager<ApplicationUser> userManager )
		{
			_context = context;
			_userManager = userManager;

		}


        #region ENDPOINTS

        #region Get Pending Requests
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpGet]
        public async Task<IActionResult> GetPendingRequests()
        {
            // Retrieve pending requests from the database
            var pendingRequests = await _context.requests
                .Where(r => r.Status == RequestStatus.Pending)
                .Include(r => r.Medicines) // Include medicines in the query
                .ToListAsync();

            // Check if there are pending requests
            if ( pendingRequests == null || pendingRequests.Count == 0 )
                return NotFound("No pending requests found.");

            // Create a list to store RequestDto objects
            var requestDtos = new List<RequestDto>();

            // Iterate through each pending request and create a RequestDto object
            foreach ( var request in pendingRequests )
            {
                // Find the patient associated with the request
                var patient = await _userManager.FindByIdAsync(request.UserId);
                if ( patient == null )
                {
                    // Log or handle the case where patient is null
                    continue; // Skip this request and move to the next one
                }

                // Create a RequestDto object and populate its properties
                var requestDto = new RequestDto
                {
                    PatientName = patient.UserName,
                    MedicinesNames = request.Medicines.Select(m => m.Name).ToList(),
                };

                // Add the RequestDto object to the list
                requestDtos.Add(requestDto);
            }

            // Return the list of RequestDto objects
            return Ok(requestDtos);
        }
        #endregion

        #region Get Requests By Patient Username
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpGet("{username}")]

        public async Task<IActionResult> GetRequestsByPatientUsername(string username)
		{
			// Find the user by username
			var user = await _userManager.FindByNameAsync(username);
			if (user == null)
				return NotFound("User not found.");

			// Retrieve requests associated with the user
			var requests = await _context.requests
				.Include(r => r.Medicines) 
				.Where(r => r.UserId == user.Id)
				.ToListAsync();

			// Check if requests exist
			if (requests == null || requests.Count == 0)
				return NotFound("No requests found for this user.");

			// Create a list to store RequestDto objects
			var requestDtos = new List<RequestDto>();

			// Iterate through each request and create a RequestDto object
			foreach (var request in requests)
			{
				var requestDto = new RequestDto
				{
					PatientName = username,
					MedicinesNames = request.Medicines.Select(m => m.Name).ToList(),
					Status = request.Status
				};

				// Add the RequestDto object to the list
				requestDtos.Add(requestDto);
			}

			// Return the list of RequestDto objects
			return Ok(requestDtos);
		}
        #endregion

        #region Update Requests Statue
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpPut("{id}")]
		public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] string status)
		{
			// Find the request by id
			var request = await _context.requests.FindAsync(id);

			if (request == null)
				return NotFound();

			// Validate the status string
			if (!Enum.TryParse(status, true, out RequestStatus requestStatus))
				return BadRequest("Invalid status value.");

			// Update request status
			request.Status = requestStatus;
			_context.SaveChanges();

			return Ok(request);
		}

		#endregion

		#region Add Request

		[HttpPost]
		public async Task<IActionResult> PostRequest([FromBody] AddRequestDto requestDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Assuming you have logic to retrieve the current user
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			if (currentUser == null)
			{
				return Unauthorized();
			}

			// Create a new Request entity
			var newRequest = new Request
			{
				UserId = currentUser.Id, 
				Status = RequestStatus.Pending, 												
				Medicines = await _context.medicines
					.Where(m => requestDto.MedicinesNames.Contains(m.Name))
					.ToListAsync()
			};

			// Add the new request to the database
			_context.requests.Add(newRequest);
			await _context.SaveChangesAsync();

            // Return the newly created request
            var requestDtoResponse = new RequestDto
            {
                PatientName = currentUser.UserName,
                MedicinesNames = newRequest.Medicines.Select(m => m.Name).ToList(),
            };

            return CreatedAtAction(nameof(GetRequestsByPatientUsername), new { username = currentUser.UserName }, requestDtoResponse);
        }

        #endregion


        #endregion
    }
}
