using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

public interface IEmailService
{
    Task<bool> SendConfirmationEmailAsync(string email, string callbackUrl);
    Task<bool> SendQrCodeEmailAsync(string email, string qrCodeBase64);
    byte[] GenerateQrCode(string text);
    byte[] GenerateBarcode(string text);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<bool> SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            message.From = new MailAddress("dutengagement@outlook.com");
            message.To.Add(email);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            smtpClient.Port = 587;
            smtpClient.Host = "smtp.outlook.com";
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("dutengagement@outlook.com", "Admin@Dut01");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the email: {Message}", ex.Message);
            return false;
        }
    }

    public async Task<bool> SendConfirmationEmailAsync(string email, string callbackUrl)
    {
        string subject = "Confirm your email";
        string body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

        return await SendEmailAsync(email, subject, body);
    }

    public async Task<bool> SendQrCodeEmailAsync(string email, string qrCodeBase64)
    {
        string subject = "Your QR Code";
        string body = $"<p>Your QR code is below:</p><img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' />";

        return await SendEmailAsync(email, subject, body);
    }

    public byte[] GenerateQrCode(string text)
    {
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 500,
                Width = 500,
                Margin = 1
            }
        };

        var pixelData = qrCodeWriter.Write(text);
        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                             ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                                                            pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

    public byte[] GenerateBarcode(string text)
    {
        var barcodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 150,
                Width = 300,
                Margin = 10
            }
        };

        var pixelData = barcodeWriter.Write(text);
        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                             ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                                                            pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
