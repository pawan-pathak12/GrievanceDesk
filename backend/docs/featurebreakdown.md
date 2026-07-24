# Phase 4 — Feature / Module Breakdown

This phase splits the Smart College Complaint Portal into small, independent modules so the team can work in parallel without confusion.

Each module should be small enough for one person to own and loosely coupled from the others.

---

## Core Modules

### 1. Authentication & Roles
Handles login, registration, JWT auth, and role-based access for Student, Staff, and Admin/Principal.

### 2. Complaint Submission
Allows students to create a new complaint with title, description, category, and location.

### 3. Student Complaint View
Allows students to see their own complaints and open complaint detail pages.

### 4. Staff Complaint Review
Allows staff to view submitted complaints, review them, and filter duplicates manually.

### 5. Complaint Forwarding
Allows staff to forward cleaned complaints to the admin/principal dashboard.

### 6. Admin / Principal Dashboard
Allows admin or principal to view forwarded complaints and manage them.

### 7. Complaint Status Management
Allows admin or principal to update complaint status and add short notes.

### 8. Email Notification
Sends an email to the original student when a complaint status changes.

---

## Secondary Modules

### 9. Complaint Evidence Upload
Allows students to attach one image as evidence when submitting a complaint.

### 10. Complaint Filters
Allows staff or admin to filter complaints by category, status, or date.

### 11. Complaint Status History
Shows previous updates and actions taken on a complaint.

---

## Module Ownership Guidance

- Keep each module focused on one responsibility.
- Avoid combining too many roles or workflows in one module.
- Backend and frontend work should align with the same module boundaries.
- If a module becomes too large, split it into smaller pieces before assigning it.

---

## Recommended Module Order

1. Authentication & Roles
2. Complaint Submission
3. Student Complaint View
4. Staff Complaint Review
5. Complaint Forwarding
6. Admin / Principal Dashboard
7. Complaint Status Management
8. Email Notification
9. Evidence Upload
10. Filters
11. Status History

---

## Notes

This module breakdown is intentionally small and practical for a 24-hour hackathon.  
It keeps the complaint workflow clear:

Student → Staff review/filter → Admin/Principal action → Email back to student