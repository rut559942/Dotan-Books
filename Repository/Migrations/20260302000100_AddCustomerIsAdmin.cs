using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Repository.Migrations
{
    [DbContext(typeof(StoreContext))]
    [Migration("20260302000100_AddCustomerIsAdmin")]
    public partial class AddCustomerIsAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Customers', 'IsAdmin') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers]
    ADD [IsAdmin] bit NOT NULL CONSTRAINT [DF_Customers_IsAdmin] DEFAULT (0);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Customers', 'IsAdmin') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Customers] DROP CONSTRAINT IF EXISTS [DF_Customers_IsAdmin];
    ALTER TABLE [dbo].[Customers] DROP COLUMN [IsAdmin];
END
");
        }
    }
}
