/* ShieldBoxDemo.sql
   Esquema demo para SQL Server. Ejecutar con permisos de creación de base.
*/
IF DB_ID(N'ShieldBoxDemo') IS NULL
BEGIN
    CREATE DATABASE ShieldBoxDemo;
END
GO
USE ShieldBoxDemo;
GO

IF OBJECT_ID(N'dbo.AuditEvents', N'U') IS NOT NULL DROP TABLE dbo.AuditEvents;
IF OBJECT_ID(N'dbo.Movements', N'U') IS NOT NULL DROP TABLE dbo.Movements;
IF OBJECT_ID(N'dbo.Approvals', N'U') IS NOT NULL DROP TABLE dbo.Approvals;
IF OBJECT_ID(N'dbo.TransferRequests', N'U') IS NOT NULL DROP TABLE dbo.TransferRequests;
IF OBJECT_ID(N'dbo.Policies', N'U') IS NOT NULL DROP TABLE dbo.Policies;
IF OBJECT_ID(N'dbo.Wallets', N'U') IS NOT NULL DROP TABLE dbo.Wallets;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL CONSTRAINT UQ_Roles_RoleName UNIQUE
);

CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    UserName NVARCHAR(120) NOT NULL,
    LoginName NVARCHAR(80) NOT NULL CONSTRAINT UQ_Users_LoginName UNIQUE,
    PasswordHash NVARCHAR(300) NOT NULL,
    RoleId INT NOT NULL CONSTRAINT FK_Users_Roles REFERENCES dbo.Roles(RoleId),
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    FailedLoginAttempts INT NOT NULL CONSTRAINT DF_Users_FailedLoginAttempts DEFAULT 0,
    LockedUntil DATETIME2(0) NULL,
    LastLoginAt DATETIME2(0) NULL
);

CREATE TABLE dbo.Wallets (
    WalletId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Wallets PRIMARY KEY,
    ProviderName NVARCHAR(80) NOT NULL,
    DisplayName NVARCHAR(120) NOT NULL,
    PermissionMode NVARCHAR(30) NOT NULL CONSTRAINT DF_Wallets_PermissionMode DEFAULT N'ReadOnly',
    IsActive BIT NOT NULL CONSTRAINT DF_Wallets_IsActive DEFAULT 1
);

CREATE TABLE dbo.Policies (
    PolicyId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Policies PRIMARY KEY,
    PolicyName NVARCHAR(120) NOT NULL,
    DailyLimit DECIMAL(18,2) NOT NULL,
    WeeklyLimit DECIMAL(18,2) NOT NULL,
    MonthlyLimit DECIMAL(18,2) NOT NULL,
    DoubleApprovalFrom DECIMAL(18,2) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Policies_IsActive DEFAULT 1,
    UpdatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Policies_UpdatedAt DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.TransferRequests (
    TransferRequestId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TransferRequests PRIMARY KEY,
    RequestCode NVARCHAR(20) NOT NULL CONSTRAINT UQ_TransferRequests_RequestCode UNIQUE,
    RequestedByUserId INT NOT NULL CONSTRAINT FK_TransferRequests_Users REFERENCES dbo.Users(UserId),
    WalletId INT NOT NULL CONSTRAINT FK_TransferRequests_Wallets REFERENCES dbo.Wallets(WalletId),
    Recipient NVARCHAR(160) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL CONSTRAINT CK_TransferRequests_Amount CHECK (Amount > 0),
    Concept NVARCHAR(120) NOT NULL,
    Evidence NVARCHAR(500) NULL,
    Status NVARCHAR(30) NOT NULL CONSTRAINT CK_TransferRequests_Status CHECK (Status IN (N'Pending',N'Approved',N'Rejected')),
    RiskLevel NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_TransferRequests_CreatedAt DEFAULT SYSUTCDATETIME(),
    ApprovedAt DATETIME2(0) NULL
);

CREATE TABLE dbo.Approvals (
    ApprovalId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Approvals PRIMARY KEY,
    TransferRequestId INT NOT NULL CONSTRAINT FK_Approvals_TransferRequests REFERENCES dbo.TransferRequests(TransferRequestId),
    ApprovedByUserId INT NOT NULL CONSTRAINT FK_Approvals_Users REFERENCES dbo.Users(UserId),
    ApprovalOrder TINYINT NOT NULL,
    Decision NVARCHAR(20) NOT NULL CONSTRAINT CK_Approvals_Decision CHECK (Decision IN (N'Approved',N'Rejected')),
    ApprovedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Approvals_ApprovedAt DEFAULT SYSUTCDATETIME(),
    EventHash CHAR(64) NULL,
    CONSTRAINT UQ_Approvals_Order UNIQUE (TransferRequestId, ApprovalOrder)
);

CREATE TABLE dbo.Movements (
    MovementId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Movements PRIMARY KEY,
    ExternalCode NVARCHAR(80) NOT NULL CONSTRAINT UQ_Movements_ExternalCode UNIQUE,
    WalletId INT NOT NULL CONSTRAINT FK_Movements_Wallets REFERENCES dbo.Wallets(WalletId),
    MovementType NVARCHAR(30) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    MovementAt DATETIME2(0) NOT NULL,
    LinkedRequestId INT NULL CONSTRAINT FK_Movements_TransferRequests REFERENCES dbo.TransferRequests(TransferRequestId),
    ReconciliationResult NVARCHAR(20) NOT NULL,
    ReviewNote NVARCHAR(300) NULL
);

CREATE TABLE dbo.AuditEvents (
    AuditEventId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditEvents PRIMARY KEY,
    Actor NVARCHAR(120) NOT NULL,
    EventType NVARCHAR(80) NOT NULL,
    EntityType NVARCHAR(80) NOT NULL,
    EntityCode NVARCHAR(80) NULL,
    EventData NVARCHAR(MAX) NULL,
    Severity NVARCHAR(20) NOT NULL,
    EventAt DATETIME2(0) NOT NULL CONSTRAINT DF_AuditEvents_EventAt DEFAULT SYSUTCDATETIME(),
    PreviousHash CHAR(64) NULL,
    EventHash CHAR(64) NULL
);

CREATE INDEX IX_TransferRequests_Status_CreatedAt ON dbo.TransferRequests(Status, CreatedAt DESC);
CREATE INDEX IX_Movements_ReconciliationResult ON dbo.Movements(ReconciliationResult, MovementAt DESC);
CREATE INDEX IX_AuditEvents_EventAt ON dbo.AuditEvents(EventAt DESC);
GO

INSERT dbo.Roles(RoleName) VALUES (N'Administrador'), (N'Aprobador'), (N'Operador');
DECLARE @DemoHash NVARCHAR(300) = N'PBKDF2-SHA256$120000$ABEiM0RVZneImaq7zN3u/w==$FevJRgE0jQPjQfenE+IyCxJ0OHW8DJ9ZdqDchG0xBRc=';
INSERT dbo.Users(UserName, LoginName, PasswordHash, RoleId)
SELECT N'Martín García', N'mgarcia', @DemoHash, RoleId FROM dbo.Roles WHERE RoleName = N'Administrador';
INSERT dbo.Users(UserName, LoginName, PasswordHash, RoleId)
SELECT N'Laura Fernández', N'lfernandez', @DemoHash, RoleId FROM dbo.Roles WHERE RoleName = N'Aprobador';
INSERT dbo.Users(UserName, LoginName, PasswordHash, RoleId)
SELECT N'Juan Pérez', N'jperez', @DemoHash, RoleId FROM dbo.Roles WHERE RoleName = N'Operador';
INSERT dbo.Users(UserName, LoginName, PasswordHash, RoleId)
SELECT N'Lucía Gómez', N'lgomez', @DemoHash, RoleId FROM dbo.Roles WHERE RoleName = N'Operador';

INSERT dbo.Wallets(ProviderName, DisplayName, PermissionMode)
VALUES (N'Mercado Pago', N'Mercado Pago corporativo', N'ReadOnly'),
       (N'Ualá Bis', N'Ualá Bis corporativa', N'ReadOnly'),
       (N'Banco', N'Cuenta bancaria corporativa', N'ReadOnly');

INSERT dbo.Policies(PolicyName, DailyLimit, WeeklyLimit, MonthlyLimit, DoubleApprovalFrom)
VALUES (N'Operador de caja', 350000, 900000, 2500000, 350000),
       (N'Retiro de excedente', 120000, 500000, 1200000, 120000),
       (N'Nuevo destinatario', 0, 0, 0, 1),
       (N'Excepción por emergencia', 80000, 0, 0, 80000);

DECLARE @OperatorId INT = (SELECT UserId FROM dbo.Users WHERE LoginName = N'jperez');
DECLARE @AdminId INT = (SELECT UserId FROM dbo.Users WHERE LoginName = N'mgarcia');
DECLARE @ApproverId INT = (SELECT UserId FROM dbo.Users WHERE LoginName = N'lfernandez');
DECLARE @MpId INT = (SELECT WalletId FROM dbo.Wallets WHERE ProviderName = N'Mercado Pago');
DECLARE @UaId INT = (SELECT WalletId FROM dbo.Wallets WHERE ProviderName = N'Ualá Bis');

INSERT dbo.TransferRequests(RequestCode, RequestedByUserId, WalletId, Recipient, Amount, Concept, Evidence, Status, RiskLevel)
VALUES (N'SB-1045', @OperatorId, @MpId, N'Distribuidora Norte', 480000, N'Pago a proveedor', N'Factura F-0004-1832 · OC-2098', N'Pending', N'High'),
       (N'SB-1044', @OperatorId, @UaId, N'Caja chica', 125000, N'Reposición de caja chica', N'Comprobante adjunto', N'Pending', N'Medium'),
       (N'SB-1042', @OperatorId, @MpId, N'Banco corporativo', 125000, N'Retiro de excedente', N'Control de cierre', N'Approved', N'Low');

DECLARE @ApprovedRequestId INT = (SELECT TransferRequestId FROM dbo.TransferRequests WHERE RequestCode = N'SB-1042');
INSERT dbo.Approvals(TransferRequestId, ApprovedByUserId, ApprovalOrder, Decision)
VALUES (@ApprovedRequestId, @ApproverId, 1, N'Approved'), (@ApprovedRequestId, @AdminId, 2, N'Approved');

INSERT dbo.Movements(ExternalCode, WalletId, MovementType, Amount, MovementAt, LinkedRequestId, ReconciliationResult, ReviewNote)
VALUES (N'MP-88421', @MpId, N'payout', 220000, DATEADD(MINUTE,-45,SYSUTCDATETIME()), NULL, N'Critical', N'Movimiento sin solicitud vinculada'),
       (N'MP-88420', @MpId, N'payout', 125000, DATEADD(MINUTE,-80,SYSUTCDATETIME()), @ApprovedRequestId, N'Reconciled', NULL),
       (N'UA-77102', @UaId, N'payout', 125000, DATEADD(MINUTE,-100,SYSUTCDATETIME()), NULL, N'Review', N'Requiere evidencia');

INSERT dbo.AuditEvents(Actor, EventType, EntityType, EntityCode, EventData, Severity, PreviousHash, EventHash)
VALUES (N'Sistema', N'SyncCompleted', N'Integration', N'Mercado Pago', N'428 movimientos procesados', N'Info', NULL, NULL),
       (N'Laura Fernández', N'SecondApproval', N'TransferRequest', N'SB-1042', N'Solicitud autorizada', N'Success', NULL, NULL),
       (N'Sistema', N'UnlinkedPayout', N'Movement', N'MP-88421', N'Movimiento sin respaldo', N'Critical', NULL, NULL);
GO
