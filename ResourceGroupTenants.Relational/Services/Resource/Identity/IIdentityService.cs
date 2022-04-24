using ResourceGroupTenants.Core.Models.Email;
using ResourceGroupTenants.Core.Models.Identity;
using ResourceGroupTenants.Core.Models.Response;
using ResourceGroupTenants.Relational.Data.Identity;

namespace ResourceGroupTenants.Relational.Services.Resource.Identity
{
    public interface IIdentityService
    {
        Task<List<LogInCredentialsDataModel>> GetAllUsersAsync(string resourceKey);
        Task<List<LogInCredentialsDataModel>> GetUsersAsync(string resourceKey);
        Task<ApiResponse<bool>> IsEmptyOrInvalid(string token);
        Task<bool> IsUserInRoleAsync(string userId, string role);
        Task<ApiResponse<LogInCredentialsDataModel>> LogInAsync(LogInCredentialsApiModel logInCredentials);
        Task<ApiResponse<LogInCredentialsDataModel>> RegisterUserAsync(RegisterCredentialsApiModel registerCredentials);
        Task<SendEmailResponse> SendUserEmailVerificationTokenAsync(ApplicationUser user);
    }
}