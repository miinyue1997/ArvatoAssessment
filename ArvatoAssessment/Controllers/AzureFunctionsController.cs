using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvatoAssessment.Authentication;
using ArvatoAssessment.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArvatoAssessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authentication]
    [Authorization]
    public class AzureFunctionsController : ControllerBase
    {
        // GET
        [HttpGet]
        public void AzureFunction()
        {
            try
            {
                //calls to AzureFunctions
            }
            catch(Exception ex)
            {
                //log exception
                throw new Exception(ex.Message);
            }
        }
    }
}
