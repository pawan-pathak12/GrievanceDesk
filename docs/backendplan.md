# Backend Implementation Plan — GrievanceDesk

---

## 1. Current Backend Architecture

### Solution Structure

```
SmartCollegeComplaintPortal/
├── SmartCollegeComplaintPortal.sln
├── global.json                          (.NET 9)
└── src/
    ├── SmartCollegeComplaintPortal.API/           → Controllers, Middleware, Extensions
    ├── SmartCollegeComplaintPortal.Business/      → Services, DTOs, Mapping, Interfaces
    ├── SmartCollegeComplaintPortal.Data/          → Repositories, DbContext, QueryResults
    └── SmartCollegeComplaintPortal.Domain/        → Entities, Enums, Constants, Commons
```

### Existing Files Map

#### API Layer
```
API/
├── Controllers/
│   ├── AuthController.cs              ✅ EXISTS (Register, Login, Me, ChangePassword)
│   ├── UserController.cs              ✅ EXISTS (Admin CRUD for users)
│   └── SwaggerLoginController.cs      ✅ EXISTS (Swagger auth gate)
├── Contracts/
│   └── ApiResponse.cs                 ✅ EXISTS (ApiResponse + ErrorResponse records)
├── Extensions/
│   ├── CorsExtensions.cs              ✅ EXISTS
│   ├── JwtSetupExtensions.cs          ✅ EXISTS
│   ├── MiddlewareExtensions.cs        ✅ EXISTS
│   ├── RatelimitingExtensions.cs      ✅ EXISTS
│   ├── SeriloggerExtensions.cs        ✅ EXISTS
│   └── SwaggerAuthExtensions.cs       ✅ EXISTS
├── Middlewares/
│   ├── ExceptionMiddleware.cs         ✅ EXISTS
│   └── SwaggerAuthMiddleware.cs       ✅ EXISTS
├── Helpers/                           (empty)
├── Program.cs                         ✅ EXISTS
├── appsettings.json                   ✅ EXISTS
└── appsettings.Development.json       ✅ EXISTS
```

#### Business Layer
```
Business/
├── Services/Auths/
│   ├── AuthService.cs                 ✅ EXISTS (will MODIFY)
│   ├── TokenService.cs                ✅ EXISTS
│   ├── CurrentUserService.cs          ✅ EXISTS
│   ├── UserService.cs                 ✅ EXISTS
│   ├── OtpService.cs                  ✅ EXISTS
│   ├── EmailService.cs                ✅ EXISTS
│   └── AzureBlobStorageService.cs     ✅ EXISTS
├── Interfaces/Auths/
│   ├── IAuthService.cs                ✅ EXISTS
│   ├── ITokenService.cs               ✅ EXISTS
│   ├── ICurrentUserService.cs         ✅ EXISTS
│   ├── IUserService.cs                ✅ EXISTS
│   ├── IOtpService.cs                 ✅ EXISTS
│   ├── IEmailService.cs               ✅ EXISTS
│   └── IFileStorageService.cs         ✅ EXISTS
├── DTOs/
│   ├── Auth/                          ✅ EXISTS (LoginDto, CheckEmailDto, etc.)
│   ├── Users/                         ✅ EXISTS (CreateUserDto, UserResponseDto, etc.)
│   └── Otps/                          ✅ EXISTS
├── Mapping/
│   ├── UserProfile.cs                 ✅ EXISTS
│   └── OtpProfile.cs                  ✅ EXISTS
├── Configurations/
│   └── AzureBlobStorageOptions.cs     ✅ EXISTS
├── Constants/
│   ├── EntityTypes.cs                 ✅ EXISTS
│   └── FileValidationConstants.cs     ✅ EXISTS
└── DependencyInjection.cs             ✅ EXISTS (will MODIFY)
```

#### Data Layer
```
Data/
├── Database/
│   ├── SystemDbContext.cs             ✅ EXISTS (Dapper — IDbConnection factory)
│   └── SystemEfCoreDbContext.cs       ✅ EXISTS (EF Core — will MODIFY)
├── Interfaces/
│   ├── IUserRepository.cs            ✅ EXISTS
│   ├── IOtpRepository.cs             ✅ EXISTS
│   └── IFileRepository.cs            ✅ EXISTS
├── Repositories/
│   ├── UserRepository.cs             ✅ EXISTS
│   ├── OtpRepository.cs              ✅ EXISTS
│   └── FileRepository.cs             ✅ EXISTS
├── QueryResult/                       (empty)
└── DependencyInjection.cs             ✅ EXISTS (will MODIFY — has migration assembly bug)
```

#### Domain Layer
```
Domain/
├── Entities/Auths/
│   ├── User.cs                        ✅ EXISTS (will MODIFY — add MemberId)
│   ├── Otp.cs                         ✅ EXISTS
│   ├── RefreshToken.cs                ✅ EXISTS
│   └── FileMetadata.cs                ✅ EXISTS
├── Enums/
│   └── UserRole.cs                    ✅ EXISTS (Admin=0, User=1)
├── Constants/
│   └── ClaimConstants.cs              ✅ EXISTS
└── Commons/Pagination/                ✅ EXISTS
```

### Known Bug

In `Data/DependencyInjection.cs` line 19:
```diff
- b => b.MigrationsAssembly("StudentCourseManagement.Data")
+ b => b.MigrationsAssembly("SmartCollegeComplaintPortal.Data")
```
This **must** be fixed or migrations will fail.

---

## 2. Naming Decision

**Keep all folder names and namespaces as `SmartCollegeComplaintPortal`.**

Only change display/branding names:
- Swagger title → "GrievanceDesk API"
- JWT Issuer → "GrievanceDesk"
- Email SenderName → "GrievanceDesk"
- Frontend title → "GrievanceDesk"

This saves ~1.5 hours and avoids build-breaking risks.

---

## 3. Complete Database Design

### ER Diagram

```mermaid
erDiagram
    Members ||--o| Users : "registers as"
    Users ||--o{ Complaints : "submits"
    Users ||--o{ ComplaintUpdates : "performs"
    Complaints ||--o{ ComplaintUpdates : "has history"
    Users ||--o{ Otps : "verifies"
    Users ||--o{ RefreshTokens : "authenticates"

    Members {
        int MemberId PK
        string FirstName
        string LastName
        string Email UK
        int Role
        bool IsRegistered
        datetime CreatedAt
        datetime UpdatedAt
    }

    Users {
        int UserId PK
        int MemberId FK_UK
        string FirstName
        string LastName
        string Email UK
        string Password
        int Role
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    Complaints {
        int Id PK
        int UserId FK
        string Title
        string Description
        int Category
        string Location
        int Status
        bool IsDuplicate
        bool IsForwarded
        bool IsEdited
        datetime CreatedAt
        datetime UpdatedAt
    }

    ComplaintUpdates {
        int Id PK
        int ComplaintId FK
        int UpdatedByUserId FK
        int UpdateType
        int OldStatus
        int NewStatus
        string NoteText
        datetime CreatedAt
    }
```

### Table: Members

```sql
CREATE TABLE Members (
    MemberId        INT IDENTITY(1,1) PRIMARY KEY,
    FirstName       NVARCHAR(100) NOT NULL,
    LastName        NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(200) NOT NULL,
    Role            INT NOT NULL DEFAULT 1,            -- 0=Admin, 1=User
    IsRegistered    BIT NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2 NULL,

    CONSTRAINT UQ_Members_Email UNIQUE (Email)
);

CREATE UNIQUE INDEX IX_Members_Email ON Members(Email);
```

### Table: Users (MODIFY Existing)

Add `MemberId` column to existing Users table:

```sql
ALTER TABLE Users ADD MemberId INT NULL;
ALTER TABLE Users ADD CONSTRAINT FK_Users_Members
    FOREIGN KEY (MemberId) REFERENCES Members(MemberId);
CREATE UNIQUE INDEX IX_Users_MemberId ON Users(MemberId);
```

### Table: Complaints

```sql
CREATE TABLE Complaints (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    Title           NVARCHAR(200) NOT NULL,
    Description     NVARCHAR(MAX) NOT NULL,
    Category        INT NOT NULL DEFAULT 0,
    Status          INT NOT NULL DEFAULT 0,            -- Pending=0
    IsDuplicate     BIT NOT NULL DEFAULT 0,
    IsEdited        BIT NOT NULL DEFAULT 0,
    EditedAt        datetime2 not null , 
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Complaints_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE INDEX IX_Complaints_UserId ON Complaints(UserId);
CREATE INDEX IX_Complaints_Status ON Complaints(Status);
CREATE INDEX IX_Complaints_Category ON Complaints(Category);
```

### Table: ComplaintUpdates

```sql
CREATE TABLE ComplaintUpdates (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ComplaintId     INT NOT NULL,
    UpdatedByUserId INT NOT NULL,
    UpdateType      INT NOT NULL,
    OldStatus       INT NULL,
    NewStatus       INT NULL,
    NoteText        NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_ComplaintUpdates_Complaints FOREIGN KEY (ComplaintId) REFERENCES Complaints(Id),
    CONSTRAINT FK_ComplaintUpdates_Users FOREIGN KEY (UpdatedByUserId) REFERENCES Users(UserId)
);

CREATE INDEX IX_ComplaintUpdates_ComplaintId ON ComplaintUpdates(ComplaintId);
```

---

## 4. All Enums

### [KEEP] `Domain/Enums/UserRole.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Enums
{
    public enum UserRole
    {
        Admin = 0,
        User = 1
    }
}
```

### [NEW] `Domain/Enums/ComplaintCategory.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Enums
{
    public enum ComplaintCategory
    {
        General = 0,
        Infrastructure = 1,
        Service = 2,
        Staff = 3,
        Safety = 4,
        Cleanliness = 5,
        Other = 6
    }
}
```

### [NEW] `Domain/Enums/ComplaintStatus.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Enums
{
    public enum ComplaintStatus
    {
        Pending = 0,
        InProgress = 1,
        Resolved = 2,
        Closed = 3,
        Irrelevant = 4
    }
}
```

### [NEW] `Domain/Enums/ComplaintUpdateType.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Enums
{
    public enum ComplaintUpdateType
    {
        Created = 0,
        Edited = 1,
        StatusChanged = 2,
        NoteAdded = 3,
        MarkedDuplicate = 4,
        MarkedForwarded = 5,
        MarkedIrrelevant = 6
    }
}
```

---

## 5. All Entities

### [NEW] `Domain/Entities/Members/Member.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Entities.Members
{
    public class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsRegistered { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
```

### [MODIFY] `Domain/Entities/Auths/User.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Entities.Auths
{
    public class User
    {
        public int UserId { get; set; }
        public int? MemberId { get; set; }                    // ← NEW: FK to Members
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
```

### [NEW] `Domain/Entities/Complaints/Complaint.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Entities.Complaints
{
    public class Complaint
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ComplaintCategory Category { get; set; } = ComplaintCategory.General;
        public string Location { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public bool IsDuplicate { get; set; } = false;
        public bool IsForwarded { get; set; } = false;
        public bool IsEdited { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
```

### [NEW] `Domain/Entities/Complaints/ComplaintUpdate.cs`
```csharp
namespace SmartCollegeComplaintPortal.Domain.Entities.Complaints
{
    public class ComplaintUpdate
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int UpdatedByUserId { get; set; }
        public ComplaintUpdateType UpdateType { get; set; }
        public ComplaintStatus? OldStatus { get; set; }
        public ComplaintStatus? NewStatus { get; set; }
        public string? NoteText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

---

## 6. Query Result Models (Dapper Read Models)

### [NEW] `Data/QueryResult/MemberResult.cs`
```csharp
public class MemberResult
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public bool IsRegistered { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### [NEW] `Data/QueryResult/ComplaintSummaryResult.cs`
```csharp
public class ComplaintSummaryResult
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Category { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Status { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsForwarded { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // From JOIN with Users
    public string SubmitterFirstName { get; set; } = string.Empty;
    public string SubmitterLastName { get; set; } = string.Empty;
    public string SubmitterEmail { get; set; } = string.Empty;
}
```

### [NEW] `Data/QueryResult/ComplaintDetailResult.cs`
```csharp
public class ComplaintDetailResult
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Category { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Status { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsForwarded { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // From JOIN
    public string SubmitterFirstName { get; set; } = string.Empty;
    public string SubmitterLastName { get; set; } = string.Empty;
    public string SubmitterEmail { get; set; } = string.Empty;
}
```

### [NEW] `Data/QueryResult/ComplaintUpdateResult.cs`
```csharp
public class ComplaintUpdateResult
{
    public int Id { get; set; }
    public int ComplaintId { get; set; }
    public int UpdatedByUserId { get; set; }
    public int UpdateType { get; set; }
    public int? OldStatus { get; set; }
    public int? NewStatus { get; set; }
    public string? NoteText { get; set; }
    public DateTime CreatedAt { get; set; }

    // From JOIN
    public string UpdatedByFirstName { get; set; } = string.Empty;
    public string UpdatedByLastName { get; set; } = string.Empty;
}
```

### [NEW] `Data/QueryResult/DashboardStatsResult.cs`
```csharp
public class DashboardStatsResult
{
    public int TotalComplaints { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Irrelevant { get; set; }
}

public class AdminDashboardStatsResult : DashboardStatsResult
{
    public int TotalMembers { get; set; }
    public int RegisteredMembers { get; set; }
    public int TodayComplaints { get; set; }
}
```

---

## 7. All Repository Interfaces & Methods

### [NEW] `Data/Interfaces/IMemberRepository.cs`

```csharp
public interface IMemberRepository
{
    // CREATE
    Task<int> CreateAsync(Member member);

    // READ
    Task<MemberResult?> GetByIdAsync(int memberId);
    Task<MemberResult?> GetByEmailAsync(string email);
    Task<IEnumerable<MemberResult>> GetAllAsync(string? search, int? role);

    // UPDATE
    Task<bool> UpdateAsync(int memberId, string firstName, string lastName,
                           string email, int role, DateTime updatedAt);
    Task<bool> MarkAsRegisteredAsync(int memberId, DateTime updatedAt);

    // DELETE
    Task<bool> DeleteAsync(int memberId);

    // CHECK
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> IsRegisteredAsync(string email);
}
```

**Dapper Queries:**

| Method | SQL |
|---|---|
| `CreateAsync` | EF Core `DbContext.Members.Add()` |
| `GetByIdAsync` | `SELECT * FROM Members WHERE MemberId = @MemberId` |
| `GetByEmailAsync` | `SELECT * FROM Members WHERE Email = @Email` |
| `GetAllAsync` | `SELECT * FROM Members WHERE (@Search IS NULL OR FirstName LIKE @Search OR LastName LIKE @Search OR Email LIKE @Search) AND (@Role IS NULL OR Role = @Role) ORDER BY CreatedAt DESC` |
| `UpdateAsync` | `UPDATE Members SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Role=@Role, UpdatedAt=@UpdatedAt WHERE MemberId=@MemberId` |
| `MarkAsRegisteredAsync` | `UPDATE Members SET IsRegistered = 1, UpdatedAt = @UpdatedAt WHERE MemberId = @MemberId` |
| `DeleteAsync` | `DELETE FROM Members WHERE MemberId = @MemberId AND IsRegistered = 0` |
| `ExistsByEmailAsync` | `SELECT COUNT(1) FROM Members WHERE Email = @Email` |
| `IsRegisteredAsync` | `SELECT IsRegistered FROM Members WHERE Email = @Email` |

---

### [NEW] `Data/Interfaces/IComplaintRepository.cs`

```csharp
public interface IComplaintRepository
{
    // ──── COMPLAINT CRUD ────

    // CREATE
    Task<int> CreateAsync(Complaint complaint);

    // READ - User
    Task<IEnumerable<ComplaintSummaryResult>> GetByUserIdAsync(int userId);
    Task<ComplaintDetailResult?> GetByIdAsync(int complaintId);

    // READ - Admin
    Task<IEnumerable<ComplaintSummaryResult>> GetAllAsync(
        int? status, int? category, string? search);
    Task<IEnumerable<ComplaintSummaryResult>> GetIrrelevantAsync();
    Task<IEnumerable<ComplaintSummaryResult>> GetIrrelevantByUserIdAsync(int userId);

    // UPDATE
    Task<bool> UpdateAsync(int complaintId, string title, string description,
                           int category, string location, DateTime updatedAt);
    Task<bool> UpdateStatusAsync(int complaintId, int newStatus, DateTime updatedAt);
    Task<bool> UpdateDuplicateFlagAsync(int complaintId, bool isDuplicate, DateTime updatedAt);
    Task<bool> UpdateForwardedFlagAsync(int complaintId, bool isForwarded, DateTime updatedAt);
    Task<bool> MarkAsEditedAsync(int complaintId, DateTime updatedAt);

    // DELETE
    Task<bool> DeleteAsync(int complaintId);

    // CHECK
    Task<bool> ExistsAsync(int complaintId);
    Task<int?> GetOwnerUserIdAsync(int complaintId);
    Task<int?> GetStatusAsync(int complaintId);

    // ──── COMPLAINT UPDATES (History) ────

    // CREATE
    Task<int> CreateUpdateAsync(ComplaintUpdate update);

    // READ
    Task<IEnumerable<ComplaintUpdateResult>> GetUpdatesByComplaintIdAsync(int complaintId);

    // ──── DASHBOARD ────

    Task<DashboardStatsResult> GetUserDashboardStatsAsync(int userId);
    Task<AdminDashboardStatsResult> GetAdminDashboardStatsAsync();
}
```

**Key Dapper Queries:**

| Method | SQL |
|---|---|
| `CreateAsync` | EF Core `DbContext.Complaints.Add()` |
| `GetByUserIdAsync` | `SELECT c.*, u.FirstName AS SubmitterFirstName, u.LastName AS SubmitterLastName, u.Email AS SubmitterEmail FROM Complaints c JOIN Users u ON c.UserId = u.UserId WHERE c.UserId = @UserId AND c.Status != 4 ORDER BY c.CreatedAt DESC` |
| `GetByIdAsync` | `SELECT c.*, u.FirstName AS SubmitterFirstName, u.LastName AS SubmitterLastName, u.Email AS SubmitterEmail FROM Complaints c JOIN Users u ON c.UserId = u.UserId WHERE c.Id = @Id` |
| `GetAllAsync` | `SELECT c.*, u.FirstName ..., u.LastName ..., u.Email ... FROM Complaints c JOIN Users u ON c.UserId = u.UserId WHERE (@Status IS NULL OR c.Status = @Status) AND (@Category IS NULL OR c.Category = @Category) AND (@Search IS NULL OR c.Title LIKE '%' + @Search + '%') AND c.Status != 4 ORDER BY c.CreatedAt DESC` |
| `GetIrrelevantAsync` | `SELECT ... WHERE c.Status = 4 ORDER BY c.UpdatedAt DESC` |
| `UpdateStatusAsync` | `UPDATE Complaints SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id` |
| `DeleteAsync` | `DELETE FROM Complaints WHERE Id = @Id AND Status = 0` |
| `GetOwnerUserIdAsync` | `SELECT UserId FROM Complaints WHERE Id = @Id` |
| `GetStatusAsync` | `SELECT Status FROM Complaints WHERE Id = @Id` |
| `CreateUpdateAsync` | EF Core `DbContext.ComplaintUpdates.Add()` |
| `GetUpdatesByComplaintIdAsync` | `SELECT cu.*, u.FirstName AS UpdatedByFirstName, u.LastName AS UpdatedByLastName FROM ComplaintUpdates cu JOIN Users u ON cu.UpdatedByUserId = u.UserId WHERE cu.ComplaintId = @ComplaintId ORDER BY cu.CreatedAt ASC` |
| `GetUserDashboardStatsAsync` | `SELECT COUNT(*) as TotalComplaints, SUM(CASE WHEN Status=0 THEN 1 ELSE 0 END) as Pending, SUM(CASE WHEN Status=1 THEN 1 ELSE 0 END) as InProgress, SUM(CASE WHEN Status=2 THEN 1 ELSE 0 END) as Resolved, SUM(CASE WHEN Status=3 THEN 1 ELSE 0 END) as Closed, SUM(CASE WHEN Status=4 THEN 1 ELSE 0 END) as Irrelevant FROM Complaints WHERE UserId = @UserId` |
| `GetAdminDashboardStatsAsync` | Same as above but without WHERE UserId filter + `SELECT COUNT(*) FROM Members` + `SELECT COUNT(*) FROM Members WHERE IsRegistered = 1` + `SELECT COUNT(*) FROM Complaints WHERE CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE)` |

---

## 8. All Service Interfaces & Methods

### [NEW] `Business/Interfaces/Members/IMemberService.cs`

```csharp
public interface IMemberService
{
    Task<ServiceResult> CreateAsync(AddMemberRequest request);
    Task<ServiceResult> GetByIdAsync(int memberId);
    Task<ServiceResult> GetAllAsync(string? search, int? role);
    Task<ServiceResult> UpdateAsync(int memberId, UpdateMemberRequest request);
    Task<ServiceResult> DeleteAsync(int memberId);
}
```

**Service Logic:**

| Method | Business Rules |
|---|---|
| `CreateAsync` | Check email doesn't already exist in Members. Create member. |
| `GetByIdAsync` | Return member details. 404 if not found. |
| `GetAllAsync` | Return all members with optional search/role filter. |
| `UpdateAsync` | Check member exists. If email changed, check new email not taken. Update. |
| `DeleteAsync` | Check member exists. Check `IsRegistered == false`. If registered, reject delete. |

---

### [NEW] `Business/Interfaces/Complaints/IComplaintService.cs`

```csharp
public interface IComplaintService
{
    // ──── User Operations ────
    Task<ServiceResult> CreateAsync(CreateComplaintRequest request);
    Task<ServiceResult> GetMyComplaintsAsync();
    Task<ServiceResult> GetMyComplaintByIdAsync(int complaintId);
    Task<ServiceResult> UpdateAsync(int complaintId, UpdateComplaintRequest request);
    Task<ServiceResult> DeleteAsync(int complaintId);
    Task<ServiceResult> GetMyIrrelevantAsync();

    // ──── Admin Operations ────
    Task<ServiceResult> GetAllAsync(int? status, int? category, string? search);
    Task<ServiceResult> GetComplaintByIdAsync(int complaintId);
    Task<ServiceResult> UpdateStatusAsync(int complaintId, UpdateStatusRequest request);
    Task<ServiceResult> AddNoteAsync(int complaintId, AddNoteRequest request);
    Task<ServiceResult> MarkDuplicateAsync(int complaintId, MarkDuplicateRequest request);
    Task<ServiceResult> MarkForwardedAsync(int complaintId);
    Task<ServiceResult> MarkIrrelevantAsync(int complaintId, MarkIrrelevantRequest request);
    Task<ServiceResult> GetAllIrrelevantAsync();

    // ──── Dashboard ────
    Task<ServiceResult> GetUserDashboardAsync();
    Task<ServiceResult> GetAdminDashboardAsync();
}
```

**Service Logic — User Operations:**

| Method | Business Rules |
|---|---|
| `CreateAsync` | 1. Get current user ID from JWT. 2. **Validate content** (blocked words check on Title + Description). 3. Create Complaint with Status=Pending. 4. Create ComplaintUpdate (Type=Created). 5. Return complaint ID. |
| `GetMyComplaintsAsync` | Get current user ID. Return complaints where UserId matches. Exclude Irrelevant (status != 4). |
| `GetMyComplaintByIdAsync` | Get current user ID. Get complaint by ID. **Verify ownership** (complaint.UserId == currentUserId). Include complaint updates (history). |
| `UpdateAsync` | 1. Get current user ID. 2. **Verify ownership**. 3. **Check status == Pending** (reject if not). 4. **Validate content** (blocked words). 5. Update complaint. 6. Mark `IsEdited = true`. 7. Create ComplaintUpdate (Type=Edited). |
| `DeleteAsync` | 1. Get current user ID. 2. **Verify ownership**. 3. **Check status == Pending** (reject if not). 4. Delete complaint. |
| `GetMyIrrelevantAsync` | Get current user ID. Return complaints where UserId matches AND Status = Irrelevant. |

**Service Logic — Admin Operations:**

| Method | Business Rules |
|---|---|
| `GetAllAsync` | Return all complaints with optional filters. Exclude Irrelevant unless explicitly filtered. |
| `GetComplaintByIdAsync` | Get complaint by ID (no ownership check — admin can see all). Include complaint updates. |
| `UpdateStatusAsync` | 1. Get complaint. 2. Validate status transition. 3. Update status. 4. Create ComplaintUpdate (Type=StatusChanged, OldStatus, NewStatus). 5. **Send email notification** to complaint owner. |
| `AddNoteAsync` | 1. Get complaint. 2. Create ComplaintUpdate (Type=NoteAdded, NoteText). 3. Optionally send email. |
| `MarkDuplicateAsync` | 1. Get complaint. 2. Set `IsDuplicate = true/false`. 3. Create ComplaintUpdate (Type=MarkedDuplicate). |
| `MarkForwardedAsync` | 1. Get complaint. 2. Set `IsForwarded = true`. 3. Create ComplaintUpdate (Type=MarkedForwarded). |
| `MarkIrrelevantAsync` | 1. Get complaint. 2. Set Status = Irrelevant. 3. Create ComplaintUpdate (Type=MarkedIrrelevant). 4. **Send email notification**. |
| `GetAllIrrelevantAsync` | Return all complaints where Status = Irrelevant. |

---

### Auth Flow Modification

#### [MODIFY] `Business/Services/Auths/AuthService.cs`

Modify the `CheckEmailAsync` method:

```
BEFORE:
  Check if email exists in Users table → if exists, reject (already registered)

AFTER:
  1. Check if email exists in Members table → if NOT found, reject ("Not authorized")
  2. Check if Members.IsRegistered == true → if true, reject ("Already registered")
  3. If found and not registered → proceed with OTP flow
```

Modify the `CreatePasswordAsync` (final registration step):

```
AFTER password is set and User record is created:
  1. Set Members.IsRegistered = true
  2. Copy FirstName, LastName, Role from Members table to Users table
  3. Set User.MemberId = Members.MemberId
```

---

## 9. All DTOs

### Member DTOs

#### [NEW] `Business/DTOs/Members/AddMemberRequest.cs`
```csharp
public class AddMemberRequest
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.User;
}
```

#### [NEW] `Business/DTOs/Members/UpdateMemberRequest.cs`
```csharp
public class UpdateMemberRequest
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}
```

#### [NEW] `Business/DTOs/Members/MemberResponse.cs`
```csharp
public class MemberResponse
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsRegistered { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

### Complaint DTOs

#### [NEW] `Business/DTOs/Complaints/CreateComplaintRequest.cs`
```csharp
public class CreateComplaintRequest
{
    [Required, StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public ComplaintCategory Category { get; set; }

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;
}
```

#### [NEW] `Business/DTOs/Complaints/UpdateComplaintRequest.cs`
```csharp
public class UpdateComplaintRequest
{
    [Required, StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public ComplaintCategory Category { get; set; }

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;
}
```

#### [NEW] `Business/DTOs/Complaints/ComplaintSummaryResponse.cs`
```csharp
public class ComplaintSummaryResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;       // enum name string
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;          // enum name string
    public bool IsDuplicate { get; set; }
    public bool IsForwarded { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string SubmitterName { get; set; } = string.Empty;   // "FirstName LastName"
    public string SubmitterEmail { get; set; } = string.Empty;
}
```

#### [NEW] `Business/DTOs/Complaints/ComplaintDetailResponse.cs`
```csharp
public class ComplaintDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsDuplicate { get; set; }
    public bool IsForwarded { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string SubmitterName { get; set; } = string.Empty;
    public string SubmitterEmail { get; set; } = string.Empty;

    // History timeline
    public List<ComplaintUpdateResponse> Updates { get; set; } = new();
}
```

#### [NEW] `Business/DTOs/Complaints/ComplaintUpdateResponse.cs`
```csharp
public class ComplaintUpdateResponse
{
    public int Id { get; set; }
    public string UpdateType { get; set; } = string.Empty;     // enum name
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? NoteText { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedByName { get; set; } = string.Empty;  // "FirstName LastName"
}
```

#### [NEW] `Business/DTOs/Complaints/UpdateStatusRequest.cs`
```csharp
public class UpdateStatusRequest
{
    [Required]
    public ComplaintStatus NewStatus { get; set; }
}
```

#### [NEW] `Business/DTOs/Complaints/AddNoteRequest.cs`
```csharp
public class AddNoteRequest
{
    [Required, StringLength(2000, MinimumLength = 3)]
    public string NoteText { get; set; } = string.Empty;
}
```

#### [NEW] `Business/DTOs/Complaints/MarkDuplicateRequest.cs`
```csharp
public class MarkDuplicateRequest
{
    public bool IsDuplicate { get; set; } = true;
    public string? NoteText { get; set; }
}
```

#### [NEW] `Business/DTOs/Complaints/MarkIrrelevantRequest.cs`
```csharp
public class MarkIrrelevantRequest
{
    [StringLength(2000)]
    public string? Reason { get; set; }
}
```

---

### Dashboard DTOs

#### [NEW] `Business/DTOs/Dashboard/UserDashboardResponse.cs`
```csharp
public class UserDashboardResponse
{
    public int TotalComplaints { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Irrelevant { get; set; }
}
```

#### [NEW] `Business/DTOs/Dashboard/AdminDashboardResponse.cs`
```csharp
public class AdminDashboardResponse
{
    public int TotalComplaints { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Irrelevant { get; set; }
    public int TotalMembers { get; set; }
    public int RegisteredMembers { get; set; }
    public int TodayComplaints { get; set; }
}
```

---

## 10. Content Validator

### [NEW] `Business/Helpers/ContentValidator.cs`

```csharp
namespace SmartCollegeComplaintPortal.Business.Helpers
{
    public static class ContentValidator
    {
        private static readonly HashSet<string> BlockedWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "fuck", "shit", "idiot", "bastard", "asshole",
            "damn", "bitch", "crap", "stupid", "moron",
            "mugi", "muji", "randi"
            // Add more as needed
        };

        public static (bool IsClean, string? BlockedWord) Validate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (true, null);

            var words = text.Split(
                new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (BlockedWords.Contains(word.Trim()))
                    return (false, word.Trim());
            }

            return (true, null);
        }

        public static (bool IsClean, string? Message) ValidateComplaint(
            string title, string description)
        {
            var titleCheck = Validate(title);
            if (!titleCheck.IsClean)
                return (false, "Title contains inappropriate content. Please revise.");

            var descCheck = Validate(description);
            if (!descCheck.IsClean)
                return (false, "Description contains inappropriate content. Please revise.");

            return (true, null);
        }
    }
}
```

---

## 11. AutoMapper Profiles

### [NEW] `Business/Mapping/MemberProfile.cs`
```csharp
public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<MemberResult, MemberResponse>()
            .ForMember(d => d.Role, o => o.MapFrom(s => ((UserRole)s.Role).ToString()));
    }
}
```

### [NEW] `Business/Mapping/ComplaintProfile.cs`
```csharp
public class ComplaintProfile : Profile
{
    public ComplaintProfile()
    {
        CreateMap<ComplaintSummaryResult, ComplaintSummaryResponse>()
            .ForMember(d => d.Category, o => o.MapFrom(s => ((ComplaintCategory)s.Category).ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => ((ComplaintStatus)s.Status).ToString()))
            .ForMember(d => d.SubmitterName, o => o.MapFrom(s => s.SubmitterFirstName + " " + s.SubmitterLastName));

        CreateMap<ComplaintDetailResult, ComplaintDetailResponse>()
            .ForMember(d => d.Category, o => o.MapFrom(s => ((ComplaintCategory)s.Category).ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => ((ComplaintStatus)s.Status).ToString()))
            .ForMember(d => d.SubmitterName, o => o.MapFrom(s => s.SubmitterFirstName + " " + s.SubmitterLastName));

        CreateMap<ComplaintUpdateResult, ComplaintUpdateResponse>()
            .ForMember(d => d.UpdateType, o => o.MapFrom(s => ((ComplaintUpdateType)s.UpdateType).ToString()))
            .ForMember(d => d.OldStatus, o => o.MapFrom(s => s.OldStatus.HasValue ? ((ComplaintStatus)s.OldStatus.Value).ToString() : null))
            .ForMember(d => d.NewStatus, o => o.MapFrom(s => s.NewStatus.HasValue ? ((ComplaintStatus)s.NewStatus.Value).ToString() : null))
            .ForMember(d => d.UpdatedByName, o => o.MapFrom(s => s.UpdatedByFirstName + " " + s.UpdatedByLastName));
    }
}
```

---

## 12. All Controllers & Endpoints

### [NEW] `API/Controllers/AdminMembersController.cs`

```
[Route("api/admin/members")]
[Authorize(Roles = "Admin")]

POST   /api/admin/members              → CreateAsync(AddMemberRequest)
GET    /api/admin/members               → GetAllAsync(search?, role?)
GET    /api/admin/members/{id}          → GetByIdAsync(id)
PUT    /api/admin/members/{id}          → UpdateAsync(id, UpdateMemberRequest)
DELETE /api/admin/members/{id}          → DeleteAsync(id)
```

### [NEW] `API/Controllers/ComplaintsController.cs`

```
[Route("api/complaints")]
[Authorize(Roles = "User")]

POST   /api/complaints                  → CreateAsync(CreateComplaintRequest)
GET    /api/complaints/my               → GetMyComplaintsAsync()
GET    /api/complaints/{id}             → GetMyComplaintByIdAsync(id)
PUT    /api/complaints/{id}             → UpdateAsync(id, UpdateComplaintRequest)
DELETE /api/complaints/{id}             → DeleteAsync(id)
GET    /api/complaints/my/irrelevant    → GetMyIrrelevantAsync()
```

### [NEW] `API/Controllers/AdminComplaintsController.cs`

```
[Route("api/admin/complaints")]
[Authorize(Roles = "Admin")]

GET    /api/admin/complaints                    → GetAllAsync(status?, category?, search?)
GET    /api/admin/complaints/{id}               → GetComplaintByIdAsync(id)
PUT    /api/admin/complaints/{id}/status        → UpdateStatusAsync(id, UpdateStatusRequest)
POST   /api/admin/complaints/{id}/notes         → AddNoteAsync(id, AddNoteRequest)
PUT    /api/admin/complaints/{id}/duplicate      → MarkDuplicateAsync(id, MarkDuplicateRequest)
PUT    /api/admin/complaints/{id}/forward        → MarkForwardedAsync(id)
PUT    /api/admin/complaints/{id}/irrelevant     → MarkIrrelevantAsync(id, MarkIrrelevantRequest)
GET    /api/admin/complaints/irrelevant          → GetAllIrrelevantAsync()
```

### [NEW] `API/Controllers/DashboardController.cs`

```
[Route("api/dashboard")]
[Authorize]

GET    /api/dashboard/user              → GetUserDashboardAsync()      [Roles = "User"]
GET    /api/dashboard/admin             → GetAdminDashboardAsync()     [Roles = "Admin"]
```

---

## 13. DI Registration Changes

### [MODIFY] `Data/DependencyInjection.cs`

```diff
  services.AddScoped<IFileRepository, FileRepository>();
  services.AddScoped<IUserRepository, UserRepository>();
  services.AddScoped<IOtpRepository, OtpRepository>();
+ services.AddScoped<IMemberRepository, MemberRepository>();
+ services.AddScoped<IComplaintRepository, ComplaintRepository>();

  // Fix migration assembly bug
- b => b.MigrationsAssembly("StudentCourseManagement.Data")
+ b => b.MigrationsAssembly("SmartCollegeComplaintPortal.Data")
```

### [MODIFY] `Business/DependencyInjection.cs`

```diff
  services.AddScoped<IAuthService, AuthService>();
  services.AddScoped<IUserService, UserService>();
  services.AddScoped<IOtpService, OtpService>();
  services.AddScoped<IEmailService, EmailService>();
+ services.AddScoped<IMemberService, MemberService>();
+ services.AddScoped<IComplaintService, ComplaintService>();
```

---

## 14. DbContext Changes

### [MODIFY] `Data/Database/SystemEfCoreDbContext.cs`

```diff
  public DbSet<FileMetadata> FileMetadatas { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<Otp> Otps { get; set; }
+ public DbSet<Member> Members { get; set; }
+ public DbSet<Complaint> Complaints { get; set; }
+ public DbSet<ComplaintUpdate> ComplaintUpdates { get; set; }
```

---

## 15. Migration Commands

```bash
# Step 1: Create migration
dotnet ef migrations add AddMembersAndComplaints ^
    --project src/SmartCollegeComplaintPortal.Data ^
    --startup-project src/SmartCollegeComplaintPortal.API

# Step 2: Apply migration
dotnet ef database update ^
    --project src/SmartCollegeComplaintPortal.Data ^
    --startup-project src/SmartCollegeComplaintPortal.API
```

---

## 16. Complete File Summary

| # | Layer | Action | File |
|---|---|---|---|
| 1 | Domain | NEW | `Enums/ComplaintCategory.cs` |
| 2 | Domain | NEW | `Enums/ComplaintStatus.cs` |
| 3 | Domain | NEW | `Enums/ComplaintUpdateType.cs` |
| 4 | Domain | NEW | `Entities/Members/Member.cs` |
| 5 | Domain | NEW | `Entities/Complaints/Complaint.cs` |
| 6 | Domain | NEW | `Entities/Complaints/ComplaintUpdate.cs` |
| 7 | Domain | MODIFY | `Entities/Auths/User.cs` (add MemberId) |
| 8 | Data | NEW | `QueryResult/MemberResult.cs` |
| 9 | Data | NEW | `QueryResult/ComplaintSummaryResult.cs` |
| 10 | Data | NEW | `QueryResult/ComplaintDetailResult.cs` |
| 11 | Data | NEW | `QueryResult/ComplaintUpdateResult.cs` |
| 12 | Data | NEW | `QueryResult/DashboardStatsResult.cs` |
| 13 | Data | NEW | `Interfaces/IMemberRepository.cs` |
| 14 | Data | NEW | `Interfaces/IComplaintRepository.cs` |
| 15 | Data | NEW | `Repositories/MemberRepository.cs` |
| 16 | Data | NEW | `Repositories/ComplaintRepository.cs` |
| 17 | Data | MODIFY | `Database/SystemEfCoreDbContext.cs` |
| 18 | Data | MODIFY | `DependencyInjection.cs` |
| 19 | Business | NEW | `DTOs/Members/AddMemberRequest.cs` |
| 20 | Business | NEW | `DTOs/Members/UpdateMemberRequest.cs` |
| 21 | Business | NEW | `DTOs/Members/MemberResponse.cs` |
| 22 | Business | NEW | `DTOs/Complaints/CreateComplaintRequest.cs` |
| 23 | Business | NEW | `DTOs/Complaints/UpdateComplaintRequest.cs` |
| 24 | Business | NEW | `DTOs/Complaints/ComplaintSummaryResponse.cs` |
| 25 | Business | NEW | `DTOs/Complaints/ComplaintDetailResponse.cs` |
| 26 | Business | NEW | `DTOs/Complaints/ComplaintUpdateResponse.cs` |
| 27 | Business | NEW | `DTOs/Complaints/UpdateStatusRequest.cs` |
| 28 | Business | NEW | `DTOs/Complaints/AddNoteRequest.cs` |
| 29 | Business | NEW | `DTOs/Complaints/MarkDuplicateRequest.cs` |
| 30 | Business | NEW | `DTOs/Complaints/MarkIrrelevantRequest.cs` |
| 31 | Business | NEW | `DTOs/Dashboard/UserDashboardResponse.cs` |
| 32 | Business | NEW | `DTOs/Dashboard/AdminDashboardResponse.cs` |
| 33 | Business | NEW | `Interfaces/Members/IMemberService.cs` |
| 34 | Business | NEW | `Interfaces/Complaints/IComplaintService.cs` |
| 35 | Business | NEW | `Services/Members/MemberService.cs` |
| 36 | Business | NEW | `Services/Complaints/ComplaintService.cs` |
| 37 | Business | NEW | `Helpers/ContentValidator.cs` |
| 38 | Business | NEW | `Mapping/MemberProfile.cs` |
| 39 | Business | NEW | `Mapping/ComplaintProfile.cs` |
| 40 | Business | MODIFY | `DependencyInjection.cs` |
| 41 | Business | MODIFY | `Services/Auths/AuthService.cs` |
| 42 | API | NEW | `Controllers/AdminMembersController.cs` |
| 43 | API | NEW | `Controllers/ComplaintsController.cs` |
| 44 | API | NEW | `Controllers/AdminComplaintsController.cs` |
| 45 | API | NEW | `Controllers/DashboardController.cs` |

**Total: 45 files (37 new + 8 modified)**

---

## 17. Build Order

| Step | What | Files | Time |
|---|---|---|---|
| **1** | Domain: Enums | 3 files | 5 min |
| **2** | Domain: Entities | 3 new + 1 modify | 10 min |
| **3** | Data: QueryResult models | 5 files | 10 min |
| **4** | Data: DbContext + DI fix | 2 modify | 5 min |
| **5** | Run EF Migration | command | 5 min |
| **6** | Data: Member Repo (Interface + Impl) | 2 files | 30 min |
| **7** | Data: Complaint Repo (Interface + Impl) | 2 files | 45 min |
| **8** | Business: All DTOs | 12 files | 20 min |
| **9** | Business: ContentValidator | 1 file | 10 min |
| **10** | Business: AutoMapper profiles | 2 files | 10 min |
| **11** | Business: Member Service + DI | 2 files + modify | 30 min |
| **12** | Business: Complaint Service + DI | 2 files + modify | 60 min |
| **13** | Business: Modify AuthService | 1 modify | 30 min |
| **14** | API: All 4 Controllers | 4 files | 30 min |
| **15** | Build + Test via Swagger | — | 30 min |
| | **Total** | **45 files** | **~5.5 hrs** |

---

## 18. Verification Checklist

After building, test this exact flow via Swagger:

```text
□ 1. POST /api/admin/members          → Add a member (email: test@test.com, Role: User)
□ 2. POST /api/auth/check-email       → Check email test@test.com → should succeed
□ 3. POST /api/auth/send-otp          → OTP sent
□ 4. POST /api/auth/verify-otp        → OTP verified
□ 5. POST /api/auth/register          → Password set, user created
□ 6. POST /api/auth/login             → Get JWT token
□ 7. GET  /api/auth/me                → See profile
□ 8. GET  /api/dashboard/user         → See empty dashboard (0 complaints)
□ 9. POST /api/complaints             → Create complaint "Broken AC in Room 301"
□ 10. POST /api/complaints            → Try blocked word in title → should REJECT
□ 11. GET  /api/complaints/my         → See 1 complaint
□ 12. GET  /api/complaints/{id}       → See complaint detail (empty history)
□ 13. PUT  /api/complaints/{id}       → Edit complaint → should show IsEdited=true
□ 14. GET  /api/dashboard/user        → See 1 total, 1 pending
□ --- Switch to Admin JWT ---
□ 15. GET  /api/admin/complaints      → See 1 complaint
□ 16. PUT  /api/admin/complaints/{id}/status → Change to InProgress
□ 17. POST /api/admin/complaints/{id}/notes  → Add "Maintenance dispatched"
□ 18. GET  /api/admin/complaints/{id} → See detail + 3 history entries (Created, Edited, StatusChanged)
□ 19. GET  /api/dashboard/admin       → See stats
□ --- Switch back to User JWT ---
□ 20. GET  /api/complaints/{id}       → See updated status + full history timeline
□ 21. PUT  /api/complaints/{id}       → Try to edit → should REJECT (status != Pending)
□ 22. DELETE /api/complaints/{id}     → Try to delete → should REJECT (status != Pending)
□ DONE ✅
```
