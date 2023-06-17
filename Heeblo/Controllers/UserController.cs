using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Heeblo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;

        public UserController(IUser user)
        {
            this._user = user;
        }
        [HttpGet]
        public IActionResult GetAllUser()
        {
            var res = _user.GetAllUser();
            return Ok(res);
        }
        [HttpGet("{pid}")]
        public IActionResult GetUserByPublication(int pid)
        {
            var res = _user.GetUserByPublication(pid);
            return Ok(res);
        }
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var res = _user.GetUserById(id);
            return Ok(res);
        }
        [HttpPost]
        public IActionResult SaveUser(hbl_tbl_user user)
        {
            var res = _user.SaveUser(user);
            return Ok(res);
        }
        
    }
}
