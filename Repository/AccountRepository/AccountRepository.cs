using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Dto.AccountDto;
using TaskManagement.Models;

namespace TaskManagement.Repository.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public AccountRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ApplicationUser?> GetUserById(string Id)
        {
            return await _userManager.FindByIdAsync(Id);
        }
        public async Task<IdentityResult> Register(RegisterDto registerDto)
        {
            if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Username already exists" });
            }

            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists" });
            }

            ApplicationUser user = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            return result;
        }

        public async Task<TokenDto> Login(LoginDto loginDto)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);

            // check if user exists
            if (user == null)
            {
                return new TokenDto();
            }

            // check if password is true
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return new TokenDto();
            }


            // create token
            // get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // create claims
            List<Claim> userClaims = new List<Claim>();
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
            userClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // create security key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            // create signing credentials
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // generate token
            JwtSecurityToken myToken = new JwtSecurityToken(
                audience: _configuration["JWT:Audience"],
                issuer: _configuration["JWT:Issuer"],
                expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["JWT:DurationInDays"])),
                claims: userClaims,
                signingCredentials: signingCredentials
                );

            var token = new TokenDto()
            {
                token = new JwtSecurityTokenHandler().WriteToken(myToken),
                Expires = myToken.ValidTo
            };

            return token;
        }
    }
}
