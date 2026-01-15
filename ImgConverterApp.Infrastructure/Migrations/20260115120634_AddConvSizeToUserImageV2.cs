using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImgConverterApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConvSizeToUserImageV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ConvertedSizeInBytes",
                table: "UserImages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedSizeInBytes",
                table: "UserImages");
        }
    }
}
