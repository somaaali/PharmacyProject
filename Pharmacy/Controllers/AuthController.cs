
namespace Pharmacy.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;
		public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
		}

		//Route For Seeding my roles to DB
		[HttpPost]
		[Route("seed-roles")]
		public async Task<IActionResult> SeedRoles()
		{
			bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
			bool isPatientRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.PATIENT);

			if (isAdminRoleExists && isPatientRoleExists)
				return Ok("Roles seeding is Already Done");


			await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
			await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.PATIENT));

			return Ok("Role Seeding Done Successfully");

		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);

			if (isExistsUser != null)
				return BadRequest("username Already Exist");



			ApplicationUser newUser = new ApplicationUser()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,	
				Email = registerDto.Email,
				UserName = registerDto.UserName,
				Address = registerDto.Address,
				Phone = registerDto.Phone,
				SecurityStamp = Guid.NewGuid().ToString(),
			};

			var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

			if (!createUserResult.Succeeded)
			{
				var errorString = "User Creation Failed Beacause: ";
				foreach (var error in createUserResult.Errors)
				{
					errorString += " # " + error.Description;
				}
				return BadRequest(errorString);

			}

			// Add a Default USER Role to all users
			await _userManager.AddToRoleAsync(newUser, StaticUserRoles.PATIENT);

			return Ok("User Created Successfully");

		}

		[HttpPost]
		[Route("Make-Admin")]
		public async Task<IActionResult> MakeAdmin(UpdatePermissionDto updatePermissionDto)
		{
			var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

			if (user is null)
				return BadRequest("Invalid UserName!!!!");

			await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);
			await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.PATIENT);

			return Ok("User is Admin now");
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> LoginAsync(LoginDto loginDto)
		{
			var user = await _userManager.FindByNameAsync(loginDto.UserName);

			if (user is null)
				return Unauthorized("Invalid Credentials");

			var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

			if (!isPasswordCorrect)
				return Unauthorized("Invalid Credentials");

			var userRoles = await _userManager.GetRolesAsync(user);
			var authClaims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, user.UserName),
		new Claim(ClaimTypes.NameIdentifier, user.Id),
		new Claim("JWTID", Guid.NewGuid().ToString()),
		new Claim("FirstName", user.FirstName),
		new Claim("LastName", user.LastName),
		new Claim("Addres", user.Address),
		new Claim("Phone", user.Phone),
	};

			// Add role ID to claims
			foreach (var userRole in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				var role = await _roleManager.FindByNameAsync(userRole);
				if (role != null)
				{
					authClaims.Add(new Claim("RoleId", role.Id)); // Add RoleId claim
				}
			}

			var token = GenerateNewJsonWebToken(authClaims);

			return Ok(new { Token = token, Roles = userRoles }); // Return token and roles
		}


		private string GenerateNewJsonWebToken(List<Claim> claims)
		{
			var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var tokenObject = new JwtSecurityToken(
					issuer: _configuration["JWT:ValidIssuer"],
					audience: _configuration["JWT:ValidAudience"],
					expires: DateTime.Now.AddHours(1),
					claims: claims,
					signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
				);

			string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

			return token;

		}
	}
}
