using Calendar_Web_App.Data;
using Calendar_Web_App.Interfaces;
using Calendar_Web_App.Middleware;
using Calendar_Web_App.Models;
using Calendar_Web_App.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;



var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((context, config) =>
{
    var env = context.HostingEnvironment;

    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
});




Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();




builder.Services.Configure<StaticFileOptions>(options =>
{
    options.DefaultContentType = "application/javascript";
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    
});



builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 5;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
}).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.MaximumReceiveMessageSize = 20000000000;
});


builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "The field is Required");
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; 
            });


builder.Services.AddControllersWithViews();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000);
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();


app.UseMiddleware<ControllerMiddleware>();

app.UseAntiforgery();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
