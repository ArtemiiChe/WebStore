using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebStore.ViewModels;
using WebStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IEmployeesData _EmployeesData;

        public EmployeesController(IEmployeesData EmployeesData, IConfiguration Configuration)  {
            _EmployeesData = EmployeesData;
            _Configuration = Configuration;
        }
        
        public IActionResult ReadConfig() => Content(_Configuration["CustomData"]);
        public IActionResult Index()
        {
            return View("GetEmployes", _EmployeesData.GetAll());
        }
         
        public IActionResult GetEmployes()
        {
            return View(_EmployeesData.GetAll());
        }

        public IActionResult GetEmployee(int employeeId)
        {
            var employee = _EmployeesData.GetAll().Any(it => it.Id == employeeId) ?
                _EmployeesData.GetAll().First(it => it.Id == employeeId) :
                new EmployeeView { Id = 0, SecondName = "Нет", FirstName = "и не может быть", Patronymic = "такого сотрудника", Age = 35, BirtDay = new DateTime(1984, 8, 20) };
            return View(employee);
        }

        [Authorize(Roles = Role.Administrator)]
        public IActionResult AddNewEmployee()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult AddNewEmployee(EmployeeView employee)
        {
            try
            {
                _EmployeesData.Add(employee);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"Ошибка редактирования: {ex.Message}");
            }
        }
        [Authorize(Roles = Role.Administrator)]
        public IActionResult EditEmployee(int emploeeId)
        {
            try
            {
                var employee = _EmployeesData.GetAll().First(it => it.Id == emploeeId);
                return View(employee);
            }
            catch (Exception ex)
            {
                return Content($"Ошибка редактирования: {ex.Message}");
            }
            
        }
        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult EditEmployee(EmployeeView employee)
        {
            if (employee is null)
                throw new ArgumentOutOfRangeException(nameof(employee));

            if (employee.Age < 18)
                ModelState.AddModelError(nameof(EmployeeView.Age), "Возраст не может быть меньше 18 лет");

            if (employee.FirstName == "123" && employee.SecondName == "qwe")
                ModelState.AddModelError("", "Странное сочетание имени и фамилии");

            if (!ModelState.IsValid)
                return View(employee);

            var id = employee.Id;
            if (id == 0)
                _EmployeesData.Add(employee);
            else
                _EmployeesData.Edit(id, employee);

            _EmployeesData.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = Role.Administrator)]
        public IActionResult Delete(int Id)
        {
            var employee = _EmployeesData.GetById(Id);
            if (employee is null)
                return NotFound();
            return View(employee);
        }

        [HttpPost]
        [Authorize(Roles = Role.Administrator)]
        public IActionResult DeleteConfirmed(int Id)
        {
            _EmployeesData.Delete(Id);
            return RedirectToAction("Index");
        }
    }
}
