USE [UsersManagements]
Go

-- =============================================
-- Author:		Behzad Khosravifar
-- Create date: 1395/03/18
-- Last Update: 1395/03/19
-- Description:	Create and Get Web sale distribute menus
-- =============================================
CREATE PROCEDURE sp_CreateAndGetWebSaleDistributeMenus @UserId INT
AS
    BEGIN        
        BEGIN TRY
            BEGIN TRANSACTION T

            DECLARE @ParentMenuID INT ,
                @ReportSettingKey NVARCHAR(50) = 'SaleOnlineReporterMenuID' ,
                @ProgramID INT = 2

            SELECT  @ParentMenuID = [Value]
            FROM    [PrgSettings]
            WHERE   [Key] = @ReportSettingKey

            IF ( @ParentMenuID IS NULL
                 OR NOT EXISTS ( SELECT 1
                                 FROM   PrgMenu pm
                                 WHERE  pm.ProgramID = @ProgramID
                                        AND pm.MenuID = @ParentMenuID )
               )
                BEGIN
                    SELECT  @ParentMenuID = MAX(pm.MenuID) + 1
                    FROM    PrgMenu pm
    
    --===============================================
    -- Add parent menu
    --===============================================
    
    -- 2161 امکانات سيستم
    --     2162 امکانات
    --         3014 گزارشات آنلاين  @ParentMenuID
    
                    INSERT  INTO PrgMenu
                            ( MenuID ,
                              ProgramID ,
                              MenuName ,
                              ParentMenuID ,
                              MenuOrder ,
                              ForceDisabled ,
                              Link ,
                              PageName ,
                              Icon
                            )
                    VALUES  ( @ParentMenuID ,
                              @ProgramID ,
                              'گزارشات آنلاين' ,
                              2162 ,
                              10 ,
                              0 ,
                              'Home' ,
                              'Index' ,
                              NULL
                            )
    
                    IF NOT EXISTS ( SELECT  1
                                    FROM    PrgSettings ps
                                    WHERE   ps.ProgramID = @ProgramID
                                            AND ps.[Key] = @ReportSettingKey )
                        BEGIN
                            INSERT  INTO PrgSettings
                            VALUES  ( @ProgramID, @ReportSettingKey,
                                      @ParentMenuID )
                        END
    
                    INSERT  INTO MenusToRoles
                    VALUES  ( @ProgramID, 1, @ParentMenuID, 1, 1 )
    
    --===============================================
    -- Add sub menus
    --===============================================
                    INSERT  INTO PrgMenu
                    VALUES  ( @ParentMenuID + 1,	--MenuID
                              @ProgramID,	--ProgramID
                              'گزارش رسيدي', @ParentMenuID,	--ParentID
                              1,	--order
                              0, 'Reports', 'Receipts', NULL ),
                            ( @ParentMenuID + 2,	--MenuID
                              @ProgramID,	--ProgramID
                              'درخواست مشتریان', @ParentMenuID,	--ParentID
                              @ProgramID,	--order
                              0, 'Reports', 'CustomersOrders', NULL ),
                            ( @ParentMenuID + 3,	--MenuID
                              @ProgramID,	--ProgramID
                              'انبار', @ParentMenuID,	--ParentID
                              3,	--order
                              0, 'Warehouse', 'Warehouse', NULL )
    
    -----------------------------------------------
    -------- Add roles to all sub menus -----------
                    DECLARE @cnt INT = 1 ,
                        @MenusCount INT = 1;

                    SELECT  @MenusCount = COUNT(1)
                    FROM    PrgMenu pm
                    WHERE   pm.ProgramID = @ProgramID
                            AND pm.ParentMenuID = @ParentMenuID 

                    WHILE @cnt <= @MenusCount
                        BEGIN
                            INSERT  INTO MenusToRoles
                            VALUES  ( @ProgramID,	--ProgramID
                                      1,	--RoleID
                                      @ParentMenuID + @cnt,	--MenuID
                                      1,	--IsEnable
                                      0 --IsVisible
                                      )
        
                            SET @cnt = @cnt + 1;
                        END;
                
                -- Add Warehouse role 13
                    INSERT  INTO MenusToRoles
                    VALUES  ( @ProgramID,	--ProgramID
                              13,	-- warehouse RoleID
                              @ParentMenuID + 3,	--MenuID
                              1,	--IsEnable
                              0 --IsVisible
                              )
                    
    -----------------------------------------------
                END

            SELECT  *
            FROM    MenusToRolesView mtrv
            WHERE   mtrv.ProgramID = @ProgramID
                    AND mtrv.ParentMenuID = @ParentMenuID
                    AND mtrv.UserID = @UserId
        
            COMMIT TRANSACTION T
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION T
	
            SELECT  ERROR_NUMBER() AS ErrorNumber ,
                    ERROR_SEVERITY() AS ErrorSeverity ,
                    ERROR_STATE() AS ErrorState ,
                    ERROR_PROCEDURE() AS ErrorProcedure ,
                    ERROR_LINE() AS ErrorLine ,
                    ERROR_MESSAGE() AS ErrorMessage;  
        END CATCH


    END
GO