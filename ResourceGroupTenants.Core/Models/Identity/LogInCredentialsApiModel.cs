using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Identity
{
    public class LogInCredentialsApiModel
    { 
        public string UsernameOrEmail { get; set; }
       
        public string Password { get; set; }

    }
}
