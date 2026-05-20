using Microsoft.AspNetCore.Identity;
using TaskManagement.Dto.AccountDto;
using TaskManagement.Models;

namespace TaskManagement.Repository.AccountRepository
{
    public interface IAccountRepository
    {
        Task<ApplicationUser?> GetUserById(string Id);
        Task<IdentityResult> Register(RegisterDto registerDto);
        Task<TokenDto> Login(LoginDto loginDto);
    }
}
