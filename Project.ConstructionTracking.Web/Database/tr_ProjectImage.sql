/* =============================================================
   tr_ProjectImage — รูปภาพประจำโครงการ (1 โครงการ : 1 รูป Active)
   ใช้แสดงบนหน้า ProjectList และ Unitlist
   ตั้งค่าผ่านเมนู Setting > Project Image
   ============================================================= */

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tr_ProjectImage')
BEGIN
    CREATE TABLE dbo.tr_ProjectImage
    (
        ID          uniqueidentifier NOT NULL,
        ProjectID   uniqueidentifier NULL,
        ResourceID  uniqueidentifier NULL,
        FlagActive  bit              NULL CONSTRAINT DF_tr_ProjectImage_FlagActive DEFAULT ((1)),
        CreateDate  datetime         NULL CONSTRAINT DF_tr_ProjectImage_CreateDate DEFAULT (getdate()),
        CreateBy    uniqueidentifier NULL,
        UpdateDate  datetime         NULL CONSTRAINT DF_tr_ProjectImage_UpdateDate DEFAULT (getdate()),
        UpdateBy    uniqueidentifier NULL,
        CONSTRAINT PK_tr_ProjectImage PRIMARY KEY CLUSTERED (ID)
    );

    ALTER TABLE dbo.tr_ProjectImage WITH CHECK
        ADD CONSTRAINT FK_tr_ProjectImage_tm_Project
        FOREIGN KEY (ProjectID) REFERENCES dbo.tm_Project (ProjectID);

    ALTER TABLE dbo.tr_ProjectImage WITH CHECK
        ADD CONSTRAINT FK_tr_ProjectImage_tm_Resource
        FOREIGN KEY (ResourceID) REFERENCES dbo.tm_Resource (ID);

    /* ค้นรูป Active ของโครงการ — ใช้ทุกครั้งที่เปิดหน้า ProjectList / Unitlist */
    CREATE NONCLUSTERED INDEX IX_tr_ProjectImage_ProjectID_FlagActive
        ON dbo.tr_ProjectImage (ProjectID, FlagActive) INCLUDE (ResourceID);
END
GO
