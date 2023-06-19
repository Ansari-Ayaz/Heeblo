using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Heeblo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProject _proj;

        public ProjectController(IProject proj)
        {
            this._proj = proj;
        }
        [HttpGet("{uid}")]
        [HttpGet]
        public IActionResult GetAllProjects(int uid)
        {
            var res = _proj.GetAllProjects(uid);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public IActionResult GetProjectById(int id)
        {
            var res = _proj.GetProjectById(id);
            return Ok(res);
        }
        [HttpPost]
        public IActionResult SaveProject(hbl_tbl_project proj)
        {
            var res = _proj.SaveProject(proj);
            return Ok(res);
        }
        
    }
}
