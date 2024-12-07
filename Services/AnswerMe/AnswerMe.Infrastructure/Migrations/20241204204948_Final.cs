using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Messageid1",
                table: "Medias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Messageid1",
                table: "Medias",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
