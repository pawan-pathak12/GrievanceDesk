
# Phase 2 — MVP Draft

This document defines the first draft MVP for the Smart College Complaint Portal.

The goal is to keep the scope realistic for a 24-hour hackathon while matching the actual complaint workflow:

Student -> Staff filter -> Admin/Principal action -> Student email update

---

## 1. Core Features (Must Build)

### 1. Complaint Submission
Students must be able to submit a complaint with basic details such as title, description, category, and location. This is the core action that starts the complaint flow.

### 2. View My Complaints
Students must be able to see the complaints they submitted. This gives transparency and helps them track their own issues.

### 3. Complaint Detail View
Students must be able to open a complaint and see its current status and related updates. This completes the basic tracking flow.

### 4. Staff Complaint Review
Staff must be able to view all complaints submitted by students. Staff acts as the middle layer in the system and reviews incoming complaints before forwarding them.

### 5. Duplicate Complaint Filtering
Staff must be able to identify and filter duplicate complaints manually. This helps reduce noise and ensures only unique complaints are passed forward.

### 6. Forward Cleaned Complaints to Admin/Principal
After filtering, staff must be able to forward the cleaned complaint list to the admin/principal. This is the main workflow for complaint management in the system.

### 7. Admin/Principal Dashboard
Admin or principal must be able to see the forwarded complaints in one place. This gives the final decision-maker a clean and organized complaint view.

### 8. Complaint Status Update
Admin or principal must be able to update the status of a complaint. This closes the complaint handling loop.

### 9. Email Notification on Status Update
When a complaint is updated, the system must send an email to the student who originally created it. This keeps the student informed without needing to check repeatedly.

### 10. Role-Based Access
The system must support these roles:
- Student
- Staff
- Admin / Principal

Each role must see only its own allowed actions and dashboards.

---

## 2. Secondary Features (If Time Allows)

### 1. Complaint Evidence Upload
Students can attach evidence images when submitting complaints. This makes complaints clearer and easier to verify.

### 2. Staff Notes
Staff can add a short note while reviewing or filtering complaints. This improves internal handling.

### 3. Complaint Status History
Show a simple history of complaint updates on the detail page. This improves transparency.

### 4. Complaint Filtering
Staff or admin can filter complaints by category or status. This makes the dashboard easier to use.

---

## 3. Stretch Goals

### 1. Search Complaints
Allow staff or admin to search complaints by keyword, category, or location.

### 2. Complaint Priority Tagging
Allow admin/principal to mark complaints as low, medium, or high priority.

### 3. Analytics Summary
Show counts of complaint types, duplicate complaints, and resolved complaints.

### 4. Better Notification Flow
Send emails not only on status change but also on important updates or comments.

---

## 4. Feature Prioritization

### 🔴 Must Have
- Complaint submission.
- View my complaints.
- Complaint detail view.
- Staff complaint review.
- Duplicate complaint filtering.
- Forward cleaned complaints to admin/principal.
- Admin/principal dashboard.
- Complaint status update.
- Email notification to original student after update.
- Roles: student, staff, admin/principal.

### 🟡 Should Have
- Complaint evidence upload.
- Staff notes.
- Complaint filtering.
- Complaint status history.

### 🟢 Nice to Have
- Search complaints.
- Priority tagging.
- Analytics summary.
- Better notification flow.

---

## 5. Reality Check

The workflow is realistic for a hackathon if staff filtering stays manual and simple. The main risk is trying to build automatic duplicate detection or a complicated approval chain.

### Too risky
- Automatic duplicate detection.
- Complex escalation logic.
- Multi-step forwarding workflows.
- Advanced analytics.

### Unnecessary for MVP
- Full reporting dashboard.
- Priority automation.
- Detailed resolution workflows.
- Multiple notification channels.

### Can be simplified
- Duplicate filtering can be manual by staff.
- Principal can just see the cleaned complaint list.
- Email notification can be one simple message on update.
- Evidence upload can be optional.

### Leaner MVP suggestion
If the team needs a leaner version, keep only:
- complaint submission,
- student complaint list/detail,
- staff review and duplicate filtering,
- admin/principal dashboard,
- status update,
- email notification to the original student.

That is enough to show the complete complaint flow without overbuilding.

---

## 6. Technical Fit

| Core Feature | ASP.NET Core API | MS SQL | Dapper | React | Azure |
|---|---|---|---|---|---|
| Complaint submission | Very good | Very good | Very good | Very good | Good |
| View my complaints | Very good | Very good | Very good | Very good | Good |
| Complaint detail view | Very good | Very good | Very good | Very good | Good |
| Staff complaint review | Very good | Very good | Very good | Very good | Good |
| Duplicate filtering | Very good | Very good | Very good | Very good | Good |
| Forward cleaned complaints | Very good | Very good | Very good | Very good | Good |
| Admin/principal dashboard | Very good | Very good | Very good | Very good | Good |
| Status update | Very good | Very good | Very good | Very good | Good |
| Email notification | Very good | Very good | Very good | Very good | Good |
| Role-based access | Very good | Good | Good | Very good | Good |

### Third-party service needs
- Email notification may use an SMTP provider or existing email service in the template.
- No other third-party service is required for the MVP.

---

## 7. Final MVP Recommendation

The best MVP is:

### Smart College Complaint Portal
- Student submits a complaint.
- Student views own complaints and complaint detail.
- Staff reviews complaints and filters duplicates manually.
- Staff forwards cleaned complaints to admin/principal.
- Admin/principal sees the cleaned complaint list.
- Admin/principal updates complaint status.
- System sends an email to the original student when a complaint is updated.
- Roles are: student, staff, admin/principal.

This version is strong for a 24-hour hackathon because:
- it solves the real problem clearly,
- it uses your existing backend template well,
- it gives a complete role-based workflow,
- and it is easy to demo.

---

## Out of Scope for MVP
- Automatic duplicate detection.
- Advanced analytics.
- Priority automation.
- Multi-step escalation.
- Multiple notification channels.
- Complex principal workflow.
- Detailed reporting dashboard.
- Search and filtering beyond basic manual review.