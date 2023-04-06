using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BarionClientLibrary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using MyClickDoctorBE.Templates;
using Quartz;

namespace MyClickDoctorBE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddDbContext<MyClickDoctorContext>(item => item.UseSqlServer(Configuration.GetConnectionString("myconn")));
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                      .AllowAnyMethod()
                      .SetIsOriginAllowed((host) => true)
                      .AllowCredentials();
            }));
            services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
            {
                builder.AllowAnyHeader()
                      .AllowAnyMethod()
                      .SetIsOriginAllowed((host) => true)
                      .AllowCredentials();
            }));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                //cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<MyClickDoctorV4>(option =>
            option.UseSqlServer("myconn"));
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//You can set Time  
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            string url = Configuration.GetSection("Logging:BarionPayments:BaseUrl").Value;
            string POSKe = Configuration.GetSection("Logging:BarionPayments:POSKey").Value;
            string Paye = Configuration.GetSection("Logging:BarionPayments:Payee").Value;
            var barionSettings = new BarionSettings
            {
                BaseUrl = new Uri(url),
                POSKey = Guid.Parse(POSKe),
                Payee = Paye,
            };
            services.AddSingleton(barionSettings);
            services.AddTransient<BarionClient>();
            services.AddHttpClient<BarionClient>();
            services.AddSignalR();
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();

                // Create a "key" for the job
                var jobKey = new JobKey("EmailJob");

                // Register the job with the DI container
                q.AddJob<ScheduleEmail>(opts => opts.WithIdentity(jobKey));

                // Create a trigger for the job
                q.AddTrigger(opts => opts
                    .ForJob(jobKey) // link to the HelloWorldJob
                    .WithIdentity("EmailJob-trigger") // give the trigger a unique name
                                                      //    .WithCronSchedule("0/5 * * * * ?"));
             .WithCronSchedule("0/59 * * * * ?"));
                //  .WithCronSchedule("0 0/1 * * * ?")); //for every 50 minutes
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddTransient<IEmailService, EmailService>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<UserAuthentication>();
            loggerFactory.AddFile("Logs/myapp-{Date}.txt");
            //app.UseMiddleware<ChangeCulture>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseMvc();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseSignalR(options =>
            {
                options.MapHub<MessageHub>("/NotifyCall");
            });
            //app.UseSpa(spa =>
            //{
            //    // To learn more about options for serving an Angular SPA from ASP.NET Core,  
            //    // see https://go.microsoft.com/fwlink/?linkid=864501  

            //    spa.Options.SourcePath = "V2";

            //    if (env.IsDevelopment())
            //    {
            //        //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");  
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }
            //});
            //
           // app.UseCors();
            
        }
    }
}
