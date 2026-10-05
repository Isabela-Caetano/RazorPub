using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPub.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogContentImageAndSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentImagePath",
                table: "BlogPosts",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "BlogPosts",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentImagePath",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "BlogPosts");
        }
    }
}
