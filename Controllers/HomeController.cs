using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using reCAPTCHA_Enterprise_Sample.Models;
using reCAPTCHA_Enterprise_Sample.Service;
using System;
using System.Diagnostics;

namespace reCAPTCHA_Enterprise_Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            TempData["sitekey"] = "6Ld_Os8fAAAAACNQDEOxHKvxBWmN2wK6_GUXAZ08";
            TempData["recaptchaAction"] = "Verify_CheckBox";

            return View();
        }

        public IActionResult Privacy()
        {
            TempData["sitekey"] = "6LeUx88fAAAAAHsA_b3jPKRVg8Yyyua5Ub2qjE8I";
            TempData["recaptchaAction"] = "Verify_GetToken";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public string CreateAssessment(string token, string recaptchaSiteKey, string recaptchaAction)
        {
            try
            {
                return new CreateAssessmentSample().createAssessment(
                            projectID: "xxx",
                            recaptchaSiteKey: recaptchaSiteKey,
                            token: token,
                            recaptchaAction: recaptchaAction).ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}