# Phase 9 — API Planning (Backend)

This phase defines the API contract for the Smart College Complaint Portal.

The goal is to produce a clear, minimal endpoint plan that backend and frontend can follow without repeated re-discussion.

---

## 1. Standard Response Format

All API responses will use your project’s standard response wrapper:

- `ApiResponse` for successful responses.
- `ErrorResponse` for failed responses.

### Success Response Shape
- `IsSuccess`
- `StatusCode`
- `Data`
- `Errors` (optional)

### Error Response Shape
- `IsSuccess`
- `StatusCode`
- `Data`
- `Errors`

---

## 2. AuthController

Your existing `AuthController` should be documented with its current registration and login flow.

### Endpoints

| Method | Endpoint | Auth Required | Role | Operation |
|---|---|---:|---|---|
| POST | `/api/auth/register/check-email` | No | Public | Start registration by checking email |
| POST | `/api/auth/register/verify-otp` | No | Public | Verify OTP |
| POST | `/api/auth/register/create-password` | No | Public | Create password and finish registration |
| POST | `/api/auth/login` | No | Public | Login and issue JWT |
| POST | `/api/auth/change-password` | Yes | Authenticated | Change current user password |
| GET | `/api/auth/me` | Yes | Authenticated | Get current user profile |

### Notes
- `register/check-email`, `register/verify-otp`, and `register/create-password` are part of the existing registration flow.
- `login` returns the JWT access token inside `ApiResponse.Data`.
- `change-password` and `me` require authentication.

---

## 3. UserController

Your existing `UserController` is admin-only and should be documented as the user management controller.

### Endpoints

| Method | Endpoint | Auth Required | Role | Operation |
|---|---|---:|---|---|
| POST | `/admin/create-new` | Yes | Admin | Create new user |
| GET | `/api/user/{id}` | Yes | Admin | Get user by id |
| GET | `/api/user/all` | Yes | Admin | Get all users |
| GET | `/api/user/email/{email}` | Yes | Admin | Get user by email |
| PUT | `/api/user/{id}` | Yes | Admin | Update user |
| DELETE | `/api/user/{id}` | Yes | Admin | Delete user |

### Notes
- This controller is restricted to `Admin`.
- `DELETE /api/user/{id}` blocks deleting your own account.
- `POST /admin/create-new` is currently routed outside the controller prefix and should be documented exactly as implemented.

---

## 4. Complaint API Overview

These are the complaint-related endpoints for the MVP.

### ComplaintsController
- `POST /api/complaints`
- `GET /api/complaints/my`
- `GET /api/complaints/{id}`

### StaffComplaintsController
- `GET /api/staff/complaints`
- `GET /api/staff/complaints/{id}`
- `PUT /api/staff/complaints/{id}/duplicate`
- `PUT /api/staff/complaints/{id}/forward`

### AdminComplaintsController
- `GET /api/admin/complaints`
- `GET /api/admin/complaints/{id}`
- `PUT /api/admin/complaints/{id}/status`
- `POST /api/admin/complaints/{id}/notes`

### FileController
- `POST /api/files/complaints/{complaintId}`

---

## 5. Endpoints by Controller

### ComplaintsController

| Method | Endpoint | Auth Required | Role | Operation |
|---|---|---:|---|---|
| POST | `/api/complaints` | Yes | Student | Create complaint |
| GET | `/api/complaints/my` | Yes | Student | List own complaints |
| GET | `/api/complaints/{id}` | Yes | Student | View complaint detail |

---

### StaffComplaintsController

| Method | Endpoint | Auth Required | Role | Operation |
|---|---|---:|---|---|
| GET | `/api/staff/complaints` | Yes | Staff | View all complaints for review |
| GET | `/api/staff/complaints/{id}` | Yes | Staff | View complaint detail |
| PUT | `/api/staff/complaints/{id}/duplicate` | Yes | Staff | Mark complaint as duplicate |
| PUT | `/api/staff/complaints/{id}/forward` | Yes | Staff | Forward complaint to admin/principal |

---

### AdminComplaintsController

| Method | Endpoint | Auth Required | Role | Operation |
|---|---|---:|---|---|
| GET | `/api/admin/complaints` | Yes | Admin / Principal | View forwarded complaints |
| GET | `/api/admin/complaints/{id}` | Yes | Admin / Principal | View complaint detail |
| PUT | `/api/admin/complaints/{id}/status` | Yes | Admin / Principal | Update complaint status |
| POST | `/api/admin/complaints/{id}/notes` | Yes | Admin / Principal | Add note/update |

---

### FileController

| Method | Endpoint | Auth Required | Role | Operation |
|---|---|---:|---|---|
| POST | `/api/files/complaints/{complaintId}` | Yes | Student | Upload complaint evidence |

---

## 6. CRUD Mapping

### Create
- `POST /api/auth/register/check-email`
- `POST /api/auth/register/verify-otp`
- `POST /api/auth/register/create-password`
- `POST /api/auth/login`
- `POST /api/complaints`
- `POST /api/admin/complaints/{id}/notes`
- `POST /api/files/complaints/{complaintId}`
- `POST /admin/create-new`

### Read
- `GET /api/auth/me`
- `GET /api/user/{id}`
- `GET /api/user/all`
- `GET /api/user/email/{email}`
- `GET /api/complaints/my`
- `GET /api/complaints/{id}`
- `GET /api/staff/complaints`
- `GET /api/staff/complaints/{id}`
- `GET /api/admin/complaints`
- `GET /api/admin/complaints/{id}`

### Update
- `POST /api/auth/change-password`
- `PUT /api/user/{id}`
- `PUT /api/staff/complaints/{id}/duplicate`
- `PUT /api/staff/complaints/{id}/forward`
- `PUT /api/admin/complaints/{id}/status`

### Delete
- `DELETE /api/user/{id}`

---

## 7. Authentication Requirements

### Public
- `POST /api/auth/register/check-email`
- `POST /api/auth/register/verify-otp`
- `POST /api/auth/register/create-password`
- `POST /api/auth/login`

### Authenticated
- All other endpoints require JWT.

### Special Notes
- `change-password` and `me` require logged-in user.
- UserController endpoints require `Admin`.
- No password reset endpoint is needed in the MVP.

---

## 8. Required Roles

- `POST /api/complaints` → Student
- `GET /api/complaints/my` → Student
- `GET /api/complaints/{id}` → Student
- `GET /api/staff/complaints` → Staff
- `GET /api/staff/complaints/{id}` → Staff
- `PUT /api/staff/complaints/{id}/duplicate` → Staff
- `PUT /api/staff/complaints/{id}/forward` → Staff
- `GET /api/admin/complaints` → Admin / Principal
- `GET /api/admin/complaints/{id}` → Admin / Principal
- `PUT /api/admin/complaints/{id}/status` → Admin / Principal
- `POST /api/admin/complaints/{id}/notes` → Admin / Principal
- `POST /api/files/complaints/{complaintId}` → Student
- `POST /api/auth/change-password` → Authenticated
- `GET /api/auth/me` → Authenticated
- `UserController` routes → Admin

---

## 9. Validation Requirements

Use basic ASP.NET Core validation only.

### Registration DTOs
- `[Required]`
- `[EmailAddress]`
- `[StringLength]`

### Login DTO
- `[Required]`
- `[EmailAddress]`
- `[StringLength]`

### Complaint DTOs
- `[Required]`
- `[StringLength]`

### Staff/Admin DTOs
- `[Required]`
- `[StringLength]`

### File Upload Validation
- File field name: `Evidence`
- Allowed types: `jpg`, `jpeg`, `png`
- Max size: 2–5 MB
- Enforce in code, not FluentValidation

---

## 10. File Upload Requirements

### Upload Endpoint
- `POST /api/files/complaints/{complaintId}`

### Requirements
- Accept one complaint evidence file.
- Store metadata using the existing `FileMetadata` entity.
- Link file to complaint through:
  - `EntityType = "Complaint"`
  - `EntityId = complaintId`
- Only students can upload evidence for their own complaints.

### Notes
- The upload can be local storage for hackathon speed.
- Azure Blob Storage can be used only if already ready.

---

## 11. External Integrations

### Email
- Use the existing email service from the backend template.
- Trigger email when complaint status changes or a note is added.

### File Storage
- Use local `/uploads` or Azure Blob if available.
- No other third-party integrations are needed for the MVP.

---

## 12. DTO Definitions

### Auth DTOs

#### CheckEmailDto
- `Email`

#### VerifyOtpDto
- `Email`
- `Code`

#### CreatePasswordDto
- `Email`
- `Password`

#### LoginDto
- `Email`
- `Password`

#### ChangePasswordDto
- `CurrentPassword`
- `NewPassword`

#### AuthResponse
- `UserId`
- `FullName`
- `Email`
- `Role`
- `Token`

---

### User DTOs

#### CreateUserByAdminRequest
- `FullName`
- `Email`
- `Role`
- `Password`

#### UpdateUserDto
- `FullName`
- `Email`
- `Role`
- `IsActive`

#### UserQueryParams
- `Search`
- `Role`
- `PageNumber`
- `PageSize`

---

### Complaint DTOs

#### CreateComplaintRequest
- `Title`
- `Description`
- `Category`
- `Location`

#### ComplaintSummaryResponse
- `Id`
- `Title`
- `Category`
- `Location`
- `Status`
- `CreatedAt`
- `IsDuplicate`
- `ForwardedToAdmin`

#### ComplaintDetailResponse
- `Id`
- `Title`
- `Description`
- `Category`
- `Location`
- `Status`
- `IsDuplicate`
- `ForwardedToAdmin`
- `CreatedAt`
- `UpdatedAt`
- `Updates`
- `Attachments`

#### ComplaintUpdateResponse
- `Id`
- `UpdatedByName`
- `UpdateType`
- `OldStatus`
- `NewStatus`
- `NoteText`
- `CreatedAt`

#### ComplaintAttachmentResponse
- `FileMetadataId`
- `OriginalFileName`
- `RelativePath`
- `ContentType`
- `Size`
- `UploadedAt`

---

### Staff DTOs

#### MarkDuplicateRequest
- `NoteText`
- `IsDuplicate`

#### ForwardComplaintRequest
- `NoteText`
- `ForwardedToRole`

---

### Admin DTOs

#### UpdateComplaintStatusRequest
- `NewStatus`

#### AddComplaintNoteRequest
- `NoteText`

---

### File Upload DTOs

#### FileUploadResponse
- `FileMetadataId`
- `OriginalFileName`
- `RelativePath`
- `ContentType`
- `Size`
- `UploadedAt`

---

## 13. Internal Email Logic

This is not a public API endpoint.

When an admin/principal updates a complaint status or adds a note:
- get the student who created the complaint,
- send an email to that student,
- store the result in `EmailLogs` if you keep logging enabled.

No separate email endpoint is required for the MVP.

---

## 14. Team Usage Rule

Once this API plan is approved:
- Backend should implement endpoints exactly as listed.
- Frontend should build screens only for these endpoints.
- No endpoint structure should be changed without team approval.
- Any contract change must be reflected in this document first.