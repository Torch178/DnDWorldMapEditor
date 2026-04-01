BEGIN TRANSACTION;
CREATE TABLE [GridCharacter] (
    [Id] int NOT NULL IDENTITY,
    [GridSpaceId] int NOT NULL,
    [CharacterId] int NOT NULL,
    CONSTRAINT [PK_GridCharacter] PRIMARY KEY ([Id])
);

CREATE TABLE [GridEncounter] (
    [Id] int NOT NULL IDENTITY,
    [GridSpaceId] int NOT NULL,
    [EncounterId] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    CONSTRAINT [PK_GridEncounter] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260401045921_UpdateContext_GridEncounter_GridCharacter_Models', N'9.0.14');

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorldMap]') AND [c].[name] = N'MapSize');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [WorldMap] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [WorldMap] ALTER COLUMN [MapSize] nvarchar(6) NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260401195733_UpdateMaxSizeConstraint_WorldMap_MapSize', N'9.0.14');

COMMIT;
GO

