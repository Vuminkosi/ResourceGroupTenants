using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ResourceGroupTenants.Relational
{
    public class AuthorizeTokenAttribute:AuthorizeAttribute {
        public AuthorizeTokenAttribute() {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
