using Dna;

using Microsoft.IdentityModel.Tokens;

using ResourceGroupTenants.Relational.Data.Identity;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational
{
    public static class JwtTokenExtensions
    {

        /// <summary>
        /// Generate a JWT Bearer token containing the user name details
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(this ApplicationUser user)
        {
            var claims = new[] {
                // Unique ID for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                // The email using the identity name so  it fills out the httpContext.User value
                new Claim(JwtRegisteredClaimNames.Jti, user.Email),
                  // The username using the identity name so  it fills out the httpContext.User value
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),

                // Add user Id so that UserManager.GetUserAsync can find the user based on Id
                new Claim(ClaimTypes.NameIdentifier,user.Id), 
            };

            // Create the credentials used to sign in
            var credintials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(FrameworkDI.Configuration["Jwt:SecretKey"])),
                SecurityAlgorithms.HmacSha256
                );


            // Generate the Jwt Token
            var token = new JwtSecurityToken(
                issuer: FrameworkDI.Configuration["Jwt:JwtIssuer"],
                audience: FrameworkDI.Configuration["Jwt:JwtAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credintials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
