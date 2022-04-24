using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Resources
{
    public class Tenant
    {

        public string? ResourceCode { get; set; }
        public string DefaultDataBase => "SaaSTenant";
        public string TenantDataBaseName => "SaaSTenant" + this.ResourceCode;

        public string? ConnectionString { get; set; }
    }
}
