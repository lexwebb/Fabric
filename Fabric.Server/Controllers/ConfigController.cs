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
        private readonly IFabricStore _fabricStore;

        public ConfigController(IFabricStore fabricStore) {
            this._fabricStore = fabricStore;
        }

        [HttpGet("{*pathInfo}")]
        public async Task<JsonResult> GetAsync(string pathInfo) {
            var querySwitches = Request.Query;

            if (GetQueryValue("children") == "true") {
                return Json(await _fabricStore.GetDataPage(pathInfo));
            }

            return Json(await _fabricStore.GetDataPage(pathInfo));
        }

        private string GetQueryValue(string key) {
            var querySwitches = Request.Query;

            if (querySwitches.ContainsKey(key)) {
                return querySwitches[key];
            }

            return null;
        }
    }
}
