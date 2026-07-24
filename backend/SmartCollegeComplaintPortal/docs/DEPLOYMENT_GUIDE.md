# Deployment Guide

## When to use

Complete this checklist after:

- Cloning the template
- Renaming the solution
- Renaming the projects
- Updating namespaces
- Verifying the project builds successfully

---

## Azure

Required:

- Azure App Service
- Publish Profile
- Azure sql db 
---

## Environment Configuration

- Add database connection string in environment variables (`env`).
- Add JWT and email-related configuration in `appsettings.json` and corresponding environment variables.
- Add file upload credentials/configuration in `appsettings.json`.

### Required Configurations

- Database Connection String
- JWT Settings
  - Secret Key
  - Issuer
  - Audience
  - Expiry
- Email Settings
  - SMTP Host
  - Port
  - Username
  - Password
  - Sender Email
- File Upload Settings(look in connection string to know what to keep )
  - Container Name
  - connection string 
---

## GitHub Secrets

Repository or Environment Secrets:

- `AZURE_WEBAPP_NAME`
- `AZURE_WEBAPP_PUBLISH_PROFILE`

---

## GitHub Environment

Required Environment:

- `production`

---

## Deployment

After pushing to the `main` branch:

- CI builds the project.
- If CI succeeds, the API is published.
- The published output is deployed automatically to Azure App Service.

No manual deployment steps are required.

---

## Notes

- The workflow automatically detects the solution file.
- The workflow automatically detects the ASP.NET Core Web API project.
- No project names are hardcoded.
- After renaming the solution, projects, and namespaces, no workflow changes are required.

## Important

This document is intended to be a quick deployment reference only.

Do not add additional explanations unless they are necessary.
