using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ResourceGroupTenants.Core.Models;
using ResourceGroupTenants.Core.Models.Response;
using ResourceGroupTenants.Relational.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Controllers
{
    [ApiController]
    [AuthorizeToken]
    [Produces("application/json")]
    public class AtomicController<TData, TEntity> : ControllerBase where TData : IRepository<TEntity> where TEntity : BaseModel, new()
    {
        protected readonly TData _service;

        public AtomicController(TData service)
        {
            this._service = service;
        }

        #region Services

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse<TEntity>>> GetByIdAsync([FromRoute] string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var severResponse = await _service.GetByIdAsync(id);
                    return Ok(new ApiResponse<TEntity>(severResponse));
                }
                return BadRequest(new ApiResponse("Invalid Id"));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse<IEnumerable<TEntity>>>> GetAsync([FromRoute] string resourceCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resourceCode))
                {
                    return BadRequest(new ApiResponse("Invalid Resource Code"));
                }
                var severResponse = await _service.GetAllAsync(x => x.ResourceCode == resourceCode && x.IsMarkedForDelete == false);

                return Ok(new ApiResponse<IEnumerable<TEntity>>(severResponse));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse<TEntity>>> AddAsync([FromBody] TEntity model)
        {
            try
            {
                var response = await _service.AddOrUpdateAsync(model, false);
                return Ok(new ApiResponse<TEntity>(response));

            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse<TEntity>>> UpdateAsync([FromBody] TEntity model)
        {
            try
            {
                var response = await _service.AddOrUpdateAsync(model, false);
                return Ok(new ApiResponse<TEntity>(response));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<ApiResponse>> DeleteAsync([FromRoute] string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    await _service.DeleteAsync(id);

                    return Ok(new ApiResponse());
                }
                else
                    return BadRequest(new ApiResponse("Invalid Id"));

            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                    return BadRequest(new ApiResponse(ex.InnerException.Message));
                else
                    return BadRequest(new ApiResponse(ex.Message));
            }
        }

        #endregion
    }
}
