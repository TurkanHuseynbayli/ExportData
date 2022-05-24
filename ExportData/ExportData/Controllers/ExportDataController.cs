using ClosedXML.Excel;
using ExportData.DAL;
using ExportData.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExportData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportDataController : ControllerBase
    {

        private readonly AppDbContext _context;
        public ExportDataController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/<ExportDataController>
        [HttpGet]
        public ActionResult<List<Student>> Get()
        {

            var stu = _context.Students.ToList();

            return stu;
        }

        // GET api/<ExportDataController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> Get(int id)
        {
            Student stu = await _context.Students.FindAsync(id);
            if (stu == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Student NotFound!" });
            return stu;
        }
        //public async Task<ActionResult> HangFire(int? id)
        //{
        //    var jobId = BackgroundJob.Schedule(() => ExportExcel(id), TimeSpan.FromMinutes(2));
        //    return null;
        //}
        [HttpPost("Exports")]
        public async Task<ActionResult> HangFire(string client)
        {
            string jobId = BackgroundJob.Enqueue(() => Console.WriteLine($"Student named '{client}' was expelled"));
            return Ok(jobId);
        }

        [HttpPost("Export")]
        public async Task<IActionResult> ExportExcel(int? id)
        {
           using (var workbook =new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Student");
                ws.Range("A2:E2").Merge();
                ws.Cell(1, 2).Value = "Student";
                ws.Cell(1, 1).Style.Font.Bold=true;
                ws.Cell(1, 1).Style.Alignment.Horizontal=XLAlignmentHorizontalValues.Center;
                ws.Cell(1, 1).Style.Font.FontSize = 30;

                ws.Cell(4, 1).Value = "Id";
                ws.Cell(4,2).Value = "Name";
                ws.Cell(4, 3).Value = "Surname";
                ws.Cell(4, 4).Value = "Age";
                ws.Range("A4:D4").Style.Fill.BackgroundColor = XLColor.Alizarin;

                Student stu = await _context.Students.FindAsync(id);

                if (stu == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Student NotFound!" });

                ws.Cell(5, 1).Value = stu.Id;
                ws.Cell(5, 2).Value = stu.Name;
                ws.Cell(5, 3).Value = stu.Surname;
                ws.Cell(5, 4).Value = stu.Age;

                ExportDataType exportDataType = new ExportDataType();
                exportDataType.Name = "Student.xlsx";
                exportDataType.TypeCode = "xlsx";

                _context.ExportDataTypes.AddAsync(exportDataType);

                ExportDatas exportData= new ExportDatas();
                exportData.Status = true;
                exportData.RelationId = stu.Id;
                exportData.ExportDataTypeId=stu.Id; 

                _context.ExportDatas.AddAsync(exportData);
                _context.SaveChangesAsync();

                HangFire(stu.Name);

                using (var strem=new MemoryStream())
                {
                    workbook.SaveAs(strem);
                    var content=strem.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument-spreadsheetml.sheet",
                        "Student.xlsx"
                               );
                }
            }
         
            //var client = new BackgroundJobClient();

            //client.Enqueue(() => Console.WriteLine("Easy!"));


            // var jobId = BackgroundJob.Enqueue(()=>_context.Students.ToList());
        }


  

     

        // POST api/<ExportDataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ExportDataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExportDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
