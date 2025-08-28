BEGIN TRANSACTION;
MERGE [dbo].[TaskType] AS target
    USING (
    SELECT 1, N'ZipModule', N'Task description for Task1'
    UNION ALL SELECT 2, N'LogModule', N'Task description for Task2'
    ) AS source ([TaskTypeId], [Name], [Description])
    ON (target.TaskTypeId = source.TaskTypeId)
    WHEN MATCHED THEN
UPDATE SET
    Name = source.Name,
    Description = source.Description
    WHEN NOT MATCHED THEN
INSERT ([TaskTypeId], [Name], [Description])
VALUES (source.TaskTypeId, source.Name, source.Description);
COMMIT;
