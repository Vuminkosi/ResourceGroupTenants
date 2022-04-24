
using Microsoft.AspNetCore.Mvc;

using ResourceGroupTenants.Core.Models.Masters;
using ResourceGroupTenants.Relational.Services.Masters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Controllers.Masters
{
    [ApiController]
    [AuthorizeToken]
    [Produces("application/json")]
    [Route("api/{resourceCode}/Products")]
    public class ProductsController : AtomicController<IProductsService, ProductModel>
    {
        public ProductsController(IProductsService service) : base(service)
        {
        }
    }
}
