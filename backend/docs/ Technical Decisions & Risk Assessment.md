# Phase 3 — Technical Decisions & Risk Assessment

This phase locks the technical decisions for the Smart College Complaint Portal and identifies risky features before module breakdown and team division.

The goal is to keep the stack stable and reduce surprises during a 24-hour hackathon.

---

## 1. Technical Decisions

### Project Architecture
- Backend: ASP.NET Core Web API.
- Structure: N-Tier / layered architecture.
- Layers: Controllers → Services → Repositories → Data.
- Frontend: React with JavaScript in a separate repository.

### Database Technology
- Database: MS SQL Server.
- ORM use:
  - EF Core: migrations only.
  - Dapper: data access and queries.
- Keep schema simple and focused on the MVP.

### Authentication & Authorization
- Authentication: JWT.
- Registration: email-based registration from the existing template.
- Authorization: role-based access.
- Roles:
  - Student
  - Staff
  - Admin / Principal

### Deployment Platform
- Backend: Azure App Service.
- Frontend: Azure App Service or equivalent static hosting if needed.
- Database: Azure SQL or the hackathon-approved SQL Server setup.
- Keep environment setup simple and reproducible.

### Third-Party Services
- Email provider: existing email service from the backend template.
- File storage: optional local storage or Azure Blob Storage if time allows.
- No extra third-party APIs are required for the core MVP.

### Coding Conventions
- Use consistent naming for classes, methods, DTOs, and endpoints.
- Keep files organized by feature and layer.
- Use clear service and repository separation.
- Keep error handling consistent across the API.
- Keep logging simple and useful for debugging.

### Git Workflow
- Branch strategy:
  - `main` for stable code.
  - `dev` for integration if needed.
  - Feature branches for each module.
- Use pull requests for merging.
- Avoid direct edits to approved planning files.
- Keep commit history clean and meaningful.

---

## 2. Risk Assessment

### Risk 1: Email Sending
- Why it’s risky: email configuration, delivery, and testing can take time.
- MVP / Secondary / Stretch: Secondary.
- Simplest version: send one plain-text email when complaint status changes.
- Fallback plan: log the email content or show a mock notification if sending fails.
- Extra setup needed: SMTP credentials or existing template mail settings.

### Risk 2: Evidence File Upload
- Why it’s risky: multipart handling, validation, storage, and serving files can slow the team.
- MVP / Secondary / Stretch: Secondary.
- Simplest version: allow one image only.
- Fallback plan: make the image optional or disable upload if it blocks progress.
- Extra setup needed: storage path or Azure Blob configuration if used.

### Risk 3: Duplicate Complaint Filtering
- Why it’s risky: automatic duplicate detection is complex and can waste time.
- MVP / Secondary / Stretch: MVP only as manual review.
- Simplest version: staff manually reviews and marks duplicates.
- Fallback plan: staff forwards only unique complaints based on visual review.
- Extra setup needed: none.

### Risk 4: Principal Workflow
- Why it’s risky: extra role routing can add confusion if over-designed.
- MVP / Secondary / Stretch: MVP only as a simple read/action dashboard.
- Simplest version: principal sees filtered complaints forwarded by staff.
- Fallback plan: use the admin dashboard as the principal view if needed.
- Extra setup needed: role assignment for principal account.

### Risk 5: Role-Based Access
- Why it’s risky: incorrect authorization can break flows or expose data.
- MVP / Secondary / Stretch: MVP.
- Simplest version: enforce simple role-based controller access.
- Fallback plan: temporarily restrict features by role in the UI while backend auth is stabilized.
- Extra setup needed: seeded users or demo accounts.

### Risk 6: React Frontend Integration
- Why it’s risky: API contract mismatch can slow development.
- MVP / Secondary / Stretch: MVP.
- Simplest version: build only the screens needed for the core flows.
- Fallback plan: use mock data temporarily until APIs are ready.
- Extra setup needed: API base URL and environment variables.

---

## 3. Technical Decisions Summary

The project will use ASP.NET Core Web API with N-Tier architecture, MS SQL Server, Dapper for queries, and EF Core for migrations. Authentication will use JWT with role-based access for Student, Staff, and Admin/Principal. The frontend will be built in React in a separate repository.

Deployment will use Azure App Service for backend and frontend, with the existing email service from the template handling status-update notifications. The MVP will keep risky features simple: manual duplicate filtering, single-image upload if time allows, and plain email updates only.

---

## 4. Risky Features List

| Feature | Why It’s Risky | Classification | Simplest Version | Fallback Plan |
|---|---|---|---|---|
| Email sending | Needs configuration and testing | Secondary | Plain-text email on status update | Log email content if sending fails |
| File upload | Multipart and storage issues | Secondary | One optional image | Disable upload if blocked |
| Duplicate filtering | Automatic detection is complex | MVP (manual) | Staff manually marks duplicates | Forward only visually unique complaints |
| Principal workflow | Can add role confusion | MVP | Principal sees forwarded complaints only | Use admin dashboard as fallback |
| Role-based access | Security-sensitive | MVP | Basic controller authorization | Limit UI actions temporarily |
| React integration | API mismatch risk | MVP | Core screens only | Use mock data until APIs are ready |

---

## 5. Output for Next Phases

By the end of this phase, the team should use this document to guide:

- Phase 4: Module breakdown.
- Phase 5: Team division.
- Phase 7: Development order.

This keeps the project stable and prevents technical changes after development starts.