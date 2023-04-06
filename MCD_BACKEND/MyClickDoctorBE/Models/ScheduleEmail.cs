using Microsoft.Extensions.Logging;
using MyClickDoctorBE.Models;
using Quartz;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class ScheduleEmail : IJob
{
    private readonly ILogger<ScheduleEmail> _logger;
    public ScheduleEmail(ILogger<ScheduleEmail> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        //  AppDbCommonLogic.SendSecondEmailtoUser();
        DateTime now = DateTime.UtcNow;
        string dayname = now.DayOfWeek.ToString();
        //if(dayname== "Tuesday")
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(ApplicationGlobalVariable.WebsiteUrl);
        //        client.DefaultRequestHeaders.Clear();
        //        // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage Res = client.GetAsync("api/Email/GetDoctorToSendEmail").Result;
        //        if (Res.IsSuccessStatusCode)
        //        {
        //            var EmpResponse = Res.Content.ReadAsStringAsync().Result;
        //        }
        //        else
        //        {
        //            var EmpResponse = Res.Content.ReadAsStringAsync().Result;
        //        }
        //    }
        //}
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(ApplicationGlobalVariable.WebsiteUrl);
            client.DefaultRequestHeaders.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/Email/GetAppointmentFifteenMinEmail").Result;
            if (Res.IsSuccessStatusCode)
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
        }
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(ApplicationGlobalVariable.WebsiteUrl);
            client.DefaultRequestHeaders.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/Email/GetAppointmentOneHoursEmail").Result;
            if (Res.IsSuccessStatusCode)
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
        }
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(ApplicationGlobalVariable.WebsiteUrl);
            client.DefaultRequestHeaders.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/Email/GetAppointmenttwentyfourHoursEmail").Result;
            if (Res.IsSuccessStatusCode)
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
        }
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(ApplicationGlobalVariable.WebsiteUrl);
            client.DefaultRequestHeaders.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage Res = client.GetAsync("api/Email/SendEmailtoKOLnobodybookAppoinment").Result;
            if (Res.IsSuccessStatusCode)
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
            }
        }
        return Task.CompletedTask;
    }
}
