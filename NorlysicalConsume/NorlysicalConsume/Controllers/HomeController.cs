using Microsoft.AspNetCore.Mvc;
using NorlysicalConsume.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using NorlysicalConsume.Configuration;
using System;
using NorlysicalConsume.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorlysicalConsume.Controllers
{
    public class HomeController : Controller
    {
        private readonly Settings _settings;
        public HomeController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<IActionResult> Index()
        {
            List<Employee> employeeList = new List<Employee>();
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(new Uri(_settings.WebAPIURL), _settings.EmployeeAPI);
                using (var response = await httpClient.GetAsync(apiURL))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    employeeList = JsonConvert.DeserializeObject<List<Employee>>(apiResponse);
                }
            }
            return View(employeeList);
        }

        //[HttpGet]
        public ViewResult GetEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployee(int id)
        {
            Employee employee = new Employee();
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(_settings.WebAPIURL).Append(_settings.EmployeeAPI, $"{id}");
                using (var response = await httpClient.GetAsync(apiURL))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        employee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
            }
            return View(employee);
        }


        public async Task<IActionResult> AddEmployee()
        {
            List<Office> officeList = new List<Office>();
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(new Uri(_settings.WebAPIURL), _settings.OfficeAPI);
                using (var response = await httpClient.GetAsync(apiURL))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    officeList = JsonConvert.DeserializeObject<List<Office>>(apiResponse);
                }
            }
            officeList.Insert(0, new Office() { Id = 0, Location = "No office selected" });
            var items = new SelectList(officeList, "Id", "Location");
            ViewBag.offices = items;
            return View();
        }

        //add
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            Employee receivedEmployee = new Employee();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                var apiURL = new Uri(new Uri(_settings.WebAPIURL), _settings.EmployeeAPI);
                using (var response = await httpClient.PostAsync(apiURL, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedEmployee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                }
            }
            if(receivedEmployee != null && receivedEmployee.Id != 0)
            {
                return View(receivedEmployee);
            }
            else
            {
                Employee tempEmp = new Employee { Id = -1, FirstName = "not valid employee", LastName = "not valid employee" };
                return View(tempEmp);
            }
        }

        //update
        public async Task<IActionResult> UpdateEmployee(int id)
        {
            Employee employee = new Employee();
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(_settings.WebAPIURL).Append(_settings.EmployeeAPI, $"{id}");
                using (var response = await httpClient.GetAsync(apiURL))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    employee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                }
            }

            List<Office> officeList = new List<Office>();
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(new Uri(_settings.WebAPIURL), _settings.OfficeAPI);
                using (var response = await httpClient.GetAsync(apiURL))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    officeList = JsonConvert.DeserializeObject<List<Office>>(apiResponse);
                }
            }

            officeList.Insert(0, new Office() { Id = 0, Location = "No office selected" });
            var items = new SelectList(officeList, "Id", "Location");
            ViewBag.offices = items;

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            Employee receivedEmployee = new Employee();
            using (var httpClient = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(employee.Id.ToString()), "Id");
                content.Add(new StringContent(employee.FirstName), "FirstName");
                content.Add(new StringContent(employee.LastName), "LastName");
                content.Add(new StringContent(employee.BirthDate.ToString()), "BirthDate");
                content.Add(new StringContent(employee.MainOfficeId.ToString()), "MainOfficeId");
                
                var apiURL = new Uri(new Uri(_settings.WebAPIURL), _settings.EmployeeAPI);
                using (var response = await httpClient.PutAsync(apiURL, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    
                    if (!apiResponse.Equals("Office exceeds max occupancy", StringComparison.InvariantCultureIgnoreCase) && !apiResponse.Equals("Employee validation error", StringComparison.InvariantCultureIgnoreCase))
                    {
                        receivedEmployee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                        ViewBag.Result = "Success";
                    }
                    else
                    {
                        ViewBag.Result = "Failure";
                    }
                }
            }
            return View(receivedEmployee);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int EmployeeId)
        {
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(_settings.WebAPIURL).Append(_settings.EmployeeAPI, $"{EmployeeId}");
                using (var response = await httpClient.DeleteAsync(apiURL))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("Index");
        }


        //office stuff
        public async Task<IActionResult> OfficeIndex()
        {
            List<Office> officeList = new List<Office>();
            using (var httpClient = new HttpClient())
            {
                var apiURL = new Uri(new Uri(_settings.WebAPIURL), _settings.OfficeAPI);
                using (var response = await httpClient.GetAsync(apiURL))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    officeList = JsonConvert.DeserializeObject<List<Office>>(apiResponse);
                }
            }
            return View(officeList);
        }


    }
}

    