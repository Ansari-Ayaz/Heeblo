using Heeblo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Heeblo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HeebloController : ControllerBase
    {
        private readonly IHeeblo _heeblo;

        public HeebloController(IHeeblo heeblo)
        {
            this._heeblo = heeblo;
        }
        //[HttpGet("{content}")]
        //public IActionResult Plagiarism(string content)
        //{
        //    var res = _heeblo.Plagiarism(content);
        //    return Ok(res);
        //}
        //[HttpGet("{content}")]
        //public IActionResult AiDetect(string content)
        //{
        //    var res = _heeblo.AiDetect(content);
        //    return Ok(res);
        //}
        //[HttpGet("{content}")]
        //public IActionResult Grammer(string content)
        //{
        //    var res = _heeblo.Grammer(content);
        //    return Ok(res);
        //}
    }
}
