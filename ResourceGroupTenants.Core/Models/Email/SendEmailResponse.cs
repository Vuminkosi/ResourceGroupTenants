using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Email
{
    public class SendEmailResponse
    { 
        public bool Successful => !(Errors?.Count > 0);
 
        public List<string> Errors { get; set; }

        public void AddError(string error)
        {
            if (Errors != null)
            {
                Errors.Add(error);
            }
            else
            {
                Errors = new List<string> { error };
            }
        }
    }
}
