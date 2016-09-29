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