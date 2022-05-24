using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExportData.Migrations
{
    public partial class ExportData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    RelationId = table.Column<int>(type: "int", nullable: false),
                    ExportDataTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportDatas_ExportDataTypes_ExportDataTypeId",
                        column: x => x.ExportDataTypeId,
                        principalTable: "ExportDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportDatas_ExportDataTypeId",
                table: "ExportDatas",
                column: "ExportDataTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportDatas");
        }
    }
}
