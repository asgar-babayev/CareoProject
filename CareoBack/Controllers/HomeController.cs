using CareoBack.DAL;
using CareoBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CareoBack.Controllers
{
    public class HomeController : Controller
    {
        readonly Context _context;

        public HomeController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }
    }
}
