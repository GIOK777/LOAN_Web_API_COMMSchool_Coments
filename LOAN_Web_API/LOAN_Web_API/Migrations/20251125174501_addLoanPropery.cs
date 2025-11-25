using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOAN_Web_API.Migrations
{
    /// <inheritdoc />
    public partial class addLoanPropery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "Loans",
                newName: "PeriodInMonths");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PeriodInMonths",
                table: "Loans",
                newName: "Period");
        }
    }
}
