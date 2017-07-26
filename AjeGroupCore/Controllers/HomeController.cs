using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AjeGroupCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Acerca de Nosotros";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contáctenos";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
