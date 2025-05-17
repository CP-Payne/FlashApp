using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;

namespace FlashApp.Common.Helpers.Validation
{
    public static class ValidationHelper
    {
        public static bool BeAValidGuid(string guidString)
        {
            return Guid.TryParse(guidString, out _);
        }

        public static List<Error> ToError(
            this FluentValidation.Results.ValidationResult validationResult
        )
        {
            return validationResult
                .Errors.Select(failure =>
                    Error.Validation(code: failure.PropertyName, description: failure.ErrorMessage)
                )
                .ToList();
        }
    }
}
