using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EduManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grades",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    class_id = table.Column<long>(type: "bigint", nullable: false),
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    attendance_score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    midterm_score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    final_score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    final_gpa = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grades", x => x.id);
                    table.CheckConstraint("ck_grades_score_range", "(attendance_score IS NULL OR attendance_score BETWEEN 0 AND 100) AND (midterm_score IS NULL OR midterm_score BETWEEN 0 AND 100) AND (final_score IS NULL OR final_score BETWEEN 0 AND 100)");
                    table.ForeignKey(
                        name: "FK_grades_classes_class_id",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grades_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grades_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_grades_class_id",
                table: "grades",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "idx_grades_student_id",
                table: "grades",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_grades_subject_id",
                table: "grades",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "uq_grade_student_class",
                table: "grades",
                columns: new[] { "student_id", "class_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grades");
        }
    }
}
