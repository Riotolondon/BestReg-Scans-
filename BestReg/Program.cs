using BestReg.Data;
using BestReg.Services;
using Firebase.Auth.Providers;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Load Firebase credentials from JSON file
var firebaseCredentialsPath = builder.Configuration["Firebase:Credentials"];
if (string.IsNullOrEmpty(firebaseCredentialsPath))
{
    throw new InvalidOperationException("Firebase credentials path not found in configuration.");
}

var credential = GoogleCredential.FromFile(firebaseCredentialsPath);

// Initialize Firebase
FirebaseApp.Create(new AppOptions
{
    Credential = credential,
});

// Setup database connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure services for DbContext and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register services
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddSingleton<RoleSyncService>();

// Add Firebase Authentication services
builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddSingleton<IFirebaseTokenValidator, FirebaseTokenValidator>();

// Register FirebaseAuthClient
builder.Services.AddSingleton<FirebaseAuthClient>(sp =>
{
    return new FirebaseAuthClient(new FirebaseAuthConfig
    {
        ApiKey = "AIzaSyCyJg1GUGose5LfuwjP9CuWYixhczkUZmw",
        AuthDomain = "newchilddb.firebaseapp.com",
        Providers = new FirebaseAuthProvider[]
        {
            new EmailProvider(),
            new GoogleProvider().AddScopes("email")
        }
    });
});

// Configure JWT Bearer for Firebase Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme; // Use Identity as default
})
.AddJwtBearer(options =>
{
    options.Authority = "https://securetoken.google.com/newchilddb";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://securetoken.google.com/newchilddb",
        ValidateAudience = true,
        ValidAudience = "newchilddb",
        ValidateLifetime = true
    };
});

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
