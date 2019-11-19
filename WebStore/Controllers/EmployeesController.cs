using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebStore.ViewModels;
using WebStore.Infrastructure.Interfaces;


namespace WebStore.Controllers
{
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

        public IActionResult AddNewEmployee()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddNewEmployee(EmployeeView employee)
        {
            employee.Id = _EmployeesData.GetAll().Count() + 1;
            _EmployeesData.Add(employee);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditEmployee(int emploeeId)
        {
            var employee = _EmployeesData.GetAll().First(it => it.Id == emploeeId);
            return View(employee);
        }
        [HttpPost]
        public IActionResult EditEmployee(EmployeeView employee)
        {
            if (employee is null)
                throw new ArgumentOutOfRangeException(nameof(employee));

            if (!ModelState.IsValid)
                View(employee);

            var id = employee.Id;
            if (id == 0)
                _EmployeesData.Add(employee);
            else
                _EmployeesData.Edit(id, employee);

            _EmployeesData.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Id)
        {
            var employee = _EmployeesData.GetById(Id);
            if (employee is null)
                return NotFound();
            return View(employee);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int Id)
        {
            _EmployeesData.Delete(Id);
            return RedirectToAction("Index");
        }
    }
}
