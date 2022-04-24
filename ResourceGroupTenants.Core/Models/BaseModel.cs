using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models
{
    public class BaseModel
    {
        public string? Id { get; set; }
        public string? ResourceCode { get; set; }

        public double UpdateDate { get; set; }= DateTime.Now.ToOADate();
        public bool IsMarkedForDelete { get; set; } = false;
    }
}
