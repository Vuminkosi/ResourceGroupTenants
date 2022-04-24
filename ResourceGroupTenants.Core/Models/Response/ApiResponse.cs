using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Core.Models.Response
{
    public class ApiResponse
    {
        public ApiResponse()
        {

        }
        public ApiResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ApiResponse(object response)
        {
            Response = response;
        }

        /// <summary>
        /// Indicates if the api call was successful
        /// </summary>
        public bool Successful => ErrorMessage == null;
        /// <summary>
        /// The Error message for the failed API call
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// The API Response object
        /// </summary>
        public object Response { get; set; }
    }
    /// <summary>
    /// The response for all web api calls made with response of <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse()
        {

        }

        public ApiResponse(T response) : base(response)
        {
            Response = response;
        }

        /// <summary>
        /// The API response object as T
        /// </summary>
        public new T Response { get => (T)base.Response; set => base.Response = value; }
    }


}
