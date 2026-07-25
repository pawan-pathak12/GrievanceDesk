# 🚀 GrievanceDesk

> **GrievanceDesk** is a centralized organization portal that simplifies communication between organizations and their members. It provides secure member onboarding, organization-wide notices, grievance management, and administrative tools through a single, easy-to-use platform.

---

# ✨ Core Modules

## 👤 Member Portal

- Secure Registration
- Login
- Email OTP Verification
- Forgot Password
- Profile Management

---

## 📢 Notice Management

### Member

- View Published Notices
- Read Notice Details

### Admin

- Create Notices
- Edit Notices
- Publish / Unpublish Notices
- Soft Delete Notices

---

## 📝 Grievance Management

### Member

- Submit Grievances
- View My Grievances
- View Grievance Details
- Track Grievance Status
- View Activity History

### Admin

- View All Grievances
- Update Grievance Status
- Add Resolution Notes
- Mark as Duplicate
- Mark as Irrelevant
- View Complaint Analytics
- View Complaint Timeline

---

## ⚙️ Platform Features

- JWT Authentication
- Role-Based Authorization
- Email OTP Verification
- Password Reset
- SMTP Email Notifications
- Structured API Responses
- Rate Limiting
- Serilog Logging
- Entity Framework Core Migrations

---

# 🛠 Tech Stack

| Frontend | Backend | Database |
|----------|----------|----------|
| React 19 | ASP.NET Core (.NET 9) | SQL Server |
| Vite | Dapper | Entity Framework Core |
| React Router | AutoMapper | EF Core Migrations |
| Axios | JWT Authentication | |

---

# 📋 Prerequisites

Install the following before running the project.

- .NET SDK 9
- Node.js (LTS)
- SQL Server Express or SQL Server Developer Edition
- Git

---

# ⚙️ Backend Setup

## 1. Clone Repository

```powershell
git clone <repository-url>
cd <repository-folder>
```

---

## 2. Restore Packages

```powershell
cd backend

dotnet restore
```

---

## 3. Configure User Secrets

Initialize User Secrets

```powershell
dotnet user-secrets init
```

Configure required secrets

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GrievanceDeskDb;Trusted_Connection=True;TrustServerCertificate=True"

dotnet user-secrets set "Jwt:Key" "YOUR_16_CHARACTER_SECRET_KEY"

dotnet user-secrets set "Swagger:Key" "YOUR_16_CHARACTER_SECRET_KEY"

dotnet user-secrets set "Swagger:Username" "admin"

dotnet user-secrets set "Swagger:Password" "admin123"

dotnet user-secrets set "EmailSettings:SenderPassword" "YOUR_GMAIL_APP_PASSWORD"
```

> **Important**
>
> - JWT and Swagger secret keys should contain **at least 16 characters**.
> - Enable **Google 2-Step Verification** before generating an App Password.
> - Use the generated **Google App Password** as `EmailSettings:SenderPassword`.

---

## 4. Database Setup

Apply Entity Framework Core migrations.

```powershell
dotnet ef database update
```

---

## 5. Run Backend

```powershell
dotnet run
```

Backend URL

```
https://localhost:7025
```

---

# 💻 Frontend Setup

## 1. Install Dependencies

```powershell
cd frontend

npm install
```

---

## 2. Configure Environment

Create a `.env` file inside the frontend directory.

```env
VITE_API_URL=https://localhost:7025/api
```

---

## 3. Run Frontend

```powershell
npm run dev
```

Frontend URL

```
http://localhost:5173
```

---

# 🔑 Demo Data

To test the application quickly, insert an administrator account directly into the database.

Example:

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
    'Admin',
    'System',
    'Administrator',
    'admin@grievancedesk.com',
    '<PASSWORD_HASH>',
    0,
    1,
    GETUTCDATE()
);
```

Replace `<PASSWORD_HASH>` with an ASP.NET Identity password hash for your desired password.

---

# 📝 Notes

- SQL Server Express or SQL Server Developer Edition is required.
- Configure the database connection string before running the backend.
- SMTP configuration is required for registration OTPs and password reset emails.
- Enable **Google 2-Step Verification** and generate a **Google App Password** for SMTP authentication.
- JWT and Swagger secret keys should contain at least **16 characters**.
- Entity Framework Core migrations must be applied before starting the application.

---

# 📄 License

Developed for educational and hackathon purposes.
```