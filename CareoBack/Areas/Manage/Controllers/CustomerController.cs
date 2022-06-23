using CareoBack.DAL;
using CareoBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CareoBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CustomerController : Controller
    {
        readonly Context _context;
        readonly IWebHostEnvironment _env;

        public CustomerController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);
            if (customer.ImageFile != null)
            {
                if (customer.ImageFile.ContentType != "image/jpeg" && customer.ImageFile.ContentType != "image/png" && customer.ImageFile.ContentType != "image/webp")
                {
                    ModelState.AddModelError(customer.ImageFile.FileName, "This is not image format");
                    return View(customer);
                }
                if (customer.ImageFile.Length / 1024 > 2000)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View(customer);
                }
                string fileName = customer.ImageFile.FileName;
                if (customer.ImageFile.FileName.Length > 64)
                {
                    fileName.Substring(fileName.Length - 64, 64);
                }
                string newFileName = Guid.NewGuid().ToString() + fileName;
                string path = Path.Combine(_env.WebRootPath, "assets", "images", newFileName);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    customer.ImageFile.CopyToAsync(fs);
                }
                customer.Image = newFileName;
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(customer.ImageFile.FileName, "Image required");
            return View(customer);
        }
        public IActionResult Edit(int id)
        {
            return View(_context.Customers.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            var existCustomer = _context.Customers.FirstOrDefault(x => x.Id == customer.Id);
            if (existCustomer == null) return NotFound();
            string newFileName = null;

            if (customer.ImageFile != null)
            {
                if (customer.ImageFile.ContentType != "image/jpeg" && customer.ImageFile.ContentType != "image/png" && customer.ImageFile.ContentType != "image/webp")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View();
                }
                if (customer.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View();
                }
                string fileName = customer.ImageFile.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_env.WebRootPath, "assets", "images", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    customer.ImageFile.CopyTo(stream);
                }
            }
            if (newFileName != null)
            {
                string deletePath = Path.Combine(_env.WebRootPath, "assets", "images", existCustomer.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                existCustomer.Image = newFileName;
            }

            existCustomer.Name = customer.Name;
            existCustomer.Description = customer.Description;
            existCustomer.Title = customer.Title;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var menu = _context.Customers.FirstOrDefault(x => x.Id == id);
            _context.Customers.Remove(menu);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
