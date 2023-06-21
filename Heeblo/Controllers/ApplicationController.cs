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
        [HttpGet]
        [HttpGet("{pid}")]
        public IActionResult GetApplicationByPid(int pid)
        {
            var res = _app.GetApplicationByPid(pid);
            return Ok(res);
        }

        [HttpGet]
        [HttpGet("{appId}")]
        public IActionResult GetUserDetailsByAppId(int appId)
        {
            var res = _app.GetUserDetailsByAppId(appId);
            return Ok(res);
        }
        [HttpGet("{id}")]
        public IActionResult GetApplicationById(int id)
        {
            var res = _app.GetApplicationById(id);
            return Ok(res);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult ApplicationStatus(AppFeedback feedback)
        {
            var res = _app.ApplicationStatus(feedback);
            return Ok(res);
        }
        //[HttpGet("{pid}")]
        //public IActionResult GetAllAppCountByPid(int pid)
        //{
        //    var res = _app.GetAllAppCountByPid(pid);
        //    return Ok(res);
        //}
        [HttpPost]
        public IActionResult SaveApplication([FromForm] application_view app)
        {
            //var a = Request.Form.Files;
            var res = _app.SaveApplication(app);
            return Ok(res);
        }

    }
}
