# Phase 2 — Define the MVP

## Context

### Problem Statement

Organizations handle complaints through scattered channels — phone calls, emails, WhatsApp, manual registers. Complaints get lost, nobody is accountable, and users never know what happened to their grievance.

**GrievanceDesk** solves this by providing a centralized, organization-controlled complaint management platform with a transparent workflow from submission to resolution.

### What We Already Have (Template Kit)

| Component | Status | Time Saved |
|---|---|---|
| JWT Authentication | ✅ Ready | ~3 hours saved |
| Email-based Registration (OTP flow) | ✅ Ready | ~2 hours saved |
| Email Service (SMTP) | ✅ Ready | ~1 hour saved |
| Generic Repository/Service/Controller pattern | ✅ Ready | ~2 hours saved |
| Database setup (Dapper + EF Core) | ✅ Ready | ~1 hour saved |
| CI/CD (GitHub Actions) | ✅ Ready | ~1 hour saved |
| Azure App Service deployment | ✅ Ready | ~1 hour saved |
| Serilog logging, Rate limiting, CORS, Swagger | ✅ Ready | ~1 hour saved |
| API Response wrapper (ApiResponse/ErrorResponse) | ✅ Ready | ~0.5 hours saved |
| Exception middleware | ✅ Ready | ~0.5 hours saved |

**Estimated time saved from template: ~13 hours**

### What We Actually Need to Build

- Member entity + Member management (Admin pre-registers users)
- Modify auth flow to check against Members table before registration
- Complaint entity + CRUD operations
- ComplaintUpdate entity (history/audit trail)
- Admin complaint management endpoints
- Content validation (blocked words)
- Email notifications on status change
- Dashboard stats endpoints
- Complete React frontend (auth pages, user pages, admin pages)

---

## 1. Core Features (Must Build)

These are the features **absolutely necessary** to demonstrate the complete complaint lifecycle from submission to resolution.

### F1. Organization-Controlled Registration

**What**: Admin pre-adds members (FirstName, LastName, Email, Role) into a Members table. Only people in this table can register.

**Why**: This is the core differentiator — the system is organization-controlled, not open to the public. It makes the system trustworthy and applicable to any organization. Without this, it's just another generic complaint form.

**Template leverage**: Registration flow (OTP → Password) already exists. We only need to add the Members table check before the existing flow triggers.

---

### F2. Complaint Submission (User)

**What**: Authenticated users can create a complaint with Title, Description, Category, and Location.

**Why**: This is the entry point of the entire system. Without it, there is no product.

---

### F3. View My Complaints (User)

**What**: Users can see a list of all their submitted complaints with current status.

**Why**: This is the "tracking" promise. Users need to see what they submitted and where it stands.

---

### F4. Complaint Detail with History Timeline (User)

**What**: Users can view full complaint details plus a chronological timeline of every action taken on it (created, status changed, note added, etc.)

**Why**: This is the **transparency** and **accountability** feature. It's what separates this from a Google Form. Every action is visible. This is demo-impressive.

---

### F5. Admin — View All Complaints

**What**: Admins can see all complaints across the system, with status badges.

**Why**: This is the admin's primary screen. Without it, admin can't manage anything.

---

### F6. Admin — Update Complaint Status

**What**: Admin can change complaint status: Pending → In Progress → Resolved → Closed.

**Why**: This is the core of the workflow. Without status updates, there is no lifecycle management.

---

### F7. Admin — Add Resolution Notes

**What**: Admin can add notes/comments to any complaint. Notes appear in the history timeline.

**Why**: This provides context for the user ("Maintenance team dispatched"). It's the communication channel between admin and user. Critical for accountability.

---

### F8. Complaint History / Audit Trail

**What**: Every action (created, edited, status changed, note added, marked duplicate/forwarded/irrelevant) is logged in a ComplaintUpdates table with who did it and when.

**Why**: This is the **accountability backbone**. Judges will see that nothing can be hidden or lost. This is what makes the system trustworthy.

---

### F9. Dashboard Statistics

**What**: User sees personal stats (my total, pending, resolved...). Admin sees system-wide stats (total complaints, pending, in progress, resolved, today's count, member count).

**Why**: Dashboards are the **first thing judges see**. A dashboard with real numbers immediately shows the system has depth. High visual impact, relatively low development effort.

---

### F10. Role-Based Access Control

**What**: Users can only see/manage their own complaints. Admins can see/manage all complaints. Endpoints are protected by role.

**Why**: Security fundamental. Without this, the system has no integrity.

---

## 2. Secondary Features (If Time Allows)

These improve the product significantly but are not required to demonstrate the core workflow.

### S1. Edit Complaint (User — Only While Pending)

**What**: Users can edit their complaint title/description, but only while status is still Pending. Shows an "Edited" badge after editing.

**Why**: Gives users control. The Pending-only restriction shows thoughtful design. "Edited" badge shows transparency.

---

### S2. Delete Complaint (User — Only While Pending)

**What**: Users can delete their own complaint, but only while status is still Pending.

**Why**: Users should be able to withdraw a complaint they submitted by mistake. The restriction prevents deleting complaints that admin is already working on.

---

### S3. Mark as Irrelevant (Admin)

**What**: Admin can mark a complaint as Irrelevant. Status changes to Irrelevant. These complaints appear in a separate list for both admin and user.

**Why**: Not all complaints are valid. This gives admin a clean way to handle invalid complaints without deleting them (maintaining accountability).

---

### S4. Mark as Duplicate / Forwarded (Admin)

**What**: Admin can flag complaints as duplicate or forwarded. These are badges, not status changes. Logged in history.

**Why**: Helps admin organize complaints. Shows judges the system handles real-world scenarios.

---

### S5. Content Validation (Blocked Words)

**What**: In-memory list of offensive words. Complaints with blocked words are rejected with a message.

**Why**: Shows the system has content moderation. Easy to implement (HashSet check), impressive to demonstrate.

---

### S6. Email Notifications

**What**: Send email to complaint owner when status changes (In Progress, Resolved, Closed, Irrelevant).

**Why**: Closes the communication loop. User doesn't need to keep checking the system. Email service already exists in the template.

---

### S7. Admin — Member Management

**What**: Admin can add, view, update, and remove members from the organization directory.

**Why**: Without this, members must be added directly to the database. A UI for this makes the system self-sufficient.

---

## 3. Stretch Goals

Nice-to-have features if everything finishes early. High demo impact.

### G1. Search & Filter Complaints

**What**: Admin can filter complaints by status, category. Search by title.

**Why**: Makes the admin panel feel professional and complete. Easy to add (query parameters on existing endpoints).

---

### G2. Complaint Sorting

**What**: Sort complaints by newest, oldest, status.

**Why**: Quality-of-life feature. Simple to implement.

---

### G3. Print / Export Complaint Detail

**What**: Button to print or export a single complaint detail as PDF.

**Why**: High demo impact. Shows real-world usability. Can be done with `window.print()` on frontend (no backend work).

---

### G4. Password Reset Flow

**What**: "Forgot Password" → OTP verification → Reset password.

**Why**: Expected by users. OTP flow already exists in template, can be adapted.

---

### G5. Dark Mode

**What**: Toggle between light and dark theme on the frontend.

**Why**: Visual wow factor. Judges love it. Pure CSS/React state, no backend work.

---

## 4. Feature Prioritization

| Priority | Feature | ID | Estimated Time |
|---|---|---|---|
| 🔴 Must Have | Organization-Controlled Registration | F1 | 1.5 hrs |
| 🔴 Must Have | Complaint Submission | F2 | 1 hr |
| 🔴 Must Have | View My Complaints | F3 | 0.5 hr |
| 🔴 Must Have | Complaint Detail + History Timeline | F4 | 1.5 hrs |
| 🔴 Must Have | Admin — View All Complaints | F5 | 0.5 hr |
| 🔴 Must Have | Admin — Update Status | F6 | 0.5 hr |
| 🔴 Must Have | Admin — Add Resolution Notes | F7 | 0.5 hr |
| 🔴 Must Have | Complaint History / Audit Trail | F8 | 1 hr |
| 🔴 Must Have | Dashboard Statistics | F9 | 1 hr |
| 🔴 Must Have | Role-Based Access Control | F10 | 0.5 hr |
| 🟡 Should Have | Edit Complaint (Pending only) | S1 | 0.5 hr |
| 🟡 Should Have | Delete Complaint (Pending only) | S2 | 0.25 hr |
| 🟡 Should Have | Mark as Irrelevant | S3 | 0.5 hr |
| 🟡 Should Have | Mark as Duplicate / Forwarded | S4 | 0.5 hr |
| 🟡 Should Have | Content Validation (Blocked Words) | S5 | 0.5 hr |
| 🟡 Should Have | Email Notifications | S6 | 0.5 hr |
| 🟡 Should Have | Admin Member Management | S7 | 1 hr |
| 🟢 Nice to Have | Search & Filter | G1 | 0.5 hr |
| 🟢 Nice to Have | Sorting | G2 | 0.25 hr |
| 🟢 Nice to Have | Print / Export | G3 | 0.5 hr |
| 🟢 Nice to Have | Password Reset | G4 | 1 hr |
| 🟢 Nice to Have | Dark Mode | G5 | 0.5 hr |

### Time Budget

| Category | Features | Backend | Frontend | Total |
|---|---|---|---|---|
| 🔴 Must Have | F1–F10 | ~4 hrs | ~4.5 hrs | **~8.5 hrs** |
| 🟡 Should Have | S1–S7 | ~2 hrs | ~1.75 hrs | **~3.75 hrs** |
| 🟢 Nice to Have | G1–G5 | ~0.5 hr | ~2.25 hrs | **~2.75 hrs** |
| | **Total** | **~6.5 hrs** | **~8.5 hrs** | **~15 hrs** |

**Available time: ~18 hours. Estimated total: ~15 hours. Buffer: ~3 hours for debugging, testing, demo prep.**

---

## 5. Reality Check

### ⚠️ Features That Are Too Risky

| Feature | Risk | Recommendation |
|---|---|---|
| Full project rename (solution + all namespaces) | Could break build, waste 1-2 hours fixing references | **Keep folder names. Rename only display titles (Swagger title, page headers). Namespaces stay.** |
| Email notifications (S6) | Email delivery can fail, SMTP config issues on Azure | **Keep it simple. Use existing service. If SMTP fails, don't let it block the demo. Make it async/fire-and-forget.** |
| Password Reset (G4) | Involves new OTP flow, edge cases | **Defer to stretch goal. Not needed for demo. Say "planned feature" in pitch.** |

### ⚠️ Features That Are Unnecessary for Demo

| Feature | Why Skip |
|---|---|
| Complex sorting (G2) | SQL `ORDER BY` is trivial. Do it if time allows, skip if not. |
| Bulk member import | Nobody will ask about CSV import at a hackathon. |
| Advanced analytics | A few stat numbers on the dashboard is enough. No need for charts. |

### ⚠️ Features That Could Consume Too Much Time

| Feature | Time Trap | Mitigation |
|---|---|---|
| History Timeline UI (F4) | Making it look good takes time | Use a simple vertical timeline with CSS. Don't over-design. |
| Admin Member Management UI (S7) | Full CRUD UI is time-consuming | **Simplify: Just a form to add members + a table listing them. No edit/delete UI initially. Add members via Swagger if needed.** |
| Content Validation (S5) | Edge cases (partial word matches, Unicode) | **Keep it dead simple: split on spaces, check exact word match. Don't handle substrings or regex.** |

### ✅ Recommended Simplifications

| Original | Simplified Version |
|---|---|
| Separate Irrelevant page | Just filter complaints list by status = Irrelevant. Same page, one filter click. |
| Member CRUD UI | Add member form + list only. Edit/delete via Swagger/API. |
| Edit complaint form | Reuse Create form, pre-fill values. Same component. |
| Email notifications | Fire-and-forget in background. If it fails, log it, don't break the flow. |

### 🎯 Leaner MVP (If Behind Schedule)

If you're running out of time, cut to this absolute minimum:

| Feature | Keep/Cut |
|---|---|
| Registration + Login | ✅ KEEP |
| Create Complaint | ✅ KEEP |
| View My Complaints | ✅ KEEP |
| Complaint Detail + History | ✅ KEEP |
| Admin View All | ✅ KEEP |
| Admin Update Status | ✅ KEEP |
| Admin Add Notes | ✅ KEEP |
| Dashboard | ✅ KEEP |
| Edit/Delete Complaint | ⚡ CUT (add if time) |
| Duplicate/Forward/Irrelevant | ⚡ CUT (add if time) |
| Content Validation | ⚡ CUT (add if time) |
| Email Notifications | ⚡ CUT (add if time) |
| Member Management UI | ⚡ CUT (seed via Swagger) |

**Absolute minimum = 8 features. Estimated: ~8.5 hours. Leaves maximum time for frontend polish.**

---

## 6. Technical Fit

### F1. Organization-Controlled Registration

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | Add Members entity, modify existing auth service |
| MS SQL | ✅ Perfect | Simple Members table with FK to Users |
| Dapper | ✅ Perfect | Email lookup query is one-liner |
| React | ✅ Perfect | Multi-step registration form |
| Azure | ✅ Perfect | No extra services needed |
| Third-party? | ❌ None | Uses existing OTP + Email service |

### F2–F3. Complaint CRUD (Create + View List)

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | Standard controller + service + repository pattern |
| MS SQL | ✅ Perfect | Simple Complaints table |
| Dapper | ✅ Perfect | Ideal for list queries with joins |
| React | ✅ Perfect | Form + table components |
| Azure | ✅ Perfect | No extra services |
| Third-party? | ❌ None | |

### F4. Complaint Detail + History Timeline

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | JOIN Complaints + ComplaintUpdates + Users |
| MS SQL | ✅ Perfect | ComplaintUpdates table with FKs |
| Dapper | ✅ Perfect | Multi-mapping query to join complaint + updates |
| React | ✅ Perfect | Timeline component (vertical list with CSS) |
| Azure | ✅ Perfect | No extra services |
| Third-party? | ❌ None | |

### F5–F7. Admin Complaint Management

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | Separate AdminComplaintsController with `[Authorize(Roles = "Admin")]` |
| MS SQL | ✅ Perfect | UPDATE queries on Complaints table |
| Dapper | ✅ Perfect | Parameterized updates |
| React | ✅ Perfect | Admin panel layout with action buttons |
| Azure | ✅ Perfect | No extra services |
| Third-party? | ❌ None | |

### F8. Complaint History / Audit Trail

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | INSERT into ComplaintUpdates on every action |
| MS SQL | ✅ Perfect | Append-only table, ideal for audit |
| Dapper | ✅ Perfect | Simple INSERT + SELECT queries |
| React | ✅ Perfect | Timeline UI component |
| Azure | ✅ Perfect | No extra services |
| Third-party? | ❌ None | |

### F9. Dashboard Statistics

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | Single endpoint returning counts |
| MS SQL | ✅ Perfect | `SELECT COUNT(*) ... GROUP BY Status` queries |
| Dapper | ✅ Perfect | Aggregate queries are Dapper's strength |
| React | ✅ Perfect | Stat cards (number + label) |
| Azure | ✅ Perfect | No extra services |
| Third-party? | ❌ None | |

### S5. Content Validation (Blocked Words)

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | Static HashSet in a helper class, checked in service layer |
| MS SQL | N/A | No database needed — in-memory only |
| Dapper | N/A | |
| React | ✅ Perfect | Display error message from API |
| Azure | ✅ Perfect | No extra services |
| Third-party? | ❌ None | |

### S6. Email Notifications

| Aspect | Fit | Notes |
|---|---|---|
| ASP.NET Core API | ✅ Perfect | Call existing IEmailService in ComplaintService |
| MS SQL | N/A | |
| Dapper | N/A | |
| React | N/A | No frontend work needed |
| Azure | ⚠️ Watch out | Azure may block port 25/587 SMTP. **Test early.** Use SendGrid if SMTP blocked. |
| Third-party? | ⚠️ Maybe | If Gmail SMTP fails on Azure, may need SendGrid (free tier: 100 emails/day) |

### Summary

| Feature | Stack Fit | Extra Service Needed? |
|---|---|---|
| F1. Registration | ✅ | None |
| F2. Create Complaint | ✅ | None |
| F3. View Complaints | ✅ | None |
| F4. Detail + Timeline | ✅ | None |
| F5. Admin View All | ✅ | None |
| F6. Update Status | ✅ | None |
| F7. Add Notes | ✅ | None |
| F8. History Trail | ✅ | None |
| F9. Dashboard | ✅ | None |
| F10. RBAC | ✅ | None |
| S5. Blocked Words | ✅ | None |
| S6. Email | ⚠️ | Possibly SendGrid if SMTP blocked on Azure |

**Verdict: Everything fits perfectly with the existing stack. No extra services needed except possibly SendGrid for email on Azure.**

---

## 7. Final MVP Recommendation (DRAFT)

> [!IMPORTANT]
> This is a **draft**. Review and provide feedback. Once approved, we will generate the finalized MVP document.

### The MVP

**GrievanceDesk** — A universal, organization-controlled complaint management platform.

### What Judges Will See in the Demo

```text
1. Admin adds a member → (shows org-controlled access)
2. Member registers → email check → OTP → password → logged in
3. User submits complaint → content validated → stored
4. User views dashboard → sees their stats
5. User views complaint list → sees status badge
6. Switch to Admin → sees all complaints across system
7. Admin updates status → adds resolution note
8. Switch back to User → sees updated status + full history timeline
9. Admin marks a complaint as Irrelevant → user sees it
10. Dashboard numbers update in real-time
```

**Demo time: ~3 minutes. Covers the entire lifecycle.**

### What Makes This MVP Strong

| Strength | Why |
|---|---|
| **Real problem** | Every org has complaints. This is universally relatable. |
| **Complete lifecycle** | Submit → Track → Manage → Resolve → History. Nothing is half-built. |
| **Organization-controlled** | Not a public form. Shows enterprise thinking. |
| **Accountability** | Full audit trail. Nothing can be hidden or lost. |
| **Universal** | Same system works for college, bank, hospital, corporate. |
| **Production-ready stack** | .NET 9, MS SQL, JWT, Azure. Not a toy demo. |
| **Content moderation** | Blocked words filter shows responsible design. |

### Feature Lineup

| # | Feature | Priority | Build Order |
|---|---|---|---|
| 1 | Members Table + Admin adds members | 🔴 Must | Phase 1 |
| 2 | Registration (check Members → OTP → Password) | 🔴 Must | Phase 1 |
| 3 | Login + JWT + Role Auth | 🔴 Must | Phase 1 |
| 4 | Create Complaint (with content validation) | 🔴 Must | Phase 2 |
| 5 | View My Complaints | 🔴 Must | Phase 2 |
| 6 | Complaint Detail + History Timeline | 🔴 Must | Phase 2 |
| 7 | Admin — View All Complaints | 🔴 Must | Phase 3 |
| 8 | Admin — Update Status | 🔴 Must | Phase 3 |
| 9 | Admin — Add Resolution Notes | 🔴 Must | Phase 3 |
| 10 | Complaint History (audit trail) | 🔴 Must | Phase 3 |
| 11 | Dashboard (User + Admin) | 🔴 Must | Phase 4 |
| 12 | Edit Complaint (Pending only, "Edited" badge) | 🟡 Should | Phase 5 |
| 13 | Delete Complaint (Pending only) | 🟡 Should | Phase 5 |
| 14 | Mark Irrelevant / Duplicate / Forwarded | 🟡 Should | Phase 5 |
| 15 | Email Notifications | 🟡 Should | Phase 5 |
| 16 | Content Validation (Blocked Words) | 🟡 Should | Phase 5 |
| 17 | Admin Member Management UI | 🟡 Should | Phase 5 |
| 18 | Search & Filter | 🟢 Nice | Phase 6 |
| 19 | Print/Export complaint | 🟢 Nice | Phase 6 |
| 20 | Dark Mode | 🟢 Nice | Phase 6 |

### Time Allocation

| Block | Hours | What |
|---|---|---|
| Backend Core (Must Have) | ~4 hrs | Entities, repos, services, controllers |
| Frontend Core (Must Have) | ~4.5 hrs | Auth pages, complaint pages, admin pages, dashboard |
| Backend Should Have | ~2 hrs | Edit/delete, flags, email, validation |
| Frontend Should Have | ~1.75 hrs | Edit form, delete button, admin actions |
| Nice to Have | ~2.75 hrs | Search, print, dark mode |
| Testing + Demo Prep | ~3 hrs | End-to-end test, seed data, polish |
| **Total** | **~18 hrs** | **Fits in remaining time** |

### One-Line Pitch

> **"We're not building a complaint box. We're building accountability infrastructure."**

---

> **Awaiting team approval. Once confirmed, we proceed to architecture and implementation.**
