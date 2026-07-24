# Controller Generator Prompt

## Goal

Generate ONE controller at a time.

Wait for confirmation before generating the next controller.

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


## Before Starting(if agen)
Read this entire prompt first.
Analyze every example provided below before generating any code.

1. Current project root folder.
2. Controller folder path.
3. Sample controller.
4. Service Interface.
5. DTOs used by the service.

---

## Controller Example 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StudentCourseManagement.API.Contracts;
using StudentCourseManagement.Business.DTOs.Users;
using StudentCourseManagement.Business.Interfaces.Auth;
using StudentCourseManagement.Domain.Commons.Pagination;

namespace StudentCourseManagement.API.Controllers.Identities
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general-policy")]
    [Authorize(Roles = "Admin")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateNonStudentUserAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParams query)
        {
            var result = await _userService.GetAllUsersAsync(query);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmailAsync(email);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var ownUserIdClaim = User.FindFirst("sub")?.Value;

            if (!int.TryParse(ownUserIdClaim, out var ownUserId))
                return StatusCode(401, new ErrorResponse(false, 401, null, new List<string> { "Invalid user id in token" }));

            if (ownUserId == id)
                return StatusCode(400, new ErrorResponse(false, 400, null, new List<string> { "You cannot delete your own account" }));

            var result = await _userService.DeleteUserAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto updateUser)
        {
            var result = await _userService.UpdateUserAsync(updateUser);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

    }
}

--

## Analyze

Understand the sample controller.

Follow:

- Route conventions
- Authorization conventions
- Response style
- Validation style
- Dependency Injection
- Swagger attributes (if used)
- Action ordering
- Naming conventions
- ApiResponse / ErrorResponse response pattern
- Authorize attribute usage
- Rate limiting attributes
- Route conventions
- API response format
- Dependency Injection
- Existing controller conventions
---

## Generate

Generate ONLY:

- Controller

Do not generate:

- Repository
- Service
- Entity
- DTO
- Mapping Profile

Assume those already exist.

---

## Rules

Generate one controller only.

Match the sample controller exactly.

Do not invent endpoints.

Do not invent routes.

Do not invent authorization rules.

Reuse existing DTOs.

Keep namespaces identical.

---

### Authorization Rules

Always apply authorization following the sample controller.

- Use `[Authorize]` or `[Authorize(Roles = "...")]` where appropriate.
- Never leave protected endpoints without authorization.
- Follow the project's authorization conventions.
- If authorization requirements are unclear, ask instead of guessing.
- Do not invent new roles.


### API Response Rules

All controllers must use the project's standard response wrappers.

Success responses must return:

```csharp
return Ok(new ApiResponse(true, result.StatusCode, result.Data));

Failure responses must return:

return StatusCode(
    result.StatusCode,
    new ErrorResponse(
        false,
        result.StatusCode,
        null,
        result.Errors));

Never return:

Ok(dto)
BadRequest(...)
NotFound(...)
Unauthorized(...)
Anonymous objects

unless the sample project explicitly does so.

Always follow the existing ApiResponse/ErrorResponse pattern.


---

### Controller Responsibilities

Controllers should only:

- Receive HTTP requests
- Validate route/body binding
- Call the service
- Convert Result into ApiResponse/ErrorResponse

Controllers should NOT contain:

- Business logic
- SQL
- Entity manipulation
- Complex validation
## Output

Generate:

- Controller

Nothing else.

---
### Route Rules

Follow the sample exactly.

Reuse:

- Route naming
- Route prefixes
- Action ordering
- Route parameters
- Query parameters

Do not invent REST endpoints.
---

### Service Rules

Only call methods that exist on the provided service interface.

Do not invent service methods.

If a required service method is missing, list it separately instead of assuming it exists.
--

### Missing Information

If information required to generate the controller is missing:

- Ask for it.
- Do not guess.
- Do not invent DTOs.
- Do not invent endpoints.
- Do not invent authorization.
--

## Output Order

1. Missing service methods (if any)
2. Controller
--
## After Generation

Stop.

Wait for me to compile and test.

Generate the next controller only after I type **Next**.