# API_ENDPOINTS Prompt

## Goal

Analyze the current ASP.NET Core API codebase and generate:

Generate the following documentation:
- docs/API_ENDPOINTS.md
- docs/API_DTOS.md

Both documents must reflect the current implementation only.

Do not assume or invent anything.

The document must reflect the current implementation only.

Do not assume or invent endpoints.

---

## Analyze

Read the following:

- Controllers
- Route attributes
- HTTP method attributes
- Authorization attributes
- DTOs
- Request models
- Response models
- Validation attributes
- Role-based authorization
- API versioning (if used)

---

## Document Structure

# API Endpoints

## Authentication

Include:

- Authentication method
- Authorization header format
- Available Roles
- Any global authorization requirements

---

For every Controller create a section.

Example:

# AuthController

Start with a summary table.

| Method | Endpoint | Auth | Role | Description |

---

After the table document every endpoint.

Include:

- HTTP Method
- Route
- Purpose
- Authentication Required
- Required Role(s)
- Route Parameters
- Query Parameters
- Request DTO
- Response DTO
- Success Status Codes
- Possible Error Status Codes
- Validation Requirements
- Notes (if applicable)

---

## Rules

- Only document endpoints that exist.
- Do not invent DTOs.
- Do not invent request bodies.
- Do not invent response bodies.
- Do not invent roles.
- Do not duplicate DTO definitions.
- Keep descriptions concise.
- Use Markdown tables whenever possible.
- Group endpoints by Controller.
- Sort endpoints logically.
- Ensure routes exactly match the codebase.
- Every DTO referenced by an endpoint must exist in API_DTOS.md.
- Do not duplicate DTO definitions.
- If a DTO is reused, reference it instead of documenting it again.

---

## Validation

Before finishing:

- Verify every controller is documented.
- Verify every endpoint appears exactly once.
- Verify all routes match the implementation.
- Verify all authorization requirements are correct.
- Verify all DTO names are correct.

---

## DTO Documentation

Generate a second file:

docs/API_DTOS.md

For every DTO used by the API, document:

- DTO Name
- Purpose
- Used By (Endpoint(s))
- Properties
- Property Type
- Required / Optional
- Validation Attributes
- Default Values (if any)
- Example JSON

-also add all the enum that will be used in the project in this file but with section : #Enum 
Group DTOs by Controller or Feature.

Document each DTO only once even if used by multiple endpoints.

## Output

Create:

docs/API_ENDPOINTS.md
docs/API_DTOS.md
Overwrite the existing file if it already exists.