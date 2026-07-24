# Phase 8 — Database Planning

This phase defines the database design for the Smart College Complaint Portal while reusing the existing auth-related entities in the backend template.

The goal is to keep the schema simple, MVP-focused, and aligned with the current starter kit.

---

## 1. Existing Auth Entities in Template

Your backend template already includes these auth-related entities:

### Otp
Used for email-based OTP verification.

| Column | Type | Notes |
|---|---|---|
| OtpId | int | Primary key |
| Email | string | Optional |
| Code | string | Required |
| IsActive | bool | Active OTP flag |
| ExpiresAt | DateTime | OTP expiry time |
| IsUsed | bool | Whether OTP is already used |
| CreatedAt | DateTime | Creation time |
| AttemptCount | int | Number of tries |

### FileMetadata
Used for file upload tracking and metadata storage.

| Column | Type | Notes |
|---|---|---|
| FileMetadataId | int | Primary key |
| OriginalFileName | string | Required |
| StoredFileName | string | Required |
| RelativePath | string | Required |
| ContentType | string | Required |
| Size | long | File size |
| IsPublic | bool | Public/private flag |
| UploadedAt | DateTime | Upload time |
| UploadedByUserId | int | FK to Users.Id |
| EntityType | string | Related entity name |
| EntityId | int | Related entity id |
| IsActive | bool | Active record flag |
| UpdatedAt | DateTime? | Last updated time |

### RefreshToken
Used for JWT refresh token support.

| Column | Type | Notes |
|---|---|---|
| RefreshTokenId | int | Primary key |
| Token | string | Token value |
| UserId | int | FK to Users.Id |
| ExpiresAt | DateTime | Expiry time |
| CreatedAt | DateTime | Creation time |
| RevokedAt | DateTime? | Revocation time |
| IsRevoked | bool | Revoked flag |

---

## 2. Main Complaint Entities

### 1. Users
Stores login and role information for all users.

| Column | Type | Notes |
|---|---|---|
| Id | int | Primary key, identity |
| FullName | nvarchar(200) | Required |
| Email | nvarchar(200) | Unique, required |
| Password | nvarchar(500) | Required |
| Role | UserRole| Student, Staff, Admin, Principal |
| IsActive | bit | Default true |
| CreatedAt | datetime2 | Default current UTC time |
| UpdatedAt | datetime2 | Nullable |

---

### 2. Complaints
Stores the complaint created by a student.

| Column | Type | Notes |
|---|---|---|
| Id | int | Primary key, identity |
| StudentId | int | FK to Users.Id |
| Title | nvarchar(200) | Required |
| Description | nvarchar(max) | Required |
| Category | ComplainCategory | Required |
| Location | nvarchar(200) | Required |
| Status | ComplainStatus | Open, InProgress, Resolved, Closed |
| IsDuplicate | bit | Default false |
| ForwardedToAdmin | bit | Default false |
| ForwardedToAdminId    | nullable int | default null  |
| CreatedAt | datetime2 | Default current UTC time |
| UpdatedAt | datetime2 | Nullable |

### Notes for Complaints
- This table is for the **student-created complaint only**.
- Staff should not create a separate complaint record here.
- Staff can review, mark duplicates, and forward the same complaint.
- Do not store staff review history directly in this table.

---

### 3. ComplaintUpdates
Stores staff/admin review actions, status changes, notes, and forwarding history.

| Column | Type | Notes |
|---|---|---|
| Id | int | Primary key, identity |
| ComplaintId | int | FK to Complaints.Id |
| UpdatedByUserId | int | FK to Users.Id |
| UpdateType | ComplainUpdatedType | StatusChange, Note, Forwarded, DuplicateMarked |
| OldStatus | ComplainStatus | Nullable |
| NewStatus | ComplainStatus | Nullable |
| NoteText | nvarchar(max) | Nullable |
| IsDuplicate | bit | Default false |
| CreatedAt | datetime2 | Default current UTC time |

### Notes for ComplaintUpdates
- Use this table for **all staff/admin actions** on a complaint.
- If staff marks a complaint as duplicate, store that action here.
- If staff forwards a complaint to admin/principal, store that action here.
- If admin/principal updates status, store that action here.
- This table becomes the full history of what happened to the complaint.

---

## 3. Using Existing FileMetadata

For complaint evidence uploads, use the existing `FileMetadata` entity instead of creating a new attachment table.

### How it will be used
- `EntityType` = `"Complaint"`
- `EntityId` = complaint `Id`
- `UploadedByUserId` = student `Id`
- `IsPublic` = true or false depending on your access rules

### Suggested usage
- Store complaint images in the file storage system.
- Save the metadata in `FileMetadata`.
- Link the file to the complaint using `EntityType` and `EntityId`.

This keeps the design reusable and avoids creating duplicate file tables.

---

## 4. Relationships

- One User can create many Complaints.
- One Complaint can have many ComplaintUpdates.
- One Complaint can have many FileMetadata records through `EntityType` and `EntityId`.
- One User can have many RefreshTokens.
- Otp is used independently for email verification.

---

## 5. Foreign Keys

- `Complaints.StudentId` → `Users.Id`
- `ComplaintUpdates.ComplaintId` → `Complaints.Id`
- `ComplaintUpdates.UpdatedByUserId` → `Users.Id`
- `RefreshToken.UserId` → `Users.Id`
- `FileMetadata.UploadedByUserId` → `Users.Id`

`FileMetadata` uses a polymorphic reference pattern through `EntityType` and `EntityId`, so it does not need a direct FK to `Complaints`.

---

## 6. Recommended Indexes

- `Users.Email` unique index.
- `Complaints.StudentId` index.
- `Complaints.Status` index.
- `Complaints.Category` index.
- `ComplaintUpdates.ComplaintId` index.
- `FileMetadata.EntityType + EntityId` composite index.
- `FileMetadata.UploadedByUserId` index.
- `RefreshToken.UserId` index.
- `Otp.Email` index.

---

## 7. MVP Schema Recommendation

Use these complaint-related tables for the hackathon MVP:

- `Users`
- `Complaints`
- `ComplaintUpdates`
- `FileMetadata`
- `RefreshToken`
- `Otp`

This works well because:
- auth support already exists,
- file upload support already exists,
- refresh token support already exists,
- and complaint tracking stays simple.

---

## 8. Final Note

Keep `Complaints` only for the student-created complaint record.  
Keep all staff/admin review actions, duplicate marking, forwarding, and status changes inside `ComplaintUpdates`.  
This makes the workflow clean and avoids mixing student submission data with staff processing history.