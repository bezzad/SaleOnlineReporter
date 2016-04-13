USE [SaleDistributeIdentity]
GO

-- =============================================
-- Author:		Behzad Khosravifar
-- Create date: 1395/01/24
-- Description:	Set UsersManagements users and roles to ASP.NET Identity Entity Tables
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateDatabase]
AS 
    BEGIN
        
	------------------------------------------------------
	--####################################################
	--####                                           #####
	--#### 1  Insert UM.Roles From ProgramID 13 to 2 #####
	--####                                           #####
	--#### 2  Set Program Access in UM.PrgAccess by  #####
	--####    insert users where ProgramID 13 to 2   #####
	--####                                           #####
	--#### 3  Insert UM.UserInRoles data where       #####
	--####    ProgramID 13 to 2                      #####
	--####                                           #####
	--#### 4  Remove SDW.UserRoles and SDW.Users     #####
	--####    and SDW.Roles from ASP Identity Tables #####
	--####                                           #####
	--#### 5  Copy UM.Users where ProgramID 2        #####
	--####    to SDW.Users of ASP Identity table     #####
	--####                                           #####
	--#### 6  Copy UM.Roles where ProgramID 2        #####
	--####    to SDW.Roles of ASP Identity table     #####
	--####                                           #####
	--#### 7  Copy UM.UserInRoles where ProgramID 2  #####
	--####    to SDW.UserRoles of ASP Identity table #####
	--####                                           #####
	--####################################################
	------------------------------------------------------



--####################################################
--#### 1  Insert UM.Roles From ProgramID 13 to 2 #####
--####################################################
------------------------------------------------------
-- اضافه کردن سطح دسترسي فروشنده
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 7 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            7 ,
                            'فروشنده'
           
------------------------------------------------------
-- اضافه کردن سطح دسترسي مديريت
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 8 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            8 ,
                            'مديريت'
       
------------------------------------------------------
-- اضافه کردن سطح دسترسي مدير فروش
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 9 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            9 ,
                            'مدير فروش'

--######################################################         
--------------------------------------------------------



--####################################################
--#### 2  Set Program Access in UM.PrgAccess by  #####
--####    insert users where ProgramID 13 to 2   #####
--####################################################
------------------------------------------------------
-- اضافه کردن سطح دسترسي به کاربران برنامه 13 در برنامه ي 2

        INSERT  INTO PrgAccess
                SELECT  pa.UserID ,
                        2 ,
                        pa.AccessTypeID ,
                        pa.CheckComputerAndLogin
                FROM    PrgAccess pa
                WHERE   pa.ProgramID = 13
                        AND pa.UserID NOT IN (
                        SELECT  UserID
                        FROM    PrgAccess
                        WHERE   PrgAccess.ProgramID = 2 )


--######################################################
--------------------------------------------------------



--####################################################
--#### 3  Insert UM.UserInRoles data where       #####
--####    ProgramID 13 to 2                      #####
--####################################################
------------------------------------------------------
-- اضافه کردن کاربران برنامه ي کد 13 به کد 2          
           
        INSERT  INTO UsersManagements.dbo.UsersInRoles
                ( UserID ,
                  ProgramID ,
                  RoleID
                )
                SELECT  userRole.*
                FROM    ( SELECT    uir.UserID ,
                                    2 AS ProgramID ,
                                    CASE uir.RoleID
                                      WHEN 1 THEN 8 --مديريت
                                      WHEN 2 THEN 9 --مدير فروش
                                      WHEN 3 THEN 1 --سرپرست مرکز پخش
                                      WHEN 4 THEN 6 --متصدي
                                      WHEN 5 THEN 7 --فروشنده
                                      ELSE uir.RoleID
                                    END AS RoleID
                          FROM      UsersManagements.dbo.UsersInRoles uir
                          WHERE     uir.ProgramID = 13
                        ) userRole
                        LEFT JOIN UsersManagements.dbo.UsersInRoles uir2 ON uir2.UserID = userRole.UserID
                                                              AND uir2.ProgramID = userRole.ProgramID
                                                              AND uir2.RoleID = userRole.RoleID
                WHERE   uir2.RoleID IS NULL

--######################################################
--------------------------------------------------------




--####################################################
--#### 4  Remove SDW.UserRoles and SDW.Users     #####
--####    and SDW.Roles from ASP Identity Tables #####
--####################################################
------------------------------------------------------
-- حذف داده هاي جداول پايگاه داده ASP Identity

        TRUNCATE TABLE SaleDistributeIdentity.dbo.UserRoles
        TRUNCATE TABLE SaleDistributeIdentity.dbo.Roles
        TRUNCATE TABLE SaleDistributeIdentity.dbo.Users
    
        DECLARE @AdminUserID NVARCHAR(MAX) = NEWID(),
            @AdminRoleID NVARCHAR(MAX) = NEWID() 
   
--######################################################
--------------------------------------------------------



--####################################################
--#### 5  Copy UM.Users where ProgramID 2        #####
--####    to SDW.Users of ASP Identity table     #####
--####################################################
------------------------------------------------------
-- کپي کاربران به پايگاه داده ي ASP Identity

        INSERT  INTO SaleDistributeIdentity.dbo.Users
                ( UserId ,
                  Email ,
                  EmailConfirmed ,
                  PasswordHash ,
                  SecurityStamp ,
                  PhoneNumber ,
                  PhoneNumberConfirmed ,
                  TwoFactorEnabled ,
                  LockoutEndDateUtc ,
                  LockoutEnabled ,
                  AccessFailedCount ,
                  UserName ,
                  FullName ,
                  IMEI ,
                  Discriminator
                )
                SELECT  CONVERT(NVARCHAR(128), MAX(u.UserID)) UserId ,
                        NULL ,
                        0 ,
                        MAX(SUBSTRING(master.dbo.fn_varbintohexstr(u.UserPass),
                                      3, 32)) ,
                        NEWID() ,
                        NULL ,
                        0 ,
                        0 ,
                        NULL ,
                        0 ,
                        0 ,
                        u.EmployeeId ,
                        MAX(u.FullName) ,
                        MAX(u.SystemComputerName) ,
                        'ApplicationUser'
                FROM    UsersManagements.dbo.Users u
                WHERE   u.UserID > 0
                        AND u.EmployeeId IS NOT NULL
                        AND EmployeeId > 0
                GROUP BY u.EmployeeId
                ORDER BY u.EmployeeId
                
                    
        INSERT  INTO SaleDistributeIdentity.dbo.Users
                ( UserId ,
                  Email ,
                  EmailConfirmed ,
                  PasswordHash ,
                  SecurityStamp ,
                  PhoneNumber ,
                  PhoneNumberConfirmed ,
                  TwoFactorEnabled ,
                  LockoutEndDateUtc ,
                  LockoutEnabled ,
                  AccessFailedCount ,
                  UserName ,
                  FullName ,
                  IMEI ,
                  Discriminator
                )
                SELECT  @AdminUserID ,
                        'behzad.khosravifar@gmail.com' ,
                        1 ,
                        SUBSTRING(master.dbo.fn_varbintohexstr(HASHBYTES('MD5',
                                                              'admin')), 3, 32) ,
                        NEWID() ,
                        '+989149149202' ,
                        1 ,
                        0 ,
                        NULL ,
                        0 ,
                        0 ,
                        'admin' ,
                        'Behzad' ,
                        '' ,
                        'ApplicationUser'
    
   
--######################################################
--------------------------------------------------------



--####################################################
--#### 6  Copy UM.Roles where ProgramID 2        #####
--####    to SDW.Roles of ASP Identity table     #####
--####################################################
------------------------------------------------------
-- کپي سطح دسترسي به پايگاه داده ي ASP Identity

        INSERT  INTO SaleDistributeIdentity.dbo.Roles
                SELECT  r.RoleID ,
                        r.RoleName
                FROM    UsersManagements.dbo.Roles r
                        LEFT JOIN SaleDistributeIdentity.dbo.Roles r2 ON r.RoleID = r2.Id
                WHERE   r.ProgramID = 2
                        AND r2.Id IS NULL
                        
        INSERT  INTO SaleDistributeIdentity.dbo.Roles
        VALUES  ( @AdminRoleID, 'Admin' ),
                ( NEWID(), 'Developer' ),
                ( NEWID(), 'NoAccess' ) 
    
   
--######################################################
--------------------------------------------------------




--####################################################
--#### 7  Copy UM.UserInRoles where ProgramID 2  #####
--####    to SDW.UserRoles of ASP Identity table #####
--####################################################
------------------------------------------------------
-- کپي سطح دسترسي کاربران به پايگاه داده ي ASP Identity

        INSERT  INTO SaleDistributeIdentity.dbo.UserRoles
                ( UserId ,
                  RoleId ,
                  IdentityUser_Id
                )
                SELECT  uir.UserID ,
                        uir.RoleID ,
                        NULL
                FROM    UsersManagements.dbo.UsersInRoles uir
                        LEFT JOIN SaleDistributeIdentity.dbo.UserRoles ur ON ur.UserId = uir.UserID
                                                              AND ur.RoleID = uir.RoleID
                WHERE   uir.ProgramID = 2
                        AND ur.UserId IS NULL  
                        
                        
        INSERT  INTO SaleDistributeIdentity.dbo.UserRoles
                ( UserId ,
                  RoleId ,
                  IdentityUser_Id
                )
                SELECT  @AdminUserID ,
                        @AdminRoleID ,
                        NULL
    
   
--######################################################
--------------------------------------------------------
        
    END
