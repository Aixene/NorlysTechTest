using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Norlysical2.Models;
using System.Net;

namespace Norlysical2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private AppDb Db;

        public EmployeeController(AppDb db)
        {
            Db = db;
        }

        //read
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Db.Connection.OpenAsync();
            var query = new EmployeeQuery(Db);
            var result = await query.AllEmployeesAsync();
            return new OkObjectResult(result);
        }

        //read
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0)
                return BadRequest("Value must be passed in the request body.");

            await Db.Connection.OpenAsync();
            var query = new EmployeeQuery(Db);
            var result = await query.FindEmployeeAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        //add
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee emp)
        {
            if (emp.isValid())
            {
                await Db.Connection.OpenAsync();
                var officeQuery = new OfficeQuery(Db);
                var office = await officeQuery.FindOfficeAsync(emp.MainOfficeId);
                if ((office != null) && await office.HasVacantSpace())
                {
                    emp.Db = Db;
                    await emp.InsertAsync();
                    return new OkObjectResult(emp);
                } 
                else
                {
                    return this.Content("Office exceeds max occupancy");
                }

            }
            else
            {
                return this.Content("Employee validation error");
            }
        }

        //update
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] Employee emp)
        {
            if (emp.isValid())
            {
                await Db.Connection.OpenAsync();
                var query = new EmployeeQuery(Db);
                var result = await query.FindEmployeeAsync(emp.Id);
                if (result is null)
                    return new NotFoundResult();
                var OfficeQuery = new OfficeQuery(Db);
                var office = await OfficeQuery.FindOfficeAsync(emp.MainOfficeId);
                if ((result.MainOfficeId == emp.MainOfficeId) || ((office != null) && await office.HasVacantSpace()))
                { 
                    result.FirstName = emp.FirstName;
                    result.LastName = emp.LastName;
                    result.BirthDate = emp.BirthDate;
                    result.MainOfficeId = emp.MainOfficeId;
                    await result.UpdateAsync();
                    return new OkObjectResult(result);
                }
                else
                {
                    return this.Content("Office exceeds max occupancy");
                }
            }
            else
            {
                return this.Content("Employee validation error");
            }
            
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new EmployeeQuery(Db);
            var result = await query.FindEmployeeAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkResult();
        }

    }
}
