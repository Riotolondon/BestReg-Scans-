using BestReg.Data;
using BestReg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

[Authorize]
public class QrCodeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QrCodeController> _logger;

    public QrCodeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<QrCodeController> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    // GET: /QrCode/ShowQrCode
    public async Task<IActionResult> ShowQrCode()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || string.IsNullOrEmpty(user.QrCodeBase64))
        {
            _logger.LogWarning("User not found or QR code not available.");
            return NotFound("User not found or QR code not available.");
        }

        ViewBag.QrCodeBase64 = user.QrCodeBase64;
        return View();
    }

    // GET: /QrCode/DownloadQrCode
    public async Task<IActionResult> DownloadQrCode()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || string.IsNullOrEmpty(user.QrCodeBase64))
        {
            _logger.LogWarning("User not found or QR code not available.");
            return NotFound("User not found or QR code not available.");
        }

        var qrCodeBytes = Convert.FromBase64String(user.QrCodeBase64);
        return File(qrCodeBytes, "image/png", "QRCode.png");
    }


    // Action to display the QR code scanning page
    public IActionResult ScanQRCode(string scanType, string role)
    {
        if (string.IsNullOrWhiteSpace(scanType) || string.IsNullOrWhiteSpace(role))
        {
            _logger.LogWarning("Invalid scanType or role.");
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ScanType = scanType;
        ViewBag.Role = role;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ProcessQRCode(string qrCodeData, string scanType, string role)
    {
        if (string.IsNullOrWhiteSpace(qrCodeData) || string.IsNullOrWhiteSpace(scanType) || string.IsNullOrWhiteSpace(role))
        {
            _logger.LogWarning("QR Code data, scanType, or role is empty.");
            return RedirectToAction("Index", "Home");
        }

        // Handle SchoolAuthority role
        if (role == "SchoolAuthority")
        {
            if (scanType == "SchoolCheckIn")
            {
                return RedirectToAction("SchoolCheckIn", "SchoolAuthority", new { qrCodeData });
            }
            else if (scanType == "SchoolCheckOut")
            {
                return RedirectToAction("SchoolCheckOut", "SchoolAuthority", new { qrCodeData });
            }
            else
            {
                _logger.LogWarning($"Invalid scanType for SchoolAuthority: {scanType}");
                return RedirectToAction("Index", "Home", new { error = "Invalid scanType for SchoolAuthority." });
            }
        }
        // Handle BusDriver role
        else if (role == "BusDriver")
        {
            if (scanType == "BusCheckInHome")
            {
                return RedirectToAction("BusCheckInHome", "BusDriver", new { qrCodeData });
            }
            else if (scanType == "BusCheckOutSchool")
            {
                return RedirectToAction("BusCheckOutSchool", "BusDriver", new { qrCodeData });
            }
            else if (scanType == "BusCheckInSchool")
            {
                return RedirectToAction("BusCheckInSchool", "BusDriver", new { qrCodeData });
            }
            else if (scanType == "BusCheckOutHome")
            {
                return RedirectToAction("BusCheckOutHome", "BusDriver", new { qrCodeData });
            }
            else
            {
                _logger.LogWarning($"Invalid scanType for BusDriver: {scanType}");
                return RedirectToAction("Index", "Home", new { error = "Invalid scanType for BusDriver." });
            }
        }

        // Log invalid scanType or role
        _logger.LogWarning($"Invalid scanType or role: {scanType}, {role}. Redirecting to the Index page.");
        return RedirectToAction("Index", "Home");
    }

}
