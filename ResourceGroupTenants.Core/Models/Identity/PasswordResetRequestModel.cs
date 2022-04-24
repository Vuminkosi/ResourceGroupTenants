using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Identity
{
    public class PasswordResetRequestModel : BaseModel
    { 
        public DateTime Date { get; set; }
         
        public string UserName { get; set; } 
        public string Email { get; set; } 
        public string NewPassword { get; set; }

    }
}
