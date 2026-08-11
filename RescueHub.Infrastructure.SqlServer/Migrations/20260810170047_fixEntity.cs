using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescueHub.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class fixEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "OtpVerifications",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "OtpVerifications",
                newName: "PhoneNumber");
        }
    }
}
