using Dna;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using ResourceGroupTenants.Core.Models.Resources;
using ResourceGroupTenants.Relational.Data;
using ResourceGroupTenants.Relational.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Services
{

    public class TenantService : AtomicRepository<ResourceGroupModel>,ITenantService
    { 
        private readonly HttpContext _httpContext;
        private Tenant _currentTenant;
        private string defaultConnectionString;
        public TenantService(ResourceGroupDBContext resourceContext, IHttpContextAccessor contextAccessor):base(resourceContext)
        {
            defaultConnectionString = Framework.Construction.Configuration.GetConnectionString("DefaultConnection"); 
            _httpContext = contextAccessor.HttpContext;
            if (_httpContext != null)
            {
                if (_httpContext.Request.Headers.TryGetValue("resourceCode", out var resourceCode))
                {
                    SetTenant(resourceCode);
                }
                else
                    SetDefaultConnectionStringToCurrentTenant();
            }
        } 
        public void SetTenant(string resourceCode)
        {
            _currentTenant = new Tenant();
            var resourceGroup = this.GetFirstOrDefault(a => a.ResourceCode == resourceCode);
            if (resourceGroup is not null)
            {
                _currentTenant.ResourceCode = resourceGroup.ResourceCode;
                _currentTenant.ConnectionString = defaultConnectionString;
                var newCon = _currentTenant.ConnectionString.Replace(_currentTenant.DefaultDataBase, _currentTenant.TenantDataBaseName);
                _currentTenant.ConnectionString = newCon;
            }
            else
                SetDefaultConnectionStringToCurrentTenant();

            if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
            {
                SetDefaultConnectionStringToCurrentTenant();
            }
        }
        private void SetDefaultConnectionStringToCurrentTenant()
        {
            _currentTenant = new Tenant();
            _currentTenant.ConnectionString = defaultConnectionString;
        }
        public string GetConnectionString()
        {
            return _currentTenant?.ConnectionString;
        }

        public Tenant GetTenant()
        {
            return _currentTenant;
        }

        public async Task<bool> CheckIfResourceIsAvailableAsync(ResourceGroupModel resource) => await this.HasAnyAsync(model => model.ResourceCode == resource.ResourceCode);

        public async Task<bool> CheckIfAdminNameIsTakenAsync(ResourceGroupModel resource)
        {
            return await this.HasAnyAsync(model => model.Admin == resource.Admin);
        }

        public async Task<ResourceGroupModel> GetByCodeAsync(string resourceCode,CancellationToken cancellationToken = default)
        {
            return await this.GetFirstOrDefaultAsync(model => model.ResourceCode == resourceCode,cancellationToken);
        }
    }
}
