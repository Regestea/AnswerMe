using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupAdmins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdmins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "GroupChats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RoomImage = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupChats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "GroupInvitations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserCount = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpirationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInvitations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    UserSenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupInvitationToken = table.Column<string>(type: "text", nullable: true),
                    ReplyMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OnlineStatusUsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOnlineDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineStatusUsers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateChats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    User1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateChats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RoomLastSeen",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastSeenUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomLastSeen", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    BlurHash = table.Column<string>(type: "text", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Messageid = table.Column<Guid>(type: "uuid", nullable: false),
                    Messageid1 = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_Medias_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medias_Messages_Messageid",
                        column: x => x.Messageid,
                        principalTable: "Messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medias_MessageId",
                table: "Medias",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_Messageid",
                table: "Medias",
                column: "Messageid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAdmins");

            migrationBuilder.DropTable(
                name: "GroupChats");

            migrationBuilder.DropTable(
                name: "GroupInvitations");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropTable(
                name: "OnlineStatusUsers");

            migrationBuilder.DropTable(
                name: "PrivateChats");

            migrationBuilder.DropTable(
                name: "RoomLastSeen");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
