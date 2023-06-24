using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;

namespace Heeblo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            this.configuration = configuration;
            _logger = logger;
        }
        public IActionResult AllUsers()
        {
            if (!Authorized(1)) return RedirectToAction("Login", "Auth");
            return View();
        }
        public IActionResult AdminProj()
        {
            if (!Authorized(1)) return RedirectToAction("Login", "Auth");
            return View();
        }
        public IActionResult Verified()
        {
            return View();
        }
        public IActionResult EmailSent()
        {
            return View();
        }
        public IActionResult EmailSentReset()
        {
            return View();
        }

        public IActionResult WriterNoProject()
        {
            if (!Authorized(2)) return RedirectToAction("Login", "Auth");
            return View();
        }
        public IActionResult WriterThankyou()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult WriterUpload()
        {
            var user = HttpContext.Session.GetString("user")??null;
            if (!Authorized(2)) return RedirectToAction("Login", "Auth");
            return View();
        }
        public IActionResult Index()
        {
            if (!Authorized(3)) return RedirectToAction("Login", "Auth");
            return View();
        }

        public IActionResult Applications()
        {
            if (!Authorized(3)) return RedirectToAction("Login", "Auth");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Request(string pid)
        {
            var decryptedPid =  AESEncryption.Decrypt(pid);
            HttpContext.Session.SetObjectAsJson("pid", decryptedPid);
            return RedirectToAction("LoginSignUp", "Auth");
        }
        public IActionResult VerifyForReset(string uid)
        {
            var decryptedUid = AESEncryption.Decrypt(uid);

            var client = new RestClient(configuration["Config:API"]);
            var request = new RestRequest("User/VerifyUser/" + decryptedUid, Method.GET);
            var response = client.Execute<bool>(request).Data;
            if (response)
            {
                HttpContext.Session.SetObjectAsJson("uid", decryptedUid);
                return RedirectToAction("ResetPassword", "Auth");
            }
            else
            {
                return View("Login");
            }

        }
        public IActionResult Verify(string uid)
        {
            var decryptedUid = AESEncryption.Decrypt(uid);

            var client = new RestClient(configuration["Config:API"]);
            var request = new RestRequest("User/VerifyUser/"+ decryptedUid, Method.GET);
            var response = client.Execute<bool>(request).Data;
            if (response)
            {
                return RedirectToAction("Verified", "Home");
            }
            else
            {
                return View("Login");
            }

        }
        public bool Authorized(int role)
        {
            var serializedUser = HttpContext.Session.GetString("user") ?? null;
            if (serializedUser == null ) return false;
            var user = JsonConvert.DeserializeObject<hbl_tbl_user>(serializedUser);
            if (user.role != role) return false;
            return true;
        }
    }
}