using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OG.Identity.Domain.Models;
using OrderFood.Domain.Identity.Models;
using OrderFood.Infrastructure;
using OrderFood.Infrastructure.Context;
using OrderFood.Infrastructure.Identity;
using OrderFood.Infrastructure.UnitOfWorks;
using OrderFood.Web.Binders;
using OrderFood.Web.Services;

namespace OrderFood.Web
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
        {
            services.AddWebServices(Configuration);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";
                options.LogoutPath = "/Accounts/Logout";
                options.AccessDeniedPath = "/Accounts/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                };
            });

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));

            services.AddTransient<IEmailSender, SmtpEmailSender>(opt => new SmtpEmailSender(
                Configuration["EmailSender:Host"],
                Configuration.GetValue<int>("EmailSender:Port"),
                Configuration["EmailSender:UserName"],
                Configuration["EmailSender:Password"],
                Configuration.GetValue<bool>("EmailSender:EnableSSL")
            ));

            services.AddControllersWithViews(options => options.ModelBinderProviders.Insert(0, new DecimalBinderProvider()));
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<OGUser> userManager, RoleManager<IdentityRole> roleManager, DBContext context)
        {
            if (env.IsDevelopment())
            {
                DBContextSeed.Seed(context);
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                endpoints.MapRazorPages();
            });

            IdentitySeed.Seed(Configuration, userManager, roleManager).Wait();
        }
    }
}
