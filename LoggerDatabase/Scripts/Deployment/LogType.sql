MERGE [dbo].[LogType] AS target
    USING (
    SELECT 0, N'Unknown', N''
    UNION ALL SELECT 1, N'Information', N''
    UNION ALL SELECT 2, N'Debug', N''
    UNION ALL SELECT 3, N'Warning', N''
    UNION ALL SELECT 4, N'Error', N''
    UNION ALL SELECT 5, N'Critical', N''
    ) AS source ([Id], [Name], [Description])
    ON (target.LogTypeID = source.Id)
    WHEN MATCHED THEN
UPDATE SET
    Name = source.Name,
    Description = source.Description
    WHEN NOT MATCHED THEN
INSERT ([LogTypeID], [Name], [Description])
VALUES (source.Id, source.Name, source.Description);
