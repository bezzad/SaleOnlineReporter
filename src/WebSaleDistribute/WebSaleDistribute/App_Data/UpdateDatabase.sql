
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateDatabase]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_UpdateDatabase]

EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_UpdateDatabase]
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
	--#### 4  Set Program Access in UM.PrgAccess by  #####
	--####    insert officer users where ProgramID 2 #####
	--####                                           #####
	--#### 5  Insert UM.UserInRoles data where       #####
	--####    ProgramID 2 and officer users          #####
	--####                                           #####
	--#### 6  Set Program Access in UM.PrgAccess by  #####
	--####    insert visitor users where ProgramID 2 #####
	--####                                           #####
	--#### 7  Insert UM.UserInRoles data where       #####
	--####    ProgramID 2 and visitors users         #####
	--####                                           #####
	--#### 8  Remove SDW.UserRoles and SDW.Users     #####
	--####    and SDW.Roles from ASP Identity Tables #####
	--####                                           #####
	--#### 9  Copy UM.Users where ProgramID 2        #####
	--####    to SDW.Users of ASP Identity table     #####
	--####                                           #####
	--#### 10 Copy UM.Roles where ProgramID 2        #####
	--####    to SDW.Roles of ASP Identity table     #####
	--####                                           #####
	--#### 11 Copy UM.UserInRoles where ProgramID 2  #####
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
                            ''فروشنده''
           
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
                            ''مديريت''
       
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
                            ''مدير فروش''

------------------------------------------------------
-- اضافه کردن سطح دسترسي مدير فروش
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 10 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            10 ,
                            ''حسابرسی''

------------------------------------------------------
-- اضافه کردن سطح دسترسي مدير فروش
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 11 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            11 ,
                            ''فاکتور زنی''

------------------------------------------------------
-- اضافه کردن سطح دسترسي مدير فروش
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 12 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            12 ,
                            ''مسیر زن''

------------------------------------------------------
-- اضافه کردن سطح دسترسي مدير فروش
        IF NOT EXISTS ( SELECT TOP ( 1 )
                                1
                        FROM    UsersManagements.dbo.Roles
                        WHERE   ProgramID = 2
                                AND RoleID = 13 ) 
            INSERT  INTO UsersManagements.dbo.Roles
                    ( ProgramID, RoleID, RoleName )
                    SELECT TOP ( 1 )
                            2 ,
                            13 ,
                            ''انباردار''

--######################################################         
--------------------------------------------------------



--####################################################
--#### 2  Set Program Access in UM.PrgAccess by  #####
--####    insert users where ProgramID 13 to 2   #####
--####################################################
------------------------------------------------------
-- اضافه کردن سطح دسترسي به کاربران برنامه 13 در برنامه ي 2

        INSERT  INTO UsersManagements.dbo.PrgAccess
                SELECT  pa.UserID ,
                        2 ,
                        pa.AccessTypeID ,
                        pa.CheckComputerAndLogin
                FROM    UsersManagements.dbo.PrgAccess pa
                WHERE   pa.ProgramID = 13
                        AND pa.UserID NOT IN (
                        SELECT  UserID
                        FROM    UsersManagements.dbo.PrgAccess
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
--#### 4  Set Program Access in UM.PrgAccess by  #####
--####    insert officer users where ProgramID 2 #####
--####################################################
------------------------------------------------------
-- اضافه کردن دسترسی به متصدیان
		INSERT  INTO UsersManagements.dbo.PrgAccess
        ( UserID ,
          ProgramID ,
          AccessTypeID ,
          CheckComputerAndLogin
        )
        SELECT  u.UserID ,  -- UserID - int
                2 ,-- ProgramID - smallint
                255 , -- AccessTypeID - tinyint
                0 -- CheckComputerAndLogin - tinyint
        FROM    SaleBranch.dbo.Employee e
                INNER JOIN usersmanagements.dbo.users u ON u.EmployeeID = e.EmployeeID
                LEFT JOIN usersmanagements.dbo.PrgAccess pa ON pa.userid = u.userid
        WHERE   e.isActive = 1
                AND e.employeetypeid = 5 -- متصدی منطقه
                AND pa.programid = 2
                AND pa.AccessTypeID IS NULL;


--######################################################
--------------------------------------------------------



--####################################################
--#### 5  Insert UM.UserInRoles data where       #####
--####    ProgramID 2 and officer users          #####
--####################################################
------------------------------------------------------
-- اضافه کردن رول متصدی منطقه به متصدیانی که این رول 6 را ندارند
		INSERT  INTO UsersManagements.dbo.UsersInRoles
        ( UserID ,
          ProgramID ,
          RoleID
        )
        SELECT  u.UserID ,  -- UserID - int
                2 ,-- ProgramID - smallint
                6  -- RoleID - smallint 		 
        FROM    SaleBranch.dbo.Employee e
                INNER JOIN UsersManagements.dbo.Users u ON u.EmployeeId = e.EmployeeId
                LEFT JOIN UsersManagements.dbo.UsersInRoles ur ON ur.UserID = u.UserID
        WHERE   e.isActive = 1
                AND e.employeetypeid = 5 -- متصدی منطقه
                AND ur.ProgramID = 2
                AND ur.RoleID <> 6
        EXCEPT
        SELECT  u.UserID ,  -- UserID - int
                2 ,-- ProgramID - smallint
                6  -- RoleID - smallint 		  
        FROM    SaleBranch.dbo.Employee e
                INNER JOIN UsersManagements.dbo.Users u ON u.EmployeeId = e.EmployeeId
                LEFT JOIN UsersManagements.dbo.UsersInRoles ur ON ur.UserID = u.UserID
        WHERE   e.isActive = 1
                AND e.employeetypeid = 5 -- متصدی منطقه
                AND ur.ProgramID = 2
                AND ur.RoleID = 6;

--######################################################
--------------------------------------------------------



--####################################################
--#### 6  Set Program Access in UM.PrgAccess by  #####
--####    insert visitor users where ProgramID 2 #####
--####################################################
------------------------------------------------------
-- اضافه کردن دسترسی به ویزیتورها
		INSERT  INTO UsersManagements.dbo.PrgAccess
        ( UserID ,
          ProgramID ,
          AccessTypeID ,
          CheckComputerAndLogin
        )
        SELECT  u.UserID ,  -- UserID - int
                2 ,-- ProgramID - smallint
                255 , -- AccessTypeID - tinyint
                0 -- CheckComputerAndLogin - tinyint
        FROM    SaleBranch.dbo.Employee e
                INNER JOIN UsersManagements.dbo.Users u ON u.EmployeeId = e.EmployeeId
                LEFT JOIN UsersManagements.dbo.PrgAccess pa ON pa.UserID = u.UserID
        WHERE   e.isActive = 1
                AND e.employeetypeid = 1 -- فروشنده
                AND pa.ProgramID = 2
                AND pa.AccessTypeID IS NULL;

--######################################################
--------------------------------------------------------



--####################################################
--#### 7  Insert UM.UserInRoles data where       #####
--####    ProgramID 2 and visitors users         #####
--####################################################
------------------------------------------------------
-- اضافه کردن رول فروشنده به ویزیتورهایی که این رول 7 را ندارند
		INSERT  INTO UsersManagements.dbo.UsersInRoles
        ( UserID ,
          ProgramID ,
          RoleID
        )
        SELECT  u.UserID ,  -- UserID - int
                2 ,-- ProgramID - smallint
                7  -- RoleID - smallint 
        FROM    SaleBranch.dbo.Employee e
                INNER JOIN UsersManagements.dbo.Users u ON u.EmployeeId = e.EmployeeId
                LEFT JOIN UsersManagements.dbo.UsersInRoles ur ON ur.UserID = u.UserID
        WHERE   e.isActive = 1
                AND e.employeetypeid = 1 -- فروشنده
                AND ur.ProgramID = 2
                AND ur.RoleID <> 7
        EXCEPT
        SELECT  u.UserID ,  -- UserID - int
                2 ,-- ProgramID - smallint
                7  -- RoleID - smallint 	
        FROM    SaleBranch.dbo.Employee e
                INNER JOIN UsersManagements.dbo.Users u ON u.EmployeeId = e.EmployeeId
                LEFT JOIN UsersManagements.dbo.UsersInRoles ur ON ur.UserID = u.UserID
        WHERE   e.isActive = 1
                AND e.employeetypeid = 1 -- فروشنده
                AND ur.ProgramID = 2
                AND ur.RoleID = 7;

--######################################################
--------------------------------------------------------



--####################################################
--#### 8  Remove SDW.UserRoles and SDW.Users     #####
--####    and SDW.Roles from ASP Identity Tables #####
--####################################################
------------------------------------------------------
-- حذف داده هاي جداول پايگاه داده ASP Identity

        DECLARE @Query VARCHAR(MAX) = ''USE SaleDistributeIdentity '' + CHAR(13);

        SELECT  @Query += ''ALTER TABLE [SaleDistributeIdentity].[''
                + TABLE_SCHEMA + ''].['' + TABLE_NAME + ''] DROP [''
                + CONSTRAINT_NAME + '']; '' + CHAR(13)
        FROM    INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE
        WHERE   CONSTRAINT_NAME IN (
                SELECT  CONSTRAINT_NAME
                FROM    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                WHERE   CONSTRAINT_TYPE = ''FOREIGN KEY'' )
                           
        PRINT @Query       
       
        EXEC(@Query)

        TRUNCATE TABLE SaleDistributeIdentity.dbo.UserRoles
        TRUNCATE TABLE SaleDistributeIdentity.dbo.Roles
        TRUNCATE TABLE SaleDistributeIdentity.dbo.Users
    
        DECLARE @AdminUserID NVARCHAR(MAX) = ''A4E121AE-BECE-4F48-B53F-CF8DCE77E1C7'' ,
            @AdminRoleID NVARCHAR(MAX) = ''4619641B-9534-4DEB-ABAD-BA4B6D2506BA''
   
--######################################################
--------------------------------------------------------



--####################################################
--#### 9  Copy UM.Users where ProgramID 2        #####
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
                        ''shoniz_'' + CONVERT(NVARCHAR(128), MAX(u.UserID))
                        + ''@shoniz.com'' ,
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
                        ''ApplicationUser''
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
                        ''behzad.khosravifar@gmail.com'' ,
                        1 ,
                        SUBSTRING(master.dbo.fn_varbintohexstr(HASHBYTES(''MD5'', ''H\,g,d@13'')), 3, 32) , --Password
                        NEWID() ,
                        ''+989149149202'' ,
                        1 ,
                        0 ,
                        NULL ,
                        0 ,
                        0 ,
                        ''Admin'' ,
                        ''مدیر برنامه'' ,
                        ''353975074012655'' ,
                        ''ApplicationUser''
    
   
--######################################################
--------------------------------------------------------



--####################################################
--#### 10 Copy UM.Roles where ProgramID 2        #####
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
        VALUES  ( @AdminRoleID, ''Admin'' ),
                ( ''DCBF07D0-1712-4C3D-9825-DA615017A2FD'', ''Developer'' ),
                ( ''5C7CFD35-73C3-4B35-AA64-ABE5DA3D16DC'', ''NoAccess'' ) 
    
   
--######################################################
--------------------------------------------------------




--####################################################
--#### 11 Copy UM.UserInRoles where ProgramID 2  #####
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

';
