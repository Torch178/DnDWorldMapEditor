BEGIN TRANSACTION;
DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorldMap]') AND [c].[name] = N'Name');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [WorldMap] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [WorldMap] ALTER COLUMN [Name] nvarchar(100) NOT NULL;

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorldMap]') AND [c].[name] = N'Description');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [WorldMap] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [WorldMap] ALTER COLUMN [Description] nvarchar(1000) NULL;

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WorldMap]') AND [c].[name] = N'BackgroundImage');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [WorldMap] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [WorldMap] ALTER COLUMN [BackgroundImage] nvarchar(450) NOT NULL;

ALTER TABLE [WorldMap] ADD [MapSize] nvarchar(5) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260328191839_UpdateWorldMapModel_AddMapSizeProp', N'9.0.14');

COMMIT;
GO

