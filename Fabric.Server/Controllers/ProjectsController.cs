using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fabric.Core.Data;
using Fabric.Core.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Server.Controllers
{
    [ExceptionHandlerFilter]
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        // GET api/projects
        [HttpGet]
        public IEnumerable<FabricProject> Get() {
            return FabricDatabase.Projects.FindAll();
        }

        // GET api/projects/test
        [HttpGet("{name}")]
        public FabricProject Get(string name)
        {
            return FabricDatabase.Projects.Find(p => p.Name == name).FirstOrDefault()
                ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {name}");
        }

        // POST api/projects
        [HttpPost]
        public void Post([FromBody]FabricProject value) {
            var projects = FabricDatabase.Projects;

            projects.Insert(value);

            projects.EnsureIndex(x => x.Name);
        }

        // PUT api/projects/test
        [HttpPut("{name}")]
        public void Put(string name, [FromBody]FabricProject value) {
            var projects = FabricDatabase.Projects;
            var project = projects.Find(p => p.Name == name).FirstOrDefault()
                ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {name}");

            project.Name = value.Name;
            project.Environments = value.Environments;

            projects.EnsureIndex(x => x.Name);
        }

        // DELETE api/projects/test
        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            var projects = FabricDatabase.Projects;
            var project = projects.Find(p => p.Name == name).FirstOrDefault()
                          ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {name}");
            projects.Delete(p => p.Name == name);

            projects.EnsureIndex(x => x.Name);
        }
    }
}
