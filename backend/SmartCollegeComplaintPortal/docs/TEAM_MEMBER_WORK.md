# Team Responsibilities

This document describes each member’s responsibilities for the hackathon.  
All major decisions (MVP, architecture, division of work) are made during the planning phase by the Team Leader (with AI assistance). Once decided, this document and related planning docs are updated and followed during development.

---

## Member 1 — Team Leader, Backend Lead & DevOps

**Primary focus:** Planning, backend, database, integrations, deployment, decision-making, progress tracking.

### Responsibilities

- **Planning & Decision Making**
  - Lead planning sessions (with AI assistance) using `planning_guide.md`
  - Finalize MVP scope, features, and priorities
  - Make final technical and architectural decisions
  - Track overall project progress and adjust tasks as needed
  - Unblock team members when they face issues

- **Backend & Database**
  - Design database schema and ER diagram
  - Implement core backend architecture (N-Tier, repositories, services)
  - Implement authentication & authorization (JWT, roles, policies)
  - Develop core APIs (MVP features first)
  - Write and review critical backend code
  - Ensure API documentation (`endpoints.md`, `dtos.md`) is kept up to date

- **Integrations & DevOps**
  - Handle any third-party integrations (email, file storage, etc.)
  - Manage GitHub repository:
    - Branch strategy, PR flow, code reviews
  - Configure and maintain CI/CD (GitHub Actions)
  - Deploy backend and frontend to Azure
  - Monitor deployment and fix critical infra issues during the hackathon

- **Quality & Standards**
  - Enforce coding conventions and project structure
  - Perform backend code reviews
  - Ensure security basics (auth, validation, error handling) are in place

---

## Member 2 — Frontend Developer

**Primary focus:** React implementation and API integration.

### Responsibilities

- Implement frontend using React + JavaScript
- Consume APIs according to `endpoints.md` and `dtos.md`
- Implement:
  - Routing
  - State management
  - Core pages (student flows, admin flows)
- Handle frontend bug fixing and performance issues
- Coordinate with Member 1 for any API changes or clarifications
- Ensure frontend works with the deployed Azure backend

---

## Member 3 — UI/UX & Presentation

**Primary focus:** Presentation and demo delivery.

### Responsibilities

- Prepare the presentation deck:
  - Problem statement
  - Solution overview
  - Key features
  - Tech stack
  - Impact and future scope
- Lead demo preparation:
  - Work with Team Leader to define demo flow
  - Prepare sample data (e.g., sample complaints with different statuses)
  - rehearse presentation with the team
- Deliver the final demo/presentation to judges
- Assist with basic UI/UX suggestions if time allows (no major frontend ownership)

---

## Member 4 — QA & Support Developer (Progress Tracker)

**Primary focus:** Testing and progress tracking.

### Responsibilities

- **Testing**
  - API testing:
    - Use Swagger/Postman to verify endpoints
    - Check validation, auth, roles, error responses
  - Frontend testing:
    - Follow critical user journeys (student + admin)
    - Report bugs clearly (steps to reproduce, expected vs actual)
  - Perform regression testing after major changes

- **Progress Tracking**
  - Maintain a simple task/progress board (e.g., TODO/DOING/DONE)
  - Regularly check status of:
    - Backend tasks (with Member 1)
    - Frontend tasks (with Member 2)
  - Report current status and blockers to the Team Leader
  - Help update documentation (e.g., `README.md`, testing notes) as needed

- **Support Development**
  - Assist frontend development with small tasks when time allows
  - Assist backend development with simple CRUD endpoints or DTOs when needed

---

## How This Document Is Used

1. **Planning Phase (with AI)**
   - Team Leader (Member 1) uses `planning_guide.md` with AI to:
     - Understand the problem
     - Define MVP
     - Decide technical stack and architecture
     - Break down modules and tasks
   - Based on that, this `team.md` and other docs (`planning.md`, `endpoints.md`, etc.) are updated.

2. **Development Phase**
   - Once the plan is locked:
     - Everyone follows their responsibilities as defined here
     - Changes to scope or roles are decided by the Team Leader
   - All major decisions and changes are reflected back into the `.md` docs to keep everything consistent.

3. **During the Hackathon**
   - Team Leader ensures:
     - Plan is realistic for 24 hours
     - Work is balanced and blockers are removed quickly
     - Deployment and demo preparation are not left for the last minute
   - Member 4 keeps the progress board updated and informs the Team Leader of any delays or risks.