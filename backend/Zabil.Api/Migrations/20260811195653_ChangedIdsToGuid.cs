using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Zabil.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangedIdsToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Dev data only: existing int Ids can't be cast to uuid, so wipe the affected
            // tables rather than attempting a data-preserving remap.
            migrationBuilder.Sql(
                "TRUNCATE TABLE \"FbMedia\", \"FbPosts\", \"FbSyncLogs\", \"UserIdentities\", \"UserPostMedia\", \"UserPosts\", \"Users\" RESTART IDENTITY CASCADE;");

            // FK columns must match their principal column's type at all times, so drop the
            // cross-table constraints before changing any types and re-add them afterward.
            migrationBuilder.Sql("ALTER TABLE \"FbMedia\" DROP CONSTRAINT \"FK_FbMedia_FbPosts_PostId\";");
            migrationBuilder.Sql("ALTER TABLE \"UserIdentities\" DROP CONSTRAINT \"FK_UserIdentities_Users_UserId\";");
            migrationBuilder.Sql("ALTER TABLE \"UserPosts\" DROP CONSTRAINT \"FK_UserPosts_Users_UserId\";");
            migrationBuilder.Sql("ALTER TABLE \"UserPostMedia\" DROP CONSTRAINT \"FK_UserPostMedia_UserPosts_PostId\";");

            migrationBuilder.Sql("ALTER TABLE \"Users\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"Users\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"UserPosts\" ALTER COLUMN \"UserId\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"UserPosts\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"UserPosts\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"UserPostMedia\" ALTER COLUMN \"PostId\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"UserPostMedia\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"UserPostMedia\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"UserIdentities\" ALTER COLUMN \"UserId\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"UserIdentities\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"UserIdentities\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"FbSyncLogs\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"FbSyncLogs\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"FbPosts\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"FbPosts\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"FbMedia\" ALTER COLUMN \"PostId\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"FbMedia\" ALTER COLUMN \"Id\" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE \"FbMedia\" ALTER COLUMN \"Id\" TYPE uuid USING NULL::uuid;");

            migrationBuilder.Sql(
                "ALTER TABLE \"FbMedia\" ADD CONSTRAINT \"FK_FbMedia_FbPosts_PostId\" FOREIGN KEY (\"PostId\") REFERENCES \"FbPosts\" (\"Id\") ON DELETE CASCADE;");
            migrationBuilder.Sql(
                "ALTER TABLE \"UserIdentities\" ADD CONSTRAINT \"FK_UserIdentities_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Users\" (\"Id\") ON DELETE CASCADE;");
            migrationBuilder.Sql(
                "ALTER TABLE \"UserPosts\" ADD CONSTRAINT \"FK_UserPosts_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Users\" (\"Id\") ON DELETE CASCADE;");
            migrationBuilder.Sql(
                "ALTER TABLE \"UserPostMedia\" ADD CONSTRAINT \"FK_UserPostMedia_UserPosts_PostId\" FOREIGN KEY (\"PostId\") REFERENCES \"UserPosts\" (\"Id\") ON DELETE CASCADE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserPosts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserPosts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserPostMedia",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserPostMedia",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserIdentities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserIdentities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FbSyncLogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FbPosts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "FbMedia",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FbMedia",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
