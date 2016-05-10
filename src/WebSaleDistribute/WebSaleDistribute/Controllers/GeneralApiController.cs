using Dapper;
using Elmah;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Web.Http;
using WebSaleDistribute.Core;
using ZXing.QrCode.Internal;

namespace WebSaleDistribute.Controllers
{
    [AllowAnonymous]
    public class GeneralApiController : ApiController
    {
        [HttpGet]
        [Route("General/GenerateQrByStoring/{width}/{height}/{content}/{recordKey}/{dbConnectionStr}")]
        public IHttpActionResult GenerateQRByStoring(int width, int height, string content, int recordKey, string dbConnectionStr)
        {
            try
            {
                var qrImg = GenerateQR(width, height, content);

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
                                    )", new { output_tkey = recordKey, qrText = content, qrImage = qrImg.ToByteArray(System.Drawing.Imaging.ImageFormat.Png) });

                return Ok("True");
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return new System.Web.Http.Results.ExceptionResult(exp, this);
            }
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
                    int.Parse((string)json.recordKey), (string)json.dbConnectionString);
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                return new System.Web.Http.Results.ExceptionResult(exp, this);
            }
        }


        [HttpGet]
        [Route("General/GenerateQR")]
        public Bitmap GenerateQR(int width, int height, string text)
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
    }
}
