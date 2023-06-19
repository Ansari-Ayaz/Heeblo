using Heeblo.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Verified()
        {
            return View();
        }
        public IActionResult ApplicationList()
        {
            return View();
        }
        public IActionResult AddProject()
        {
            return View();
        }
        public IActionResult EmailSent()
        {
            return View();
        }

        public IActionResult WriterNoProject()
        {
            return View();
        }
        public IActionResult WriterThankyou()
        {
            return View();
        }
        public IActionResult WriterUpload()
        {
            return View();
        }
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Applications()
        {
          
            return View();
        }

        public IActionResult Privacy()
        {
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

    }
}