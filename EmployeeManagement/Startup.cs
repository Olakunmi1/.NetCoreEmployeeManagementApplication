using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration Configuration)
        {
            configuration = Configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            //registering the Custom Interface
            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();

            //Registering the Custom Handler meant for d Policy 

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();

            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            //below we are applying Authorization Globally instead of applying it on each controller
            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            //regsitering the connection string / Dbcontext
            var connectionstring = configuration["connectionStrings:employeeDbConnectionString"];
            services.AddDbContextPool<AppDbContext>(c => c.UseSqlServer(connectionstring));

            //registering the identity service passing in the built in user and role.. 
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    //Registering a servcie for Default token provider
                    .AddDefaultTokenProviders();

            //resseting the default LifeSpan of all types of Token ..
            services.Configure<DataProtectionTokenProviderOptions>(options =>
                                options.TokenLifespan = TimeSpan.FromHours(5));

            //How to configure PAssword option
            //configuring paswordOptions for our own application
            services.Configure<IdentityOptions>(option => {
                option.Password.RequiredLength = 10;
                option.Password.RequiredUniqueChars = 3;

                //requiring that our email is confirmed before the user is logged in...
                option.SignIn.RequireConfirmedEmail = true;

                //the configurtion below Lockout an acct after 5 failed attempts
                //and number of mins needed before the acct is opened for a retry
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });

            // Configuring/reg service for claims based Authorization
            //"Add Policy" helps to name the Policy, while "polciy" parameter helps
            //to assign the claims to the Policy

            services.AddAuthorization(options =>
            {
                //Uisng the built in RequireClaim authorization
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role", "true"));

                //bringing in our Custom requirements
                options.AddPolicy("EditRolePolicy",
                    policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            });

            //the line below enables me to use mvc, by making EnableEndpointRouting =False
            services.AddControllers(options => options.EnableEndpointRouting = false);
           
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
                //this middleware helps to handle Global Exception(exceptions in general)
                app.UseExceptionHandler("/Error/");

                //this middleware handles 404 error
                //this middlewares redirect the user to a controller and a specific error
                //code that goes into the palceholder"{}" below
                app.UseStatusCodePagesWithReExecute("/Error/{0}"); 
            }

            //enables authentication
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=index}/{id?}");
            });
            app.UseRouting();
          
            //a middleware for serving up static files eg hmtl, images 
            app.UseStaticFiles();

            // app.UseMvcWithDefaultRoute();  
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => 
                {
                    await context.Response.WriteAsync("Hello World!"); 
                    await context.Response.WriteAsync(configuration ["MyKey"]); 
                });
            });
        }
    }
}
