using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreJob.Server.Store.Mysql.Migrations
{
    public partial class V101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "registry_hosts",
                table: "job_executer");

            migrationBuilder.CreateTable(
                name: "registry_host",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    executer_id = table.Column<int>(type: "int", nullable: false),
                    host = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: false),
                    order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                    table.ForeignKey(
                        name: "FK_registry_host_job_executer_executer_id",
                        column: x => x.executer_id,
                        principalTable: "job_executer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registry_host_executer_id",
                table: "registry_host",
                column: "executer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registry_host");

            migrationBuilder.AddColumn<string>(
                name: "registry_hosts",
                table: "job_executer",
                type: "varchar(1000) CHARACTER SET utf8mb4",
                maxLength: 1000,
                nullable: true);
        }
    }
}
