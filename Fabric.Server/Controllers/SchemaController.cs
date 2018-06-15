using System;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Core;
using Fabric.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Unity;

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
                    return _fabricStore.Database.Resolver.Resolve<ISchemaManager>().Schemas.FirstOrDefault(s => s.SchemaName == schemaName);
                }
                return _fabricStore.Database.Resolver.Resolve<ISchemaManager>().Schemas;
            })));
        }

        [HttpPut("{schemaName}")]
        public async Task<JsonResult> UpdateAsync(string schemaName, [FromBody] JObject schemaRaw) {
            return Json(await Task.Run(new Func<object>(() => {
                _fabricStore.Database.Resolver.Resolve<ISchemaManager>().Update(schemaName, schemaRaw.ToString());
                return "success";
            })));
        }
    }
}
