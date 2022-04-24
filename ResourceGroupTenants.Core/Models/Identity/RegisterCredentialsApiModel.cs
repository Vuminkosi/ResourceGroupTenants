using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Identity
{
    public class RegisterCredentialsApiModel:BaseModel
    { 
         
        /// <summary>
        ///  The user username
        /// </summary>
        public string UserName { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }

        public string ResourceCode { get; set; }
        public string Code => FirstName?.Substring(0, 1) + LastName?.Substring(0, 1);

        

    }
}
