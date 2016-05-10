/************************************************************
 * Call Sale Web Service Restful method to create QR Code and save on sql database
 ************************************************************/

DECLARE @xmlOut VARCHAR(MAX),
        @RequestText AS VARCHAR(8000),
        @TargetSqlConn NVARCHAR(1000) =
        'Data Source=localhost;Initial Catalog=TestDB;Password=123;Persist Security Info=True;User ID=sa;',
        @Width VARCHAR(5) = 100,
        @Height VARCHAR(5) = 100,
        @RecordId VARCHAR(10) = 11,
        @Content NVARCHAR(1000) = 'New GUID=' + CONVERT(NVARCHAR(100), NEWID()),
        @url NVARCHAR(MAX) =
        'http://localhost:33033/general/GenerateQRByStoring/';

SET @url += @Width + '/' + @Height + '/' + @Content + '/' + @RecordId + '/' + @TargetSqlConn;

EXEC spHTTPRequest 
     @url,
     'GET',
     @RequestText,
     '',
     '',
     @xmlOut OUT

SELECT @xmlOut 
