# Backend Checklist

Use this checklist after receiving the project requirements.

The goal is to complete the backend efficiently during a hackathon by following a consistent workflow.

---

# Phase 1 — Project Initialization

## Template Setup

- [ ] Clone the backend template repository
- [ ] Rename the solution
- [ ] Rename all projects
- [ ] Update namespaces
- [ ] Initialize a new Git repository
- [ ] Create a new GitHub repository
- [ ] Push the initial project
- [ ] Restore NuGet packages
- [ ] Build the solution
- [ ] Verify the application starts successfully

---

# Phase 2 — Planning

## Database

- [ ] Identify all entities
- [ ] Identify relationships
- [ ] Design the database schema
- [ ] Review the database design with the team

## API

- [ ] List all required modules
- [ ] List required CRUD operations
- [ ] Identify required authentication
- [ ] Identify required user roles
- [ ] Identify external integrations (Email, Payment, Storage, etc.)

---

# Phase 3 — Feature Implementation

Repeat this workflow for every module.

## Entity

- [ ] Create Entity

## DTOs

- [ ] Create Request DTOs
- [ ] Create Response DTOs

## Repository

- [ ] Interface
- [ ] Implementation

## Service

- [ ] Interface
- [ ] Implementation

## Controller

- [ ] Create API endpoints
- [ ] Test every endpoint using Swagger/Postman

## Dependency Injection

- [ ] Register Repository
- [ ] Register Service

## Frontend Support

- [ ] Complete endpoints required by the frontend as early as possible
- [ ] Share updated `API_ENDPOINTS.md` with the frontend developer

---

# Phase 4 — Database

- [ ] Configure `OnModelCreating()`
- [ ] Create Migration
- [ ] Update Database
- [ ] Seed required initial data (if needed)

---

# Phase 5 — Authentication & Authorization

The template already includes JWT authentication.

Verify and update only what the project requires.

- [ ] Verify JWT configuration
- [ ] Configure Roles (if required)
- [ ] Add Authorization Policies (if required)
- [ ] Verify protected endpoints

---

# Phase 6 — Validation

- [ ] Request validation
- [ ] Duplicate checks
- [ ] Business rule validation
- [ ] Exception handling
- [ ] Consistent API responses

---

# Phase 7 — Testing

For every endpoint verify:

- [ ] Success case
- [ ] Invalid request
- [ ] Validation failure
- [ ] Unauthorized request
- [ ] Forbidden request (Role based)
- [ ] Not Found
- [ ] Edge cases

---

# Phase 8 — Documentation

- [ ] Generate `API_ENDPOINTS.md`
- [ ] Update `DATABASE_GUIDE.md`
- [ ] Update `FRONTEND_GUIDE.md`

---

# Phase 9 — Deployment

## Azure

- [ ] Create Azure App Service
- [ ] Create Azure SQL Database

## Azure Configuration

- [ ] Add Connection String
- [ ] Add JWT configuration
- [ ] Add Email configuration
- [ ] Add all required Application Settings

## GitHub

- [ ] Configure GitHub Secrets
- [ ] Configure GitHub Environment (production)

## CI/CD

- [ ] Verify GitHub Actions
- [ ] Push to `main`
- [ ] Verify CI succeeds
- [ ] Verify automatic deployment
- [ ] Verify production API

---

# Final Review

## Backend

- [ ] Remove debug code
- [ ] Remove unused code
- [ ] Remove temporary data
- [ ] Verify logging
- [ ] Verify configuration
- [ ] Verify Swagger

## API

- [ ] Verify endpoint naming
- [ ] Verify HTTP status codes
- [ ] Verify authorization
- [ ] Verify validation

## Project

- [ ] Verify all required features are completed
- [ ] Final backend review before presentation