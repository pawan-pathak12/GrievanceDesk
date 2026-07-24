# Phase 6 — Technical Decisions

This phase finalizes the technical decisions for the Smart College Complaint Portal before development starts.

The goal is to keep the stack stable so the team can build without changing direction later.

---

## 1. Confirm Frontend Tech
- Frontend framework: React.
- Language: JavaScript.
- Use a simple component-based structure.
- Keep state and API calls organized by feature.

---

## 2. Confirm Project Architecture
- Backend architecture: N-Tier / layered architecture.
- Main layers:
  - Controllers
  - Services
  - Repositories
  - Data / DbContext
- Keep frontend and backend in separate repositories.
- Keep folders organized by feature and layer.

---

## 3. Confirm Database Technology
- Database: MS SQL Server.
- EF Core: migrations only.
- Dapper: data access and queries.
- Keep schema simple and focused on the MVP.

---

## 4. Confirm Authentication Method
- Authentication: JWT.
- Registration: email-based registration from the existing template.
- Authorization: role-based access.
- Roles:
  - Student
  - Staff
  - Admin / Principal

---

## 5. Confirm Deployment Platform
- Backend: Azure App Service.
- Frontend: Azure App Service or equivalent static hosting if needed.
- Database: Azure SQL or approved SQL Server setup.
- Keep deployment steps simple and repeatable.

---

## 6. Confirm Third-Party Services
- Email service: use the existing email setup from the backend template.
- File storage: optional local storage or Azure Blob if time allows.
- No extra third-party APIs are required for the MVP.

---

## 7. Confirm Coding Conventions
- Use consistent naming for classes, methods, DTOs, endpoints, and files.
- Keep feature-related files grouped together.
- Use clear and simple error handling.
- Keep logging useful but minimal.
- Follow the existing backend template style.

---

## 8. Confirm Git Workflow
- Main branch: stable, ready code.
- Dev branch: integration branch if needed.
- Feature branches: one per module or task.
- Use PRs before merging to main.
- Keep commits small, descriptive, and frequent.
- Do not change approved planning files without team agreement.

---

## 9. Decision Summary
The project will use React for the frontend and ASP.NET Core N-Tier architecture for the backend. The database will remain MS SQL Server with EF Core migrations and Dapper queries. Authentication will use JWT with email-based registration, and deployment will use Azure App Service.

The team will work through feature branches and PRs, keeping the codebase and planning documents stable once development begins.