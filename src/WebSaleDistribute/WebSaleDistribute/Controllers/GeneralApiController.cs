using Dapper;
using Elmah;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Web.Http;
using WebSaleDistribute.Core;
using ZXing.QrCode.Internal;
using AdoManager;

namespace WebSaleDistribute.Controllers
{
    [AllowAnonymous]
    public class GeneralApiController : ApiController
    {

        private Bitmap GenerateQR(int width, int height, string text)
        {
            var bw = new ZXing.BarcodeWriter();
            var encOptions = new ZXing.Common.EncodingOptions() { Width = width, Height = height, Margin = 0 };
            //
            // Error correction
            // Sometimes your QRCode will get damaged or covered up by something – like an image overlay for instance – 
            // therefore the designers of the QRCode has added four levels; 7% (L), 15 % (M), 25% (Q), 30% (H) of error 
            // correction were a error correction of level H should result in a QRCode that are still valid even when it’s 
            // 30% obscured – for more info on error correction check this 
            encOptions.Hints.Add(ZXing.EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;

            var result = new Bitmap(bw.Write(text));

            return result;
        }

        [HttpPost]
        [Route("General/GenerateQrByStoring")]
        public IHttpActionResult GenerateQRByStoring(JObject jsonData)
        {
            try
            {
                dynamic json = jsonData;

                return GenerateQRByStoring(int.Parse((string)json.width),
                    int.Parse((string)json.height), (string)json.content,
                    int.Parse((string)json.recordKey), (string)json.dbConnectionString, (bool?)json.encryptContent ?? false);
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return new System.Web.Http.Results.ExceptionResult(exp, this);
            }
        }

        [HttpGet]
        [Route("General/GenerateQrByStoring/{width}/{height}/{content}/{recordKey}/{dbConnectionStr}/{encryptContent}")]
        public IHttpActionResult GenerateQRByStoring(int width, int height, string content, int recordKey, string dbConnectionStr, bool encryptContent = true)
        {
            try
            {
                var text = encryptContent ? content.Encrypt() : content;

                var qrImg = GenerateQR(width, height, text);

                var cm = new AdoManager.ConnectionManager(new AdoManager.Connection(dbConnectionStr));
                cm.SqlConn.Execute(@"INSERT INTO QR_Codes
                                    (
                                     output_tkey,
                                     qrText,
                                     qrImage
                                    )
                                    VALUES
                                    (
                                     @output_tkey,
                                     @qrText,
                                     @qrImage
                                    )", new { output_tkey = recordKey, qrText = text, qrImage = qrImg.ToByteArray(System.Drawing.Imaging.ImageFormat.Png) });

                return Ok("True");
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return new System.Web.Http.Results.ExceptionResult(exp, this);
            }
        }


        [HttpGet]
        [Route("General/GenerateQR/{width}/{height}/{content}/{encryptContent}")]
        public byte[] GenerateQR(int width, int height, string content, bool encryptContent = true)
        {
            try
            {
                var text = encryptContent ? content.Encrypt() : content;

                var qrImg = GenerateQR(width, height, text);

                var qrBytes = qrImg.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);

                return qrBytes;
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return null;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("General/GetOfficerEmployees/{OfficerEmployeeID}")]
        public IHttpActionResult GetOfficerEmployee(int officerEmployeeID)
        {
            var result = Connections.SaleBranch.SqlConn.Query("SELECT EmployeeName,EmployeeID FROM dbo.udft_Employee(@RunDate) WHERE OfficerEmployeeID=@OfficerEmployeeID",
                new { OfficerEmployeeID = officerEmployeeID, RunDate = DateTime.Now.GetPersianDate() },
                commandType: System.Data.CommandType.Text);

            return Ok(result);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("General/GetEmployeePath/{EmployeeID}")]
        public IHttpActionResult GetEmployeePath(int employeeID)
        {
            var result = Connections.SaleBranch.SqlConn.Query("SELECT PathCode,PathName FROM dbo.udft_VisitPath(@RunDate) WHERE VisitorEmployeeID=@EmployeeID",
                new { EmployeeID = employeeID, RunDate = DateTime.Now.GetPersianDate() },
                commandType: System.Data.CommandType.Text);

            return Ok(result);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("General/GetCustomerPointStatus/{PathCode}/{ClassNames}")]
        public IHttpActionResult GetCustomerPointStatus(int pathCode,string classNames)
        {
            var result = Connections.SaleBranch.SqlConn.ExecuteReader("sp_GetCustomerPointStatus",
                new { PathCode = pathCode, ClassNames= classNames },
                commandType: System.Data.CommandType.StoredProcedure).ToDataTable();
            return Ok(result);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("General/CustomerPointUpdateStatus/{customerId}/{status}/{UserName}")]
        public IHttpActionResult CustomerPointUpdateStatus(int customerId, int status,int userName)
        {
            
            var result = Connections.SaleBranch.SqlConn.ExecuteReader("sp_CustomerPoint_Update_Status",
                new { CustomerID = customerId, Status = status, UserName = userName },
                commandType: System.Data.CommandType.StoredProcedure);
            return Ok(result);
        }
    }
}
