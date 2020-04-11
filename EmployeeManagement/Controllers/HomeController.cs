using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
   // [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        
        //this give acces to the physical path of WWWroot, so we have acces to images folder
        private readonly IWebHostEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment)
        {
            this.employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public ViewResult Index() 
        {
            var model = employeeRepository.GetAllEmployee();
            return View(model);

        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var employee = employeeRepository.GetEmployee(id);
            if (employee == null)
            {
                throw new Exception();
            }
            var employeeEditViewModel = new EmployeeEditViewModel
            {
                ID = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Deparment = employee.Deparment,
                ExistingPhotoPath = employee.StringPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeEditViewModel model)
        {
            //ModelState helps to check for validation
            if (ModelState.IsValid)
            {
                var employee = employeeRepository.GetEmployee(model.ID);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Deparment = model.Deparment;
                if (model.Photos != null)
                { 
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.StringPath = UploadFileProcess(model);
                }

                employeeRepository.UpdateEmployee(employee);
                return RedirectToAction("Index");
            }
            return View(); 
        }

        private string UploadFileProcess(EmployeeCreateViewModel model)
        {
            string UniqueFileName = null;
            if (model.Photos != null && model.Photos.Count > 0)
            {
                foreach (IFormFile photos in model.Photos)
                {
                    //this provides us with the physical path to WWWroot folder
                    //then we combine the path with the "images" path den sved in a var
                    string UploadFilePath = Path.Combine(hostingEnvironment.WebRootPath, "images");

                    //Inorder to have unique filenames we use "Guid"
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + photos.FileName;
                    string filePath = Path.Combine(UploadFilePath, UniqueFileName);
                    //copyTo copies to the images folder then filemode.create ""avails it n d server"
                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photos.CopyTo(fileStream);
                    }
                }
            }
            return UniqueFileName;
        }

        public ActionResult Details(int? id) 
        {
            //throw new Exception("Error In Details View");
            var employee = employeeRepository.GetEmployee(id.Value);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);

            }
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "EmployeeDetails"
            };
                // var response = employeeRepository.GetAllEmployee();
                // Employee model = employeeRepository.GetEmployee(2);

                return View(homeDetailsViewModel);
        }

        //This serves the create Page
        [HttpGet]
        public ActionResult Create()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid) 
            {
                string UniqueFileName = UploadFileProcess(model);
                var newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Deparment = model.Deparment,
                    StringPath = UniqueFileName
                };
                employeeRepository.AddEmployee(newEmployee);
                return RedirectToAction("Details", new { id = newEmployee.Id });
            }
            return View();
           
        }

        public ActionResult Delete(int id)
        {
            employeeRepository.DeleteEmployee(id);
            return RedirectToAction("Index", "home");
        }

    }
}