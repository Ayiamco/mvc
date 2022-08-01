using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcPractice.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DapperMVCCRUD.Models;
using MvcPractice.Dapper;
using MvcPractice.EF;

namespace MvcPractice.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Lga()
        {
            //var val = DapperOrm.ReturnList<Lga>("SP_GetAllLgas");
            var val = Ado.GetLgas();
            //return new JsonResult(val) ;
            return View(new {data=val});
        }

        public IActionResult LgaData()
        {
            //var val = DapperOrm.ReturnList<Lga>("SP_GetAllLgas");
            var val = Ado.GetLgas();
            //return new JsonResult(val) ;
            return Json(new { data = val });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
