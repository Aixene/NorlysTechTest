using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Norlysical2.Models;

namespace Norlysical2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficeController : ControllerBase
    {
        private AppDb Db;

        public OfficeController(AppDb db)
        {
            Db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Db.Connection.OpenAsync();
            var query = new OfficeQuery(Db);
            var result = await query.AllOfficesAsync();
            return new OkObjectResult(result);
        }

        //read
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0)
                return BadRequest("Value must be passed in the request body.");

            await Db.Connection.OpenAsync();
            var query = new OfficeQuery(Db);
            var result = await query.FindOfficeAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }



    }
}
