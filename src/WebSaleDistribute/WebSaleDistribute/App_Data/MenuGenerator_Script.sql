USE [UsersManagements]
GO

-- =============================================
-- Author:		Behzad Khosravifar
-- Create date: 1395/03/18
-- Last Update: 1395/03/19
-- Description:	Create and Get Web sale distribute menus
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateAndGetWebSaleDistributeMenus] @UserId INT
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

			    -- Add Some Menus to role 7 فروشنده
                    INSERT  INTO MenusToRoles
                    VALUES  ( @ProgramID,	--ProgramID
                              7,	--RoleID
                              @ParentMenuID + 1,	--MenuID
                              1,	--IsEnable
                              0 --IsVisible
                              ),
								-- منوی درخواست
                            ( @ProgramID,	--ProgramID
                              7,	--RoleID
                              @ParentMenuID + 2,	--MenuID
                              1,	--IsEnable
                              0 --IsVisible
                              )


					-- Add Some Menus to role 6 متصدی
                    INSERT  INTO MenusToRoles
                    VALUES  ( @ProgramID,	--ProgramID
                              6,	--RoleID
                              @ParentMenuID + 1,	--MenuID
                              1,	--IsEnable
                              0 --IsVisible
                              ),
								-- منوی درخواست
                            ( @ProgramID,	--ProgramID
                              6,	--RoleID
                              @ParentMenuID + 2,	--MenuID
                              1,	--IsEnable
                              0 --IsVisible
                              )
				
                
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


USE [SaleDistributeIdentity]
GO

-- =============================================
-- Author:		Behzad Khosravifar
-- Create date: 1395/04/31
-- Description:	Check Tablet Employees Roles for Menu
-- =============================================
CREATE PROCEDURE [dbo].[sp_CheckAndCreateEmployeesRoles]
	@EmployeId INT
AS
BEGIN
	BEGIN TRY
    BEGIN TRANSACTION T		

    PRINT 'Start Sync User Data'
    DECLARE
        @UserId INT ,
        @UserFullName NVARCHAR(MAX) ,
        @IMEI VARCHAR(50) ,
        @ProgramID INT = 2;

    PRINT 'Fetching User Data from Users table of UsersManagements db'

    SELECT  @UserId = UserID ,
            @UserFullName = FullName ,
            @IMEI = SystemComputerName
    FROM    UsersManagements.dbo.Users
    WHERE   EmployeeId = @EmployeId

    SELECT  @UserId 'UserID' ,
            @EmployeId 'EmployeeID' ,
            @UserFullName 'EmployeeName' ,
            @IMEI 'IMEI'

    PRINT 'Fetch User Data Completed'

    PRINT 'Adding User to PrgAccess by role 255'
    IF NOT EXISTS ( SELECT  1
                    FROM    UsersManagements.dbo.PrgAccess
                    WHERE   UserID = @UserId )
        BEGIN
    
            INSERT  INTO UsersManagements.dbo.PrgAccess
                    ( UserID ,
                      ProgramID ,
                      AccessTypeID ,
                      CheckComputerAndLogin
                    )
            VALUES  ( @UserId , -- UserID - int
                      @ProgramID , -- ProgramID - smallint
                      255 , -- AccessTypeID - tinyint
                      0  -- CheckComputerAndLogin - tinyint
                    )
            PRINT 'User added to PrgAccess.'
        END
    ELSE
        PRINT 'User already added to PrgAccess!'
            

    PRINT 'Adding User to UsersInRoles by role 7'
    IF NOT EXISTS ( SELECT  1
                    FROM    UsersManagements.dbo.UsersInRoles
                    WHERE   UserID = @UserId )
        BEGIN
            INSERT  INTO UsersManagements.dbo.UsersInRoles
                    ( UserID, ProgramID, RoleID )
            VALUES  ( @UserId, -- UserID - int
                      @ProgramID, -- ProgramID - smallint
                      7  -- RoleID - smallint
                      )
					  PRINT 'User added to UsersInRoles.'
        END
    ELSE
        PRINT 'User already added to UsersInRoles!'
    

	PRINT 'Updating SaleDistributeIdentity database ...'
    EXEC SaleDistributeIdentity.dbo.sp_UpdateDatabase
	PRINT 'Update Completed.'

	PRINT 'Creating and Fetching Menus for this user ...'
    EXEC UsersManagements.dbo.sp_CreateAndGetWebSaleDistributeMenus @UserId = @UserId
	PRINT 'Menus Created.'

    COMMIT TRANSACTION T
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION T	
		
    EXEC UsersManagements.dbo.sp_CatchError @RaisError = 1, -- bit
        @ExtraData = NULL, -- nvarchar(max)
        @ErrorId = NULL -- bigint
END CATCH
END
GO