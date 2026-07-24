# Team Leader Guide

Use this guide throughout the hackathon to keep the team organized and focused.

As Team Leader, you have final decision power on:
- MVP scope
- Technical decisions
- Task assignments
- What to cut if time is running short

You are also responsible for tracking overall progress (with support from Member 4).

---

# Before the Hackathon

## Preparation

- [ ] Prepare and test the backend template
- [ ] Verify CI/CD pipeline works
- [ ] Verify Azure deployment works
- [ ] Verify Azure account access and subscription limits
- [ ] Verify GitHub access and permissions
- [ ] Verify required software is installed (IDE, Node, .NET, etc.)
- [ ] Verify all team members can run the project locally
- [ ] Ensure `planning_guide.md`, `team.md`, and related docs are ready

---

# Phase 1 — Problem Statement

Immediately after the problem statement is released:

- [ ] Read the complete problem statement
- [ ] Use AI (with this guide) to analyze the problem:
  - Run Phase 1 prompt from `planning_guide.md`
  - Review AI output with the team
- [ ] Discuss AI suggestions with the team
- [ ] Finalize the MVP (using Phase 2 prompt from `planning_guide.md`)
- [ ] Remove unnecessary features
- [ ] Confirm the final project idea and scope

---

# Phase 2 — Team Planning

- [ ] Review `planning_guide.md` with the team
- [ ] Review and confirm `team.md` responsibilities:
  - Member 1: Team Leader, Backend, DB, DevOps, decisions, progress
  - Member 2: Frontend implementation & API integration
  - Member 3: Presentation & demo delivery
  - Member 4: QA (API + frontend) + progress tracking
- [ ] Ensure everyone understands their tasks
- [ ] Create the GitHub repository
- [ ] Share repository access with the team
- [ ] Confirm communication method (Discord/WhatsApp/Slack, etc.)
- [ ] Set up a simple progress board (TODO/DOING/DONE) – Member 4 will maintain it

---

# Phase 3 — Backend Preparation

- [ ] Clone the backend template
- [ ] Rename solution and projects
- [ ] Update namespaces and base configuration
- [ ] Push initial commit
- [ ] Verify everyone can pull and run the project
- [ ] Share initial `endpoints.md` and `dtos.md` structure (even if empty)

---

# Phase 4 — Database

- [ ] Finalize entities for MVP
- [ ] Review relationships and keys
- [ ] Approve database design
- [ ] Create initial migration
- [ ] Update database and verify with the team
- [ ] Document key entities briefly (for reference)

---

# Phase 5 — API

- [ ] Finalize API modules (e.g., Auth, Complaints, Admin)
- [ ] Decide endpoint structure and naming
- [ ] Prioritize endpoints needed by the frontend first
- [ ] Complete authentication and role-based access
- [ ] As each module is done:
  - Update `endpoints.md` and `dtos.md`
  - Share with frontend developer (Member 2)
- [ ] Ensure Member 4 can test APIs with Swagger/Postman

---

# Phase 6 — Monitor Progress

Throughout the hackathon:

- [ ] Check progress regularly (e.g., every 2–3 hours)
- [ ] Use Member 4’s progress board to see:
  - What’s done
  - What’s in progress
  - What’s blocked
- [ ] Help teammates when blocked (especially backend/frontend integration)
- [ ] Reassign work if necessary to keep MVP on track
- [ ] Keep everyone focused on the MVP
- [ ] Avoid unnecessary features and scope creep
- [ ] Track remaining time and adjust priorities accordingly
- [ ] Make final decisions on what to cut if time is tight

---

# Phase 7 — Deployment

- [ ] Create Azure resources (App Service, DB, etc.)
- [ ] Configure GitHub Secrets (connection strings, JWT keys, etc.)
- [ ] Verify CI/CD pipeline deploys correctly
- [ ] Deploy backend and frontend (even in minimal form)
- [ ] Verify production environment:
  - APIs reachable
  - Frontend connected to correct API URL
  - Authentication working end-to-end

---

# Phase 8 — Testing

Work closely with Member 4 (QA):

- [ ] Test backend APIs:
  - Auth, roles, main CRUD flows
- [ ] Test frontend integration:
  - Student flow (submit, view, track complaints)
  - Admin flow (view, update status, add notes)
- [ ] Verify authentication and role separation
- [ ] Verify database operations (no data loss, correct relations)
- [ ] Verify deployment (test on live Azure URL)
- [ ] Fix critical bugs first; ignore cosmetic issues unless time permits

---

# Phase 9 — Presentation Preparation

Work with Member 3 (Presentation):

- [ ] Review completed features with Member 3
- [ ] Decide what to show in the demo (stick to MVP + 1–2 nice extras)
- [ ] Remove or hide unfinished features if necessary
- [ ] Prepare demo flow:
  - Show problem statement
  - Show student journey
  - Show admin journey
  - Highlight key technical points (stack, security, deployment)
- [ ] Support Member 3 in preparing slides:
  - Provide tech stack details
  - Provide architecture overview (high-level)
  - Provide impact and future scope points
- [ ] Practice the demo with the team
- [ ] Assign speaking responsibilities (who says what)

---

# Phase 10 — Final Submission

Before submitting:

- [ ] Push all code to the main branch
- [ ] Verify deployment is live and stable
- [ ] Verify repository is clean and accessible to judges
- [ ] Verify documentation:
  - `README.md` with setup and run instructions
  - Any additional docs judges might need
- [ ] Verify judges can access the application:
  - Public URL works
  - Demo credentials (if any) are provided
- [ ] Confirm all team members are ready for submission and demo

---

# Team Leader Reminders

Always remember:

- Keep the MVP first; everything else is optional.
- Don’t let the team spend too much time on one feature.
- Deliver a working product before adding extra features.
- Keep communication clear and short.
- Help remove blockers quickly (especially integration issues).
- Leave time for testing and presentation.
- Stay calm and make decisions based on the remaining time.
- You have final decision power; use it to protect the MVP and timeline.
- Use Member 4’s progress tracking to guide your decisions.