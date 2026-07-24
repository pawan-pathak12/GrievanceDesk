# Service Generator Prompt

## Goal

Generate ONE service at a time.

Wait for confirmation before generating another service.

---

# Agent Mode Instructions

> Use these instructions only if you are running as an AI coding agent with access to the entire project.

---

## Your Role

Act as a senior ASP.NET Core backend developer working on an existing production-quality codebase.

Your primary goal is **accuracy and consistency**, not speed.

Always prefer following the existing project over inventing a "better" implementation.

---

## How to Work

Read this entire prompt before generating anything.

Study:

- Existing project structure
- Existing namespaces
- Existing coding style
- Existing architecture
- Existing naming conventions
- Existing Result pattern
- Existing DTO conventions

Do not begin code generation until you fully understand the project.

---

## Agent Workflow

You are responsible for generating **ONE item at a time**.

Example:

Repository 1

↓

Stop

↓

Review your own code

↓

Check consistency

↓

Check namespaces

↓

Check dependencies

↓

Check compile-time issues

↓

Report any potential problems

↓

Wait for my confirmation

↓

Generate Repository 2

Repeat until every repository is completed.

The same workflow applies to:

- Services
- Controllers

Never generate multiple files unless explicitly instructed.

---

## Self Review

Before returning any code, perform a self-review.

Verify:

- Namespaces are correct.
- Dependency Injection matches the project.
- Method names are consistent.
- Result pattern is followed.
- Validation is present where required.
- Existing DTOs are reused.
- AutoMapper usage is correct.
- No duplicated code exists.
- No unnecessary code exists.
- Repository/service/controller responsibilities are respected.
- Code follows the sample implementation.

If you find an issue, fix it before returning the code.

---

## Do Not Guess

If required information is missing:

Stop.

Ask for the missing information.

Never invent:

- DTOs
- Repository methods
- Services
- Routes
- Authorization
- Business rules

---

## After Every Generation

Return:

### Completed

- Repository/Service/Controller generated successfully.

### Self Review

Summarize what you checked.

Example:

✓ Namespace verified

✓ Dependencies verified

✓ Result pattern followed

✓ DataAnnotations respected

✓ Existing architecture followed

✓ No assumptions made

Then stop and wait for my confirmation before continuing.

## Before Starting

Read this entire prompt first.

Analyze every example provided below before generating any code.

Do not begin code generation until you fully understand:

- Coding style
- Naming conventions
- Folder structure
- DTO conventions
- Result pattern
- Validation style

Only then ask me for:
1. Current project root folder.
2. Service folder path.
3. Sample service.(if not given)
4. Repository interface.(of service that is being generated)
5. Entity.
6. Existing DTOs (if available).


## Example Service 

using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StudentCourseManagement.Business.DTOs.AcademicModule.Students;
using StudentCourseManagement.Business.Interfaces;
using StudentCourseManagement.Business.Interfaces.Auth;
using StudentCourseManagement.Data.Interfaces.AcademicModule;
using StudentCourseManagement.Domain.Commons;
using StudentCourseManagement.Domain.Commons.Pagination.Academics;
using StudentCourseManagement.Domain.Constants;
using StudentCourseManagement.Domain.Entities.AcademicModules;
using StudentCourseManagement.Domain.Enums;

namespace StudentCourseManagement.Business.Services.AcademicModule
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public StudentService(IStudentRepository repository,
            ICurrentUserService currentUserService, IMapper mapper
             , IMemoryCache cache, ILogger<StudentService> logger)
        {
            _repository = repository;
            this._currentUserService = currentUserService;
            _mapper = mapper;
            this._cache = cache;
            this._logger = logger;
        }
        public async Task<Result> CreateAsync(CreateStudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);
            // succes , error , id 
            var emailExists = await _repository.EmailExistsAsync(student.Email);
            if (emailExists)
            {
                return Result.Failure($"Email {student.Email} already Exists");
            }
            student.IsActive = true;
            student.CreatedAt = DateTime.UtcNow;
            student.StudentCode = await GenerateNextStudentCodeAsync();

            var studentId = await _repository.AddAsync(student);

            return Result.Success(studentId);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                return Result.NotFound($"Student with ID {id} not found.");

            var isStudentActive = await _repository.IsStudentActiveAsync(id);
            if (!isStudentActive)
                return Result.Failure("Student is not active.");

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return Result.Failure("Delete failed.");
            _cache.Remove(CacheKeys.StudentById(id));
            return Result.Success("Student deleted successfully.");
        }

        public async Task<Result> GetAllAsync(StudentQueryParams query)
        {
            var pagedResult = await _repository.GetAllAsync(query);
            return Result.Success(pagedResult);
        }

        public async Task<Result> GetByIdAsync(int id)
        {
            var cacheKey = CacheKeys.StudentById(id);
            if (_cache.TryGetValue(cacheKey, out Student? cached) && cached != null)
            {
                _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
                return Result.Success(_mapper.Map<StudentResponseDto>(cached));
            }

            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                return Result.NotFound($"Student with ID {id} not found.");

            var responseDto = _mapper.Map<StudentResponseDto>(student);
            return Result.Success(responseDto);
        }

        public async Task<Result> UpdateAsync(int id, UpdateStudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);
            if (id != student.StudentId)
                return Result.Failure("Route ID and body ID do not match.");

            var studentExist = await _repository.GetByIdAsync(id);
            if (studentExist == null)
                return Result.NotFound($"Student with ID {id} not found.");

            var isStudentActive = await _repository.IsStudentActiveAsync(id);
            if (!isStudentActive)
                return Result.Failure("Student is not active.");

            var updated = await _repository.UpdateAsync(studentExist.StudentId, student);
            if (!updated)
                return Result.Failure("Update failed.");
            _cache.Remove(CacheKeys.StudentById(id));
            return Result.Success(null, "Student updated successfully.");
        }

        public async Task<Result> GetStudentOwnEnrollments(int studentId)
        {
            var studentGuard = EnforceStudentOwnership(studentId);
            if (studentGuard != null)
            {
                return studentGuard;
            }

            // return student enrollment record accessed by admin and staff 
            var enrollments = await _repository.GetEnrollmentsAsync(studentId);

            return Result.Success(enrollments);
        }

        public async Task<Result> GetStudentInvoices(int studentId)
        {
            var currentUserRole = _currentUserService.GetRole();

            if (currentUserRole == UserRole.Student.ToString())
            {
                if (_currentUserService.GetUserId() != studentId)
                {
                    return Result.Forbidden(
                        "You are not authorized to access this student's enrollments.");
                }
                // return only own student invoice data access by only student 

                return Result.Success(
                    await _repository.GetInvoicesAsync(studentId));
            }
            // return student invoices record accessed by admin and staff 
            var invoices = await _repository.GetInvoicesAsync(studentId);

            return Result.Success(invoices);
        }

        #region Helper
        private async Task<string> GenerateNextStudentCodeAsync()
        {
            var highestStudentCode = await _repository.GetHighestStudentCodeAsync();

            if (string.IsNullOrWhiteSpace(highestStudentCode))
            {
                return "STU-0001";
            }

            var numberPart = highestStudentCode.Replace("STU-", "");

            if (!int.TryParse(numberPart, out var currentNumber))
            {
                return "STU-0001";
            }

            var nextNumber = currentNumber + 1;

            return $"STU-{nextNumber:D4}";
        }
        private Result? EnforceStudentOwnership(int studentId)
        {
            if (_currentUserService.GetRole() == UserRole.Student.ToString()
                && _currentUserService.GetUserId() != studentId)
            {
                return Result.Forbidden("You are not authorized to access this student's data.");
            }
            return null; // authorized
        }
        #endregion
    }
}

---
---

## Example DTO
 public class CreateStudentDto
 {
     [Required]
     [StringLength(50)]
     public string FirstName { get; set; } = string.Empty;
     [Required]
     [StringLength(50)]
     public string LastName { get; set; } = string.Empty;

     [Required]
     [EmailAddress]
     [StringLength(100)]
     public string Email { get; set; } = string.Empty;
     [Required]
     [Range(1000000000, 9999999999)]
     public long PhoneNumber { get; set; }
     public DateTime? DOB { get; set; }

     [Required]
     [RegularExpression("^(Male|Female|Other)$")]
     public string Gender { get; set; } = string.Empty;

     [Required]
     [StringLength(200)]
     public string Address { get; set; } = string.Empty;

 }

 ---
 ## Example Result
 public class Result
{
    public bool IsSuccess { get; }
    public int? Id { get; set; }
    public object? Data { get; set; }
    public List<string> Errors { get; }
    public int StatusCode { get; }
    public string? Message { get; set; }

    private Result(bool isSuccess, object? data, List<string>? errors,
        int statusCode, int? id, string? message = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors ?? new List<string>();
        StatusCode = statusCode;
        Id = id;
        Message = message;
    }

    // ── Success Factories ──

    public static Result Success(object? data = null, int statusCode = 200)
        => new(true, data, null, statusCode, 0);

    public static Result Success()
        => new(true, null, null, 200, 0);

    public static Result Success(int id)
        => new(true, null, null, 200, id);

    public static Result Success(string message)
        => new(true, null, null, 200, 0, message);

    public static Result Success(object data, string message)
        => new(true, data, null, 200, 0, message);

    // ── Failure Factories ──

    public static Result Failure(string error, int statusCode = 400)
        => new(false, null, new List<string> { error }, statusCode, 0);

    public static Result Failure(List<string> errors, int statusCode = 400)
        => new(false, null, errors, statusCode, 0);

    public static Result NotFound(string error)
        => new(false, null, new List<string> { error }, 404, 0);

    public static Result Unauthorized(string error)
        => new(false, null, new List<string> { error }, 401, 0);

    public static Result Forbidden(string error)
        => new(false, null, new List<string> { error }, 403, 0);

    // ── Backward compatibility ──
    // Keep single Error property for existing code that reads it
    [System.Text.Json.Serialization.JsonIgnore]
    public string? Error => Errors.Count > 0 ? Errors[0] : null;
}

---
---

## Analyze

Understand the sample service.

Reuse:

- Coding style
- Method ordering
- Validation style
- Exception handling
- Result pattern
- AutoMapper usage
- Dependency Injection

---

## Generate

Generate ONLY:

- Service Interface
- Service Implementation

If DTOs do not exist, also generate:

- Create DTO
- Update DTO
- Response DTO

If AutoMapper Profile does not exist, generate it.

Do NOT generate:

- Repository
- Controller
- Entity
For every Command DTO (any request DTO except Response DTOs):

- Add appropriate DataAnnotations validation attributes.
- Use validation attributes consistent with the project.
- Apply attributes such as Required, StringLength/MaxLength, Range, EmailAddress, Phone(only if data type is string ), Url, RegularExpression, etc., where appropriate.
- Do not add validation attributes to Response DTOs.
---

## Rules

Generate one service only.

Reuse existing DTOs whenever possible.

If DTOs already exist, do not regenerate them.

If new DTOs are required, generate only the missing ones.

Keep namespaces identical to the project.

### Business Logic

Keep business logic inside the Service.

Do not move business rules into the Controller or Repository.

Repositories should only handle data access.

Controllers should only handle HTTP concerns.

### Result Pattern

All service methods must return the project's existing `Result` class.

Do not create a new response wrapper.

Do not return anonymous objects.

Do not return tuples.

Do not throw exceptions for expected business validation failures.

Always use the appropriate factory methods from the existing `Result` class.

Examples:

Success:
- `Result.Success()`
- `Result.Success(id)`
- `Result.Success(data)`
- `Result.Success(message)`
- `Result.Success(data, message)`

Failure:
- `Result.Failure(error)`
- `Result.Failure(errors)`
- `Result.NotFound(error)`
- `Result.Unauthorized(error)`
- `Result.Forbidden(error)`

Use the most appropriate factory method for the situation.

Only throw exceptions for unexpected system errors (database connectivity, unexpected runtime failures, etc.). Handle expected business cases by returning a `Result`.

Keep error messages clear, concise, and user-friendly.

Ensure all service methods consistently follow this Result pattern.
### DTO Validation Rules

Whenever generating Create or Update DTOs:

- Always include DataAnnotations validation attributes.
- Validate required fields.
- Validate string lengths.
- Validate numeric ranges.
- Validate email, phone, URL, and other special formats when applicable.
- Use nullable reference types correctly.
- Do not add unnecessary validation.
- Do not add DataAnnotations to Response DTOs.
- Keep validation consistent with the entity and business rules.
---

### Missing Information

If information required to generate the service is missing:

- Ask for it first.
- Do not guess.
- Do not invent entities.
- Do not invent repository methods.
- Do not invent DTOs unless instructed.

Do not modify the repository interface.

Only use methods that already exist.

If a repository method is missing, list it separately instead of assuming it exists.

Reuse AutoMapper wherever appropriate.

Avoid manual property mapping unless the sample project follows that approach.

Output Order

1. Missing Repository Methods (if any)
2. DTOs (only if needed)
3. Mapping Profile (only if needed)
4. Interface
5. Service

---

## After Generation

Stop.

Do not generate another service.

Wait for me to:

- Review
- Compile
- Test

Only after I type "Next" should you continue.