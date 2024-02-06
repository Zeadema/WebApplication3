using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));
        builder.Services.AddDbContext<AuthDbContext>();
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
        }).AddEntityFrameworkStores<AuthDbContext>();
        builder.Services.AddDataProtection();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddDistributedMemoryCache(); // Save session in memory
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10); 
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("keys"))
            .ProtectKeysWithDpapi();

        builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", options =>
        {
            options.Cookie.Name = "Cookie";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MustBelongToHRDepartment",
            policy => policy.RequireClaim("Department", "HR"));
        });

        builder.Services.ConfigureApplicationCookie(Config =>
        {
            Config.LoginPath = "/Login";
        });

        var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler(exceptionHandlerApp =>
			{
				exceptionHandlerApp.Run(async context =>
				{
					var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

					// Check for 404 error
					if (context.Response.StatusCode == StatusCodes.Status404NotFound)
					{
						context.Response.ContentType = "text/plain";
						await context.Response.WriteAsync("404 - Not Found");
					}
					else
					{
						// Handle other errors
						context.Response.StatusCode = StatusCodes.Status500InternalServerError;
						context.Response.ContentType = "text/plain";
						await context.Response.WriteAsync("An exception was thrown.");

						if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
						{
							await context.Response.WriteAsync(" The file was not found.");
						}

						if (exceptionHandlerPathFeature?.Path == "/")
						{
							await context.Response.WriteAsync(" Page: Home.");
						}
					}
				});
			});

			app.UseHsts();
		}


		//app.UseStatusCodePages();
		app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.UseMiddleware<SessionExpirationMiddleware>();
        app.MapRazorPages();

        app.Run();
    }
}