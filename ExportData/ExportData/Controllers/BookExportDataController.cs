using ClosedXML.Excel;
using ExportData.DAL;
using ExportData.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExportData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookExportDataController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BookExportDataController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/<BookExportDataController>
        [HttpGet]
        public ActionResult<List<Book>> Get()
        {

            var book = _context.Books.ToList();

            return book;
        }

        [HttpPost("Exports")]
        public async Task<ActionResult> HangFire(string client)
        {
            string jobId = BackgroundJob.Enqueue(() => Console.WriteLine($"Book named '{client}' was expelled"));
            return Ok(jobId);
        }

        // GET api/<BookExportDataController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            Book book = await _context.Books.FindAsync(id);
            if (book == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Book NotFound!" });
            return book;
        }

        [HttpPost("Export")]
        public async Task<IActionResult> ExportExcel(int? id)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Book");
                ws.Range("A2:E2").Merge();
                ws.Cell(1, 2).Value = "Book";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(1, 1).Style.Font.FontSize = 30;

                ws.Cell(4, 1).Value = "Id";
                ws.Cell(4, 2).Value = "Name";
              
                ws.Range("A4:E4").Style.Fill.BackgroundColor = XLColor.Alizarin;

                Book book = await _context.Books.FindAsync(id);
                if (book == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Book NotFound!" });

                ws.Cell(5, 1).Value = book.Id;
                ws.Cell(5, 2).Value = book.Name;

                ExportDataType exportDataType = new ExportDataType();
                exportDataType.Name = "Book.xlsx";
                exportDataType.TypeCode = "xlsx";

                _context.ExportDataTypes.AddAsync(exportDataType);

                ExportDatas exportData = new ExportDatas();
                exportData.Status = true;
                exportData.RelationId = book.Id;
                exportData.ExportDataTypeId = book.Id;

                _context.ExportDatas.AddAsync(exportData);
                _context.SaveChangesAsync();

                HangFire(book.Name);

                using (var strem = new MemoryStream())
                {
                    workbook.SaveAs(strem);
                    var content = strem.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument-spreadsheetml.sheet",
                        "Book.xlsx"


                        );
                }
            }
        }


        // POST api/<BookExportDataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BookExportDataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookExportDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
