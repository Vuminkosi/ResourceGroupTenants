using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Identity
{
    public class UpdateUserPasswordApiModel
    { 
        /// <summary>
        /// the user first name
        /// </summary>
        public string CurrentPassword { get; set; }
        /// <summary>
        /// The user Last name
        /// </summary>
        public string NewPassword { get; set; }
    }
}
