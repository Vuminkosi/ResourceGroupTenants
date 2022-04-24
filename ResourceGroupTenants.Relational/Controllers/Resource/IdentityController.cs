using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using ResourceGroupTenants.Core.Models.Identity;
using ResourceGroupTenants.Core.Models.Response;
using ResourceGroupTenants.Relational.Data;
using ResourceGroupTenants.Relational.Data.Identity;
using ResourceGroupTenants.Relational.Extensions;
using ResourceGroupTenants.Relational.Services;
using ResourceGroupTenants.Relational.Services.Resource.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Controllers.Resource
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/{resourceCode}/Identity")]
    public class IdentityController : ControllerBase
    {
        #region Private protected Members
        /// <summary>
        /// The scoped application context
        /// </summary>
        protected readonly IIdentityService identityService;
        private readonly TenantDBContext dBContext;
        protected readonly ITenantService resourceGroupService;
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
        public IdentityController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITenantService resourceGroupService, IIdentityService identityService, TenantDBContext dBContext)
        {

            mUserManager = userManager;
            mSignInManager = signInManager;
            this.resourceGroupService = resourceGroupService;
            this.identityService = identityService;
            this.dBContext = dBContext;
        }
        #endregion


        #region Login / Register / Verify / Reset Password / Delete User Account

        [HttpPost("UpdateUserProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateUserProfileAsync([FromBody] LogInCredentialsDataModel model)
        { 
            try
            {
                //Keep track of the email change
                var emailChanged = false;

                var user = default(ApplicationUser);

                if (model.Email != null && model.Username != null)
                    // Get user claims
                    user = await mUserManager.FindByEmailAsync(model.Email);
                else if (model.Username != null && user == null)
                    // Get user claims
                    user = await mUserManager.FindByNameAsync(model.Username);

                //else if (user == null)
                //    // Get user claims
                //    user = await mUserManager.GetUserAsync(HttpContext.User);

                //If we have no user
                if (user == null)
                    // return error
                    return BadRequest(new ApiResponse
                    {
                        ErrorMessage = "User not found"
                    });

                // if we have a first name
                if (model.FirstName != null)
                    //Update the details
                    user.FirstName = model.FirstName;

                // if we have a last  name
                if (model.LastName != null)
                    //Update the details
                    user.LastName = model.LastName;


                // if we have a User name
                if (model.Username != null)
                    //Update the details
                    user.UserName = model.Username;


                // if we have an email
                if (model.Email != null
                    // Add it is not the same
                    && !string.Equals(model.Email.Replace(" ", ""), user.NormalizedEmail))
                {
                    //Update the details
                    user.Email = model.Email;

                    // Un-verify the email
                    user.EmailConfirmed = false;

                    // Set email as changed
                    emailChanged = true;
                }

                user.ResourceCode = model.ResourceCode;


                #region Save Profile
                // Attempts to commit updated to the data store
                var result = await mUserManager.UpdateAsync(user);

                try
                {
                    // if successful send email verification
                    if (result.Succeeded && emailChanged)
                        // Send the user and email verification token
                        await this.identityService.SendUserEmailVerificationTokenAsync(user);
                }
                catch (Exception)
                {
                }
                // if successful
                if (result.Succeeded)
                    return Ok(new ApiResponse());
                else
                    // Return failed response
                    return BadRequest(new ApiResponse { ErrorMessage = result.Errors.AggregateErrors() });
                #endregion
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }


        [HttpPost("UpdateUserPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateUserPasswordAsync([FromBody] UpdateUserPasswordApiModel model)
        {
            try
            {
                // Get user claims
                var user = await mUserManager.GetUserAsync(HttpContext.User);

                //If we have no user
                if (user == null)
                    // return error
                    return BadRequest(new ApiResponse
                    {
                        ErrorMessage = "User not found"
                    });

                #region Update Password
                // Attempts to commit updated to the data store
                var result = await mUserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                // if successful
                if (result.Succeeded)
                    return Ok(new ApiResponse());
                else
                    // Return failed response
                    return BadRequest(new ApiResponse { ErrorMessage = result.Errors.AggregateErrors() });
                #endregion
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAsync([FromQuery] string id)
        {


            try
            {
                var user = default(ApplicationUser);


                user = await mUserManager.FindByIdAsync(id);

                if (user is not null)
                {
                    //Gets list of Roles associated with current user
                    // delete business Units

                    var rolesForUser = await mUserManager.GetRolesAsync(user);


                    if (rolesForUser.Count() > 0)
                        foreach (var item in rolesForUser.ToList())
                            // item should be the name of the role
                            await mUserManager.RemoveFromRoleAsync(user, item);



                    //Delete User
                    await mUserManager.DeleteAsync(user);

                    return Ok(new ApiResponse<bool> { Response = true });
                }



                return BadRequest(new ApiResponse<bool>
                {
                    ErrorMessage = "User Not Found"
                });
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }

        }


        [HttpPost("PasswordReset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ResetPasswordAsync([FromBody] PasswordResetRequestModel requestModel)
        {

            try
            {
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(dBContext);

                var user = await mUserManager.FindByNameAsync(requestModel.UserName);

                string hashedNewPassword = mUserManager.PasswordHasher.HashPassword(user, requestModel.NewPassword);

                await store.SetPasswordHashAsync(user, hashedNewPassword);
                await store.UpdateAsync(user);

                return Ok(new ApiResponse());
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }


        [HttpPost("RegisterUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LogInCredentialsDataModel>>> RegisterAsync([FromBody] RegisterCredentialsApiModel registerCredentials)
        {


            try
            {
                // The message when we fail to login
                var invalidErrorMessage = "Please provide all required details to register for an account";
                var errorResponse = new ApiResponse<LogInCredentialsDataModel>
                {
                    //The message when we fail to login
                    ErrorMessage = invalidErrorMessage,

                };
                // Make sue we have a username
                if (registerCredentials == null)
                    // return error message to the user
                    return BadRequest(errorResponse);

                // Make sue we have a username
                if (string.IsNullOrWhiteSpace(registerCredentials.UserName))
                    // return error message to the user
                    return BadRequest(errorResponse);

                // Create the user from the given details 
                var response = await this.identityService.RegisterUserAsync(registerCredentials);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }

        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LogInCredentialsDataModel>>> LogInAsync([FromBody] LogInCredentialsApiModel logInCredentials)
        {
            try
            {
                var response = await this.identityService.LogInAsync(logInCredentials);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpPost("CheckToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> IsTokenValidAsync([FromBody] string token)
        {
            try
            {
                var response = await this.identityService.IsEmptyOrInvalid(token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpGet("VerifyEmailToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ActionResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> VerifyEmailAsync(string userId, string emailToken)
        {


            // get User
            var user = await mUserManager.FindByIdAsync(userId);

            // Note: Issues at the minute with Url decoding
            emailToken = emailToken.Replace("%2f", "/").Replace("%2F", "/");


            // if user is null
            if (user == null)
                //TODO: Nice UI
                return Content("User Not found");


            // if we have a user...
            // Verify the  email address
            var result = await mUserManager.ConfirmEmailAsync(user, emailToken);


            // If succeeded
            if (result.Succeeded)
                //TODO: Nice UI
                return Content("Email Verified :)");

            return Content("Invalid Email Verification Token :(");
        }


        #endregion
    }
}
