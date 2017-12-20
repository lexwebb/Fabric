using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fabric.Core.Data;
using Fabric.Core.Data.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace Fabric.Server.Controllers {
    [ExceptionHandlerFilter]
    [Route("api/projects/{projectName}/[controller]")]
    public class EnvironmentsController : Controller {

        [HttpGet]
        public IEnumerable<FabricEnvironment> Get(string projectName) {
            var mapper = BsonMapper.Global;
            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");

            // When query Order, includes references
            var project = FabricDatabase.Projects
                .Include(x => x.Environments)
                .Find(x => x.Name == projectName).FirstOrDefault()
                ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {projectName}");

            return project.Environments;
        }

        [HttpGet("{name}")]
        public FabricEnvironment Get(string projectName, string name) {
            var mapper = BsonMapper.Global;
            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");

            // When query Order, includes references
            var project = FabricDatabase.Projects
                .Include(x => x.Environments)
                .Find(x => x.Name == projectName).FirstOrDefault()
                ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {projectName}");

            return project.Environments.FirstOrDefault(e => e.Name == name)
                   ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find environment with name: {name}");
        }

        [HttpPost]
        public void Post(string projectName, [FromBody]FabricEnvironment value) {
            var mapper = BsonMapper.Global;
            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");

            // When query Order, includes references
            var project = FabricDatabase.Projects
                              .Include(x => x.Environments)
                              .Find(x => x.Name == projectName).FirstOrDefault()
                          ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {projectName}");

            project.Environments.Add(value);
            FabricDatabase.Environments.Insert(value);
            FabricDatabase.Projects.Update(project);
        }

        [HttpPut("{name}")]
        public void Put(string projectName, string name, [FromBody]FabricEnvironment value) {
            var mapper = BsonMapper.Global;
            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");

            // When query Order, includes references
            var project = FabricDatabase.Projects
                              .Include(x => x.Environments)
                              .Find(x => x.Name == projectName).FirstOrDefault()
                          ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {projectName}");

            var environment = project.Environments.FirstOrDefault(e => e.Name == name)
                ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find environment with name: {name}");

            environment.Name = value.Name;
            environment.Production = value.Production;

            FabricDatabase.Environments.Update(environment);
        }

        [HttpDelete("{name}")]
        public void Delete(string projectName, string name) {
            var mapper = BsonMapper.Global;
            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");

            // When query Order, includes references
            var project = FabricDatabase.Projects
                              .Include(x => x.Environments)
                              .Find(x => x.Name == projectName).FirstOrDefault()
                          ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {projectName}");

            var environment = project.Environments.FirstOrDefault(e => e.Name == name)
                              ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find environment with name: {name}");

            var result = FabricDatabase.Environments.Delete(e => e.Id == environment.Id);

            if (result < 1) {
                throw new HttpException(HttpStatusCode.InternalServerError, $"Could not delete environment with name: {name}");
            }
        }

        [HttpDelete("{id}")]
        public void Delete(string projectName, int id) {
            var mapper = BsonMapper.Global;
            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");

            // When query Order, includes references
            var project = FabricDatabase.Projects
                              .Include(x => x.Environments)
                              .Find(x => x.Name == projectName).FirstOrDefault()
                          ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find project with name: {projectName}");

            var environment = project.Environments.FirstOrDefault(e => e.Id == id)
                              ?? throw new HttpException(HttpStatusCode.NotFound, $"Could not find environment with id: {id}");

            var result = FabricDatabase.Environments.Delete(e => e.Id == environment.Id);

            if (result < 1) {
                throw new HttpException(HttpStatusCode.InternalServerError, $"Could not delete environment with id: {id}");
            }
        }
    }
}
