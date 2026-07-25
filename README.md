# GrievanceDesk

GrievanceDesk is an organization-agnostic complaint management system for submitting, tracking, and resolving internal grievances. It supports separate user and administrator workflows and is intended for private organizations; government-specific workflows are not included.

---

# Features

- Secure registration, login, OTP verification, password reset, and JWT authentication.
- Complaint submission, status tracking, and activity history for users.
- Admin dashboard, filtering, status distribution, and complaint lifecycle management.
- Controlled complaint updates with status rules, resolution notes, duplicate handling, and final-state locks.
- SMTP-based OTP emails and configured Azure Blob Storage support.

---

# Project Structure

- `frontend/` - React and Vite single-page application.
- `backend/SmartCollegeComplaintPortal/` - .NET solution containing API, business, data, and domain projects.
- `backend/SmartCollegeComplaintPortal/src/SmartCollegeComplaintPortal.Data/Migrations/` - Entity Framework Core SQL Server migrations.
- `ui_images/` - UI reference images.

---

# Tech Stack

## Frontend

- React 19
- Vite 8
- React Router
- Oxlint

## Backend

- ASP.NET Core Web API on .NET 9
- Entity Framework Core and Dapper
- JWT Bearer authentication
- AutoMapper, Serilog, Swagger/OpenAPI
- SMTP email and Azure Blob Storage SDK

## Database

- Microsoft SQL Server

---

# Requirements

- Node.js (version not specified in the repository)
- .NET SDK 9.0.315 or a compatible 9.0 SDK
- SQL Server Express, Developer Edition, or another reachable SQL Server instance
- Git

---

# Environment Variables

## Frontend

- `VITE_API_URL` - Optional API base URL for deployed builds. Local development uses the Vite `/api` proxy and needs no frontend environment variable.

## Backend

Configure these with .NET User Secrets for local development or environment variables for deployment. Use double underscores in environment-variable names, for example `ConnectionStrings__DefaultConnection`.

- `ConnectionStrings__DefaultConnection` - SQL Server connection string.
- `Jwt__Key` - Signing key for JWT and verification tokens.
- `Jwt__Issuer` - JWT issuer.
- `Jwt__Audience` - JWT audience.
- `Jwt__ExpiresInMinutes` - Access-token lifetime in minutes.
- `EmailSettings__SmtpServer` - SMTP host.
- `EmailSettings__Port` - SMTP port.
- `EmailSettings__SenderName` - Sender display name.
- `EmailSettings__SenderEmail` - SMTP sender address.
- `EmailSettings__SenderPassword` - SMTP password or app password.
- `AzureBlobStorage__ConnectionString` - Azure Storage connection string when Blob Storage is used.
- `AzureBlobStorage__ContainerName` - Azure Blob container name when Blob Storage is used.
- `Swagger__Key`, `Swagger__Username`, `Swagger__Password` - Required when Swagger is protected outside Development.
- `AllowedOrigins__0`, `AllowedOrigins__1`, etc. - Optional deployed frontend origins for CORS.

---

# Database Setup

1. Install and start SQL Server.
2. Configure `ConnectionStrings__DefaultConnection`.
3. From `backend/SmartCollegeComplaintPortal`, apply the included migrations:

```powershell
dotnet ef database update --project src/SmartCollegeComplaintPortal.Data --startup-project src/SmartCollegeComplaintPortal.API
```

No seed data is configured in the repository.

---

# Installation

```powershell
git clone <repository-url>
cd <repository-folder>

npm ci --prefix frontend
dotnet restore backend/SmartCollegeComplaintPortal/SmartCollegeComplaintPortal.sln
```

Configure the backend secrets or environment variables listed above before starting the API.

---

# Running Locally

1. Ensure SQL Server Express, SQL Server Developer Edition, or another SQL Server instance is running.
2. Configure the database, JWT, SMTP, and any required Blob Storage settings.
3. Apply migrations from `backend/SmartCollegeComplaintPortal`.
4. Start the API:

```powershell
cd backend/SmartCollegeComplaintPortal
dotnet run --project src/SmartCollegeComplaintPortal.API
```

The development API is configured for `https://localhost:7025` and `https://localhost:5259`.

5. In another terminal, start the frontend:

```powershell
cd frontend
npm run dev
```

Vite serves the frontend on its local development URL and proxies `/api` requests to `https://localhost:7025`.

---

# Deployment

The repository includes a GitHub Actions workflow for the backend. Pushes to `main` build and publish the API to Azure App Service when `AZURE_WEBAPP_NAME` and `AZURE_WEBAPP_PUBLISH_PROFILE` are configured as GitHub secrets.

No deployed frontend configuration or public deployment URL is available in the repository.

---

# Demo Credentials

Not available.

---

# Notes

- SMTP configuration is required for registration OTPs and password-reset OTPs.
- Azure Blob Storage is registered in the backend; configure it when using file storage features.
- The production API applies pending EF Core migrations on startup.
