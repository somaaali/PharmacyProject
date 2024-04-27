using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PatientController : ControllerBase
	{

		
			private readonly UserManager<ApplicationUser> _userManager;

		
			public PatientController(UserManager<ApplicationUser> userManager)
			{
				_userManager = userManager;
			}

			[HttpGet]
			[Route("patients/{username}")]
			public async Task<IActionResult> GetPatientByusername(string username)
			{
				var user = await _userManager.FindByNameAsync(username);

				if (user == null)
				{
					return NotFound("User not found");
				}

				// Check if the user is a patient
				if (!await _userManager.IsInRoleAsync(user, "PATIENT"))
				{
					return Unauthorized("You are not authorized to view other admin information");
				}

				return Ok(user);
			}

			[HttpGet]
			[Route("patients")]
			public async Task <IActionResult> GetAllPatients()
			{

			// Retrieve all users
			var users = await _userManager.Users.ToListAsync();

			// Filter users who are patients
			var patients = users.Where(u => _userManager.IsInRoleAsync(u, "PATIENT").Result).ToList();

			return Ok(patients);
		}

			[HttpDelete]
			[Route("patients/{username}")]
			public async Task<IActionResult> DeletePatient(string username)
			{
				var user = await _userManager.FindByNameAsync(username);

				if (user == null)
				{
					return NotFound("User not found");
				}

				// Check if the user is a patient
				if (!await _userManager.IsInRoleAsync(user, "PATIENT"))
				{
					return Unauthorized("You are not authorized to Delete other admins ");
				}

				var result = await _userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return Ok("Patient deleted successfully");
				}

				return StatusCode(500, "Failed to delete patient");
			}
		}


	}

