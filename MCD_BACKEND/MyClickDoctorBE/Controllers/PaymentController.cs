using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using BarionClientLibrary;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MyClickDoctorBE.Models;
using OfficeOpenXml;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly BarionClient _barionClient;
        private readonly BarionSettings _barionSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        public PaymentController(BarionClient barionClient, BarionSettings barionSettings, IHostingEnvironment hostingEnvironment)
        {
            _barionClient = barionClient;
            _barionSettings = barionSettings;
            _hostingEnvironment = hostingEnvironment;
            
        }
        [HttpPost]
        public async Task<ActionResult> StartPayment(Product product)
        {
            var item = new Item
            {
                Name = product.Name,
                Description = product.Name,
                Quantity = 1,
                UnitPrice = product.Price,
                ItemTotal = product.Price,
                Unit = "piece",
                SKU = "SKU"
            };
            var transaction = new PaymentTransaction
            {
                Items = new[] { item },
                POSTransactionId = "T1",
                Payee = _barionSettings.Payee,
                Total = product.Price
            };
            var startPayment = new StartPaymentOperation
            {
                Transactions = new[] { transaction },
                PaymentType = PaymentType.Immediate,
                Currency = Currency.HUF,
                FundingSources = new[] { FundingSourceType.All },
                GuestCheckOut = true,
                Locale = CultureInfo.CurrentCulture,
                OrderNumber = "Order1",
                PaymentRequestId = "R1",
                CallbackUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Url.Action("Callback", "Barion")),
                RedirectUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Url.Action("PaymentFinished", "Home"))
            };
            var result = await _barionClient.ExecuteAsync(startPayment);
            if (result.IsOperationSuccessful)
            {
                var startPaymentReult = result as StartPaymentOperationResult;
                return Redirect(startPaymentReult.GatewayUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        //[HttpPost]
        //public ActionResult File(FileUploadViewModel model)
        //{
        //    string rootFolder = _hostingEnvironment.WebRootPath;
        //    string fileName = Guid.NewGuid().ToString() + model.XlsFile.FileName;
        //    FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
        //    using (var stream = new MemoryStream())
        //    {
        //        model.XlsFile.CopyToAsync(stream);
        //        using (var package = new ExcelPackage(stream))
        //        {
        //            package.SaveAs(file);
        //        }
        //    }

        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //        if (worksheet == null)
        //        {

        //            //return or alert message here
        //        }
        //        else
        //        {

        //            var rowCount = worksheet.Dimension.Rows;
        //            for (int row = 2; row <= rowCount; row++)
        //            {
        //                model.StaffInfoViewModel.StaffList.Add(new StaffInfoViewModel
        //                {
        //                    FirstName = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim(),
        //                    LastName = (worksheet.Cells[row, 2].Value ?? string.Empty).ToString().Trim(),
        //                    Email = (worksheet.Cells[row, 3].Value ?? string.Empty).ToString().Trim(),
        //                });
        //            }

        //        }
        //    }
        //}
    }
}