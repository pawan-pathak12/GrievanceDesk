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