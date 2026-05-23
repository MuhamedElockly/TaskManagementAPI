# Database seed scripts

## SeedProjectsAndTasks.sql

Inserts sample **projects** and **tasks** for an existing user.

### Before you run

1. Apply EF migrations:
   ```bash
   cd Presistence
   dotnet ef database update --startup-project ../Chatty
   ```

2. Register at least one user (Swagger: `POST /api/Auth/Register`), for example:
   ```json
   {
     "firstName": "Demo",
     "lastName": "User",
     "email": "demo@taskmanagement.com",
     "phoneNumber": "0100000000",
     "password": "demo123",
     "confirmPassword": "demo123",
     "role": 0
   }
   ```

3. Open `SeedProjectsAndTasks.sql` and set `@OwnerEmail` to that user's email (default: `demo@taskmanagement.com`).

### Run the script

**SSMS / Azure Data Studio:** Open `SeedProjectsAndTasks.sql` and execute.

**sqlcmd:**
```bash
sqlcmd -S "YOUR_SERVER" -d TaskManagementApi -E -i "Presistence\Scripts\SeedProjectsAndTasks.sql"
```

Replace `YOUR_SERVER` with your instance (e.g. `(localdb)\mssqllocaldb` or `MUHAMED-ELOCKLY\MSSQLSERVER03`).

### What gets inserted

| Project           | Tasks |
|-------------------|-------|
| Website Redesign  | 3 (mockups, layout, CI/CD) |
| Mobile App MVP    | 3 (login API, project list, push research) |
| Internal Tools    | 2 (log dashboard, backup script) |

The script is **idempotent**: re-running it does not duplicate rows (checks by project name + owner, task title + project).

### Verify in the API

1. Login as the seed user → copy JWT.
2. Swagger **Authorize** → `Bearer {token}`.
3. `GET /api/Project` — should list 3 projects.
4. `GET /api/Task/project/{projectId}` — tasks per project.
