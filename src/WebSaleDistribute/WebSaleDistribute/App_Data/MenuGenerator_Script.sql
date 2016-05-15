﻿USE [UsersManagements]

DECLARE @ParentMenuID INT ,
    @ReportSettingKey NVARCHAR(50) = 'SaleOnlineReporterMenuID'

SELECT  @ParentMenuID = [Value]
FROM    [PrgSettings]
WHERE   [Key] = @ReportSettingKey


IF ( @ParentMenuID IS NULL
     OR NOT EXISTS ( SELECT 1
                     FROM   PrgMenu pm
                     WHERE  pm.ProgramID = 2
                            AND pm.MenuID = @ParentMenuID )
   ) 
    BEGIN
        SELECT  @ParentMenuID = MAX(pm.MenuID) + 1
        FROM    PrgMenu pm
    
    --===============================================
    -- Add parent menu
    --===============================================
    
        INSERT  INTO PrgMenu
        VALUES  ( @ParentMenuID, 2, 'گزارشات آنلاين', NULL, 100, 0, 'Home', 'Index',
                  NULL )
    
        IF NOT EXISTS ( SELECT  1
                        FROM    PrgSettings ps
                        WHERE   ps.ProgramID = 2
                                AND ps.[Key] = @ReportSettingKey ) 
            BEGIN
                INSERT  INTO PrgSettings
                VALUES  ( 2, @ReportSettingKey, @ParentMenuID )
            END
    
        INSERT  INTO MenusToRoles
        VALUES  ( 2, 1, @ParentMenuID, 1, 1 )
    
    --===============================================
    -- Add sub menus
    --===============================================
        INSERT  INTO PrgMenu
        VALUES  ( @ParentMenuID + 1,	--MenuID
                  2,	--ProgramID
                  'گزارش رسيدي', @ParentMenuID,	--ParentID
                  1,	--order
                  0, 'Reports', 'Receipts', NULL ),
                ( @ParentMenuID + 2,	--MenuID
                  2,	--ProgramID
                  'گزارشات فروش', @ParentMenuID,	--ParentID
                  2,	--order
                  0, 'Reports', 'Sales', NULL ),
				( @ParentMenuID + 3,	--MenuID
                  2,	--ProgramID
                  'انبار', @ParentMenuID,	--ParentID
                  3,	--order
                  0, 'Warehouse', 'Index', NULL )
    
    -----------------------------------------------
    -------- Add roles to all sub menus -----------
        DECLARE @cnt INT = 1,  
			@MenusCount INT = 1;

		SELECT @MenusCount = Count(1)
        FROM   PrgMenu pm
        WHERE  pm.ProgramID = 2
            AND pm.ParentMenuID = @ParentMenuID 

        WHILE @cnt < @MenusCount 
            BEGIN
                INSERT  INTO MenusToRoles
                VALUES  ( 2,	--ProgramID
                          1,	--RoleID
                          @ParentMenuID + @cnt,	--MenuID
                          1,	--IsEnable
                          1 --IsVisible
                          )
        
                SET @cnt = @cnt + 1;
            END;
    -----------------------------------------------
    END


SELECT  *
FROM    PrgMenu pm
WHERE   pm.ProgramID = 2
        AND pm.ParentMenuID = @ParentMenuID 