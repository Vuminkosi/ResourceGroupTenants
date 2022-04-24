using ResourceGroupTenants.Core.Models.Masters;
using ResourceGroupTenants.Relational.Data;
using ResourceGroupTenants.Relational.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Services.Masters
{
    public interface IProductsService : IRepository<ProductModel>
    {
    }
    public class ProductsService : AtomicRepository<ProductModel>, IProductsService
    {
        public ProductsService(TenantDBContext dBContext) : base(dBContext)
        {
        }
    }
}
