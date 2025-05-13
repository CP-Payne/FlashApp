using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FlashApp.Controllers
{
    [ApiController]
    public class ErrorsController : ApiController
    {
        [Route("/error")]
        public IActionResult Error()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected internal server error occurred."
            );
        }
    }
}
