using Heeblo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Heeblo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            var a = 10;
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

    }
}