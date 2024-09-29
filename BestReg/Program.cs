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
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Load Firebase credentials
var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];
if (!string.IsNullOrWhiteSpace(firebaseCredentialsPath))
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialsPath),
    });
}
else
{
    throw new ArgumentException("Firebase credentials path is not configured.");
}

// Initialize Firestore
FirestoreDb firestoreDb = FirestoreDb.Create("newchilddb");



// Register FirestoreDb with DI
builder.Services.AddSingleton(firestoreDb);

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

// Register the EmailService with the dependency injection container
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddSingleton<RoleSyncService>(); // Register RoleSyncService

// Add Firebase Authentication services
builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddSingleton<IFirebaseTokenValidator, FirebaseTokenValidator>();
builder.Services.AddSingleton<FirestoreAttendanceService>();

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
