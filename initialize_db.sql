IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [AvatarUrl] nvarchar(max) NULL,
        [FullName] nvarchar(255) NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [GoogleAuth] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [ColorHex] nvarchar(9) NOT NULL,
        [Default] bit NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Categories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE TABLE [TaskLists] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(255) NOT NULL,
        [IconName] nvarchar(max) NOT NULL,
        [Default] bit NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_TaskLists] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TaskLists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE TABLE [ToDoTasks] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Note] nvarchar(1000) NOT NULL,
        [DueDate] datetime2 NULL,
        [IsCompleted] bit NOT NULL,
        [CategoryId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TaskListId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_ToDoTasks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ToDoTasks_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ToDoTasks_TaskLists_TaskListId] FOREIGN KEY ([TaskListId]) REFERENCES [TaskLists] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ToDoTasks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE TABLE [SubTasks] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [IsCompleted] bit NOT NULL,
        [ToDoTaskId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_SubTasks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubTasks_ToDoTasks_ToDoTaskId] FOREIGN KEY ([ToDoTaskId]) REFERENCES [ToDoTasks] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE INDEX [IX_Categories_UserId] ON [Categories] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE INDEX [IX_SubTasks_ToDoTaskId] ON [SubTasks] ([ToDoTaskId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE INDEX [IX_TaskLists_UserId] ON [TaskLists] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE INDEX [IX_ToDoTasks_CategoryId] ON [ToDoTasks] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE INDEX [IX_ToDoTasks_TaskListId] ON [ToDoTasks] ([TaskListId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE INDEX [IX_ToDoTasks_UserId] ON [ToDoTasks] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707112441_Init'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260707112441_Init', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708144602_AuthToken'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [Token] nvarchar(200) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [ReplacedByToken] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708144602_AuthToken'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708144602_AuthToken'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708144602_AuthToken'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708144602_AuthToken', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708151309_RemoveRefreshTokenRevocationFields'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RefreshTokens]') AND [c].[name] = N'ReplacedByToken');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [RefreshTokens] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [RefreshTokens] DROP COLUMN [ReplacedByToken];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708151309_RemoveRefreshTokenRevocationFields'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RefreshTokens]') AND [c].[name] = N'RevokedAt');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [RefreshTokens] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [RefreshTokens] DROP COLUMN [RevokedAt];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708151309_RemoveRefreshTokenRevocationFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708151309_RemoveRefreshTokenRevocationFields', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709085739_AddGoogleCalendarConnection'
)
BEGIN
    CREATE TABLE [GoogleCalendarConnections] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [EncryptedRefreshToken] nvarchar(max) NOT NULL,
        [Scopes] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_GoogleCalendarConnections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GoogleCalendarConnections_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709085739_AddGoogleCalendarConnection'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GoogleCalendarConnections_UserId] ON [GoogleCalendarConnections] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260709085739_AddGoogleCalendarConnection'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260709085739_AddGoogleCalendarConnection', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710091644_sessions'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [ReplacedByToken] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710091644_sessions'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [RevokedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710091644_sessions'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [SessionId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710091644_sessions'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_SessionId] ON [RefreshTokens] ([SessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710091644_sessions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260710091644_sessions', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712151323_remakeToStatelesstokens'
)
BEGIN
    DROP TABLE [RefreshTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712151323_remakeToStatelesstokens'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260712151323_remakeToStatelesstokens', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712152752_removeAvatarFromDatabase'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'AvatarUrl');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Users] DROP COLUMN [AvatarUrl];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712152752_removeAvatarFromDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260712152752_removeAvatarFromDatabase', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712170444_reminder'
)
BEGIN
    ALTER TABLE [ToDoTasks] ADD [RemindBeforeMinutes] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712170444_reminder'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260712170444_reminder', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712182440_syncToGoogleCalendar'
)
BEGIN
    ALTER TABLE [ToDoTasks] ADD [SyncToGoogleCalendar] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260712182440_syncToGoogleCalendar'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260712182440_syncToGoogleCalendar', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713181307_addOrderToSubTask'
)
BEGIN
    ALTER TABLE [SubTasks] ADD [Order] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713181307_addOrderToSubTask'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260713181307_addOrderToSubTask', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715073941_changeUser'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'GoogleAuth');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Users] DROP COLUMN [GoogleAuth];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715073941_changeUser'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260715073941_changeUser', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260716180603_AddGoogleSub'
)
BEGIN
    ALTER TABLE [Users] ADD [GoogleSub] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260716180603_AddGoogleSub'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Users_GoogleSub] ON [Users] ([GoogleSub]) WHERE [GoogleSub] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260716180603_AddGoogleSub'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260716180603_AddGoogleSub', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260717130924_AddKeysToDb'
)
BEGIN
    CREATE TABLE [DataProtectionKeys] (
        [Id] int NOT NULL IDENTITY,
        [FriendlyName] nvarchar(max) NULL,
        [Xml] nvarchar(max) NULL,
        CONSTRAINT [PK_DataProtectionKeys] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260717130924_AddKeysToDb'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260717130924_AddKeysToDb', N'10.0.9');
END;

COMMIT;
GO

