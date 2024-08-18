using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Medias",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Medias");
        }
    }
}
