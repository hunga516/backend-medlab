using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailsController : Controller
    {
        private readonly ApiContext _context;
        private readonly IEmailService emailService;
        private readonly HttpClient httpClient;

        public EmailsController(ApiContext context, IEmailService emailService)
        {
            this._context = context;
            this.emailService = emailService;
            this.httpClient = new HttpClient();
        }

        [HttpPost("{Id}")]
        public async Task<IActionResult> SendBookingConfirmation(int Id)
        {
            // Lấy thông tin booking
            var booking = _context.Booking.FirstOrDefault(p => p.Id == Id);
            if (booking == null)
            {
                return NotFound($"Booking with Id {Id} not found.");
            }

            // Tạo nội dung email
            var subject = "Xác nhận thông tin đặt lịch - Medlab";
            var body = $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; background-color: #1e88e5; padding: 10px 0; border-top-left-radius: 8px; border-top-right-radius: 8px;'>
            <img src='cid:companylogo' alt='Medlab Logo' style='max-width: 120px; height: auto;' />
        </div>
        <div style='padding: 20px;'>
            <h2 style='color: #1e88e5; text-align: center;'>Chào {booking.Name},</h2>
            <p style='font-size: 16px;'>Cảm ơn bạn đã đặt lịch tại <strong>Medlab</strong>. Chúng tôi đã nhận được thông tin của bạn.</p>
            <ul style='list-style: none; padding: 0; margin: 0;'>
                <li style='margin-bottom: 10px;'><strong>Ngày đặt lịch:</strong> {booking.BookingDate?.ToString("dd/MM/yyyy")}</li>
                <li style='margin-bottom: 10px;'><strong>Email liên hệ:</strong> {booking.Email}</li>
            </ul>
            <p style='font-size: 16px;'>Chúng tôi sẽ liên hệ lại với bạn trong thời gian sớm nhất.</p>
        </div>
        <div style='text-align: center; background-color: #f5f5f5; padding: 10px; border-bottom-left-radius: 8px; border-bottom-right-radius: 8px;'>
            <p style='color: #1e88e5; margin: 0; font-size: 16px;'><strong>Trân trọng,<br>Medlab Team</strong></p>
        </div>
    </div>";

            // Gửi email
            try
            {
                await emailService.SendEmail(booking.Email, subject, body);

                // Cập nhật trạng thái là Sent
                booking.Status = "Sent";
                _context.SaveChanges();

                return Ok("Email đã được gửi thành công và trạng thái cập nhật.");
            }
            catch (Exception ex)
            {
                booking.Status = "notSent";
                _context.SaveChanges();

                return StatusCode(500, $"Lỗi khi gửi email: {ex.Message}");
            }
        }
    }
}
