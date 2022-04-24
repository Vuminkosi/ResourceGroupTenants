
using ResourceGroupTenants.Core.Models.Resources;
using ResourceGroupTenants.Relational.Repository;

namespace ResourceGroupTenants.Relational.Services
{
    public interface ITenantService:IRepository<ResourceGroupModel>
    {
        Task<bool> CheckIfAdminNameIsTakenAsync(ResourceGroupModel resource);
        Task<bool> CheckIfResourceIsAvailableAsync(ResourceGroupModel resource);
        Task<ResourceGroupModel> GetByCodeAsync(string resourceCode, CancellationToken cancellationToken = default);
        string GetConnectionString();
        Tenant GetTenant();
        void SetTenant(string resourceCode);
    }
}
