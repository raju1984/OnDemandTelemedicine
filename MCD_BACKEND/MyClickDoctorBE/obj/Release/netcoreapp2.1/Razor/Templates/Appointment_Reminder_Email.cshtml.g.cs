#pragma checksum "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c4d020037f4269ebcfea0ee7325f0235b01fdc74"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Templates_Appointment_Reminder_Email), @"mvc.1.0.view", @"/Templates/Appointment_Reminder_Email.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Templates/Appointment_Reminder_Email.cshtml", typeof(AspNetCore.Templates_Appointment_Reminder_Email))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c4d020037f4269ebcfea0ee7325f0235b01fdc74", @"/Templates/Appointment_Reminder_Email.cshtml")]
    public class Templates_Appointment_Reminder_Email : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MyClickDoctorBE.Models.RemainderEmail>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("style", new global::Microsoft.AspNetCore.Html.HtmlString("background: #eff7f8; padding:0px; margin:0px;font-family: Trebuchet MS sans-serif; height: 100vh;"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(46, 25, true);
            WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n");
            EndContext();
            BeginContext(71, 17, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c4d020037f4269ebcfea0ee7325f0235b01fdc743550", async() => {
                BeginContext(77, 4, true);
                WriteLiteral("\r\n\r\n");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(88, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(90, 2751, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c4d020037f4269ebcfea0ee7325f0235b01fdc744733", async() => {
                BeginContext(202, 1085, true);
                WriteLiteral(@"

    <style type=""text/css"">
        p b {
            color: #000;
        }

        p {
            margin-block-start: 0px;
            margin-block-end: 0px;
            font-size: 13px;
            line-height: 18px;
            margin-bottom: 6px;
            color: #545454;
        }

        .unsubscribe {
            text-align: center;
            background: #ff2525;
            color: #FFF;
            text-transform: uppercase;
            text-decoration: none;
            display: block;
            padding: 15px;
            border-radius: 4px;
            letter-spacing: 1px;
            font-size: 13px;
            font-weight: 600;
        }
    </style>

    <table style=""width: 600px;margin: auto; background:#FFF; padding:0.5em 1em;height: 100%;box-shadow: #3333331c 0px 5px 10px;"">
        <tbody>
            <tr>

                <td>
                    <table style=""width: 100%;border-spacing: 10px;"">
                        <tbody>
             ");
                WriteLiteral("               <tr>\r\n                                <td><img");
                EndContext();
                BeginWriteAttribute("src", " src=\"", 1287, "\"", 1344, 2);
#line 46 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
WriteAttributeValue("", 1293, Model.ApiPath, 1293, 14, false);

#line default
#line hidden
                WriteAttributeValue("", 1307, "/TempImages/myclickdoctor_new_HUN.png", 1307, 37, true);
                EndWriteAttribute();
                BeginContext(1345, 505, true);
                WriteLiteral(@" width=""100""></td>
                                <td width=""120"">
                                    <p>Contact <br /><b style=""font-size: 16px;"">+36 1 336 21 04</b></p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>

            <tr>
                <td style=""    border-top: 1px solid #e9e9e9; border-bottom: 1px solid #e9e9e9;"">
                    <p>Kedves ");
                EndContext();
                BeginContext(1851, 10, false);
#line 58 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
                         Write(Model.Name);

#line default
#line hidden
                EndContext();
                BeginContext(1861, 157, true);
                WriteLiteral("!</p>\r\n                    <p>Ezt az emlékeztető e-mailt azért kapta, mert konzultációs időpontot foglalt a myclickdoctorban. A konzultációig hátralévő idő: ");
                EndContext();
                BeginContext(2019, 19, false);
#line 59 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
                                                                                                                                                 Write(Model.RemainingTime);

#line default
#line hidden
                EndContext();
                BeginContext(2038, 113, true);
                WriteLiteral("</p>\r\n\r\n                    <p>A konzultáció adatai a következőek:</p>\r\n                    <p><b>Konzulens:</b> ");
                EndContext();
                BeginContext(2152, 21, false);
#line 62 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
                                    Write(Model.Appointmentwith);

#line default
#line hidden
                EndContext();
                BeginContext(2173, 59, true);
                WriteLiteral("</p>\r\n                    <p><b>A konzultáció kezdete:</b> ");
                EndContext();
                BeginContext(2233, 15, false);
#line 63 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
                                                Write(Model.StartTime);

#line default
#line hidden
                EndContext();
                BeginContext(2248, 47, true);
                WriteLiteral("</p>\r\n                    <p><b>Időtartam:</b> ");
                EndContext();
                BeginContext(2296, 14, false);
#line 64 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
                                    Write(Model.Duration);

#line default
#line hidden
                EndContext();
                BeginContext(2310, 59, true);
                WriteLiteral(" perc</p>\r\n                    <p><b>Konzultáció típus:</b>");
                EndContext();
                BeginContext(2370, 21, false);
#line 65 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
                                           Write(Model.AppointmentType);

#line default
#line hidden
                EndContext();
                BeginContext(2391, 112, true);
                WriteLiteral("</p>\r\n\r\n                    <p>A konzultációhoz az alábbi linken tud csatlakozni:</p>\r\n                    <p><a");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 2503, "\"", 2578, 2);
                WriteAttributeValue("", 2510, "https://test.myclickdoctor.com/Count-down?AppId=", 2510, 48, true);
#line 68 "E:\StagingMCD_API\MyClickDoctorBE\Templates\Appointment_Reminder_Email.cshtml"
WriteAttributeValue("", 2558, Model.AppointmentId, 2558, 20, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(2579, 255, true);
                WriteLiteral(" style=\"text-decoration: none;\"> Kattints ide</a></p>\r\n\r\n                    <p><b>Üdvözlettel,</b></p>\r\n                    <p>Blasszauer Celia és a MedicalScan csapata</p>\r\n\r\n                </td>\r\n            </tr>\r\n\r\n        </tbody>\r\n\r\n    </table>\r\n");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(2841, 9, true);
            WriteLiteral("\r\n</html>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MyClickDoctorBE.Models.RemainderEmail> Html { get; private set; }
    }
}
#pragma warning restore 1591
