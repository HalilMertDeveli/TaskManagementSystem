# Task Management System

A layered **ASP.NET Core MVC** web application for managing users, work orders (tasks), priorities, notifications, and task reports. It was built as a **graduation (capstone) project** to demonstrate clean architecture, role-based access, and a full vertical slice from database to UI.

---

## Overview

The system distinguishes two primary experiences:

- **Administrators** manage the full catalog of work orders, member accounts, priorities, and notifications, and see a dashboard of aggregate metrics.
- **Members** work on assignments they receive: they browse their tasks, file progress reports, mark work complete when appropriate, maintain their profile, and read notifications.

Authentication is **cookie-based** (ASP.NET Core Cookie Authentication). After login, users are routed by role into the **Admin** or **Member** MVC area.

---

## Features by role

### Admin area

| Capability | Description |
|------------|-------------|
| **Dashboard** | Summary cards for work orders, users, and unread notifications (MediatR-driven). |
| **Work orders (tasks)** | Paginated list with search; create and edit tasks with priority, dates, assignment, and status. |
| **Users** | List, search, create, edit, delete members; administrative password reset. |
| **Priorities** | Maintain priority levels used when creating or editing tasks. |
| **Notifications** | View and manage notification entries relevant to administration. |
| **Profile** | Update the signed-in administrator’s name, surname, and password. |

All admin controllers live under `PersonalManagementSystem/Controllers/Admin/` with views under `PersonalManagementSystem/Areas/Admin/Views/`, and are protected with `[Authorize(Roles = "Admin")]`.

#### Admin UI (screenshots)

The UI may be localized; the screenshots below illustrate layout and workflows.

![Admin dashboard — summary cards for work orders, users, and notifications](docs/images/admin/admin-dashboard.png)

![Admin user management — list, search, and actions](docs/images/admin/admin-users.png)

![Admin work order list](docs/images/admin/admin-work-orders.png)

![Admin profile](docs/images/admin/admin-profile.png)

![Admin work order create/edit form](docs/images/admin/admin-task-form.png)

### Member area

| Capability | Description |
|------------|-------------|
| **My tasks** | Paginated list of tasks assigned to the signed-in member, with search. |
| **Task reports** | Per-task report list; create, edit, and delete reports for documentation of progress. |
| **Complete task** | Action to mark an assigned task as completed (via application request/handler). |
| **Notifications** | List notifications for the member; mark individual items as read/updated. |
| **Profile** | View and update name, surname, and password (`UserDetail`). |

Member controllers are under `PersonalManagementSystem/Controllers/Member/` with views under `PersonalManagementSystem/Areas/Member/Views/`, protected with `[Authorize(Roles = "Member")]`. After login, members are redirected to **Member → User → UserDetail** by default.

#### Member UI (screenshots)

The UI may be localized; the screenshots below illustrate layout and workflows.

![Member profile — card, role, and editable name, surname, and password](docs/images/member/member-profile.png)

![Member work orders — assigned tasks list with search](docs/images/member/member-work-orders-list.png)

![Member shell — sidebar navigation and notifications in the main area](docs/images/member/member-dashboard.png)

---

## Architecture

The solution follows a **layered (clean-style) layout**: domain in the center, application services depending only on abstractions, persistence implementing data access, and the web UI composing everything at the edge.

```text
PersonalManagementSystem.sln
├── PersonalManagementSystem/          ← ASP.NET Core MVC UI (HMD.TaskManagement.UI.csproj)
│   ├── Controllers/                   ← Account + Admin/* + Member/* (areas)
│   ├── Areas/Admin/Views, Areas/Member/Views
│   └── Program.cs                     ← DI, auth, EF, routing
├── HMD.TaskManagement.Application/    ← MediatR requests/handlers, DTOs, FluentValidation
├── HMD.TaskManagement.Domain/         ← Entities and domain model
└── HMD.TaskManagement.Persistence/    ← EF Core DbContext, SQL Server, migrations
```

**Request flow:** MVC controllers send **MediatR** requests into the Application layer; handlers use the Persistence layer (EF Core) and return results/DTOs to the views.

**Repository pattern:** Data access is abstracted with interfaces such as `IUserRepository`, `IAppTaskRepository`, `IPriorityRepository`, `ITaskReportRepository`, and `INotificationRepository` under `HMD.TaskManagement.Application/Interfaces/`. Implementations live in `HMD.TaskManagement.Persistence/Repositories/` and are wired in `HMD.TaskManagement.Persistence/IOCExtensions.cs` (`AddPersistenceServices`).

**FluentValidation:** Validator classes under `HMD.TaskManagement.Application/Validators/` are injected into handlers as `IValidator<TRequest>`; handlers call `Validate` / `ValidateAsync` and map failures with `ValidationExtensions.ToMap` before executing use cases (no automatic MVC filter pipeline for validators in this codebase).

**Routing** (`Program.cs`):

- Area routes: `{area:exists}/{controller=Home}/{action=Index}/{id?}` (e.g. `/Admin/Home/Index`, `/Member/AppTask/List`).
- Default route: `{controller=Account}/{action=Login}/{id?}` so unauthenticated users land on login.

**Authentication:** Cookie scheme with an application-specific cookie name, **HttpOnly**, **SameSite = Strict**, and **Secure** policy aligned with the request (HTTPS in production recommended).

**Startup:** On boot, the app optionally ensures seed **Role** rows exist (Admin = 1, Member = 2) when the database is reachable, so registration and authorization stay consistent.

---

## Tech stack

Verified from project files:

| Technology | Where |
|------------|--------|
| **.NET 8** | All projects (`net8.0`) |
| **ASP.NET Core MVC** | `PersonalManagementSystem` |
| **Entity Framework Core 8** + **SQL Server** provider | `HMD.TaskManagement.Persistence` |
| **MediatR** 12.x | `HMD.TaskManagement.Application` |
| **FluentValidation** 11.x | Validators under `HMD.TaskManagement.Application/Validators/`; invoked explicitly from MediatR handlers |
| **EF Core tools / Design-time** | UI project references `Microsoft.EntityFrameworkCore.Design` for migrations |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A **SQL Server** instance you can connect to — commonly **LocalDB** for local development (`(localdb)\MSSQLLocalDB`) or a named SQL Server instance.

---

## Configuration

Connection strings are read from **`PersonalManagementSystem/appsettings.json`** (and optional environment-specific overrides).

Set the **`ConnectionStrings:Local`** entry to your database. Use a placeholder or development-only values in source control; do not commit production credentials.

**Example shape (not a real secret — adjust server and database names):**

```json
"ConnectionStrings": {
  "Local": "Server=(localdb)\\MSSQLLocalDB;Database=YourTaskDb;Integrated Security=True;TrustServerCertificate=True"
}
```

For a full SQL Server instance, replace `Server=` with your host and use appropriate authentication (`Integrated Security` or SQL login) per your environment policy.

---

## Setup and run

From the repository root (`TaskManagementSystem`):

```bash
dotnet restore PersonalManagementSystem.sln
dotnet build PersonalManagementSystem.sln
dotnet run --project PersonalManagementSystem/HMD.TaskManagement.UI.csproj
```

Then open the HTTPS or HTTP URL shown in the console (Kestrel). The default route opens **Account/Login**.

### Database migrations

Apply migrations using the Persistence project as the EF project and the UI project as startup:

```bash
dotnet ef database update --project HMD.TaskManagement.Persistence --startup-project PersonalManagementSystem
```

Ensure `ConnectionStrings:Local` points to a valid server before running `database update`.

---

## Repository map (quick reference)

| Path | Role |
|------|------|
| `PersonalManagementSystem.sln` | Solution entry |
| `PersonalManagementSystem/Program.cs` | Middleware, cookie auth, area + default routes |
| `PersonalManagementSystem/appsettings.json` | Logging, `ConnectionStrings:Local` |
| `PersonalManagementSystem/Controllers/` | `AccountController` + area controllers |
| `HMD.TaskManagement.Application/` | Requests, handlers, validation |
| `HMD.TaskManagement.Persistence/` | `TaskManagementContext`, migrations |
| `docs/images/admin/` | README screenshots (admin) |
| `docs/images/member/` | README screenshots (member) |

---

## License / academic use

This project is intended for **educational and capstone demonstration** purposes. Adapt licensing as required by your institution or employer.
