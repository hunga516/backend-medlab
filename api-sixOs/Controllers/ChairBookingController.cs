using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;
using System.Linq;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChairBookingController : ControllerBase
    {
        private readonly ApiContext _context;

        public ChairBookingController(ApiContext context)
        {
            _context = context;
        }

        // Tạo lịch đặt mới
        [HttpPost]
        public IActionResult Create([FromBody] ChairBookings booking)
        {
            // Kiểm tra các lịch đặt xung đột với ghế đã chọn
            var conflictingBookings = _context.ChairBookings
                .Where(b => b.ChairNumber == booking.ChairNumber
                            && b.DateBooking == booking.DateBooking
                            && !(booking.EndTime <= b.StartTime || booking.StartTime >= b.EndTime))
                .ToList();

            if (conflictingBookings.Any())
            {
                var conflict = conflictingBookings.First();
                return Conflict($"Ghế {booking.ChairNumber} đã có lịch từ {conflict.StartTime} đến {conflict.EndTime}. Vui lòng chọn giờ khác.");
            }

            // Thêm lịch đặt vào cơ sở dữ liệu
            _context.ChairBookings.Add(booking);
            _context.SaveChanges();
            return Ok("Đặt lịch thành công.");
        }

        // Lấy lịch đặt theo ngày
        [HttpGet]
        public IActionResult Read([FromQuery] string date)
        {
            // Kiểm tra xem ngày có hợp lệ không
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                return BadRequest("Ngày không hợp lệ.");
            }

            // Lấy danh sách lịch đặt cho ngày đã chọn
            var chairbookings = _context.ChairBookings
                .Where(b => b.DateBooking == parsedDate)
                .ToList();

            // Kiểm tra xem có lịch đặt nào không
            if (!chairbookings.Any())
            {
                return NotFound($"Không có lịch đặt nào cho ngày {parsedDate.ToString("dd/MM/yyyy")}."); // Trả về thông báo nếu không có lịch
            }

            // Trả về danh sách lịch đặt cho ngày yêu cầu
            return Ok(chairbookings); // Dữ liệu sẽ được trả về trong phần thân của response
        }
    }
}
