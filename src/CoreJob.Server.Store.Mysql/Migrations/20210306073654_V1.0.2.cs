using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreJob.Server.Store.Mysql.Migrations
{
    public partial class V102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "selector_type",
                table: "job_info",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selector_type",
                table: "job_info");
        }
    }
}
