﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize (Roles = "Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}