using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApiContext _context;

        public BookingController(ApiContext context)
        {
            _context = context;
        }

        // Tạo mới booking
        [HttpPost]
        public JsonResult Create([FromForm] Booking booking)
        {
            booking.Status = "notSent"; // Mặc định trạng thái là notSent
            _context.Booking.Add(booking);
            _context.SaveChanges();

            return new JsonResult(Ok(booking));
        }

        // Lấy tất cả booking
        [HttpGet]
        public JsonResult Read()
        {
            var bookings = _context.Booking
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Email,
                    b.Status
                })
                .ToList();

            return new JsonResult(Ok(bookings));
        }
        
        // Lấy booking theo trạng thái
        // Lấy booking theo trạng thái
        [HttpGet("{status}")]
        public IActionResult GetByStatus(string status)
        {
            var bookings = _context.Booking
                .Where(p => p.Status == status)
                .ToList();

            if (!bookings.Any())
            {
                return NotFound($"Không có booking nào với trạng thái '{status}'.");
            }

            return Ok(bookings);
        }


        // Cập nhật thông tin booking
        [HttpPut("{id}")]
        public JsonResult Update(int id, [FromBody] Booking booking)
        {
            var existingBooking = _context.Booking.FirstOrDefault(p => p.Id == id);
            if (existingBooking == null)
            {
                return new JsonResult(NotFound($"Booking with Id {id} not found"));
            }

            existingBooking.Name = booking.Name ?? existingBooking.Name;
            existingBooking.Email = booking.Email ?? existingBooking.Email;
            existingBooking.Status = booking.Status ?? existingBooking.Status;

            _context.SaveChanges();

            return new JsonResult(Ok(existingBooking));
        }

        // Xóa booking
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            var booking = _context.Booking.FirstOrDefault(p => p.Id == id);
            if (booking == null)
            {
                return new JsonResult(NotFound($"Booking with Id {id} not found"));
            }

            _context.Booking.Remove(booking);
            _context.SaveChanges();

            return new JsonResult(Ok($"Booking with Id {id} deleted successfully"));
        }

        // Lấy chi tiết booking
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var booking = _context.Booking.FirstOrDefault(p => p.Id == id);
            if (booking == null)
            {
                return NotFound($"Booking with Id {id} not found.");
            }

            return Ok(booking);
        }
    }
}
