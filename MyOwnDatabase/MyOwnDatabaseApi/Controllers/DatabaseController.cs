using Microsoft.AspNetCore.Mvc;
using MyOwnDatabase;

namespace MyOwnDatabaseApi.Controllers
{
    [ApiController]
    [Route("api/database")]
    public class DatabaseController : ControllerBase
    {
        private readonly Database _db;

        public DatabaseController(Database db)
        {
            _db = db;
        }

        [HttpPost("create-table")]
        public IActionResult CreateTable(string name, [FromBody] List<string> columns)
        {
            _db.CreateTable(name, columns);
            return Ok("Table created");
        }

        [HttpPost("insert")]
        public IActionResult Insert(string table, [FromBody] Dictionary<string, object> values)
        {
            _db.InsertRow(table, values);
            return Ok("Row inserted");
        }

        [HttpGet("select")]
        public IActionResult Select(string table, [FromQuery] string columns = "*", [FromQuery] string where = "")
        {
            var cols = columns.Split(',').ToList();
            return Ok(_db.Select(table, cols, where));
        }

        [HttpPut("update")]
        public IActionResult Update(string table, [FromBody] Dictionary<string, object> values, [FromQuery] string where = "")
        {
            _db.Update(table, values, where);
            return Ok("Updated");
        }

        [HttpDelete("delete")]
        public IActionResult Delete(string table, [FromQuery] string where = "")
        {
            _db.Delete(table, where);
            return Ok("Deleted");
        }

        [HttpGet("join")]
        public IActionResult Join(string table1, string table2, string onCondition)
        {
            return Ok(_db.Join(table1, table2, onCondition));
        }
    }
}