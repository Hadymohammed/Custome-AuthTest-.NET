﻿using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Models
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
