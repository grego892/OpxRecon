using OpxRecon.Components;
using OpxRecon.Components.Account;
using OpxRecon.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog.Settings.Configuration;
using Serilog;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Services
ConfigureServices(builder);

// Database
ConfigureDatabase(builder);

// Authentication
ConfigureAuthentication(builder);

// Identity
ConfigureIdentity(builder);

// Logger
ConfigureLogger(builder);

// Host
ConfigureHost(builder);

var app = builder.Build();

// Database Migration
MigrateDatabase(app);

// Seed Data
//await SeedDataAsync(app);

// Middleware
ConfigureMiddleware(app);

Log.Information("--- Program.cs - Starting app.Run()");
app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    // Add MudBlazor services
    builder.Services.AddMudServices();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
        .AddIdentityCookies();

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
}

void ConfigureDatabase(WebApplicationBuilder builder)
{
    var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "OpX", "OpxRecon", "Database", "OpxRecon.db");

    var databaseDirectory = Path.GetDirectoryName(databasePath);
    if (string.IsNullOrEmpty(databaseDirectory))
    {
        Log.Error("--- Program.cs - Database directory path is null or empty.");
        throw new ArgumentNullException(nameof(databaseDirectory), "Database directory path cannot be null or empty.");
    }
    Directory.CreateDirectory(databaseDirectory);
    var connectionString = $"Data Source={databasePath}";
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    // Add authentication configuration here if needed
}

void ConfigureIdentity(WebApplicationBuilder builder)
{
    // Add identity configuration here if needed
}

void ConfigureLogger(WebApplicationBuilder builder)
{
    string _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "OpX", "OpxRecon", "Logging", "OpxRecon.log");
    var configuration = builder.Configuration;
    var options = new ConfigurationReaderOptions(typeof(Serilog.LoggerConfiguration).Assembly);

    var loggingDirectory = Path.GetDirectoryName(_logPath);
    if (loggingDirectory != null)
    {
        Directory.CreateDirectory(loggingDirectory);
    }
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration, options)
        .WriteTo.File(Path.Combine(_logPath), rollingInterval: RollingInterval.Day)
        .WriteTo.Console()
        .CreateLogger();
}

void MigrateDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            dbContext.Database.Migrate();
            Log.Information("--- Program - Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "++++++ Program.cs - An error occurred while migrating the database.");
        }
    }
}

void ConfigureMiddleware(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    // Add additional endpoints required by the Identity /Account Razor components.
    app.MapAdditionalIdentityEndpoints();
}

void ConfigureHost(WebApplicationBuilder builder)
{
    // Run as a Windows Service
    builder.Host.UseWindowsService();
}
