# GrievanceDesk

GrievanceDesk is a multi-organization complaint management platform for members and administrators. It provides secure account onboarding, complaint tracking and discussion, organization notices, and an admin resolution workflow.

---

# Features

- Email OTP registration and password reset
- JWT authentication and role-based member and admin workspaces
- Complaint submission, status tracking, activity history, and discussion threads
- Organization-wide complaint browsing for members
- Admin complaint workflow with status controls, resolution notes, and terminal states
- Notice creation, drafts, publishing, archiving, and member notice search
- Dashboard summaries and client-side complaint status distribution

---

# Project Structure

- frontend/ - React and Vite single-page application
- backend/SmartCollegeComplaintPortal/ - ASP.NET Core solution
- backend/SmartCollegeComplaintPortal/src/SmartCollegeComplaintPortal.API/ - API host and controllers
- backend/SmartCollegeComplaintPortal/src/SmartCollegeComplaintPortal.Business/ - services, DTOs, and business logic
- backend/SmartCollegeComplaintPortal/src/SmartCollegeComplaintPortal.Data/ - SQL Server access and EF Core migrations
- backend/SmartCollegeComplaintPortal/src/SmartCollegeComplaintPortal.Domain/ - entities and shared domain models

---

# Tech Stack

## Frontend

- React 19
- Vite 8
- React Router
- Lucide React icons
- Native Fetch API

## Backend

- ASP.NET Core 9
- Dapper
- Entity Framework Core 9
- ASP.NET Identity password hashing
- JWT bearer authentication
- Serilog

## Database

- Microsoft SQL Server

---

# Requirements

- Node.js LTS and npm
- .NET SDK 9
- SQL Server Express or SQL Server Developer Edition
- Git

---

# Environment Variables

## Frontend

Create frontend/.env only when the frontend is hosted separately from the Vite development proxy:

~~~env
VITE_API_URL=https://localhost:7025
~~~

- VITE_API_URL: API origin. Do not append /api; frontend requests already include it.

## Backend

The API project uses .NET user secrets. From the repository root, run:

~~~powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GrievanceDeskDb;Trusted_Connection=True;TrustServerCertificate=True" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-secret" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "Swagger:Key" "replace-with-a-long-random-secret" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "Swagger:Username" "admin" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "Swagger:Password" "replace-this-password" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj

dotnet user-secrets set "EmailSettings:SenderPassword" "your-smtp-app-password" --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj
~~~

- ConnectionStrings:DefaultConnection: SQL Server connection string.
- Jwt:Key: signing key for authentication and verification tokens.
- Swagger:Key, Swagger:Username, Swagger:Password: production Swagger protection settings.
- EmailSettings:SenderPassword: SMTP app password for OTP and reset emails.

The SMTP host, port, sender name, and sender email are configured in appsettings.json and can be overridden through user secrets when needed.

---

# Database Setup

1. Install and start SQL Server Express or SQL Server Developer Edition.
2. Set ConnectionStrings:DefaultConnection with user secrets.
3. Apply migrations:

~~~powershell
dotnet ef database update --project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.Data\SmartCollegeComplaintPortal.Data.csproj --startup-project backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API\SmartCollegeComplaintPortal.API.csproj --context SystemEfCoreDbContext
~~~

This applies the complaint discussion and anonymous-username migrations along with the rest of the schema.

For a direct test user, insert a user with an ASP.NET Identity password hash. Authentication uses email and password.

~~~sql
INSERT INTO Users (Username, FirstName, LastName, Email, Password, Role, IsActive, CreatedAt)
VALUES ('000001', 'System', 'Administrator', 'admin@grievancedesk.local', '<ASP.NET_IDENTITY_PASSWORD_HASH>', 0, 1, GETUTCDATE());
~~~

Generate the password hash through the application or ASP.NET Identity before using this sample.

---

# Installation

~~~powershell
git clone <repository-url>
cd <repository-folder>

cd frontend
npm install

cd ..\backend\SmartCollegeComplaintPortal
dotnet restore
~~~

---

# Running Locally

1. Configure backend user secrets and apply the database migrations.
2. Start the API:

~~~powershell
cd backend\SmartCollegeComplaintPortal\src\SmartCollegeComplaintPortal.API
dotnet run
~~~

The API runs at https://localhost:7025. Swagger is available in Development at /swagger.

3. In a second terminal, start the frontend:

~~~powershell
cd frontend
npm run dev
~~~

Open http://localhost:5173. Vite proxies /api requests to the local API.

---

# Deployment

Not configured in this repository. No deployment URL is available.

---

# Demo Credentials

Not available.

---

# Notes

- This project is designed for organizations and is not limited to one domain or institution.
- The API requires SQL Server and applied EF Core migrations before it can serve complaint or notice data.
- Registration, password reset, and administrator-created accounts require a working SMTP configuration.
- Azure Blob Storage is not required for the current local setup.
- New and existing users use anonymous six-digit usernames; sign-in remains email-based.
