using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncMessagesExample.Api.StateMachine.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncMessagesState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    InstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartSyncing = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndSyncing = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MessagesStates = table.Column<string>(type: "text", nullable: true),
                    FailedReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncMessagesState", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncMessagesState");
        }
    }
}
