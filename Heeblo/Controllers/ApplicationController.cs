using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Heeblo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplication _app;

        public ApplicationController(IApplication app)
        {
            this._app = app;
        }
        [HttpGet]
        public IActionResult GetAllApplication()
        {
            var res = _app.GetAllApplication();
            return Ok(res);
        }
        [HttpGet("{id}")]
        public IActionResult GetApplicationById(int id)
        {
            var res = _app.GetApplicationById(id);
            return Ok(res);
        }
        //[HttpPost("{pid}/{uid}/{created_by}")]
        //public IActionResult SaveApplication([FromForm]IFormFile file1,[FromForm]IFormFile file2, int pid,int uid,int created_by)
        //{
        //    var res = _app.SaveApplication(file1,file2,app);
        //    return Ok(res);
        //}
    }
}
