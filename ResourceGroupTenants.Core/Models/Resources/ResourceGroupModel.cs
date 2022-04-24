using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Resources
{
    public class ResourceGroupModel: BaseModel
    {
        public string? CompanyName { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyAddress { get; set; }
        public string? Admin { get; set; }
        public string? Password { get; set; }


    }
}
