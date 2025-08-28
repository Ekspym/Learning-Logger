BEGIN TRANSACTION;
MERGE [dbo].[TaskStatus] AS target
    USING (
    SELECT 1, N'Pending', N'Task is waiting to be processed'
    UNION ALL SELECT 2, N'Queued', N'Task is queued for processing'
    UNION ALL SELECT 3, N'In Progress', N'Task is currently being processed'
    UNION ALL SELECT 4, N'Completed', N'Task has been successfully completed'
    UNION ALL SELECT 5, N'Paused', N'Task processing is paused'
    UNION ALL SELECT 6, N'Canceled', N'Task has been canceled'
    UNION ALL SELECT 7, N'Failed', N'Task processing failed'
    ) AS source ([TaskStatusId], [Name], [Description])
    ON (target.TaskStatusId = source.TaskStatusId)
    WHEN MATCHED THEN
UPDATE SET
    Name = source.Name,
    Description = source.Description
    WHEN NOT MATCHED THEN
INSERT ([TaskStatusId], [Name], [Description])
VALUES (source.TaskStatusId, source.Name, source.Description);
COMMIT;
