using Azure.Core;
using BestReg.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using FirebaseAdmin.Auth;

namespace BestReg.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailService;
        //private readonly SyncService _syncService; // Inject SyncService

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IEmailService emailService)
            //SyncService syncService) // Add SyncService to constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailService = emailService;
            //_syncService = syncService; // Assign SyncService
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<IdentityRole> Roles { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "Max 50 characters allowed")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "Max 50 characters allowed")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(13, ErrorMessage = "IDNumber cannot be longer than 13 characters.")]
            [RegularExpression(@"^\d{13}$", ErrorMessage = "IDNumber must be exactly 13 digits.")]
            public string IDNumber { get; set; }

            [Required]
            public string SelectedRole { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Roles = await _roleManager.Roles.ToListAsync();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        private async Task CreateUserInFirebase(string email, string password)
        {
            var userRecordArgs = new UserRecordArgs()
            {
                Email = email,
                EmailVerified = false,
                Password = password,
                Disabled = false,
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
            Console.WriteLine($"Successfully created new user: {userRecord.Uid}");
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            Roles = _roleManager.Roles.ToList();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    IDNumber = Input.IDNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Create user in Firebase
                    await CreateUserInFirebase(Input.Email, Input.Password);

                    // Assign the selected role to the user
                    if (!string.IsNullOrEmpty(Input.SelectedRole))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, Input.SelectedRole);
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, "Failed to assign role.");
                            return Page();
                        }

                        if (Input.SelectedRole == "Student")
                        {
                            var qrCodeBytes = _emailService.GenerateQrCode(user.IDNumber);
                            user.QrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
                            await _userManager.UpdateAsync(user);
                        }
                    }

                    _logger.LogInformation("User created a new account with password.");

                    // Save user to Firestore
                    await SaveUserToFirestore(user);

                    // Automatically sign in the user after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    //// Trigger data sync from SQL to Firebase
                    //await _syncService.SyncSqlToFirebaseAsync();

                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private async Task SaveUserToFirestore(ApplicationUser user)
        {
            var firestoreDb = FirestoreDb.Create("newchilddb");
            var usersCollection = firestoreDb.Collection("users");
            var userDocument = usersCollection.Document(user.Id);

            var userData = new
            {
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IDNumber,
                user.QrCodeBase64
            };

            await userDocument.SetAsync(userData);
        }
    }
}
