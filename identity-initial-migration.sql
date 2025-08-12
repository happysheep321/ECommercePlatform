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
CREATE TABLE [Permissions] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [DisplayName] nvarchar(100) NOT NULL,
    [Type] int NOT NULL,
    [Enabled] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Description] nvarchar(256) NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [Enabled] bit NOT NULL DEFAULT CAST(1 AS bit),
    [IsSystemRole] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [UserName] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [PhoneNumber] nvarchar(50) NOT NULL,
    [Type] int NOT NULL,
    [Status] int NOT NULL,
    [RegisterTime] datetime2 NOT NULL,
    [Profile_NickName] nvarchar(50) NOT NULL,
    [Profile_AvatarUrl] nvarchar(256) NOT NULL,
    [Profile_Birthday] datetime2 NOT NULL,
    [Profile_Gender] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [RolePermissions] (
    [RoleId] uniqueidentifier NOT NULL,
    [PermissionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
    CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserAddresses] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [ReceiverName] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [Province] nvarchar(max) NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [District] nvarchar(max) NOT NULL,
    [Detail] nvarchar(500) NOT NULL,
    [IsDefault] bit NOT NULL,
    CONSTRAINT [PK_UserAddresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserAddresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserRoles] (
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'DisplayName', N'Enabled', N'Name', N'Type') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([Id], [Description], [DisplayName], [Enabled], [Name], [Type])
VALUES ('aaaa1111-0000-0000-0000-000000000001', N'用户管理页面', N'用户管理页面', CAST(1 AS bit), N'Page:User.View', 1),
('aaaa1111-0000-0000-0000-000000000002', N'编辑用户', N'编辑用户', CAST(1 AS bit), N'Permission:User.Edit', 0),
('aaaa1111-0000-0000-0000-000000000003', N'删除用户', N'删除用户', CAST(1 AS bit), N'Permission:User.Delete', 0),
('aaaa1111-0000-0000-0000-000000000004', N'订单管理页面', N'订单管理页面', CAST(1 AS bit), N'Page:Order.View', 1),
('aaaa1111-0000-0000-0000-000000000005', N'管理订单', N'管理订单', CAST(1 AS bit), N'Permission:Order.Manage', 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'DisplayName', N'Enabled', N'Name', N'Type') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Enabled', N'IsSystemRole', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Description], [Enabled], [IsSystemRole], [Name])
VALUES ('11111111-1111-1111-1111-111111111111', N'管理员', CAST(1 AS bit), CAST(1 AS bit), N'Admin'),
('22222222-2222-2222-2222-222222222222', N'卖家', CAST(1 AS bit), CAST(1 AS bit), N'Seller'),
('33333333-3333-3333-3333-333333333333', N'买家', CAST(1 AS bit), CAST(1 AS bit), N'Buyer'),
('44444444-4444-4444-4444-444444444444', N'访客', CAST(1 AS bit), CAST(1 AS bit), N'Guest');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Enabled', N'IsSystemRole', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES ('aaaa1111-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111'),
('aaaa1111-0000-0000-0000-000000000002', '11111111-1111-1111-1111-111111111111'),
('aaaa1111-0000-0000-0000-000000000003', '11111111-1111-1111-1111-111111111111'),
('aaaa1111-0000-0000-0000-000000000004', '11111111-1111-1111-1111-111111111111'),
('aaaa1111-0000-0000-0000-000000000005', '11111111-1111-1111-1111-111111111111'),
('aaaa1111-0000-0000-0000-000000000005', '22222222-2222-2222-2222-222222222222'),
('aaaa1111-0000-0000-0000-000000000004', '33333333-3333-3333-3333-333333333333');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;

CREATE INDEX [IX_UserAddresses_UserId] ON [UserAddresses] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250812002825_InitialIdentitySchema', N'9.0.6');

COMMIT;
GO

