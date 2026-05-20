using Microsoft.AspNetCore.Mvc;
using TaskManagement.Dto.AccountDto;
using TaskManagement.Repository.AccountRepository;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost("Register")] // Post Api/Account/Register
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result = await _accountRepository.Register(registerDto);
            if (result.Succeeded) // if registeration is successful
                return Ok("User registered successfully.");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")] // Post Api/Account/Login
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TokenDto result = await _accountRepository.Login(loginDto);
            if (result.token != null) // if login is successful
                return Ok(new
                {
                    Result = "User logged in successfully.",
                    Token = result.token,
                    Expires = result.Expires
                });

            ModelState.AddModelError("Error", "UserName Or Password Is InCorrect");

            return BadRequest(ModelState);
        }
    }
}
