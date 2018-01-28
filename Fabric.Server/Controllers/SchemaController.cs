using System;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Core;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Server.Controllers {
    [ExceptionHandlerFilter]
    [Route("api/[controller]")]
    public class SchemaController : Controller {
        private readonly IFabricStore _fabricStore;

        public SchemaController(IFabricStore fabricStore) {
            _fabricStore = fabricStore;
        }

        [HttpGet("{schemaName?}")]
        public async Task<JsonResult> GetAsync(string schemaName) {
            return Json(await Task.Run(new Func<object>(() => {
                if (schemaName != null) {
                    return _fabricStore.Database.SchemaManager.Schemas.FirstOrDefault(s => s.SchemaName == schemaName);
                }
                return _fabricStore.Database.SchemaManager.Schemas;
            })));
        }
    }
}
