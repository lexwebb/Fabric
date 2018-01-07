using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Core;
using Fabric.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Server.Controllers
{
    [ExceptionHandlerFilter]
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        [HttpGet]
        public IEnumerable<FabricProject> Get() {
            return FabricDatabase.Projects.FindAll();
        }
    }
}
