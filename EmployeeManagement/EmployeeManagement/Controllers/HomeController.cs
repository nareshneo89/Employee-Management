using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger,IDataProtectionProvider dataProtectionProvider, DataProtactionPurposeStrings dataProtactionPurposeStrings)        
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            protector = dataProtectionProvider.CreateProtector(dataProtactionPurposeStrings.EmployeeIdRouteValue);

        }
        //[Route("")]
        //[Route("Home")]
        //[Route("Home/Index")]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee()
                .Select(e=>
                {
                    e.EncryptedId = protector.Protect(e.Id.ToString());
                    return e;
                });
            return View(model);
        }
        //[Route("Home/Details/{Id?}")]
        public ViewResult Details(string? id)
        {
            logger.LogTrace("Log Trace");
            logger.LogDebug("Log Debug");
            logger.LogInformation("Log Information");
            logger.LogWarning("LogWarning");
            logger.LogError("Log Error");
            logger.LogCritical("Log Critical");

            int employeeId = Convert.ToInt32(protector.Unprotect(id));
                
            Employee employee = _employeeRepository.GetEmployee(employeeId);
            if(employee==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);
            }
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,//_employeeRepository.GetEmployee(id??1),
                PageTitle = "Employee Details"
            };

            
            //ViewData["Employee"] = model;                         Losily type Checking
            //ViewData["Pagetitle"] = "Employee Details";

            //ViewBag.Employee = model;                             Losily type Checking
            //ViewBag.Pagetitle = "Employee Details";


            return View(homeDetailsViewModel);
            //return View("Test");             hard code the view name and call Test.cshtml page
            //return View("MyView/Test");             hard code the Full path and call Test.cshtml page
        }
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null)
                {
                   if(model.ExistingPhotoPath!=null)
                    {
                      string FilePath=  Path.Combine(hostingEnvironment.WebRootPath, "Images", model.ExistingPhotoPath);
                        System.IO.File.Delete(FilePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }
                
                _employeeRepository.Update(employee);
                return RedirectToAction("Index");
            }
            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string UploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string FilePath = Path.Combine(UploadsFolder, uniqueFileName);
                using(var fileStream= new FileStream(FilePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }               
            }
            return uniqueFileName;
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
               

                Employee newemployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };
                _employeeRepository.Add(newemployee);
                return RedirectToAction("Details", new { id = newemployee.Id });
            }
            return View();
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        
    }
}
