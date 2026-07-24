# Enums & Entities Reference

---

## Enums

### UserRole
```csharp
public enum UserRole
{
    Admin = 0,
    User = 1
}
```

### ComplaintCategory
```csharp
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
```

### ComplaintStatus
```csharp
public enum ComplaintStatus
{
    Pending = 0,
    InProgress = 1,
    Resolved = 2,
    Completed = 3,
    Irrelevant = 4
}
```

### ComplaintUpdateType
```csharp
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
```

---

## Entities

### Member
```csharp
public class Member
{
    public int MemberId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsRegistered { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### User
```csharp
public class User
{
    public int UserId { get; set; }
    public int? MemberId { get; set; }
    public string Username { get; set; }       // Auto-generated: "User_1001"
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### Complaint
```csharp
public class Complaint
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public ComplaintCategory Category { get; set; }
    public string Location { get; set; }
    public ComplaintStatus Status { get; set; }
    public bool IsDuplicate { get; set; }
    public bool IsForwarded { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### Complain 
```csharp
---
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
    ---
    ```

### ComplaintUpdate
```csharp
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
```

### Otp
```csharp
public class Otp
{
    public int OtpId { get; set; }
    public string? Email { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AttemptCount { get; set; }
}
```

### RefreshToken
```csharp
public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public string? Token { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked { get; set; }
}
```

### FileMetadata
```csharp
public class FileMetadata
{
    public int FileMetadataId { get; set; }
    public string OriginalFileName { get; set; }
    public string StoredFileName { get; set; }
    public string RelativePath { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public bool IsPublic { get; set; }
    public DateTime UploadedAt { get; set; }
    public int UploadedByUserId { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```
