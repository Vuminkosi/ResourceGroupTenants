using Dna;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using ResourceGroupTenants.Core.Models.Email;
using ResourceGroupTenants.Core.Models.Identity;
using ResourceGroupTenants.Core.Models.Resources;
using ResourceGroupTenants.Core.Models.Response;
using ResourceGroupTenants.Core.Routes;
using ResourceGroupTenants.Relational.Data;
using ResourceGroupTenants.Relational.Data.Identity;
using ResourceGroupTenants.Relational.Emails.FluentServices;
using ResourceGroupTenants.Relational.Extensions;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ResourceGroupTenants.Relational.Services.Resource.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IFluentMailService mailService;

        #region Private protected Members
        /// <summary>
        /// The scoped application context
        /// </summary>
        protected TenantDBContext mContext;
        /// <summary>
        /// The manager handles the users creation, deletion,Roles ands searching etc...
        /// </summary>
        protected UserManager<ApplicationUser> mUserManager;
        /// <summary>
        /// The manager for handling sign in and sign out
        /// </summary>
        protected SignInManager<ApplicationUser> mSignInManager;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The injected <see cref="ApplicationDBContext"/></param>
        public IdentityService(IFluentMailService mailService, TenantDBContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.mailService = mailService;
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }
        #endregion

        public async Task<List<LogInCredentialsDataModel>> GetAllUsersAsync(string resourceKey)
        {

            // Get Users
            var users = await mContext.Users
                  .AsNoTracking().
                  Where(model => model.ResourceCode == resourceKey).ToListAsync().ConfigureAwait(false);

            // Create a collection instance
            var output = new List<LogInCredentialsDataModel>();

            // Add all users
            foreach (var model in users)
            {

                output.Add(new LogInCredentialsDataModel
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.UserName,
                    ResourceCode = model.ResourceCode,
                });
            }

            return output;

        }

        public async Task<List<LogInCredentialsDataModel>> GetUsersAsync(string resourceKey)
        {

            // Get Users
            var users = await mContext.Users
                  .AsNoTracking().
                  Where(model => model.ResourceCode == resourceKey).ToListAsync();

            // Create a collection instance
            var output = new List<LogInCredentialsDataModel>();

            // Add all users
            foreach (var model in users)
                output.Add(new LogInCredentialsDataModel
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.UserName,
                    ResourceCode = model.ResourceCode,
                });

            return output;

        }
        /// <summary>
        /// Determines whether [is empty or invalid] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <c>true</c> if [is empty or invalid] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<ApiResponse<bool>> IsEmptyOrInvalid(string token)
        {

            await Task.Delay(1);

            if (string.IsNullOrEmpty(token))
            {

                // Return invalid user token
                return new ApiResponse<bool>
                {
                    // Pass back the user details validity
                    Response = true,
                };
            }

            var jwtToken = new JwtSecurityToken(token);
            // Return invalid user token
            return new ApiResponse<bool>
            {
                // Pass back the user details validity
                Response = (jwtToken == null) || (jwtToken.ValidFrom > DateTime.UtcNow) || (jwtToken.ValidTo < DateTime.UtcNow),
            };
        }

        public async Task<ApiResponse<LogInCredentialsDataModel>> LogInAsync(LogInCredentialsApiModel logInCredentials)
        {
            //TODO: Localize all strings
            // The message when we fail to login
            var invalidErrorMessage = "Invalid username or password";
            var errorResponse = new ApiResponse<LogInCredentialsDataModel>
            {
                //The message when we fail to login
                ErrorMessage = invalidErrorMessage,

            };

            // Make sue we have a username
            if (logInCredentials?.UsernameOrEmail == null || string.IsNullOrWhiteSpace(logInCredentials.UsernameOrEmail))
                // return error message to the user
                return errorResponse;

            // Validate if the user credentials are correct

            // Is it an email
            var isEmail = logInCredentials.UsernameOrEmail.Contains("@");

            // Get the user details
            var user = isEmail ?
                // find by email
                await mUserManager.FindByEmailAsync(logInCredentials.UsernameOrEmail) :
                // Find by user name
                await mUserManager.FindByNameAsync(logInCredentials.UsernameOrEmail);

            // if we failed to find a user
            if (user == null)
                // return error message to the user
                return errorResponse;

            // If we got here we have a user
            // Let's validate the password

            // Get if password is valid
            var IsValidPassword = await mUserManager.CheckPasswordAsync(user, logInCredentials.Password);

            // if password is wrong
            if (!IsValidPassword)
                // return error message to the user
                return errorResponse;


            // Return token to user
            return new ApiResponse<LogInCredentialsDataModel>
            {
                // Pass back the user details and a token
                Response = new LogInCredentialsDataModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName,
                    ResourceCode = user.ResourceCode,
                    Token = user.GenerateJwtToken()
                }
            };
        }

        /// <summary>
        /// Determines whether [is user in role asynchronous] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        ///   <c>true</c> if [is user in role asynchronous] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {

            var user = await mUserManager.FindByIdAsync(userId);

            return await mUserManager.IsInRoleAsync(user, role);
        }

        public async Task<ApiResponse<LogInCredentialsDataModel>> RegisterUserAsync(RegisterCredentialsApiModel registerCredentials)
        {


            // Create the user from the given details
            var user = new ApplicationUser
            {
                FirstName = registerCredentials.FirstName,
                LastName = registerCredentials.LastName,
                Email = registerCredentials.Email,
                UserName = registerCredentials.UserName,
                ResourceCode = registerCredentials.ResourceCode,
            };


            try
            {

                // try and create a user
                var result = await mUserManager.CreateAsync(user, registerCredentials.Password);


                // if the registration was successful
                if (result.Succeeded)
                {
                    // Get user account
                    var userIdentity = await mUserManager.FindByNameAsync(registerCredentials.UserName);

                    // Send the user and email verification token
                    try
                    {   //Send user verification token
                        await SendUserEmailVerificationTokenAsync(userIdentity);
                    }
                    catch (Exception) { }

                    // Return token to user
                    return new ApiResponse<LogInCredentialsDataModel>
                    {
                        // Pass back the user details and a token
                        Response = new LogInCredentialsDataModel
                        {

                            FirstName = userIdentity.FirstName,
                            LastName = userIdentity.LastName,
                            Email = userIdentity.Email,
                            Username = userIdentity.UserName,
                            ResourceCode = userIdentity.ResourceCode,
                            Token = userIdentity.GenerateJwtToken(),

                        }
                    };
                }
                else
                {
                    // Return an aggregated errors into a single error string;
                    return new ApiResponse<LogInCredentialsDataModel> { ErrorMessage = result.Errors.AggregateErrors() };
                }

            }
            catch (Exception ex)
            {
                // Log the error
                FrameworkDI.Logger.LogCriticalSource(ex.Message);

                // Notify the user of the error
                return new ApiResponse<LogInCredentialsDataModel>
                {
                    ErrorMessage = "Unknown Error on the server side. Server failed to create the user account"
                };
            }

            //return new ApiResponse<RegisterResultApiModel> {
            //    ErrorMessage = "Unknown Error on the server side",
            //    Response = new RegisterResultApiModel()
            //};
        }

        /// <summary>
        /// Send the user and email verification token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<SendEmailResponse> SendUserEmailVerificationTokenAsync(ApplicationUser user)
        {
            try
            {
                // Generate an email verification code
                var emailVerificationCode = await mUserManager.GenerateEmailConfirmationTokenAsync(user);



                var confirmationUrl = $"{ApiRoutes.HttpUrl}api/verify/email/{HttpUtility.UrlEncode(user.Id)}/{HttpUtility.UrlEncode(emailVerificationCode)}";

                StringBuilder message = new StringBuilder();
                message.AppendLine($"Hi  {user.FirstName},");
                message.AppendLine($"");
                message.AppendLine($"Thanks for creating an account with us. To continue please verify your email with us.");
                message.AppendLine($"Follow the link below to confirm your email");
                message.AppendLine($"<a href=\"http://{confirmationUrl}\">Confirm Email!</a>");
                message.AppendLine($"");
                message.AppendLine($"Regards");
                message.AppendLine($"RootInsure Team.");

                // Email the user verification code
                await mailService.SendTemplatedEmailAsync(new RazorFluentEmailMessage
                {
                    TemplateMessage = message,
                    Subject = "Verify Email",
                    ToEmail = user.Email,
                    ToName = user.FirstName,
                    IsHtml = true,
                });
                return new SendEmailResponse();
            }
            catch (Exception ex)
            {
                return new SendEmailResponse { Errors = new List<string> { ex.Message } };
            }
        }


    }
}
