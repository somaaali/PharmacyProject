
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

        #region Get Requests
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpGet]
        public async Task<IActionResult> GetRequests()
        {
            // Retrieve all requests from the database
            var allRequests = await _context.requests
                .Include(r => r.Medicines) // Include medicines in the query
                .ToListAsync();

            if ( allRequests == null || allRequests.Count == 0 )
                return NotFound("No requests found.");

            var requestDtos = new List<RequestDto>();
            foreach ( var request in allRequests )
            {
                // Find the patient 
                var patient = await _userManager.FindByIdAsync(request.UserId);
                if ( patient == null )
                {
                    continue;
                }

                var requestDto = new RequestDto
                {
					RequestId = request.Id,
                    PatientName = patient.UserName,
                    MedicinesNames = request.Medicines.Select(m => m.Name).ToList(),
                    Status = request.Status
                };

                requestDtos.Add(requestDto);
            }

            // Return the list of RequestDto objects
            return Ok(requestDtos);
        }
        #endregion

        #region Get Pending Requests
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var pendingRequests = await _context.requests
                .Where(r => r.Status == RequestStatus.Pending)
                .Include(r => r.Medicines)
                .ToListAsync();

            if ( pendingRequests == null || pendingRequests.Count == 0 )
                return NotFound("No pending requests found.");

            var requestDtos = new List<RequestDto>();

            foreach ( var request in pendingRequests )
            {
                var patient = await _userManager.FindByIdAsync(request.UserId);
                if ( patient == null )
                {
                    continue;
                }

                var requestDto = new RequestDto
                {
                    RequestId = request.Id, // Assign the correct request ID
                    PatientName = patient.UserName,
                    MedicinesNames = request.Medicines.Select(m => m.Name).ToList(),
                    Status = request.Status // Include the status
                };

                requestDtos.Add(requestDto);
            }

            return Ok(requestDtos);
        }
        #endregion


        #region Get Requests By Patient Username
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
        public async Task<IActionResult> UpdateRequestStatus( int id, [FromBody] StatusUpdateDto statusUpdate )
        {
            // Find the request by id
            var request = await _context.requests.FindAsync(id);

            if ( request == null )
                return NotFound();

            // Validate the status string
            if ( !Enum.TryParse(statusUpdate.Status, true, out RequestStatus requestStatus) )
                return BadRequest("Invalid status value.");

            // Update request status
            request.Status = requestStatus;
            _context.SaveChanges();

            return Ok(request);
        }
        #endregion

        #region Add Request

        [HttpPost]
        public async Task<IActionResult> PostRequest( [FromBody] AddRequestDto requestDto )
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if ( currentUser == null )
            {
                return Unauthorized();
            }

            var newRequest = new Request
            {
                UserId = currentUser.Id,
                Status = RequestStatus.Pending,
                Medicines = await _context.medicines
                    .Where(m => requestDto.MedicinesNames.Contains(m.Name))
                    .ToListAsync()
            };

            _context.requests.Add(newRequest);
            await _context.SaveChangesAsync();

            var requestDtoResponse = new RequestDto
            {
                RequestId = newRequest.Id, 
                PatientName = currentUser.UserName,
                MedicinesNames = newRequest.Medicines.Select(m => m.Name).ToList(),
                Status = newRequest.Status
            };

            return CreatedAtAction(nameof(GetRequestsByPatientUsername), new { username = currentUser.UserName }, requestDtoResponse);
        }

        #endregion



        #endregion
    }
}
