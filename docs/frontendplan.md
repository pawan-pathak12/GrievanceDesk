# Part 1 — Backend Anonymity Update

---

## Anonymous Complaint System

### Concept

**Admins cannot see who submitted a complaint.** They only see an auto-generated username.

| What Admin Sees | What User Sees (own complaints) |
|---|---|
| Username: `User_1001` | Their real name + complaint |
| No real name | Full details |
| No email | Full details |
| Cannot trace identity | — |

This ensures **user privacy** and encourages honest reporting without fear of retaliation.

---

### Username Generation

When a user completes registration, a username is auto-generated:

```
Username = "User_" + (1000 + UserId)
```

| UserId | Generated Username |
|---|---|
| 1 | `User_1001` |
| 2 | `User_1002` |
| 15 | `User_1015` |
| 100 | `User_1100` |

- Username is stored in the **Users** table
- Username is **unique** and **permanent**
- Username is **the only identity visible to admins** on complaints
- Users see their own username on their profile

---

### Backend Changes Required

#### 1. [MODIFY] `Domain/Entities/Auths/User.cs`

```diff
  public int UserId { get; set; }
  public int? MemberId { get; set; }
+ public string Username { get; set; } = string.Empty;      // NEW
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
```

#### 2. [MODIFY] Users Table (Migration)

```sql
ALTER TABLE Users ADD Username NVARCHAR(50) NOT NULL DEFAULT '';
CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
```

#### 3. [MODIFY] `AuthService.cs` — Registration

After creating the User record and getting the UserId:

```csharp
// Generate username after user is created
var username = $"User_{1000 + user.UserId}";
await _userRepository.UpdateUsernameAsync(user.UserId, username);
```

#### 4. [MODIFY] `IUserRepository.cs` + `UserRepository.cs`

Add method:
```csharp
Task<bool> UpdateUsernameAsync(int userId, string username);
```

SQL:
```sql
UPDATE Users SET Username = @Username WHERE UserId = @UserId
```

#### 5. [MODIFY] Query Results — Replace Real Name with Username

**For Admin-facing queries:**

```diff
  // ComplaintSummaryResult.cs
- public string SubmitterFirstName { get; set; }
- public string SubmitterLastName { get; set; }
- public string SubmitterEmail { get; set; }
+ public string SubmitterUsername { get; set; } = string.Empty;
```

```diff
  // ComplaintDetailResult.cs
- public string SubmitterFirstName { get; set; }
- public string SubmitterLastName { get; set; }
- public string SubmitterEmail { get; set; }
+ public string SubmitterUsername { get; set; } = string.Empty;
```

**Admin Dapper queries change:**

```diff
  SELECT c.*, 
-   u.FirstName AS SubmitterFirstName, 
-   u.LastName AS SubmitterLastName, 
-   u.Email AS SubmitterEmail
+   u.Username AS SubmitterUsername
  FROM Complaints c 
  JOIN Users u ON c.UserId = u.UserId
```

#### 6. [MODIFY] DTOs — Replace Real Name with Username

```diff
  // ComplaintSummaryResponse.cs
- public string SubmitterName { get; set; }
- public string SubmitterEmail { get; set; }
+ public string SubmitterUsername { get; set; } = string.Empty;
```

```diff
  // ComplaintDetailResponse.cs
- public string SubmitterName { get; set; }
- public string SubmitterEmail { get; set; }
+ public string SubmitterUsername { get; set; } = string.Empty;
```

#### 7. [MODIFY] AutoMapper — ComplaintProfile.cs

```diff
- .ForMember(d => d.SubmitterName, o => o.MapFrom(s => s.SubmitterFirstName + " " + s.SubmitterLastName))
- .ForMember(d => d.SubmitterEmail, o => o.MapFrom(s => s.SubmitterEmail));
+ .ForMember(d => d.SubmitterUsername, o => o.MapFrom(s => s.SubmitterUsername));
```

#### 8. [MODIFY] ComplaintUpdateResult + ComplaintUpdateResponse

History entries should also use username:

```diff
  // ComplaintUpdateResult.cs
- public string UpdatedByFirstName { get; set; }
- public string UpdatedByLastName { get; set; }
+ public string UpdatedByUsername { get; set; } = string.Empty;
```

```diff
  // ComplaintUpdateResponse.cs
- public string UpdatedByName { get; set; }
+ public string UpdatedByUsername { get; set; } = string.Empty;
```

**But for Admin actions in history**, show "Admin" (not admin's username):
- If the updater's Role == Admin → display "Admin"
- If the updater's Role == User → display their Username

#### 9. [MODIFY] `GET /api/auth/me` Response

Include username in profile response:

```json
{
  "userId": 1,
  "username": "User_1001",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@org.com",
  "role": "User"
}
```

User can see their own username on their profile.

#### 10. [MODIFY] JWT Claims

```diff
  claims.Add(new Claim("sub", user.UserId.ToString()));
  claims.Add(new Claim("email", user.Email));
  claims.Add(new Claim("role", user.Role.ToString()));
+ claims.Add(new Claim("username", user.Username));
  claims.Add(new Claim("firstName", user.FirstName));
  claims.Add(new Claim("lastName", user.LastName));
```

---

### Anonymity Rules Summary

| Context | Shows | Hides |
|---|---|---|
| **Admin views complaint list** | Username | Real name, email |
| **Admin views complaint detail** | Username | Real name, email |
| **Admin views history** | "Admin" or Username | Real identity |
| **User views own complaints** | Own real name + username | — |
| **User views own profile** | Real name + username | — |
| **Admin member management** | Real name + email | — (admin manages directory) |

> [!IMPORTANT]
> Admin **can** see real names in **Member Management** (that's the org directory). But on **complaints**, identity is hidden. This separation is intentional — admin manages people separately from complaint handling.

---

### Updated Files Count

| From Previous Plan | Anonymity Changes |
|---|---|
| 37 new files | No new files added |
| 8 modified files | +3 additional modifications |
| — | `User.cs` (add Username) |
| — | `AuthService.cs` (generate username) |
| — | `UserRepository.cs` (add UpdateUsername) |
| — | `TokenService.cs` (add username to JWT) |
| **Total: 45 files** | **Now: 45 files (37 new + 8 modified, same count, more changes in modified files)** |

---

---

# Part 2 — Frontend Implementation Plan

---

## Tech Stack

| Technology | Purpose |
|---|---|
| React 18+ | UI framework |
| Vite | Build tool (fast dev server) |
| JavaScript | Language (no TypeScript for speed) |
| React Router v6 | Client-side routing |
| Axios | HTTP client |
| CSS (Vanilla) | Styling (custom design system) |
| Google Fonts (Inter) | Typography |

---

## Project Structure

```
grievance-desk-web/
├── index.html
├── package.json
├── vite.config.js
├── public/
│   └── favicon.ico
└── src/
    ├── main.jsx                          → App entry point
    ├── App.jsx                           → Router setup
    ├── index.css                         → Global styles + design system
    │
    ├── api/
    │   └── axios.js                      → Axios instance with JWT interceptor
    │
    ├── context/
    │   └── AuthContext.jsx               → Auth state (user, token, login, logout)
    │
    ├── hooks/
    │   └── useAuth.js                    → Custom hook for auth context
    │
    ├── components/
    │   ├── layout/
    │   │   ├── Sidebar.jsx               → Navigation sidebar
    │   │   ├── Header.jsx                → Top bar with user info + logout
    │   │   └── Layout.jsx                → Sidebar + Header + main content area
    │   ├── common/
    │   │   ├── StatusBadge.jsx           → Colored badge for complaint status
    │   │   ├── CategoryBadge.jsx         → Badge for complaint category
    │   │   ├── StatCard.jsx              → Dashboard stat card
    │   │   ├── Timeline.jsx              → Complaint history timeline
    │   │   ├── ProtectedRoute.jsx        → Route guard (check auth + role)
    │   │   ├── LoadingSpinner.jsx        → Loading indicator
    │   │   └── EmptyState.jsx            → "No complaints yet" state
    │   └── forms/
    │       ├── ComplaintForm.jsx          → Reused for Create + Edit
    │       └── MemberForm.jsx            → Add member form
    │
    ├── pages/
    │   ├── auth/
    │   │   ├── LoginPage.jsx
    │   │   └── RegisterPage.jsx          → Multi-step (Email → OTP → Password)
    │   ├── user/
    │   │   ├── UserDashboard.jsx
    │   │   ├── CreateComplaint.jsx
    │   │   ├── MyComplaints.jsx
    │   │   ├── ComplaintDetail.jsx
    │   │   └── EditComplaint.jsx
    │   └── admin/
    │       ├── AdminDashboard.jsx
    │       ├── AllComplaints.jsx
    │       ├── AdminComplaintDetail.jsx
    │       └── MemberManagement.jsx
    │
    └── utils/
        ├── constants.js                  → Status/Category labels, colors, API URLs
        └── helpers.js                    → Date formatting, status color mapping
```

---

## Routing

```jsx
// App.jsx routes

// Public routes
/login                          → LoginPage
/register                       → RegisterPage

// User routes (Role = User)
/dashboard                      → UserDashboard
/complaints/new                 → CreateComplaint
/complaints                     → MyComplaints
/complaints/:id                 → ComplaintDetail
/complaints/:id/edit            → EditComplaint

// Admin routes (Role = Admin)
/admin/dashboard                → AdminDashboard
/admin/complaints               → AllComplaints
/admin/complaints/:id           → AdminComplaintDetail
/admin/members                  → MemberManagement

// Redirect
/                               → /login (if not auth) or /dashboard (if User) or /admin/dashboard (if Admin)
```

---

## Auth Context & JWT Handling

### `context/AuthContext.jsx`

```
State:
  - user: { userId, username, firstName, lastName, email, role }
  - token: JWT string
  - isAuthenticated: boolean
  - isLoading: boolean

Methods:
  - login(email, password) → POST /api/auth/login → store token → decode JWT → set user
  - logout() → clear token + user → redirect to /login
  - getMe() → GET /api/auth/me → update user state

Token storage:
  - localStorage.setItem("token", jwt)
  - On app load, read from localStorage → validate → set state
```

### `api/axios.js`

```javascript
// Axios instance with interceptors
const api = axios.create({
  baseURL: "https://localhost:PORT/api",
});

// Request interceptor: attach JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Response interceptor: handle 401 → logout
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);
```

---

## API Service Calls

### Auth API

| Function | Method | Endpoint |
|---|---|---|
| `checkEmail(email)` | POST | `/auth/check-email` |
| `sendOtp(email)` | POST | `/auth/send-otp` |
| `verifyOtp(email, code)` | POST | `/auth/verify-otp` |
| `register(email, password)` | POST | `/auth/register` |
| `login(email, password)` | POST | `/auth/login` |
| `getMe()` | GET | `/auth/me` |
| `changePassword(current, new)` | POST | `/auth/change-password` |

### User Complaints API

| Function | Method | Endpoint |
|---|---|---|
| `createComplaint(data)` | POST | `/complaints` |
| `getMyComplaints()` | GET | `/complaints/my` |
| `getComplaint(id)` | GET | `/complaints/{id}` |
| `updateComplaint(id, data)` | PUT | `/complaints/{id}` |
| `deleteComplaint(id)` | DELETE | `/complaints/{id}` |
| `getMyIrrelevant()` | GET | `/complaints/my/irrelevant` |

### Admin Complaints API

| Function | Method | Endpoint |
|---|---|---|
| `getAllComplaints(filters)` | GET | `/admin/complaints` |
| `getComplaintDetail(id)` | GET | `/admin/complaints/{id}` |
| `updateStatus(id, status)` | PUT | `/admin/complaints/{id}/status` |
| `addNote(id, noteText)` | POST | `/admin/complaints/{id}/notes` |
| `markDuplicate(id, data)` | PUT | `/admin/complaints/{id}/duplicate` |
| `markForwarded(id)` | PUT | `/admin/complaints/{id}/forward` |
| `markIrrelevant(id, reason)` | PUT | `/admin/complaints/{id}/irrelevant` |
| `getIrrelevant()` | GET | `/admin/complaints/irrelevant` |

### Admin Members API

| Function | Method | Endpoint |
|---|---|---|
| `addMember(data)` | POST | `/admin/members` |
| `getAllMembers(filters)` | GET | `/admin/members` |
| `getMember(id)` | GET | `/admin/members/{id}` |
| `updateMember(id, data)` | PUT | `/admin/members/{id}` |
| `deleteMember(id)` | DELETE | `/admin/members/{id}` |

### Dashboard API

| Function | Method | Endpoint |
|---|---|---|
| `getUserDashboard()` | GET | `/dashboard/user` |
| `getAdminDashboard()` | GET | `/dashboard/admin` |

---

## Page Specifications

### Login Page (`/login`)

```
┌──────────────────────────────────────┐
│          GrievanceDesk Logo          │
│                                      │
│   ┌────────────────────────────┐     │
│   │  Email                     │     │
│   └────────────────────────────┘     │
│   ┌────────────────────────────┐     │
│   │  Password                  │     │
│   └────────────────────────────┘     │
│                                      │
│   [        Login Button        ]     │
│                                      │
│   Don't have an account? Register    │
└──────────────────────────────────────┘
```

- On success → redirect based on role:
  - User → `/dashboard`
  - Admin → `/admin/dashboard`

---

### Register Page (`/register`) — Multi-Step

```
Step 1: Email                Step 2: OTP              Step 3: Password
┌──────────────────┐        ┌──────────────────┐     ┌──────────────────┐
│ Enter your email │        │ Enter OTP code   │     │ Create password  │
│ [email input]    │   →    │ [OTP input]      │  →  │ [password input] │
│ [Continue]       │        │ [Verify]         │     │ [confirm input]  │
│                  │        │ Resend OTP       │     │ [Register]       │
└──────────────────┘        └──────────────────┘     └──────────────────┘
```

- Step 1: POST `/auth/check-email` → if found, show Step 2
- Step 2: POST `/auth/verify-otp` → if valid, show Step 3
- Step 3: POST `/auth/register` → on success, auto-login → redirect to dashboard

---

### User Dashboard (`/dashboard`)

```
┌────────────────────────────────────────────────────────┐
│ Sidebar │  Welcome back, John! (User_1001)             │
│         │                                               │
│ 📊 Dash │  ┌─────────┐ ┌─────────┐ ┌─────────┐       │
│ ➕ New  │  │ Total   │ │ Pending │ │ In Prog │       │
│ 📋 My   │  │   12    │ │    3    │ │    4    │       │
│ ⚙ Prof │  └─────────┘ └─────────┘ └─────────┘       │
│         │  ┌─────────┐ ┌─────────┐ ┌─────────┐       │
│         │  │Resolved │ │ Closed  │ │Irrelevnt│       │
│         │  │    3    │ │    1    │ │    1    │       │
│         │  └─────────┘ └─────────┘ └─────────┘       │
│         │                                               │
│         │  Recent Complaints                            │
│         │  ┌───────────────────────────────────┐       │
│         │  │ Broken AC... │ Pending │ Jul 24   │       │
│         │  │ Water leak.. │ Resolved│ Jul 23   │       │
│         │  └───────────────────────────────────┘       │
└─────────┴───────────────────────────────────────────────┘
```

- Calls `GET /api/dashboard/user`
- Shows stat cards with counts
- Shows 5 most recent complaints
- Link to "View All" → `/complaints`
- Link to "New Complaint" → `/complaints/new`

---

### Create Complaint (`/complaints/new`)

```
┌──────────────────────────────────────────────┐
│  Submit New Complaint                        │
│                                              │
│  Title *                                     │
│  ┌──────────────────────────────────────┐    │
│  │                                      │    │
│  └──────────────────────────────────────┘    │
│                                              │
│  Description *                               │
│  ┌──────────────────────────────────────┐    │
│  │                                      │    │
│  │                                      │    │
│  │                                      │    │
│  └──────────────────────────────────────┘    │
│                                              │
│  Category *             Location *           │
│  ┌───────────────┐     ┌───────────────┐     │
│  │ [Dropdown]    │     │               │     │
│  └───────────────┘     └───────────────┘     │
│                                              │
│  [  Submit Complaint  ]                      │
│                                              │
│  ⚠ Content is validated for appropriateness │
└──────────────────────────────────────────────┘
```

- On submit → POST `/api/complaints`
- If blocked word detected → show error from API
- On success → redirect to `/complaints`

---

### My Complaints (`/complaints`)

```
┌────────────────────────────────────────────────────────────┐
│  My Complaints                           [+ New Complaint] │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Title              │ Category      │ Status  │ Date │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │ Broken AC Room 301 │ Infrastructure│ 🟡 Pend │ Jul24│   │
│  │ Water leak lobby   │ Cleanliness   │ 🟢 Resol│ Jul23│   │
│  │ Rude staff front   │ Staff         │ 🔴 Irrel│ Jul22│   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  Showing complaints (excludes Irrelevant)                   │
│  [View Irrelevant Complaints]                               │
└─────────────────────────────────────────────────────────────┘
```

- Calls `GET /api/complaints/my`
- Click row → navigate to `/complaints/:id`
- Status shown as colored badges
- "Edited" badge shown if `isEdited = true`
- Link to view irrelevant complaints separately

---

### Complaint Detail (`/complaints/:id`)

```
┌────────────────────────────────────────────────────────────┐
│  ← Back to My Complaints                                   │
│                                                             │
│  Broken AC in Room 301                          [EDITED]   │
│  Status: 🟡 In Progress                                    │
│  Category: Infrastructure  │  Location: Room 301           │
│  Submitted: Jul 24, 2026   │  Your ID: User_1001           │
│                                                             │
│  Description                                                │
│  The air conditioning unit in Room 301 has not been         │
│  working for the past 3 days...                             │
│                                                             │
│  [Edit] [Delete]    ← Only shown if Status = Pending       │
│                                                             │
│  ─── Complaint Timeline ────────────────────────────────   │
│                                                             │
│  ● Created                              Jul 24, 10:00 AM  │
│    Complaint submitted                                      │
│                                                             │
│  ● Edited                               Jul 24, 10:30 AM  │
│    Description updated                                      │
│                                                             │
│  ● Status Changed by Admin              Jul 24, 11:00 AM  │
│    Pending → In Progress                                    │
│                                                             │
│  ● Note Added by Admin                  Jul 24, 02:00 PM  │
│    "Maintenance team dispatched to Room 301"                │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

- Calls `GET /api/complaints/{id}`
- Shows full detail + history timeline
- Edit/Delete buttons only if `status === "Pending"`
- Edit → navigate to `/complaints/:id/edit`
- Delete → confirm dialog → DELETE `/api/complaints/{id}`
- Timeline component shows all ComplaintUpdates chronologically

---

### Edit Complaint (`/complaints/:id/edit`)

- Same form as Create, but pre-filled with current values
- Reuses `ComplaintForm.jsx` component
- On submit → PUT `/api/complaints/{id}`
- If status !== Pending → redirect back with error
- On success → redirect to `/complaints/:id`

---

### Admin Dashboard (`/admin/dashboard`)

```
┌────────────────────────────────────────────────────────────┐
│ Sidebar  │  Admin Dashboard                                 │
│          │                                                   │
│ 📊 Dash  │  ┌─────────┐ ┌─────────┐ ┌─────────┐          │
│ 📋 All   │  │ Total   │ │ Pending │ │ In Prog │          │
│ 👥 Membrs│  │   150   │ │   30    │ │   45    │          │
│ 🚫 Irrel │  └─────────┘ └─────────┘ └─────────┘          │
│          │  ┌─────────┐ ┌─────────┐ ┌─────────┐          │
│          │  │Resolved │ │ Closed  │ │Irrelevnt│          │
│          │  │   50    │ │   20    │ │    5    │          │
│          │  └─────────┘ └─────────┘ └─────────┘          │
│          │                                                   │
│          │  ┌─────────┐ ┌─────────┐ ┌─────────┐          │
│          │  │ Members │ │Registerd│ │ Today   │          │
│          │  │   200   │ │   150   │ │    8    │          │
│          │  └─────────┘ └─────────┘ └─────────┘          │
└──────────┴───────────────────────────────────────────────────┘
```

- Calls `GET /api/dashboard/admin`
- 9 stat cards

---

### Admin All Complaints (`/admin/complaints`)

```
┌────────────────────────────────────────────────────────────────┐
│  All Complaints                                                 │
│                                                                  │
│  Filter: [Status ▼]  [Category ▼]  [Search by title...]       │
│                                                                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ Title              │ Submitted By │ Category │Status│ Date│  │
│  ├───────────────────────────────────────────────────────────┤  │
│  │ Broken AC Room 301 │ User_1001    │ Infra   │🟡 Pen│Jul24│  │
│  │ Water leak lobby   │ User_1003    │ Clean   │🔵 InP│Jul23│  │
│  │ Rude staff         │ User_1007    │ Staff   │🟡 Pen│Jul22│  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ⚠ User identity is anonymous. Only usernames are shown.       │
└──────────────────────────────────────────────────────────────────┘
```

- Calls `GET /api/admin/complaints?status=&category=&search=`
- **Shows `User_XXXX` username, NOT real name**
- Click row → `/admin/complaints/:id`
- Filter dropdowns for Status and Category
- Search input for title

---

### Admin Complaint Detail (`/admin/complaints/:id`)

```
┌────────────────────────────────────────────────────────────┐
│  ← Back to All Complaints                                  │
│                                                             │
│  Broken AC in Room 301          Submitted by: User_1001    │
│  Status: 🟡 Pending           [DUPLICATE] [FORWARDED]     │
│  Category: Infrastructure  │  Location: Room 301           │
│  Submitted: Jul 24, 2026                                    │
│                                                             │
│  Description                                                │
│  The air conditioning unit in Room 301...                   │
│                                                             │
│  ─── Admin Actions ─────────────────────────────────────   │
│                                                             │
│  Update Status:  [Pending ▼] → [In Progress ▼]  [Update]  │
│                                                             │
│  Add Note:                                                  │
│  ┌──────────────────────────────────────────────────┐      │
│  │                                                  │      │
│  └──────────────────────────────────────────────────┘      │
│  [Add Note]                                                 │
│                                                             │
│  [Mark Duplicate]  [Mark Forwarded]  [Mark Irrelevant]     │
│                                                             │
│  ─── Complaint Timeline ────────────────────────────────   │
│                                                             │
│  ● Created by User_1001                 Jul 24, 10:00 AM  │
│  ● Status changed by Admin             Jul 24, 11:00 AM  │
│    Pending → In Progress                                    │
│  ● Note by Admin                       Jul 24, 02:00 PM  │
│    "Maintenance team dispatched"                            │
└─────────────────────────────────────────────────────────────┘
```

- Calls `GET /api/admin/complaints/{id}`
- **Shows `User_XXXX`, NOT real name**
- Admin action section with:
  - Status update dropdown + button
  - Add note textarea + button
  - Mark Duplicate / Forwarded / Irrelevant buttons
- Each action → API call → refresh page → show updated timeline
- Confirmation dialogs for Irrelevant and Duplicate

---

### Member Management (`/admin/members`)

```
┌────────────────────────────────────────────────────────────┐
│  Member Management                       [+ Add Member]    │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Name           │ Email           │ Role │ Registered │  │
│  ├──────────────────────────────────────────────────────┤  │
│  │ John Doe       │ john@org.com    │ User │ ✅ Yes     │  │
│  │ Jane Smith     │ jane@org.com    │ Admin│ ✅ Yes     │  │
│  │ Bob Wilson     │ bob@org.com     │ User │ ❌ No      │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  Add New Member                                             │
│  ┌────────────┐ ┌────────────┐ ┌────────────────────┐     │
│  │ First Name │ │ Last Name  │ │ Email              │     │
│  └────────────┘ └────────────┘ └────────────────────┘     │
│  Role: [User ▼]                                             │
│  [Add Member]                                               │
└─────────────────────────────────────────────────────────────┘
```

- Shows **real names** (this is the org directory, not complaints)
- Add member form inline at bottom
- Delete button only for unregistered members
- Calls member management APIs

---

## Reusable Components

### `StatusBadge.jsx`

```jsx
// Maps status string to colored badge
// Pending     → Yellow
// InProgress  → Blue
// Resolved    → Green
// Closed      → Gray
// Irrelevant  → Red
```

### `CategoryBadge.jsx`

```jsx
// Maps category string to styled badge
// General, Infrastructure, Service, Staff, Safety, Cleanliness, Other
```

### `StatCard.jsx`

```jsx
// Props: { title, value, color, icon }
// Displays: number with label in a styled card
```

### `Timeline.jsx`

```jsx
// Props: { updates: ComplaintUpdateResponse[] }
// Displays vertical timeline with:
//   - Dot + connector line
//   - Update type label
//   - Actor name (username or "Admin")
//   - Note text (if any)
//   - Status change (Old → New) if applicable
//   - Timestamp
```

### `ProtectedRoute.jsx`

```jsx
// Props: { role: "User" | "Admin" | null }
// Checks:
//   1. Is user authenticated? → if not, redirect to /login
//   2. Does user have required role? → if not, redirect to appropriate dashboard
// Wraps children with Layout component
```

### `ComplaintForm.jsx`

```jsx
// Props: { initialData?, onSubmit, submitLabel, isLoading }
// Reused for both Create and Edit
// Fields: Title, Description (textarea), Category (dropdown), Location
// Shows validation errors from API
// Shows content validation warning
```

---

## Design System (CSS)

### `index.css` — Key Tokens

```css
:root {
  /* Colors */
  --bg-primary: #0f1117;           /* Dark background */
  --bg-secondary: #1a1d27;         /* Card background */
  --bg-tertiary: #242836;          /* Input/hover background */
  --text-primary: #e4e6ef;         /* Main text */
  --text-secondary: #8b8fa3;       /* Muted text */
  --accent-primary: #6c5ce7;       /* Purple accent */
  --accent-hover: #7c6df7;

  /* Status Colors */
  --status-pending: #f0ad4e;       /* Yellow */
  --status-inprogress: #5bc0de;    /* Blue */
  --status-resolved: #5cb85c;      /* Green */
  --status-closed: #777;           /* Gray */
  --status-irrelevant: #d9534f;    /* Red */

  /* Spacing */
  --space-xs: 4px;
  --space-sm: 8px;
  --space-md: 16px;
  --space-lg: 24px;
  --space-xl: 32px;

  /* Typography */
  --font-family: 'Inter', sans-serif;
  --font-size-sm: 0.875rem;
  --font-size-md: 1rem;
  --font-size-lg: 1.25rem;
  --font-size-xl: 1.5rem;
  --font-size-2xl: 2rem;

  /* Border Radius */
  --radius-sm: 6px;
  --radius-md: 10px;
  --radius-lg: 16px;

  /* Shadows */
  --shadow-card: 0 4px 24px rgba(0, 0, 0, 0.3);
}
```

- **Dark mode by default** (impressive for hackathon demo)
- Glassmorphism cards with subtle backdrop blur
- Smooth transitions on hover (0.2s ease)
- Purple accent color for primary actions
- Status colors consistent across badges, timeline, and stat cards

---

## Frontend Build Order

| Step | What | Files | Time |
|---|---|---|---|
| **1** | Project setup (Vite + React + Router + Axios) | 5 files | 20 min |
| **2** | Design system (`index.css`) | 1 file | 20 min |
| **3** | Auth Context + Axios interceptor | 3 files | 20 min |
| **4** | Layout (Sidebar + Header + ProtectedRoute) | 4 files | 30 min |
| **5** | Login page | 1 file | 20 min |
| **6** | Register page (multi-step) | 1 file | 30 min |
| **7** | Common components (StatusBadge, StatCard, Timeline, etc.) | 6 files | 30 min |
| **8** | User Dashboard | 1 file | 20 min |
| **9** | Create Complaint (ComplaintForm + page) | 2 files | 25 min |
| **10** | My Complaints list | 1 file | 20 min |
| **11** | Complaint Detail + Timeline | 1 file | 30 min |
| **12** | Edit Complaint | 1 file | 15 min |
| **13** | Admin Dashboard | 1 file | 20 min |
| **14** | Admin All Complaints (with filters) | 1 file | 25 min |
| **15** | Admin Complaint Detail + Actions | 1 file | 30 min |
| **16** | Admin Member Management | 1 file | 25 min |
| **17** | Polish, responsive, error handling, toast messages | — | 30 min |
| | **Total** | **~32 files** | **~6 hrs** |

---

## Frontend Verification Checklist

```text
□ 1. Open /login → see login form
□ 2. Click "Register" → see multi-step form
□ 3. Enter pre-registered email → OTP sent → verify → set password → logged in
□ 4. User redirected to /dashboard → see stat cards (all zeros)
□ 5. Click "New Complaint" → fill form → submit → success
□ 6. Try submitting with blocked word → see error message
□ 7. Go to "My Complaints" → see 1 complaint with Pending badge
□ 8. Click complaint → see detail page with "Created" in timeline
□ 9. Click "Edit" → modify → save → see "Edited" badge + "Edited" in timeline
□ 10. Dashboard shows updated counts
□ 11. Logout → Login as Admin
□ 12. Admin redirected to /admin/dashboard → see system-wide stats
□ 13. Go to "All Complaints" → see complaint with "User_XXXX" (NOT real name)
□ 14. Click complaint → see detail + admin actions
□ 15. Change status to "In Progress" → timeline updates
□ 16. Add note → timeline updates
□ 17. Go to "Members" → see member list → add new member
□ 18. Logout → Login as User → see updated status + full timeline
□ 19. Try to Edit → should be blocked (status != Pending)
□ 20. Try to Delete → should be blocked (status != Pending)
□ DONE ✅
```

---

## Summary

| Part | Files | Time |
|---|---|---|
| Backend (previous plan + anonymity) | 45 files | ~5.5 hrs |
| Frontend | ~32 files | ~6 hrs |
| **Total** | **~77 files** | **~11.5 hrs** |
| Buffer (testing, debugging, demo prep) | — | ~6.5 hrs |
| **Grand Total** | | **~18 hrs (fits in remaining time)** |
