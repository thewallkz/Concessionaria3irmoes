CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('00000000000000_CreateIdentitySchema', '8.0.21');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "Clientes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Clientes" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "CPF" TEXT NOT NULL,
    "Endereco" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Telefone" TEXT NOT NULL
);

CREATE TABLE "Veiculos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Veiculos" PRIMARY KEY AUTOINCREMENT,
    "Modelo" TEXT NOT NULL,
    "Marca" TEXT NOT NULL,
    "Preco" TEXT NOT NULL,
    "Motor" TEXT NOT NULL,
    "Potencia" TEXT NOT NULL,
    "Torque" TEXT NOT NULL,
    "Capacidade" TEXT NOT NULL,
    "Autonomia" TEXT NOT NULL
);

CREATE TABLE "Vendas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Vendas" PRIMARY KEY AUTOINCREMENT,
    "DataVenda" TEXT NOT NULL,
    "ValorFinal" TEXT NOT NULL,
    "ClienteId" INTEGER NOT NULL,
    "VeiculoId" INTEGER NOT NULL,
    CONSTRAINT "FK_Vendas_Clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES "Clientes" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Vendas_Veiculos_VeiculoId" FOREIGN KEY ("VeiculoId") REFERENCES "Veiculos" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Vendas_ClienteId" ON "Vendas" ("ClienteId");

CREATE INDEX "IX_Vendas_VeiculoId" ON "Vendas" ("VeiculoId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251209182651_Inicial', '8.0.21');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Veiculos" RENAME COLUMN "Torque" TO "Quilometragem";

ALTER TABLE "Veiculos" RENAME COLUMN "Capacidade" TO "Ano";

CREATE TABLE "ef_temp_Veiculos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Veiculos" PRIMARY KEY AUTOINCREMENT,
    "Ano" TEXT NOT NULL,
    "Marca" TEXT NOT NULL,
    "Modelo" TEXT NOT NULL,
    "Motor" TEXT NOT NULL,
    "Potencia" TEXT NOT NULL,
    "Preco" TEXT NOT NULL,
    "Quilometragem" TEXT NOT NULL
);

INSERT INTO "ef_temp_Veiculos" ("Id", "Ano", "Marca", "Modelo", "Motor", "Potencia", "Preco", "Quilometragem")
SELECT "Id", "Ano", "Marca", "Modelo", "Motor", "Potencia", "Preco", "Quilometragem"
FROM "Veiculos";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Veiculos";

ALTER TABLE "ef_temp_Veiculos" RENAME TO "Veiculos";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251210230830_Atualizarmig', '8.0.21');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251210231520_AtualizarVeiculos', '8.0.21');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Veiculos" ADD "Vendido" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251211170313_AdicionarStatusVendido', '8.0.21');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251211172003_ControleVenda', '8.0.21');

COMMIT;

