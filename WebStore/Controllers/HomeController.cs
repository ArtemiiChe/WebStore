using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _Configuration;
        public HomeController(IConfiguration Configuration) => _Configuration = Configuration;
        public IActionResult ReadConfig() => Content(_Configuration["CustomData"]);
        public IActionResult Index()
        {
            return View("GetEmployes", __Employees);
        }
        private static readonly List<EmployeeView> __Employees = new List<EmployeeView>
        {
            new EmployeeView { Id = 1, SecondName = "Иванов", FirstName = "Иван", Patronymic = "Иванович", Age = 35, BirtDay = new DateTime(1984, 8, 20) },
            new EmployeeView { Id = 2, SecondName = "Петров", FirstName = "Пётр", Patronymic = "Петрович", Age = 25, BirtDay = new DateTime(1994, 7, 19) },
            new EmployeeView { Id = 3, SecondName = "Сидоров", FirstName = "Сидор", Patronymic = "Сидорович", Age = 18, BirtDay = new DateTime(2001, 6, 18) },
        };
        public IActionResult GetEmployes()
        {
            return View(__Employees);
        }

        public IActionResult GetEmployee(int employeeId)
        {
            var employee = __Employees.Any(it => it.Id == employeeId) ?
                __Employees.First(it => it.Id == employeeId) :
                new EmployeeView { Id = 0, SecondName = "Нет", FirstName = "и не может быть", Patronymic = "такого сотрудника", Age = 35, BirtDay = new DateTime(1984, 8, 20) };
            return View(employee);
        }
    }
}