using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Extensions
{
    public static class IdentityErrorsExtentions
    {
        /// <summary>
        /// Aggregates a list of descriptions in <see cref="IdentityError"/> in a single string separated by a new line
        /// </summary>
        /// <param name="errors"></param>
        /// <returns>returns single string separated by a new line</returns>
        public static string AggregateErrors(this IEnumerable<IdentityError> errors)
        {
            // Return error
            return errors?.Select(f => f.Description).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
        }

        ///// <summary>
        ///// Aggregates a list of descriptions in <see cref="ValidationFailure"/> in a single string separated by a new line
        ///// </summary>
        ///// <param name="errors"></param>
        ///// <returns>returns single string separated by a new line</returns>
        //public static string AggregateErrors(this IEnumerable<ValidationFailure> errors)
        //{
        //    // Return error
        //    return errors?.Select(f => f.ErrorMessage).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
        //}
    }
}
