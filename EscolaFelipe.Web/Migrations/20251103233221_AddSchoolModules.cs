using Microsoft.EntityFrameworkCore.Migrations;

namespace EscolaFelipe.Web.Migrations
{
    public partial class AddSchoolModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "Enrollments",
                newName: "EnrollmentDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnrollmentDate",
                table: "Enrollments",
                newName: "RegistrationDate");
        }
    }
}
