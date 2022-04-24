using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Masters
{
    public class ProductModel:BaseModel
    {
        public string? Name { get; set; } 
        public string? Description { get; set; } 
        public string? Code { get; set; }
        public string? BarCode { get; set; } 
        public double UnitPrice { get; set; }
        public double SellingPrice { get; set; }
        public string? UOM { get; set; }
        public string? Weight { get; set; }

    }
}
