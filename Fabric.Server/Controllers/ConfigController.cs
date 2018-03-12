using System.Threading.Tasks;
using Fabric.Core;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Server.Controllers {
    [ExceptionHandlerFilter]
    [Route("api/[controller]")]
    public class ConfigController : Controller {
        private readonly IFabricStore _fabricStore;

        public ConfigController(IFabricStore fabricStore) {
            _fabricStore = fabricStore;
        }

        [HttpGet("{*pathInfo}")]
        public async Task<JsonResult> GetAsync(string pathInfo) {
            return _fabricStore.IsPathCollection(pathInfo) ?
                Json(await _fabricStore.GetDataPageCollection(pathInfo), _fabricStore.Database.SerializerSettings) :
                Json(await _fabricStore.GetDataPage(pathInfo), _fabricStore.Database.SerializerSettings);
        }

        [HttpPost("{*pathInfo}")]
        public async Task<JsonResult> PostAsync(string pathInfo, [FromBody] string data) {
            var page = await _fabricStore.GetDataPage(pathInfo);

            page.PageData = data;
            page.SaveChanges();

            return Json("Ok");
        }

        private string GetQueryValue(string key) {
            var querySwitches = Request.Query;

            if (querySwitches.ContainsKey(key)) return querySwitches[key];

            return null;
        }
    }
}
