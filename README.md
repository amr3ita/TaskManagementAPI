# 🚀 Task Management System API

Welcome to the **Task Management System API** – a robust, enterprise-grade RESTful API built using **ASP.NET Core**, adhering to **Clean Architecture** principles and professional design patterns. This backend is fully optimized for performance, security, and scalability, serving as a reliable foundation for frontend applications like Angular.

---

## 🛠️ Tech Stack & Architecture

- **Framework:** .NET Core 8.0 / 9.0 (ASP.NET Core Web API)
- **Database Provider:** Entity Framework Core (EF Core) with SQL Server
- **Security & Identity:** ASP.NET Core Identity with JWT Authentication
- **Architecture Pattern:** Clean Architecture with Repository Pattern
- **Data Transfer:** Data Transfer Objects (DTOs) for strict request/response isolation

---

## 🔒 Authentication & Security

The system implements advanced secure authentication using **JWT (JSON Web Tokens)** coupled with **ASP.NET Core Identity** for user management.

- **Token Lifespan:** The issued JWT Access Token is configured to remain valid for **12 Hours** (`Expires = DateTime.UtcNow.AddHours(12)`). This provides a seamless development and testing experience while keeping user sessions active throughout work shifts.
- **Authorization:** All project and task endpoints are heavily guarded with the `[Authorize]` attribute.
- **Data Isolation:** Security is enforced at the database query level. A logged-in user can **only** view, create, update, or delete projects and tasks that strictly belong to their own `UserId` extracted safely from token claims (`ClaimTypes.NameIdentifier`).

---

## 🚦 Data Specifications: Enums Handling (Status & Priority)

To keep the application strongly typed, both **Status** and **Priority** are managed using backend `Enums`. For API testing (via Postman/Swagger) and Frontend Integration, please note how they map:

### 1. Task Status (`TaskStatusEnum`)
| Integer Value | String representation | Description |
| :---: | :--- | :--- |
| `0` | `Todo` | Task is created but not started |
| `1` | `InProgress` | Task is currently being worked on |
| `2` | `Done` | Task is completely finished |

### 2. Task Priority (`TaskPriorityEnum`)
| Integer Value | String representation | Description |
| :---: | :--- | :--- |
| `0` | `Low` | Low urgency task |
| `1` | `Medium` | Medium urgency task |
| `2` | `High` | High critical task |

> ⚙️ **JSON Configuration:** The backend is configured using `JsonStringEnumConverter` in `Program.cs`. Therefore, while the database efficiently stores these as integers (`int`), the API returns and accepts them as readable **Strings** (e.g., `"Done"`, `"High"`) inside JSON payloads to simplify Frontend binding.

---

## 🛡️ Database Integrity & High-Performance Deletion

A critical architectural decision was made regarding the relationship between **Projects** and **Tasks** (One-to-Many):

1. **`DeleteBehavior.NoAction` (or Restrict):** To avoid accidental data loss via cascade deletes, the Foreign Key constraint is explicitly set to `NoAction` in the Fluent API. SQL Server will block any direct attempt to delete a project if it contains underlying tasks.
2. **`ExecuteDeleteAsync` (Bulk Delete):** To solve this constraint efficiently, the project deletion endpoint utilizes the modern EF Core Bulk Delete feature. It issues a direct, high-performance `DELETE` SQL statement straight to the database server for all related tasks, bypassing the heavy step of loading entities into memory.
3. **Database Transactions (`BeginTransactionAsync`):** To ensure maximum data reliability, the entire deletion process is wrapped in an atomic **Database Transaction** (All-or-Nothing). If the project deletion fails after the tasks are wiped, the transaction automatically fires a `RollbackAsync()`, ensuring zero data corruption.

---

## 📌 Main API Endpoints

### 🔑 Authentication (`api/Auth`)
- `POST /api/auth/register` - Register a new user account.
- `POST /api/auth/login` - Login to receive a JWT Token (Valid for 12 hours).

### 📂 Projects (`api/Project`)
- `GET /api/project` - Retrieves all projects belonging to the logged-in user (Lightweight payload).
- `GET /api/project/{id}` - Retrieves complete details of a specific project, **including a nested list of all its tasks** for seamless Kanban Board rendering.
- `POST /api/project` - Create a new project.
- `PUT /api/project/{id}` - Update project details.
- `DELETE /api/project/{id}` - Safely and atomically wipes a project and its tasks via Transaction.

### 📋 Project Tasks (`api/ProjectTasks`)
- `GET /api/projecttask/project/{projectId}` - Fetch all tasks under a specific project.
- `GET /api/projecttask/{id}` - Get single task details by ID.
- `POST /api/projecttask` - Append a new task to a project (Validates project ownership).
- `PUT /api/projecttask/{id}` - Modify an existing task.
- `DELETE /api/projecttask/{id}` - Delete a single task.

---

## 👥 Contact & Developer Profile

Developed with passion by **Amr Walied**, a dedicated Full-Stack Software Developer specialized in the .NET Ecosystem and Angular framework.

- **📞 Phone Number:** [+201094760851](tel:+201094760851)
- **💼 LinkedIn:** [linkedin.com/in/amr-walied](https://www.linkedin.com/in/amr-walied/)
- **🐙 GitHub:** [github.com/amr3ita](https://github.com/amr3ita)

---
*Happy Coding! Formulated strictly to professional junior/mid-level engineering standards.*