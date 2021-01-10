using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreJob.Server.Store.SqlServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dashboard_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_disabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    in_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_executer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    registry_key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    registry_hosts = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    auto = table.Column<bool>(type: "bit", nullable: false),
                    in_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cron = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    executor_id = table.Column<int>(type: "int", nullable: false),
                    in_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    executor_handler = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    executor_param = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    create_user = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    executer_id = table.Column<int>(type: "int", nullable: false),
                    executer_host = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    executer_handler = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    executer_param = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    handler_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    handler_code = table.Column<int>(type: "int", nullable: false),
                    handler_msg = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    complete_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    complete_code = table.Column<int>(type: "int", nullable: false),
                    complete_msg = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "registry_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    host = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    in_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dashboard_user_name",
                table: "dashboard_user",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_job_executer_name",
                table: "job_executer",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_job_executer_registry_key",
                table: "job_executer",
                column: "registry_key");

            migrationBuilder.CreateIndex(
                name: "IX_job_info_name",
                table: "job_info",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_job_log_executer_id",
                table: "job_log",
                column: "executer_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_log_job_id",
                table: "job_log",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_registry_info_host",
                table: "registry_info",
                column: "host");

            migrationBuilder.CreateIndex(
                name: "IX_registry_info_name",
                table: "registry_info",
                column: "name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dashboard_user");

            migrationBuilder.DropTable(
                name: "job_executer");

            migrationBuilder.DropTable(
                name: "job_info");

            migrationBuilder.DropTable(
                name: "job_log");

            migrationBuilder.DropTable(
                name: "registry_info");
        }
    }
}
