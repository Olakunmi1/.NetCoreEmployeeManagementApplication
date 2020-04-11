using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.Migrations
{
    public partial class AlterEmployeeSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "James_M@gmail.com");

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Deparment", "Email", "Name" },
                values: new object[] { 3, 1, "regina_a@gmail.com", "Regina" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Deparment", "Email", "Name" },
                values: new object[] { 2, 2, "john@gmail.com", "John" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "JamesM@gmail.com");
        }
    }
}
