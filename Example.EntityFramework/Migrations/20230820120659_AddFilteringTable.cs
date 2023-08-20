using Microsoft.EntityFrameworkCore.Migrations;

namespace ExampleEntityFramework.Migrations
{
    public partial class AddFilteringTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilterResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilterHash = table.Column<string>(type: "TEXT", nullable: true),
                    ResultId = table.Column<long>(type: "INTEGER", nullable: false),
                    ExpirationTimeStamp = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterResults", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilterResults");
        }
    }
}
