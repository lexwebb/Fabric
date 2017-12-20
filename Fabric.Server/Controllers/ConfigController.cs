using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Server.Controllers
{
    [ExceptionHandlerFilter]
    [Route("api/projects/{projectName}/environments/{environmentName}/[controller]")]
    public class ConfigController : Controller
    {
        //[HttpGet]
        //public IEnumerable<FabricProject> Get() {
        //    return FabricDatabase.Projects.FindAll();
        //}
    }
}
