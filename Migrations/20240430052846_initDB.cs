using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using RazorWeb.Model;

#nullable disable

namespace RazorWeb.Migrations
{
    /// <inheritdoc />
    public partial class initDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Artical_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "Nvarchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "NText", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Artical_ID);
                });


            Randomizer.Seed = new Random(8675309);
            var fackerArticle = new Faker<Article>();

            fackerArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(5, 5));
            fackerArticle.RuleFor(a => a.Created, f => f.Date.Between(new DateTime(2024, 1, 1), new DateTime(2024, 12, 12)));
            fackerArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 4));

            for (int i = 0; i < 150; i++)
            {
                Article article = fackerArticle.Generate();

                migrationBuilder.InsertData(
                        table: "Articles",
                        columns: new[] { "Title", "Created", "Content" },
                        values: new object[]
                        {
                        article.Title,
                        article.Created,
                        article.Content
                        }
                    );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
