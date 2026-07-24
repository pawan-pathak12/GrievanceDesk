# Repository Generator Prompt

## Goal

Generate ONE repository at a time.

Do NOT generate multiple repositories.

Wait for my confirmation before generating the next repository.

 If you are an AI coding agent with access to the repository (Claude Code, Codex Agent, Gemini CLI, etc.), follow look over code_geneator_agent.md
---

## Before Starting

First read this and sample code that i pass then ask me for:

1. Current project root folder.
2. Repository folder path.
3. Sample repository implementation.
4. Entity for which the repository should be generated.
---
### Example: of code is this 

## Example   
using Dapper;
using Microsoft.Extensions.Logging;
using StudentCourseManagement.Data.Database;
using StudentCourseManagement.Data.Interfaces.AcademicModule;
using StudentCourseManagement.Data.QueryResults;
using StudentCourseManagement.Domain.Commons.Pagination;
using StudentCourseManagement.Domain.Commons.Pagination.Academics;
using StudentCourseManagement.Domain.Entities.AcademicModules;

namespace StudentCourseManagement.Data.Repositories.AcademicModule
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentSysDbContext _dbContext;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(StudentSysDbContext dbContext, ILogger<StudentRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region CURD Operation

        public async Task<int> AddAsync(Student student)
        {
            const string sql = @"
                INSERT INTO Students
                (
                    StudentCode,
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    DOB,
                    IsActive,
                    Gender,
                    Address,
                    EnrollmentDate,
                    CreatedAt,
                    UpdatedAt
                )
                VALUES
                (
                    @StudentCode,
                    @FirstName,
                    @LastName,
                    @Email,
                    @PhoneNumber,
                    @DOB,
                    @IsActive,
                    @Gender,
                    @Address,
                    @EnrollmentDate,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            student.CreatedAt = student.CreatedAt == default
                ? DateTime.UtcNow
                : student.CreatedAt;

            using var connection = _dbContext.CreateConnection();

            var studentId = await connection.QuerySingleAsync<int>(sql, student);

            _logger.LogInformation(
                "Created student with Id {StudentId}",
                studentId);

            return studentId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                UPDATE Students
                SET 
                    IsActive = 0,
                    UpdatedAt = @UpdatedAt
                WHERE StudentId = @StudentId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(
                sql,
                new
                {
                    StudentId = id,
                    UpdatedAt = DateTime.UtcNow
                });

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Student with Id {StudentId} marked inactive.",
                    id);

                return true;
            }

            _logger.LogWarning(
                "Delete failed. No active student found with Id {StudentId}.",
                id);

            return false;
        }

        public async Task<PagedResult<Student>> GetAllAsync(StudentQueryParams query)
        {
            var sortBy = query.SortBy?.Trim().ToLowerInvariant();

            var orderBy = sortBy switch
            {
                "studentid" => "StudentId",
                "studentcode" => "StudentCode",
                "firstname" => "FirstName",
                "lastname" => "LastName",
                "email" => "Email",
                "phonenumber" => "PhoneNumber",
                "createdat" => "CreatedAt",
                "updatedat" => "UpdatedAt",
                "dob" => "DOB",
                "enrollmentdate" => "EnrollmentDate",
                _ => "StudentId"
            };

            var sortOrder = query.SortOrder?.Trim().ToLowerInvariant() switch
            {
                "desc" => "DESC",
                _ => "ASC"
            };

            var sql = $@"
                SELECT COUNT(*)
                FROM Students
                WHERE
                    (@StudentId IS NULL OR StudentId = @StudentId)
                    AND (@StudentCode IS NULL OR StudentCode = @StudentCode)
                    AND (@Email IS NULL OR LOWER(Email) = LOWER(@Email))
                    AND (@IsActive IS NULL OR IsActive = @IsActive)
                    AND (@DOB IS NULL OR CAST(DOB AS DATE) = CAST(@DOB AS DATE))
                    AND
                    (
                        @Search IS NULL
                        OR StudentCode LIKE '%' + @Search + '%'
                        OR FirstName LIKE '%' + @Search + '%'
                        OR LastName LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                        OR PhoneNumber LIKE '%' + @Search + '%'
                        OR Gender LIKE '%' + @Search + '%'
                        OR Address LIKE '%' + @Search + '%'
                    );

                SELECT *
                FROM Students
                WHERE
                    (@StudentId IS NULL OR StudentId = @StudentId)
                    AND (@StudentCode IS NULL OR StudentCode = @StudentCode)
                    AND (@Email IS NULL OR LOWER(Email) = LOWER(@Email))
                    AND (@IsActive IS NULL OR IsActive = @IsActive)
                    AND (@DOB IS NULL OR CAST(DOB AS DATE) = CAST(@DOB AS DATE))
                    AND
                    (
                        @Search IS NULL
                        OR StudentCode LIKE '%' + @Search + '%'
                        OR FirstName LIKE '%' + @Search + '%'
                        OR LastName LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                        OR PhoneNumber LIKE '%' + @Search + '%'
                        OR Gender LIKE '%' + @Search + '%'
                        OR Address LIKE '%' + @Search + '%'
                    )
                ORDER BY {orderBy} {sortOrder}
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using var connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@StudentId", query.StudentId);
            parameters.Add("@StudentCode", string.IsNullOrWhiteSpace(query.StudentCode)
                ? null
                : query.StudentCode.Trim());

            parameters.Add("@Email", string.IsNullOrWhiteSpace(query.Email)
                ? null
                : query.Email.Trim());

            parameters.Add("@IsActive", query.IsActive);
            parameters.Add("@DOB", query.DOB);

            parameters.Add("@Search", string.IsNullOrWhiteSpace(query.Search)
                ? null
                : query.Search.Trim());

            parameters.Add("@Page", query.Page);
            parameters.Add("@PageSize", query.PageSize);

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadSingleAsync<int>();
            var data = (await multi.ReadAsync<Student>()).ToList();

            return new PagedResult<Student>
            {
                Data = data,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT *
                FROM Students
                WHERE StudentId = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching student record with Id {StudentId}",
                id);

            return await connection.QueryFirstOrDefaultAsync<Student>(
                sql,
                new { Id = id });
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT *
                FROM Students
                WHERE Email = @Email
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching student record with email {Email}",
                email);

            return await connection.QueryFirstOrDefaultAsync<Student>(
                sql,
                new { Email = email });
        }

        public async Task<string?> GetHighestStudentCodeAsync()
        {
            const string sql = @"
                SELECT TOP 1 StudentCode
                FROM Students
                WHERE StudentCode LIKE 'STU-%'
                ORDER BY StudentCode DESC;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<string>(sql);
        }
        public async Task<bool> UpdateAsync(int id, Student student)
        {
            const string sql = @"
                UPDATE Students
                SET
                    StudentCode = @StudentCode,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    DOB = @DOB,
                    EnrollmentDate = @EnrollmentDate,
                    IsActive = @IsActive,
                    Gender = @Gender,
                    Address = @Address,
                    UpdatedAt = @UpdatedAt
                WHERE StudentId = @StudentId;";

            student.StudentId = id;
            student.UpdatedAt = DateTime.UtcNow;

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, student);

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Updated student with Id {StudentId}.",
                    student.StudentId);

                return true;
            }

            _logger.LogWarning(
                "Failed to update student record with Id {StudentId}.",
                student.StudentId);

            return false;
        }

        #endregion

        #region Student Related Queries

        public async Task<StudentEnrollmentQueryResult?> GetEnrollmentsAsync(int studentId)
        {
            const string sql = @"
                SELECT
                    e.EnrollmentId,
                    e.StudentId,
                    e.EnrollmentStatus,
                    e.IsActive,
                    e.EnrollmentDate AS EnrolledOn,
                    e.CancellationReason,
                    c.CourseId,
                    c.Title AS CourseTitle,
                    c.Code AS CourseCode,
                    CONCAT(t.FirstName, ' ', t.LastName) AS Instructor,
                    c.Credits,
                    c.StartDate,
                    c.EndDate
                FROM Enrollments e
                JOIN Courses c ON e.CourseId = c.CourseId
                LEFT JOIN Teachers t ON c.TeacherId = t.TeacherId
                WHERE e.StudentId = @StudentId;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching enrollments for student Id {StudentId}",
                studentId);

            return await connection.QuerySingleOrDefaultAsync<StudentEnrollmentQueryResult>(
                sql,
                new { StudentId = studentId });
        }

        public async Task<IEnumerable<StudentInvoiceQueryResult>> GetInvoicesAsync(int studentId)
        {
            const string sql = @"
                    SELECT
                        fa.FeeAssessmentId,
                        fa.DueDate,
                        fa.FeeAssessmentStatus,
                        fa.IsActive,
                        fa.TotalAmount AS AssessedAmount,

                        c.CourseId,
                        c.Title AS CourseTitle,
                        c.Code AS CourseCode,

                        i.Discount,
                        i.TotalAmount,
                        i.AmountPaid,
                        i.BalanceDue

                    FROM FeeAssessments fa
                    JOIN Enrollments e
                        ON fa.EnrollmentId = e.EnrollmentId
                    JOIN Courses c
                        ON e.CourseId = c.CourseId
                    JOIN Invoices i
                        ON fa.FeeAssessmentId = i.FeeAssessmentId
                    WHERE e.StudentId = @StudentId
                      AND fa.IsActive = 1
                      AND i.IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching invoices for student Id {StudentId}",
                studentId);

            return await connection.QueryAsync<StudentInvoiceQueryResult>(
                sql,
                new { StudentId = studentId });
        }

        #endregion

        #region Validation of Student

        public async Task<bool> IsStudentActiveAsync(int studentId)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM Students
                        WHERE StudentId = @StudentId
                          AND IsActive = 1
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Checking whether student is active. StudentId: {StudentId}",
                studentId);

            return await connection.ExecuteScalarAsync<bool>(
                sql,
                new { StudentId = studentId });
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM Students
                        WHERE Email = @Email
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Checking whether student email exists. Email: {Email}",
                email);

            return await connection.ExecuteScalarAsync<bool>(
                sql,
                new { Email = email });
        }

        #endregion
    }
}
 public class StudentQueryParams : QueryParams
 {
     public int? StudentId { get; set; }
     public string? StudentCode { get; set; }
     public string? Email { get; set; }
     public DateTime? DOB { get; set; }

 }
  public class QueryParams : PaginationParams
 {
     public string? Search { get; set; }
     public string? SortBy { get; set; }
     public string SortOrder { get; set; } = "asc";
     public bool? IsActive { get; set; }
 }
  public class QueryParams : PaginationParams
 {
     public string? Search { get; set; }
     public string? SortBy { get; set; }
     public string SortOrder { get; set; } = "asc";
     public bool? IsActive { get; set; }
 }
--


---


---

## Analyze

Before writing code:

- Understand the sample repository completely.
- Follow the same coding style.
- Follow the same SQL style.
- Follow the same Dapper conventions.
- Follow the same exception handling.
- Follow the same method ordering.
- Follow the same naming conventions.
- Follow dependency injection style.

Do not improve the implementation unless I ask.

Remain consistent with the existing project.

---
### Missing Information

If information required to generate the controller is missing:

- Ask for it.
- Do not guess.
- Do not invent DTOs.
- Do not invent endpoints.
- Do not invent authorization.

---
## Generate

Generate ONLY:

- Repository Interface (if needed)
- Repository Implementation
- TQueryParms
-If needed new class for joins then give me it class also keep in xxx.Data/QueryResult

Nothing else.

Do not generate:

- Services
- Controllers
- DTOs
- AutoMapper
- Entities

---

## Rules

- Generate ONE repository only.
- Match existing folder structure.
- Match namespaces.
- Match naming conventions.
- Match formatting.
- Reuse helper methods if they exist.
- Do not invent project architecture.
- Do not generate methods that are not needed.

---

## After Generation

Stop.

Wait for me to:

- Review
- Test
- Compile

Only after I say **Next**, generate the next repository.