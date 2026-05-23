/*
================================================================================
  Task Management API — Sample data for Projects & Tasks
================================================================================
  Database: TaskManagementApi (change USE below if your catalog name differs)

  Prerequisites:
    1. Migrations applied:  dotnet ef database update --startup-project ../Chatty
    2. At least one user in AspNetUsers (register via POST /api/Auth/Register)
    3. Roles seeded (NormalUser, Admin) — happens automatically on app startup

  Usage (SQL Server Management Studio / Azure Data Studio / sqlcmd):
    - Set @OwnerEmail to the user who should own the sample projects
    - Run the entire script

  Task status values:  0 = Pending, 1 = InProgress, 2 = Completed, 3 = Cancelled
  Task priority values: 0 = Low,     1 = Medium,      2 = High
================================================================================
*/

USE [TaskManagementApi];
GO

SET NOCOUNT ON;

-- ============================================================================
-- Configuration — change this email to match a registered user
-- ============================================================================
DECLARE @OwnerEmail NVARCHAR(256) = N'demo@taskmanagement.com';

DECLARE @OwnerId NVARCHAR(450);

SELECT @OwnerId = [Id]
FROM [dbo].[AspNetUsers]
WHERE [Email] = @OwnerEmail
   OR [NormalizedEmail] = UPPER(@OwnerEmail);

-- Fallback: first user in the database
IF @OwnerId IS NULL
BEGIN
    SELECT TOP (1) @OwnerId = [Id]
    FROM [dbo].[AspNetUsers]
    ORDER BY [Id];
END

IF @OwnerId IS NULL
BEGIN
    RAISERROR(
        N'No users found in AspNetUsers. Register a user first (e.g. POST /api/Auth/Register), then re-run this script.',
        16, 1);
    RETURN;
END

PRINT N'Using OwnerId: ' + @OwnerId;
IF EXISTS (SELECT 1 FROM [dbo].[AspNetUsers] WHERE [Id] = @OwnerId)
    PRINT N'Owner email: ' + ISNULL((SELECT [Email] FROM [dbo].[AspNetUsers] WHERE [Id] = @OwnerId), N'(unknown)');

-- ============================================================================
-- Seed roles (optional — app also seeds these on startup)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = N'NORMALUSER')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), N'NormalUser', N'NORMALUSER', NEWID());
    PRINT N'Created role: NormalUser';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = N'ADMIN')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), N'Admin', N'ADMIN', NEWID());
    PRINT N'Created role: Admin';
END

GO

-- ============================================================================
-- Projects
-- ============================================================================
DECLARE @OwnerEmail NVARCHAR(256) = N'demo@taskmanagement.com';
DECLARE @OwnerId NVARCHAR(450);

SELECT @OwnerId = [Id]
FROM [dbo].[AspNetUsers]
WHERE [Email] = @OwnerEmail OR [NormalizedEmail] = UPPER(@OwnerEmail);

IF @OwnerId IS NULL
    SELECT TOP (1) @OwnerId = [Id] FROM [dbo].[AspNetUsers] ORDER BY [Id];

DECLARE @Now DATETIME2 = SYSUTCDATETIME();

-- Project 1: Website Redesign
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Projects]
    WHERE [Name] = N'Website Redesign' AND [OwnerId] = @OwnerId)
BEGIN
    INSERT INTO [dbo].[Projects] ([Name], [Description], [CreatedAt], [OwnerId])
    VALUES (
        N'Website Redesign',
        N'Rebuild the public marketing site with a modern UI and improved performance.',
        @Now,
        @OwnerId);
    PRINT N'Inserted project: Website Redesign';
END

-- Project 2: Mobile App MVP
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Projects]
    WHERE [Name] = N'Mobile App MVP' AND [OwnerId] = @OwnerId)
BEGIN
    INSERT INTO [dbo].[Projects] ([Name], [Description], [CreatedAt], [OwnerId])
    VALUES (
        N'Mobile App MVP',
        N'Deliver the first release of the task management mobile application.',
        DATEADD(DAY, -7, @Now),
        @OwnerId);
    PRINT N'Inserted project: Mobile App MVP';
END

-- Project 3: Internal Tools
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Projects]
    WHERE [Name] = N'Internal Tools' AND [OwnerId] = @OwnerId)
BEGIN
    INSERT INTO [dbo].[Projects] ([Name], [Description], [CreatedAt], [OwnerId])
    VALUES (
        N'Internal Tools',
        N'Automation scripts and admin dashboards for the operations team.',
        DATEADD(DAY, -14, @Now),
        @OwnerId);
    PRINT N'Inserted project: Internal Tools';
END

GO

-- ============================================================================
-- Tasks
-- ============================================================================
DECLARE @OwnerEmail NVARCHAR(256) = N'demo@taskmanagement.com';
DECLARE @OwnerId NVARCHAR(450);

SELECT @OwnerId = [Id]
FROM [dbo].[AspNetUsers]
WHERE [Email] = @OwnerEmail OR [NormalizedEmail] = UPPER(@OwnerEmail);

IF @OwnerId IS NULL
    SELECT TOP (1) @OwnerId = [Id] FROM [dbo].[AspNetUsers] ORDER BY [Id];

DECLARE @ProjectWebsite INT;
DECLARE @ProjectMobile INT;
DECLARE @ProjectInternal INT;

SELECT @ProjectWebsite = [Id]
FROM [dbo].[Projects]
WHERE [Name] = N'Website Redesign' AND [OwnerId] = @OwnerId;

SELECT @ProjectMobile = [Id]
FROM [dbo].[Projects]
WHERE [Name] = N'Mobile App MVP' AND [OwnerId] = @OwnerId;

SELECT @ProjectInternal = [Id]
FROM [dbo].[Projects]
WHERE [Name] = N'Internal Tools' AND [OwnerId] = @OwnerId;

DECLARE @DueSoon DATETIME2 = DATEADD(DAY, 7, SYSUTCDATETIME());
DECLARE @DueLater DATETIME2 = DATEADD(DAY, 30, SYSUTCDATETIME());

-- ----- Website Redesign tasks -----
IF @ProjectWebsite IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectWebsite AND [Title] = N'Design homepage mockups')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Design homepage mockups',
            N'Create Figma layouts for desktop and mobile homepages.',
            1, -- InProgress
            @DueSoon,
            2, -- High
            @ProjectWebsite);
        PRINT N'Inserted task: Design homepage mockups';
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectWebsite AND [Title] = N'Implement responsive layout')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Implement responsive layout',
            N'Build CSS grid/flex layout components for all breakpoints.',
            0, -- Pending
            @DueLater,
            1, -- Medium
            @ProjectWebsite);
        PRINT N'Inserted task: Implement responsive layout';
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectWebsite AND [Title] = N'Set up CI/CD pipeline')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Set up CI/CD pipeline',
            N'Configure build, test, and deploy workflow for the static site.',
            2, -- Completed
            DATEADD(DAY, -3, SYSUTCDATETIME()),
            0, -- Low
            @ProjectWebsite);
        PRINT N'Inserted task: Set up CI/CD pipeline';
    END
END

-- ----- Mobile App MVP tasks -----
IF @ProjectMobile IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectMobile AND [Title] = N'API integration for login')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'API integration for login',
            N'Connect mobile client to POST /api/Auth/Login and store JWT securely.',
            1, -- InProgress
            @DueSoon,
            2, -- High
            @ProjectMobile);
        PRINT N'Inserted task: API integration for login';
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectMobile AND [Title] = N'Project list screen')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Project list screen',
            N'Display user projects from GET /api/Project with pull-to-refresh.',
            0, -- Pending
            @DueLater,
            1, -- Medium
            @ProjectMobile);
        PRINT N'Inserted task: Project list screen';
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectMobile AND [Title] = N'Push notification research')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Push notification research',
            N'Evaluate FCM vs APNs for task reminder notifications.',
            3, -- Cancelled
            DATEADD(DAY, -1, SYSUTCDATETIME()),
            0, -- Low
            @ProjectMobile);
        PRINT N'Inserted task: Push notification research';
    END
END

-- ----- Internal Tools tasks -----
IF @ProjectInternal IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectInternal AND [Title] = N'Log aggregation dashboard')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Log aggregation dashboard',
            N'Visualize Serilog file logs with filters by level and operation name.',
            0, -- Pending
            @DueLater,
            1, -- Medium
            @ProjectInternal);
        PRINT N'Inserted task: Log aggregation dashboard';
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Tasks] WHERE [ProjectId] = @ProjectInternal AND [Title] = N'Database backup script')
    BEGIN
        INSERT INTO [dbo].[Tasks] ([Title], [Description], [Status], [DueDate], [Priority], [ProjectId])
        VALUES (
            N'Database backup script',
            N'Automated nightly backup for TaskManagementApi database.',
            2, -- Completed
            DATEADD(DAY, -5, SYSUTCDATETIME()),
            2, -- High
            @ProjectInternal);
        PRINT N'Inserted task: Database backup script';
    END
END

GO

-- ============================================================================
-- Summary
-- ============================================================================
SELECT
    u.[Email] AS [OwnerEmail],
    p.[Id] AS [ProjectId],
    p.[Name] AS [ProjectName],
    p.[CreatedAt],
    COUNT(t.[Id]) AS [TaskCount]
FROM [dbo].[Projects] p
INNER JOIN [dbo].[AspNetUsers] u ON u.[Id] = p.[OwnerId]
LEFT JOIN [dbo].[Tasks] t ON t.[ProjectId] = p.[Id]
GROUP BY u.[Email], p.[Id], p.[Name], p.[CreatedAt]
ORDER BY p.[Id];

PRINT N'Seed script completed.';
GO
