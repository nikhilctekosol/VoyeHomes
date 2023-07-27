using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebEssentials.AspNetCore.Pwa;

namespace VTravel.CustomerWeb
{
    public class Startup
    {
        public static string conStr = string.Empty;
        public static string contentRoot = string.Empty;
        public Startup(IConfiguration configuration)
        {
            contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            conStr = configuration.GetConnectionString("DefaultConnectionString");
        }
        //fgfgfgfgghghr
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddMvc();
            //services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddProgressiveWebApp();
            services.AddControllersWithViews();
            services.AddHostedService<RedisDataWorkerService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "property",
                pattern: "{id}",
                defaults: new { controller = "Property", action = "Details" });

                endpoints.MapControllerRoute(name: "page_PartnerWithUs",
                pattern: "page/partner-with-us",
                defaults: new { controller = "Page", action = "PartnerWithUs" });

                endpoints.MapControllerRoute(name: "page_GiftCard",
                pattern: "page/gift-card",
                defaults: new { controller = "Page", action = "GiftCard" });

                endpoints.MapControllerRoute(name: "page",
                pattern: "page/{id}",
                defaults: new { controller = "Page", action = "Details" });

               

                endpoints.MapControllerRoute(name: "reservation_ThankYou",
                pattern: "reservation/thankyou",
                defaults: new { controller = "Reservation", action = "ThankYou" });



                endpoints.MapControllerRoute(name: "reservation",
                pattern: "reservation/{id}",
                defaults: new { controller = "Reservation", action = "Index" });

                

                endpoints.MapControllerRoute(name: "error",
                pattern: "home/error",
                defaults: new { controller = "Home", action = "Error" });

                endpoints.MapControllerRoute(name: "propertyList",
                pattern: "{id1}/{id2}",
                defaults: new { controller = "Property", action = "List" });

               

               

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

                endpoints.MapFallback(context =>
                {
                    context.Response.Redirect("/Home/Error");
                    return Task.CompletedTask;
                });
            });

            //app.UseMvc(routes =>
            //{

            //    routes.MapRoute(
            //        name: "property",
            //        template: "{id}",
            //        defaults: new { controller = "Property", action = "Details" }
            //    );

            //    routes.MapRoute(
            //        name: "propertyList",
            //        template: "{id1}/{id2}",
            //        defaults: new { controller = "Property", action = "List" }
            //    );
            //    routes.MapRoute(
            //        name: "page",
            //        template: "page/{id}",
            //        defaults: new { controller = "Page", action = "Details" }
            //    );
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action}/{id?}",
            //        defaults: new { controller = "Home", action = "Index" }
            //    );


            //});

            app.UseResponseCaching();
        }
    }
}
