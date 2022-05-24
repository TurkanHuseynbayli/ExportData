using ExportData.Models;
using Microsoft.EntityFrameworkCore;

namespace ExportData.DAL
{
    public class AppDbContext :DbContext
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<ExportDataType> ExportDataTypes { get; set; }
        public DbSet<ExportDatas> ExportDatas { get; set; }

    }
}
