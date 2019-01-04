//--------------------------------------
// Ben Powers
// Assignment 1 (Controller View Basics)
// Student Number 7986250
// Monday, September 17th, 2018
//--------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BPSail.Models;

namespace BPSail.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            TempData["Message"] = "TempDataMessage";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
