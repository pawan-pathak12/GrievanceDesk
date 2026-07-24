# Planning Guide (24-Hour Hackathon)

Use this guide immediately after the problem statement is released.

**Do not start coding until Phase 10 (“Ready to Start”) is complete.**

---

# Phase 1 — Understand the Problem (Target: 45–60 min)

## Read Carefully

- [ ] Read the complete problem statement
- [ ] Identify mandatory requirements
- [ ] Identify optional features
- [ ] List assumptions
- [ ] Clarify unclear requirements (if Q&A is allowed)

### AI Prompt for Phase 1 — Problem Statement Analysis

# Phase 1 — Problem Statement Analysis

## Goal

You are NOT designing the application yet.

Your only goal is to help the team completely understand the problem statement before making any implementation decisions.

Do not suggest database entities, APIs, architecture, or features yet.

Read the entire problem statement carefully and analyze it from the perspective of a hackathon judge, product manager, and software architect.

output : give me in one md code not file just code 
---

## Context

We are participating in a **24-hour software hackathon**.

Our objective is to build the smallest complete solution that best solves the given problem.

Do not recommend unnecessary features.

Think about what is required to solve the actual problem.

---

# Problem Statement

(Paste the complete problem statement here.)

---

## Analyze

### 1. Explain the Problem Statement 📖

Explain the problem in simple language.

Include:

- What exactly is the organizer asking us to build?
- What is the actual problem behind the statement?
- Why does this problem exist?
- What assumptions should we avoid?

---

### 2. Core Problem & Pain Points 🔍

Identify:

- Current workflow
- Existing problems
- Why current solutions are insufficient
- Real pain points

Separate actual pain points from assumptions.

---

### 3. Innovation & Existing Solutions 💡

Research similar products, websites, startups, government portals, or research papers.

For each one explain:

- What they do well
- Their limitations
- What gap still exists

Finally answer:

**Where is the opportunity for innovation?**

Remember:

Innovation does NOT always mean AI or Blockchain.

Sometimes a better workflow or user experience is more valuable.

---

### 4. Feasibility Analysis ⚙️

Evaluate whether the problem can realistically be solved in a 24-hour hackathon.

Discuss:

- Overall complexity
- Major technical challenges
- Data availability
- Integration challenges
- Biggest implementation risks

Finally classify:

- 🟢 Easy
- 🟡 Medium
- 🔴 Difficult

Explain why.

---

### 5. Impact & Sustainability 🌍

Analyze:

- Who benefits?
- How does it improve existing processes?
- Social impact
- Business impact
- Educational impact
- Government or organizational impact (if applicable)

Also explain:

Can this realistically grow into a real product after the hackathon?

---

### 6. What Actually Needs to be Solved ✅

This is the most important section.

List only the problems that absolutely need to be solved.

Do NOT list features.

List actual objectives.

Example:

❌ User Login

❌ Dashboard

✔ Students cannot easily report issues.

✔ Administrators cannot efficiently track complaints.

✔ Users receive no status updates.

---

### 7. Key Takeaways ⭐

Summarize:

- Biggest pain point
- Biggest opportunity
- Biggest risk
- Innovation opportunity
- Overall understanding of the problem

---

## Rules

- Do NOT suggest features.
- Do NOT design the system.
- Do NOT suggest database entities.
- Do NOT suggest APIs.
- Do NOT recommend technologies.
- Do NOT create an MVP.
- Do NOT discuss architecture.

Focus ONLY on understanding the problem.

---

## After Completion

Stop.

Wait for my confirmation.

Only continue to **Phase 2 (MVP Definition)** after I explicitly say:

**Next**

---

Use the AI response as a planning guide only.

The final decisions should always be made by the team.


---

## After Completion

- [ ] Team reads and discusses AI output
- [ ] Team agrees on the core problem and pain points
- [ ] No solution design yet

Only continue to **Phase 2 (MVP Definition)** after the team says:

> **Next**

---

# Phase 2 — Define the MVP (Target: 60–90 min)

Now that we fully understand the problem statement, help us define a realistic MVP.

Our team will build this project in a 24-hour software hackathon.

## Context
### Backend
- ASP.NET Core Web API
- MS SQL Server
- N-Tier Architecture
- Dapper for data access
- EF Core for migrations
- JWT Authentication template already completed
- Email-based Registration template already completed
- Generic Repository/Service/Controller generation workflow already prepared
- Azure App Service deployment template ready
- GitHub Actions CI/CD pipeline ready

### Frontend
- React
- JavaScript

### Development Status

Our backend template is already prepared.

It already includes:

- Authentication infrastructure
- Email service
- JWT
- Database setup
- CI/CD
- Azure deployment
- Generic CRUD architecture
- Repository/Service/Controller code generation workflow
- just will be genrate wil ai most of code that is repeated like        mutimple repo
- all the thing is tested too in template kit 
So during the hackathon we mainly need to implement business logic, entities, APIs, frontend, and presentation.

---
## Your Task

Considering:

- the problem statement,
- the 24-hour timeline,
- and our existing backend template,

define the project scope.

Provide:

### 1. Core Features (Must Build)

List only the features absolutely necessary to solve the problem statement.  
Explain briefly why each feature is important.

---

### 2. Secondary Features (If Time Allows)

Features that improve the project but are not required for judging.

---

### 3. Stretch Goals

Nice demo features if everything finishes early.

---

### 4. Feature Prioritization

Categorize everything into:

- 🔴 Must Have
- 🟡 Should Have
- 🟢 Nice to Have

---

### 5. Reality Check

Review the proposed feature list. Identify:

- features that are too risky
- features that are unnecessary
- features that would consume too much time
- features that can be simplified

Suggest a leaner MVP if needed.

---

### 6. Technical Fit

For every core feature, explain whether it fits well with our stack:

- ASP.NET Core API
- MS SQL
- Dapper
- React
- Azure Deployment

Mention any feature that may require an additional third-party service or API.

---

### 7. Final MVP Recommendation
first drap MVP we will look over it and tell our views and once lock and we tell you to genrate final MVP file then give me in md format .

Provide one finalized MVP that:

- Can realistically be completed in 24 hours
- Still makes a strong impression on judges

Do not proceed to architecture or module breakdown yet.

Wait for team approval before moving to the next phase.

---

## After Completion

- [ ] Team reviews MVP
- [ ] Team agrees on final MVP scope
- [ ] Anything not in MVP is explicitly marked “out of scope”

Only continue to **Phase 3** after the team says:

> **Next**

---

# Phase 3 — Technical Decisions & Risk Assessment (Target: 45–60 min)

Finalize the project’s technical decisions and identify high-risk features before breaking down modules or dividing work.

## 1. Technical Decisions

Lock in the core technical choices so they don’t change during development.

- [ ] Confirm project architecture  
  (e.g., N-Tier, folder structure, separation of concerns)
- [ ] Confirm database technology  
  (e.g., MS SQL Server, any special extensions or constraints)
- [ ] Confirm authentication & authorization method  
  (e.g., JWT, role-based access, email-based registration)
- [ ] Confirm deployment platform  
  (e.g., Azure App Service,  app service, environment setup, new db creation )

- [ ] Confirm third-party services  
  (e.g., email provider, file storage, maps, payment, AI APIs)
- [ ] Confirm coding conventions  
  (naming, file organization, error handling, logging approach)
- [ ] Confirm Git workflow  
  (branch strategy, PR process, code review expectations, merge rules)

**Rule:** Avoid changing these technical decisions once development has started.

---

## 2. Risk Assessment

Identify difficult or high-risk features that could block progress or consume too much time.

Examples:

- Email sending
- File upload & storage
- Payments
- AI / ML integration
- Maps / geolocation
- Complex third-party APIs
- Real-time features (WebSockets, live updates)

For **each risky feature**:

- [ ] Decide whether it belongs in the MVP, secondary, or stretch
- [ ] Define the simplest viable version (e.g., single image upload, basic email text, mock integration)
- [ ] Identify fallback options if integration fails (e.g., mock data, disabled feature flag)
- [ ] Note any extra setup needed (API keys, accounts, quotas, limits)

---

## 3. Output of This Phase

By the end of Phase 3, the team should have:

- [ ] A short “Technical Decisions” summary (1–2 paragraphs or a small table)
- [ ] A “Risky Features” list with:
  - Feature name
  - Why it’s risky
  - MVP / Secondary / Stretch classification
  - Simplest version definition
  - Fallback plan

Use this output to guide:

- Module breakdown (Phase 4)
- Team division (Phase 5)
- Development order (Phase 7)

---
# Phase 4 — Feature / Module Breakdown (Target: 30–45 min)

Split the project into independent, small modules.

Example:

- [ ] Authentication & Roles
- [ ] Complaints (Student)
- [ ] Complaints (Admin)
- [ ] File Upload (Evidence)
- [ ] Notifications (Email)
- [ ] Admin Stats / Dashboard

Each module should be:

- Small enough for one person to own
- Loosely coupled from others

---

# Phase 5 — Team Division (Target: 30 min)

Review `TEAM_MEMBER_WORK.md` (or equivalent).

- [ ] Assign backend work
- [ ] Assign frontend work
- [ ] Assign UI/UX work
- [ ] Assign testing/support work
- [ ] Assign presentation responsibilities
- [ ] Confirm backup responsibilities if someone gets blocked

Every team member should clearly understand their responsibilities before coding starts.

---

# Phase 6 — Technical Decisions (Target: 30 min)

Finalize the project’s technical decisions.

-[] Comfirm frontend tech 
- [ ] Confirm project architecture (e.g., N-Tier, folder structure)
- [ ] Confirm database technology (already: MS SQL)
- [ ] Confirm authentication method (already: JWT + email)
- [ ] Confirm deployment platform (already: Azure App Service)
- [ ] Confirm third-party services (if any)
- [ ] Confirm coding conventions (naming, branching, commits)
- [ ] Confirm Git workflow (e.g., main/dev branches, PR rules)

Avoid changing technical decisions once development has started.

---
# Phase 7 — Development Order (Target: 20–30 min)
### no need to plan just look everything is fine -- just pass to ai it check if fine then continue to next phase s

Agree on the sequence in which features will be implemented so frontend and backend work stays synchronized.

Use this as a checklist during the hackathon:

1. **Backend Project Setup**
   - [ ] Create solution/projects using the template  
   - [ ] Configure DI, logging, error handling, CORS, etc.

2. **Database & Entities**
   - [ ] Finalize MVP entities and relationships  
   - [ ] Create and apply initial migration  
   - [ ] Verify tables in DB

3. **Authentication & Roles**
   - [ ] Ensure JWT + email registration works  
   - [ ] Define roles (e.g., Student/Admin) and policies  
   - [ ] Test login and role-based access on a dummy endpoint

4. **Core Backend (MVP APIs)**
   For each MVP module (e.g., Complaints):
   - [ ] Create DTOs (request/response)  
   - [ ] Create controller actions (endpoints)  
   - [ ] Implement basic service + repository logic (working version)  
   - [ ] Test endpoints with Swagger/Postman  

   Immediately after:
   - [ ] Create/update `endpoints.md`:
     - URL, HTTP method, auth required, role, short description  
   - [ ] Create/update `dtos.md` (or include in the same file):
     - Request body shape, response shape, example JSON

5. **Frontend Integration (Core Flows)**
   Frontend developers use `endpoints.md` + `dtos.md` to:
   - [ ] Create API clients/services (e.g., `complaintApi.js`)  
   - [ ] Build core pages:
     - Student: submit complaint, list complaints, view detail with status  
     - Admin: list all complaints, view detail, change status, add note  
   - [ ] Connect UI to real APIs, handle loading/error states

6. **Additional Features**
   - [ ] Implement secondary features (e.g., email notifications, filters, simple stats) using the same pattern:
     - Backend: DTOs → controller → test → update `endpoints.md`  
     - Frontend: integrate using updated docs  
   - [ ] Only start stretch goals if core + secondary are stable

7. **Testing (Critical Paths)**
   - [ ] Walk through full student flow end-to-end  
   - [ ] Walk through full admin flow end-to-end  
   - [ ] Fix critical bugs only (no nice-to-have polish yet)

8. **Deployment**
   - [ ] Deploy backend to Azure (even with minimal features)  
   - [ ] Deploy frontend and connect to deployed API  
   - [ ] Re-test core flows on the deployed environment

9. **Presentation Preparation**
   - [ ] Prepare demo data (sample complaints with different statuses)  
   - [ ] Write a short demo script (who shows what, in what order)  
   - [ ] Practice the demo once or twice

**Rule:** For every feature, follow this mini-flow:

> Backend DTOs + controller working → update `endpoints.md`/`dtos.md` → frontend integration.
---


# Phase 8 — Database Planning (Backend) (Target: 45–60 min)

After the project scope has been finalized:

- [ ] Identify entities
- [ ] Define relationships
- [ ] Identify foreign keys
- [ ] Design the database schema
- [ ] Review the schema with the team
- [ ] Finalize the ER Diagram

Keep the design simple and focused on the MVP.

---

# Phase 9 — API Planning (Backend) (Target: 45–60 min)

Before backend implementation:

Use this phase to produce a **clear, minimal API plan** that both backend and frontend can follow without constant re-discussion.

For each module, determine and document:

- [ ] **Required endpoints**
  - List all endpoints needed for the module (e.g., `POST /api/complaints`, `GET /api/admin/complaints`, etc.).
  - Group them by controller (e.g., `AuthController`, `ComplaintsController`, `AdminComplaintsController`).

- [ ] **CRUD operations**
  - Identify which endpoints correspond to:
    - Create (e.g., `POST /api/complaints`)
    - Read (e.g., `GET /api/complaints/{id}`, `GET /api/admin/complaints`)
    - Update (e.g., `PUT /api/admin/complaints/{id}/status`)
    - Delete (if any; for MVP you may not need delete)

- [ ] **Authentication requirements**
  - Mark each endpoint as:
    - Public (no auth, e.g., `/api/auth/login`, `/api/auth/register`)
    - Authenticated (requires JWT)
  - Note any special cases (e.g., password reset, if added later).

- [ ] **Required roles**
  - For each authenticated endpoint, specify:
    - `Student` only
    - `Admin` only
    - Either (if applicable)
  - Example:
    - `POST /api/complaints` → `Student`
    - `GET /api/admin/complaints` → `Admin`

- [ ] **Validation requirements (minimal)**
  - Use basic ASP.NET Core model validation attributes in DTOs:
    - `[Required]`, `[StringLength]`, `[EmailAddress]`, etc.
  - Do **not** use FluentValidation for the hackathon.
  - Only define essential validation:
    - Required fields (title, description, email, password, etc.)
    - Basic length/format constraints

- [ ] **File upload requirements (if any)**
  - Identify endpoints that accept files:
    - `POST /api/complaints` (optional image evidence)
  - Define:
    - Field name (e.g., `Evidence`)
    - Expected file type(s) (e.g., jpg/png)
    - Max size (e.g., 2–5 MB, enforced in code)

- [ ] **External integrations (if any)**
  - For Phase 1, likely none besides:
    - Existing email service (used in Phase 2 only)
  - Note any third-party services briefly (e.g., “Email via existing template”, “File storage: Azure Blob or local `/uploads`”).

---

## Output of This Phase

The result of this phase should be:

- A **single API planning document** (e.g., `phase9_api_plan.md`) that includes:
  1. **Full list of endpoints** required for the project.
  2. **Endpoints grouped by controller** in a simple table:
     - Method, URL, Auth required, Role, Operation.
  3. **DTO definitions** (request/response shapes) used in those endpoints:
     - Only the fields, not full C# code.
     - Assume responses are wrapped in your standard `ApiResponse` / `ErrorResponse`- i will give strandrt resposne style .

- This document should be:
  - **Clear enough** for frontend developers to start building API clients.
  - **Minimal and clean**, avoiding over-specification.
  - Focused on **MVP operations only** (Phase 1, plus a brief note on Phase 2 email logic as internal, not new endpoints).

Once this API plan is approved by the Team Leader, the team should **not** re-discuss endpoint structure repeatedly; instead:

- Backend implements endpoints according to this plan.
- Frontend integrates using this plan + `endpoints.md` / `dtos.md` derived from it.
- Any changes to the API contract must be approved by the Team Leader and reflected in the docs.

# Phase 10 — Ready to Start (Target: 15 min)

Before anyone starts writing code:

- [ ] MVP finalized and agreed
- [ ] Team responsibilities assigned
- [ ] Technical decisions finalized
- [ ] Development order agreed
- [ ] Risks identified and mitigated
- [ ] Database design approved
- [ ] API plan approved
- [ ] GitHub repository ready (branches, protections, CI/CD)
- [ ] Everyone understands their responsibilities

Only begin development after this checklist is complete.

---

# During the Hackathon

- [ ] Follow the development order
- [ ] Keep scope strict: MVP first, then secondary, then stretch
- [ ] Revisit risk list if something gets blocked
- [ ] Keep communication tight: short stand-ups, clear blockers

---

# Post-Hackathon (Optional)

- [ ] Document what worked / didn’t in this planning process
- [ ] Update this `planning_doc.md` for future hackathons