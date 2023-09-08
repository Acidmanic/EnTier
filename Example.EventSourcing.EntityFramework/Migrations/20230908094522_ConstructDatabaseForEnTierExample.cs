using Microsoft.EntityFrameworkCore.Migrations;

namespace Example.EventSourcing.EntityFramework.Migrations
{
    public partial class ConstructDatabaseForEnTierExample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostStgId = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostsEvents",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StreamId = table.Column<long>(type: "INTEGER", nullable: false),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    SerializedValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsEvents", x => x.EventId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "PostsEvents");
        }
    }
}
