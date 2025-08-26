using LessonLink.BusinessLogic.Models;
using Microsoft.AspNetCore.Identity;

namespace LessonLink.BusinessLogic.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<IdentityResult> UpdateAsync(User user);
    Task<IList<string>> GetUserRolesAsync(User user);
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles);
    Task<IList<string>> GetAllRolesAsync();
    Task<bool> RoleExistsAsync(string roleName);
    Task<SignInResult> CheckPasswordSignInAsync(User user, string password);
    Task<IdentityResult> DeleteAsync(User user);
}
