using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreJob.Server.Store.Mysql.Migrations
{
    public partial class V103 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error_mail_user",
                table: "job_info",
                type: "varchar(1000) CHARACTER SET utf8mb4",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_mail_user",
                table: "job_info");
        }
    }
}
