using System.Globalization;
using System.Threading.Tasks;
using BarionClientLibrary;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using MyClickDoctorBE.Models;
using MyClickDoctorBE.DatabaseEntity;
using Microsoft.Extensions.Logging;
using BarionClientLibrary.Operations.Refund;
using System.Linq;
using System.Net;

namespace MyClickDoctorBE.Controllers
{
    [Route("[controller]/[action]")]
    public class PaymentsController : Controller
    {
        private readonly BarionClient _barionClient;
        private readonly BarionSettings _barionSettings;
        private readonly ILogger<PaymentsController> _log;
        public PaymentsController(BarionClient barionClient, BarionSettings barionSettings, ILogger<PaymentsController> log)
        {
            _barionClient = barionClient;
            _barionSettings = barionSettings;
            _log = log;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> PaymentGateway(int Price, int AppointmentId, int UserId, string baseUrl)
        {
            TempData["baseURL"] = baseUrl;
            //    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var item = new Item
            {
                Name = "Appointment Fees",
                Description = "Appointment Fees",
                Quantity = 1,
                UnitPrice = Price,
                ItemTotal = Price,
                Unit = "piece",
                SKU = "SKU"
            };
            var transaction = new PaymentTransaction
            {
                Items = new[] { item },
                POSTransactionId = "T1",
                Payee = _barionSettings.Payee,
                Total = Price
            };
            var startPayment = new StartPaymentOperation
            {
                Transactions = new[] { transaction },
                PaymentType = PaymentType.Immediate,
                Currency = BarionClientLibrary.Operations.Common.Currency.HUF,
                FundingSources = new[] { FundingSourceType.All },
                GuestCheckOut = true,
                Locale = CultureInfo.CurrentCulture,
                OrderNumber = "Order1",
                PaymentRequestId = "R1",
                CallbackUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Url.Action("Callback", "Payments")),
                RedirectUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Url.Action("Callback", "Payments"))
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            // System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            _barionClient.Timeout = TimeSpan.FromSeconds(10);
            _barionClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            var result = await _barionClient.ExecuteAsync(startPayment);
            if (result.IsOperationSuccessful)
            {
                var startPaymentReult = result as StartPaymentOperationResult;
                _log.LogInformation(startPaymentReult.CallbackUrl, startPaymentReult.Errors, startPaymentReult.GatewayUrl, startPaymentReult.IsOperationSuccessful, startPaymentReult.PaymentId, startPaymentReult.PaymentRequestId, startPaymentReult.QRUrl, startPaymentReult.RecurrenceResult, startPaymentReult.RedirectUrl, startPaymentReult.Status, startPaymentReult.Transactions);
                AppDbCommonLogic.InsertPaymentWithPendingStatus(Price, 1, AppointmentId, UserId, startPaymentReult.PaymentId.ToString(), "", startPaymentReult.Transactions[0].TransactionId.ToString(), startPaymentReult.Transactions[0].POSTransactionId);
                AppDbCommonLogic.InsertPaymentLogsWithPendingStatus(Price, AppointmentId, startPaymentReult.PaymentId.ToString());
                return Redirect(startPaymentReult.GatewayUrl);
            }
            return View();
        }
        public async Task<ActionResult> Callback(string PaymentId)
        {
            try
            {
                var getPaymentState = new GetPaymentStateOperation
                {
                    PaymentId = new Guid(PaymentId)
                };
                var result = await _barionClient.ExecuteAsync(getPaymentState);
                if (result.IsOperationSuccessful)
                {
                    var paymentState = result as GetPaymentStateOperationResult;
                    _log.LogInformation(paymentState.CallbackUrl, paymentState.CompletedAt, paymentState.CreatedAt, paymentState.Currency, paymentState.Errors, paymentState.FraudRiskScore, paymentState.FundingInformation, paymentState.FundingSource, paymentState.GuestCheckout, paymentState.IsOperationSuccessful, paymentState.PaymentId, paymentState.PaymentRequestId, paymentState.PaymentType, paymentState.POSId, paymentState.POSName, paymentState.POSOwnerEmail, paymentState.RedirectUrl, paymentState.ReservedUntil, paymentState.Status, paymentState.SuggestedLocale, paymentState.Total, paymentState.Transactions, paymentState.ValidUntil);
                    if (paymentState.Status.ToString() == "Succeeded")
                    {
                        AppDbCommonLogic.InsertPaymentWithSuccessStatus(PaymentId.ToString());
                        int AppId = AppDbCommonLogic.GetAppointmentId(PaymentId.ToString());
                        AppDbCommonLogic.UpdateAppointmentStatus(AppId);
                        AppDbCommonLogic.AddBannerRecord(AppId);
                        AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                        Response.Redirect("https://test.myclickdoctor.com/layout/patient/payment-status?TransactionId='" + PaymentId.ToString() + "'&Status=1" + "&AppId=" + AppId);
                    }
                    else
                    {
                        AppDbCommonLogic.InsertPaymentWithFailedStatus(PaymentId.ToString());
                        _log.LogInformation(paymentState.CallbackUrl, paymentState.CompletedAt, paymentState.CreatedAt, paymentState.Currency, paymentState.Errors, paymentState.FraudRiskScore, paymentState.FundingInformation, paymentState.FundingSource, paymentState.GuestCheckout, paymentState.IsOperationSuccessful, paymentState.PaymentId, paymentState.PaymentRequestId, paymentState.PaymentType, paymentState.POSId, paymentState.POSName, paymentState.POSOwnerEmail, paymentState.RedirectUrl, paymentState.ReservedUntil, paymentState.Status, paymentState.SuggestedLocale, paymentState.Total, paymentState.Transactions, paymentState.ValidUntil);
                        // var baseURL = TempData["baseURL"];
                        // string redirect = baseURL + "layout/patient/payment-status?TransactionId='" + PaymentId.ToString() + "'&Status=0";
                        Response.Redirect("https://test.myclickdoctor.com/layout/patient/payment-status?TransactionId='" + PaymentId.ToString() + "'&Status=0" + "&AppId=0");
                    }
                    return Json(new { Success = true, Status = paymentState.Status });
                }
                else
                {
                    var paymentState = result as GetPaymentStateOperationResult;
                    AppDbCommonLogic.InsertPaymentWithFailedStatus(PaymentId.ToString());
                    _log.LogInformation(paymentState.CallbackUrl, paymentState.CompletedAt, paymentState.CreatedAt, paymentState.Currency, paymentState.Errors, paymentState.FraudRiskScore, paymentState.FundingInformation, paymentState.FundingSource, paymentState.GuestCheckout, paymentState.IsOperationSuccessful, paymentState.PaymentId, paymentState.PaymentRequestId, paymentState.PaymentType, paymentState.POSId, paymentState.POSName, paymentState.POSOwnerEmail, paymentState.RedirectUrl, paymentState.ReservedUntil, paymentState.Status, paymentState.SuggestedLocale, paymentState.Total, paymentState.Transactions, paymentState.ValidUntil);
                    // var baseURL = TempData["baseURL"];
                    // string redirect = baseURL + "layout/patient/payment-status?TransactionId='" + PaymentId.ToString() + "'&Status=0";
                    Response.Redirect("https://test.myclickdoctor.com/layout/patient/payment-status?TransactionId='" + PaymentId.ToString() + "'&Status=0" + "&AppId=0");
                    return Json(new { Success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Staus = ex.Message });
            }

        }
        public async Task<ActionResult> Refund(string TransactionId, string PaymentId)
        {
            var Transaction = new TransactionToRefund
            {
                TransactionId = new Guid(TransactionId),
                POSTransactionId = "T1",
                AmountToRefund = 1000.00000M,
                Comment = "Please refund",
            };
            var refund = new RefundOperation
            {
                TransactionsToRefund = new[] { Transaction },
                PaymentId = new Guid(PaymentId),
            };
            var result = await _barionClient.ExecuteAsync(refund);
            if (result.IsOperationSuccessful)
            {
                var startrefund = result as RefundOperationResult;
            }
            return Json(new { Success = false });
        }
    }
}
