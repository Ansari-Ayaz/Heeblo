using Heeblo.Implementation;
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
        [HttpGet("{role?}")]
        public IActionResult GetAllUser(int role = 0)
        {
            var res = _user.GetAllUser(role);
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
        [HttpGet]
        [HttpGet("{userCred}")]
        public IActionResult ForgotLink(string userCred)
        {
           var resp= _user.ForgotLink(userCred);
            return Ok(resp);
        }
        [HttpGet]
        [HttpGet("{uid}/{password}")]
        public IActionResult PasswordForgot(int uid,string password)
        {
            var res = _user.PasswordForgot(uid,password);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult ValidateUser(LoginReq req)
        {
            var resp = _user.ValidateUser(req);
            return Ok(resp);
        }
        [HttpGet("{uid}")]
        [HttpGet]
        public IActionResult VerifyUser(int uid)
        {
            var res = _user.VerifyUser(uid);
            return Ok(res);
        }
        [HttpGet("{uid}/{isActive}")]
        [HttpGet]
        public IActionResult ChangeIsActive(int uid, bool isActive)
        {
            var res = _user.ChangeIsActive(uid, isActive);
            return Ok(res);
        }

        [HttpGet("{encryptedData}")]
        public IActionResult Decrypt(string encryptedData)
        {
            var res = AESEncryption.Decrypt(encryptedData);
            return Ok(res);
        }
    }
}
