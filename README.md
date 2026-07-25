# GrievanceDesk

GrievanceDesk is a multi-organization platform that bridges organizations and their members by providing a centralized workspace for complaint management, discussions, notices, and administrative workflows.

---

## Features

### Authentication
- Email OTP registration
- Secure login
- Password reset via OTP
- JWT authentication
- Role-based authorization (Admin & Member)

### Complaint Management
- Submit complaints
- Browse organization complaints
- View complaint details
- Track complaint status
- Edit pending complaints
- Complaint discussion and comment threads
- Members and administrators can participate in complaint discussions
- Complaint activity history

### Notice Management
- Create notices
- Save notices as drafts
- Publish notices
- Archive notices
- Search notices
- Read-only notice access for members

### Admin Features
- Complaint workflow management
- Status updates and resolution notes
- Duplicate and irrelevant complaint handling
- Dashboard statistics
- Complaint status distribution charts

---

## Tech Stack

### Frontend
- React 19
- Vite 8
- React Router
- Lucide React
- Fetch API

### Backend
- ASP.NET Core 9 Web API
- Dapper
- Entity Framework Core 9
- ASP.NET Identity Password Hasher
- JWT Bearer Authentication
- Serilog

### Database
- Microsoft SQL Server

---

## Requirements

Install the following before running the project:

- Node.js (LTS)
- npm
- .NET SDK 9
- SQL Server Express or SQL Server Developer Edition
- Git

---

# Backend Setup

## Configure User Secrets

Sensitive configuration is stored using **.NET User Secrets**.

### Database

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GrievanceDeskDb;Trusted_Connection=True;TrustServerCertificate=True" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj
```

### JWT

```powershell
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-secret" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj
```

### Swagger

```powershell
dotnet user-secrets set "Swagger:Key" "replace-with-a-long-random-secret" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "Swagger:Username" "admin" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "Swagger:Password" "replace-this-password" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj
```

### Email (SMTP)

Enable **2-Step Verification** on your Gmail account.

Generate a **16-digit App Password**, then configure it:

```powershell
dotnet user-secrets set "EmailSettings:SenderPassword" "your-16-digit-app-password" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj
```

The SMTP server, sender email, sender name, and port are already configured in `appsettings.json`.

---

## Database Setup

1. Install SQL Server Express or SQL Server Developer Edition.
2. Configure the connection string using User Secrets.
3. Apply Entity Framework Core migrations.

```powershell
dotnet ef database update --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.Data\SmartCollegeComplaintPortal.Data.csproj --startup-project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj --context SystemEfCoreDbContext
```

---

## Demo User (Optional)

Insert a sample administrator for testing.

```sql
INSERT INTO Users
(
    Username,
    FirstName,
    LastName,
    Email,
    Password,
    Role,
    IsActive,
    CreatedAt
)
VALUES
(
    'user_000001',
    'System',
    'Administrator',
    'admin@grievancedesk.local',
    '<ASP.NET_IDENTITY_PASSWORD_HASH>',
    0,
    1,
    GETUTCDATE()
);
```

Replace `<ASP.NET_IDENTITY_PASSWORD_HASH>` with a valid ASP.NET Identity password hash.

---

# Frontend Setup

## Environment Variable (Optional)

Only required when hosting the frontend separately.

Create:

```text
frontend/.env
```

```env
VITE_API_URL=https://localhost:7025
```

Do **not** append `/api`.

---

# Installation

Clone the repository.

```powershell
git clone <repository-url>
cd <repository-folder>
```

Install frontend dependencies.

```powershell
cd frontend
npm install
```

Restore backend packages.

```powershell
cd ..\backend\SmartCollegeComplaintPortal
dotnet restore
```

---

# Running Locally

## Start Backend

```powershell
cd backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API
dotnet run
```

Backend

```
https://localhost:7025
```

Swagger

```
https://localhost:7025/swagger
```

---

## Start Frontend

```powershell
cd frontend
npm run dev
```

Frontend

```
http://localhost:5173
```

The Vite development server automatically proxies all `/api` requests to the backend.

---

## Deployment

Deployment is not configured in this repository.

---

## Notes

- Designed as a platform for organizations and their members.
- Members can submit complaints, participate in complaint discussions, and read published notices.
- Administrators can manage complaints, participate in discussions, publish notices, and oversee organizational workflows.
- Complaint discussions allow members and administrators to communicate directly within each complaint through threaded comments.
- SQL Server must be configured and migrations applied before running the application.
- SMTP configuration is required for registration and password reset emails.
- Authentication uses **Email + Password**.
- Usernames are generated automatically using the format:

