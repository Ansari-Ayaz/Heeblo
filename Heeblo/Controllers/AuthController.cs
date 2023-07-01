using Heeblo.Implementation;
using Heeblo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Security.Claims;

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
            HttpContext.Session.Remove("user");
            return View();
        }
        public IActionResult LoginPost(string cred, string pwd)
        {
            if (cred == "" || cred == null || pwd == null || pwd == "")
            {
                ViewData["Error"] = "Please enter your credential";
                return View("Login");
            }
            LoginReq uDetail = new LoginReq()
            {
                UserCred = cred,
                UserPwd = pwd
            };
            Console.WriteLine(configuration["Config:API"]);
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
                    if (pid != null) return RedirectToAction("WriterUpload", "Home");
                    else return RedirectToAction("WriterNoProject", "Home");
                }
                else if (u.role == 3) return RedirectToAction("Index", "Home");
                else return RedirectToAction("AllUsers", "Home");

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
            var email = TempData["Email"];
            var name = TempData["Name"];
            if (email != null && name != null)
            {
                ViewData["Email"] = email;
                ViewData["Name"] = name;
            }
            return View();
        }

        public IActionResult LoginSignUp()
        {
            return View();
        }
        public IActionResult ForgotPass()
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
        public IActionResult PrivacyPolicy()
        {
            return View();
        }
        public IActionResult TermsAndCondition()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult SignInWithGoogle()
        {
            var redirectUrl = Url.Action(nameof(HandleGoogleResponse), "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> HandleGoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();
            if (authenticateResult.Succeeded)
            {
                // Authentication successful, handle the user information as needed
                var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
                var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
                TempData["Email"] = email;
                TempData["Name"] = name;
                return RedirectToAction("Register", "Auth");
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }
    }
}