using Heeblo.Implementation;
using Heeblo.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace Heeblo.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }
         
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult LoginPost(string cred, string pwd)
        {

            LoginReq uDetail = new LoginReq()
            {
                UserCred = cred,
                UserPwd = pwd
            };
            var client = new RestClient(configuration["Config:API"]);
            var request = new RestRequest("User/ValidateUser/", Method.POST);
            request.AddJsonBody(uDetail);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute<Response>(request).Data;

            if (response.Resp)
            {
                var serialized = JsonConvert.SerializeObject(response.RespObj);
                hbl_tbl_user u = JsonConvert.DeserializeObject<hbl_tbl_user>(serialized);
                HttpContext.Session.SetObjectAsJson("user", u);
                ViewData["Error"] = null;
                if (u.role == 2)
                {
                    var pid = HttpContext.Session.GetString("pid") ?? null;
                    if (pid!=null) return RedirectToAction("WriterUpload", "Home");
                    else return RedirectToAction("WriterNoProject", "Home");
                }
                else return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewData["Error"] = response.RespMsg;
                return View("Login");
            }

        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult LoginSignUp()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult Landing()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}