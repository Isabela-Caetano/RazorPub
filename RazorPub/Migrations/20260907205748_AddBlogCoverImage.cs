using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPub.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "BlogPosts",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "BlogPosts");
        }
    }
}
