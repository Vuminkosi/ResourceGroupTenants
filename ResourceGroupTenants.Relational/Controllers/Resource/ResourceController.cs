using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ResourceGroupTenants.Core.Models.Resources;
using ResourceGroupTenants.Core.Models.Response;
using ResourceGroupTenants.Relational.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Controllers.Resource
{
    [ApiController]
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/ResourceGroups")]
    public class ResourceController : AtomicController<ITenantService, ResourceGroupModel>
    {
        public ResourceController(ITenantService service) : base(service)
        {
        }
        [HttpGet("GetByCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse<ResourceGroupModel>>> GetByCode([FromQuery] string resourceCode)
        {
            try
            { 
                var response = await _service.GetByCodeAsync(resourceCode);
                return Ok(new ApiResponse<ResourceGroupModel>(response));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse<ResourceGroupModel>>> CreateAsync([FromBody] ResourceGroupModel model)
        {
            try
            {
                var admminName = await this._service.CheckIfAdminNameIsTakenAsync(model); 
                // If the resource group is already registered
                if (admminName is true)
                    // Return error
                    return new ApiResponse<ResourceGroupModel>
                    {
                        //The message when we fail to login
                        ErrorMessage = "Admin name already taken"
                    };
                 
                var response = await _service.AddOrUpdateAsync(model, false);
                return Ok(new ApiResponse<ResourceGroupModel>(response));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }
    }
}
