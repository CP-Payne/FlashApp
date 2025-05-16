using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashApp.Common.Helpers.Validation
{
    public static class ValidationHelper
    {
        public static bool BeAValidGuid(string guidString)
        {
            return Guid.TryParse(guidString, out _);
        }
    }
}
