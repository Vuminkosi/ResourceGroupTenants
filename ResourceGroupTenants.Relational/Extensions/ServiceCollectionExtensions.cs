using Dna;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using ResourceGroupTenants.Relational.Data;
using ResourceGroupTenants.Relational.Data.Identity;
using ResourceGroupTenants.Relational.Emails.FluentServices;
using ResourceGroupTenants.Relational.Services;
using ResourceGroupTenants.Relational.Services.Masters;
using ResourceGroupTenants.Relational.Services.Resource.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceMigrateTenantDatabases(this IServiceCollection services, IConfiguration config)
        {

            // Add main resource manager db context
            services.AddDbContext<ResourceGroupDBContext>(options =>
               options.UseSqlServer(
                   config.GetConnectionString("ResourceConnection")));

            // apply migration for the default main resource manager
            services.ApplyResourceContextMigrations();

            // the resource manager service
            services.AddTransient<ITenantService, TenantService>();
            
            // add tenants context
            services.AddDbContext<TenantDBContext>(m => m.UseSqlServer(e => e.MigrationsAssembly(typeof(TenantDBContext).Assembly.FullName)));
            
            // get scoped tenants context
            services.ApplyTenantContextMigrations();
            // add JWT Authentication Service
            services.AddJwtServices();
            // add login policies and roles
            services.AddIdentityRolesAndPoliciesService();

            // add emailing service
            services.AddTransient<IFluentMailService, FluentMailService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IProductsService, ProductsService>();

            return services;
        }

        private static IServiceCollection ApplyTenantContextMigrations(this IServiceCollection services)
        {
            var scope = services.BuildServiceProvider().CreateScope();
            var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
            var defaultConnectionString = tenantService.GetConnectionString();
            var resources = tenantService.GetAllAsync().Result;
            if (resources is not null && resources.Any())
                foreach (var resource in from rs in resources where rs.ResourceCode is not null select rs)
                {
                    if (!string.IsNullOrWhiteSpace(resource.ResourceCode))
                        tenantService.SetTenant(resource.ResourceCode);
                    var tenant = tenantService.GetTenant();
                    if (tenant is not null)
                    {
                        string connectionString;
                        if (string.IsNullOrEmpty(tenant.ConnectionString))
                        {
                            connectionString = defaultConnectionString;
                        }
                        else
                        {
                            connectionString = tenant.ConnectionString;
                        }
                        var dbContext = scope.ServiceProvider.GetRequiredService<TenantDBContext>();
                        dbContext.Database.SetConnectionString(connectionString);
                        if (dbContext.Database.GetMigrations().Count() > 0)
                        {
                            dbContext.Database.Migrate();
                        }
                    }

                }

            return services;
        }

        public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection(sectionName);
            var options = new T();
            section.Bind(options);
            return options;
        }


        public static IServiceCollection ApplyResourceContextMigrations(this IServiceCollection services)
        {
            using (var service = services.BuildServiceProvider().CreateScope())
            {
                using (var context = service.ServiceProvider.GetService<ResourceGroupDBContext>())
                {
                    if (context is not null)
                        context.Database.Migrate();
                }
            }
            return services;
        }

        private static IServiceCollection AddJwtServices(this IServiceCollection services)
        {

            services.AddAuthentication()
                          .AddJwtBearer(options =>
                          {
                              options.TokenValidationParameters = new TokenValidationParameters
                              {
                                  ValidateIssuer = true,
                                  ValidateAudience = true,
                                  ValidateLifetime = true,
                                  ValidateIssuerSigningKey = true,
                                  ValidIssuer = Framework.Construction.Configuration["Jwt:JwtIssuer"],
                                  ValidAudience = Framework.Construction.Configuration["Jwt:JwtAudience"],
                                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Framework.Construction.Configuration["Jwt:SecretKey"])),
                              };
                          });

            return services;
        }
        public static IServiceCollection AddIdentityRolesAndPoliciesService(this IServiceCollection services)
        {

            // Add Identity adds cookie based authentication
            // Adds scoped classes for things like userManager, SiginManager, PasswordHashers etc...
            // Note : Automatically adds the validated user from a cookie to the httpCpntext.User
            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                // Adds the userStore and Role store from the context
                // That are consumed by the UserManager and Store manager
                .AddEntityFrameworkStores<TenantDBContext>()
                // Adds a provider that generates unique keys and hashes for things like
                // forgot password links, phone number verification codes etc...
                .AddDefaultTokenProviders();


            // Change password policy
            services.Configure<IdentityOptions>(options =>
            {
                // Make really weak passwords possible
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // Make sure its a unique email
                options.User.RequireUniqueEmail = true;
            });

            // Change login URL and alter application cookie info
            services.ConfigureApplicationCookie(options =>
            {

                // Redirect to /login
                options.LoginPath = "/login";

                // Change cookie timeout to expire in 8 hours
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
           .AddCookie();


            return services;
        }
    }
}
