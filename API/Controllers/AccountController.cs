using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppIdentityDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, RoleManager<IdentityRole> roleManager, AppIdentityDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // Gets the current user email.
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            // Gets the user by email.
            var user = await _userManager.FindByEmailAsync(email);

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("type")]
        public async Task<ActionResult<string>> GetUserTypeAsync()
        {
            // Gets the current user email.
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            // Gets the user by email.
            var user = await _userManager.FindByEmailAsync(email);

            // A work around for:
            // await _userManager.GetRolesAsync(user);
            // Which unfortunately not working for me.
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            var userRoleName = _context.Roles.FirstOrDefault(urn => urn.Id == userRole.RoleId).Name;

            return userRoleName;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Gets the user by email.
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // If there is not user return unauthorized.
            if (user == null) return Unauthorized();

            // Checks if the password is matches the user.
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Check if the email is already exists.
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value) return BadRequest();

            // Validate that the user type is ONLY patient or doctor.
            if (registerDto.UserType != "patient" && registerDto.UserType != "doctor") return BadRequest();

            // Check if the user type (role) exists. If not, create the role in the sqlite.
            bool isRoleExists = await _roleManager.RoleExistsAsync(registerDto.UserType);
            if (!isRoleExists)
            {
                var role = new IdentityRole();
                role.Name = registerDto.UserType;
                await _roleManager.CreateAsync(role);
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                UserType = registerDto.UserType,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            // Creates the user in sql.
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest();

            // A work around for:
            // await _userManager.AddToRoleAsync(user, registerDto.UserType);
            // Which unfortunately not working for me.
            var roleId = _roleManager.Roles.FirstOrDefault(r => r.Name == registerDto.UserType);
            IdentityUserRole<string> userRole = new IdentityUserRole<string>()
            {
                UserId = user.Id,
                RoleId = roleId.Id
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }
    }
}