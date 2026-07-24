# AI-Assisted Planning Workflow

This document explains how the team uses AI together with the planning docs (`planning_guide.md`, `team.md`, etc.) from problem selection up to development.

**Who uses this?**  
- Primarily the **Team Leader** (you), with input from the whole team during discussion phases.  
- During the hackathon, the team follows the already-finalized plans; this workflow is mainly for **pre-hackathon / planning session**.

**When is this used?**  
- Before the hackathon, in the planning meeting where you:
  - Choose the problem statement
  - Run Phase 1–Phase 4 prompts with AI
  - Finalize MVP, modules, DB, and API plans

Once Phase 10 (“Ready to Start”) is complete, you **do not** re-run this workflow; you just execute the plan.

---

## Step 1 – Problem Statement Selection (Pre-Phase 1)

### Goal
Pick one problem statement out of several options using AI-assisted analysis.

### Process

1. **Each team member selects 1–2 candidate problem statements.**
   - For each problem statement, prepare:
     - Basic information (theme, organizer, constraints)
     - Why it might be needed / impactful
     - Basic understanding of the problem (in your own words)
   - Time: 10–15 minutes per person.

2. **Discuss all shortlisted problem statements as a team.**
   - Share:
     - Initial impressions
     - perceived difficulty
     - potential impact
   - Aim to narrow down to **1 final problem statement** by consensus.

3. **(Optional) Use AI to help compare.**
   - You can ask AI:
     - “Compare these 3 problem statements for a 24-hour hackathon: feasibility, impact, originality.”
   - Use AI output as input for discussion, not as the final decision.

4. **Lock the final problem statement.**
   - Once chosen, this becomes the basis for all further planning.

---

## Step 2 – Phase 1: Problem Understanding (With AI)

### Goal
Get a deep, shared understanding of the chosen problem statement before designing anything.

### Actions

1. **Team Leader sends Phase 1 prompt to AI** with:
   - The final problem statement  
   - Theme  
   - 24-hour hackathon context  

2. **Read AI’s output as a team.**

3. **Discuss and combine:**
   - Ideas
   - Views
   - Notes
   - Any disagreements or assumptions

4. **Record your combined understanding** (in a scratch doc or directly in `planning.md`):
   - Core problem
   - Pain points
   - What actually needs to be solved

5. **Only after this discussion**, tell AI:
   - “Here is our final understanding of the problem: [paste your combined version]. Keep this as context for Phase 2.”

Do **not** move to Phase 2 until the team agrees on this understanding.

---

## Step 3 – Phase 2: MVP Definition (With AI)

### Goal
Define a realistic MVP for 24 hours with clear phases (Phase 1 core, Phase 2 stretch).

### Actions

1. **Team Leader sends Phase 2 prompt to AI** with:
   - Problem statement
   - Tech stack (ASP.NET Core, React, Azure, etc.)
   - 24-hour constraint

2. **Read AI’s MVP proposal.**

3. **Perform deep analysis as a team:**
   - Are the core features truly necessary?
   - Is the scope realistic for 24 hours?
   - What can be cut if time is tight?

4. **Discuss and combine:**
   - Ideas
   - Views
   - Notes
   - Final MVP scope (Phase 1 core + Phase 2 optional)

5. **Explain your combined MVP back to AI**:
   - “Our final MVP is: [paste your Phase 1 + Phase 2 features]. Use this for further planning.”

6. **Ask AI to generate planning files in Markdown**:
   - `mvc_plan.md` (or `mvp_plan.md`)
   - Later: modules, DB, API, etc.

> This is where you create the **first draft** of your MVP planning docs.

---

## Step 4 – Phase 3: Technical Decisions & Risk Assessment

### Goal
Lock technical decisions and identify risks before breaking down modules.

### Who uses this?

- **Team Leader** + **Backend Lead** (often the same person) primarily.
- Whole team briefly, to understand risks and constraints.

### Actions

1. **Review AI’s Phase 3 output** (or use your own template).
2. **Decide and confirm:**
   - Architecture
   - Database tech
   - Auth method
   - Deployment platform
   - Third-party services
   - Git workflow
3. **Identify risky features** (file upload, email, etc.) and decide:
   - MVP vs. stretch
   - Simplest version
   - Fallback plan

4. **Update planning docs** with final decisions.

---

## Step 5 – Phase 4: Module Breakdown

### Goal
Split the MVP into small, independent modules.

### Who uses this?

- **Team Leader** drafts this, possibly with AI help.  
- Whole team reviews to ensure modules are understandable and assignable.

### Actions

1. Use AI (or your own structure) to generate:
   - Module list (Auth, Complaints-Student, Complaints-Admin, File Upload, Email, etc.)
   - Short description of each module’s scope.

2. **Final review:**
   - Is every module small enough?
   - Can each be owned by one person?
   - Are dependencies clear (e.g., Auth first, then Complaints)?

3. Lock this as `phase4_modules.md`.

---

## Step 6 – Phases 5–7: Team Division, Tech Decisions, Development Order

These are “normal” planning steps:

- **Phase 5:** Assign modules to team members (`team.md`).  
- **Phase 6:** Confirm technical decisions (already largely done in Phase 3).  
- **Phase 7:** Agree on development order (backend setup → DB → Auth → core APIs → frontend → test → deploy).

This is mostly internal team work; AI can help format or suggest, but the team decides.

---

## Step 8 – Database (DB) Planning

### Goal
Define a minimal, MVP-focused database schema.

### Who uses this?

- **Team Leader / Backend Lead** primarily.  
- Whole team reviews to ensure it supports the MVP.

### Actions

1. Instead of generating the entire DB at once:
   - First list:
     - Table names
     - Main properties/fields
   - Present as:
     - Class diagrams/models, or
     - Simple table definitions

2. Then refine into:
   - Relationships
   - Keys (PK/FK)
   - Basic indexes (if needed)

3. Output: `phase8_database_plan.md` with:
   - Entities overview
   - ER diagram (textual or visual)
   - Migration plan

---

## Step 9 – API Planning

### Goal
Create a clean, minimal API plan that frontend and backend can follow.

### Who uses this?

- **Backend Lead** drafts it (often with AI).  
- **Frontend Dev** reviews to ensure it’s clear enough to build API clients.  
- **QA** uses it to design test cases.

### Actions

1. Generate:
   - Full list of endpoints  
   - Endpoints grouped by controller (table)  
   - DTO definitions (request/response shapes)

2. Keep it:
   - Minimal (no FluentValidation, just basic attributes)  
   - Focused on MVP operations  
   - Clear about:
     - Auth requirements  
     - Roles  
     - File upload endpoints  

3. Output: `phase9_api_plan.md`.

---

## Step 10 – Ready to Start

Before coding:

- Team Leader confirms:
  - MVP finalized  
  - Team responsibilities assigned  
  - Technical decisions finalized  
  - Development order agreed  
  - Risks identified  
  - DB design approved  
  - API plan approved  

Only then does development begin. From this point:

- The team **executes** the plan.  
- AI is used mainly for:
  - Code help  
  - Debugging  
  - Small design tweaks  
- Not for re-defining the whole plan.

---

## How This File Helps

- **For the Team Leader:**
  - A clear checklist of how to use AI in each phase.  
  - Ensures you don’t skip discussion/combination steps.  

- **For the Team:**
  - Understands that:
    - Planning is collaborative + AI-assisted.  
    - Once Phase 10 is done, the plan is locked and executed.  

- **For Future Hackathons:**
  - You can reuse and refine this workflow based on what worked or didn’t.