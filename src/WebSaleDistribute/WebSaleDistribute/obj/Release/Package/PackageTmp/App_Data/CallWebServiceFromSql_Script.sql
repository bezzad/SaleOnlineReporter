/************************************************************
 * Call Sale Web Service Restful method to create QR Code and save on sql database
 ************************************************************/

DECLARE @xmlOut VARCHAR(MAX) ,
        @RequestText AS VARCHAR(8000) ,
        @TargetSqlConn NVARCHAR(1000) = 'Data Source=' + @@SERVERNAME
        + ';Initial Catalog=' + DB_NAME() + ';Integrated Security=true;' ,
        @Width VARCHAR(5) = 100 ,
        @Height VARCHAR(5) = 100 ,
        @RecordId VARCHAR(10) = CONVERT(VARCHAR, @tkey) ,
        @Content NVARCHAR(250) = CONVERT(NVARCHAR(50), NEWID()) ,
        @ServerUrl VARCHAR(100) ,
        @url NVARCHAR(3000) = 'general/GenerateQRByStoring';
	
    SELECT  @ServerUrl = [value]
    FROM    settings s
    WHERE   s.[key] = 'WebSaleDistributeUrl'
	
    SET @url = @ServerUrl + '/' + @url + '/' + @Width + '/' + @Height + '/'
        + @Content + '/' + @RecordId + '/' + @TargetSqlConn;
	
    EXEC spHTTPRequest @url, 'GET', @RequestText, '', '', @xmlOut OUT

SELECT @xmlOut 
