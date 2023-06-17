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
        [HttpGet]
        public IActionResult Plagiarism()
        {
            var res = _heeblo.Plagiarism();
            return Ok(res);
        }
    }
}
