using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;   
        }
       [Route("/Error/{statusCode}")] 
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            //
            //gettting the statusCodeDetails and looging them using the Ilogger Interface
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested couldnt be found";
                    _logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath} " +
                        $"and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }
        [Route("/Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            //gettting the exception details and looging them using the Ilogger Interface
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError($"The Path {exceptionDetails.Path} threw an exception{exceptionDetails.Error}");

            return View();
        }

    }
}
