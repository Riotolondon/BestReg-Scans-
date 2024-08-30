using BestReg.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BestReg.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        // Add the StatusMessage property to display messages to the user
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Error confirming email for user with ID '{UserId}':", userId);
                StatusMessage = "Error confirming your email.";
                return RedirectToPage("/Index");
            }

            _logger.LogInformation("Email confirmed for user with ID '{UserId}'.", userId);
            StatusMessage = "Thank you for confirming your email.";

            // Check if the user has the "Student" role
            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                // Generate and send the QR code if the user is a Student
                var qrCodeImage = _emailService.GenerateQrCode(user.Email);
                var qrCodeBase64 = Convert.ToBase64String(qrCodeImage);
                await _emailService.SendQrCodeEmailAsync(user.Email, qrCodeBase64);

                _logger.LogInformation("QR code sent to student with ID '{UserId}'.", userId);
                StatusMessage += " A QR code has been sent to your email.";
            }
            else
            {
                _logger.LogInformation("User with ID '{UserId}' is not a student. QR code not sent.", userId);
            }

            return Page();
        }
    }
}
