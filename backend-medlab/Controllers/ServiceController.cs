using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;
using System.Linq;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ApiContext _context;

        public ServiceController(ApiContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Create
        [HttpPost]
        public IActionResult Create([FromForm] Service service)
        {
            if (service == null || string.IsNullOrEmpty(service.ServiceName) || string.IsNullOrEmpty(service.ServiceGroup))
            {
                return BadRequest("Invalid service data. All fdsdadsdields must be provided for each service.");
            }

            _context.Service.Add(service);
            _context.SaveChanges();

            return Ok();
        }


        // Read (Get all services)
        [HttpGet]
        public IActionResult Read(int page = 1, int pageSize = 10, string? ServiceName = null, string? ServiceGroup = null)
        {
            var query = _context.Service.AsQueryable();

            if (!string.IsNullOrEmpty(ServiceName))
            {
                query = query.Where(b => b.ServiceName.Contains(ServiceName));  // Tìm kiếm theo ServiceName
            }

            if (!string.IsNullOrEmpty(ServiceGroup))
            {
                query = query.Where(b => b.ServiceGroup.Contains(ServiceGroup));  // Tìm kiếm theo ServiceGroup
            }

            var totalServices = query.Count();

            if (pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var blogs = query.ToList();

            var response = new
            {
                Services = blogs,
                TotalServices = totalServices,
                TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalServices / pageSize) : 1,
                CurrentPage = pageSize > 0 ? page : 1
            };

            return Ok(response);
        }

        // Update
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] Service service)
        {
            var existingService = _context.Service.FirstOrDefault(p => p.Id == id);
            if (existingService == null)
            {
                return NotFound($"Service with Id {id} not found.");
            }

            // Only update non-null values from the provided service object
            if (!string.IsNullOrEmpty(service.ServiceName)) existingService.ServiceName = service.ServiceName;
            if (!string.IsNullOrEmpty(service.ServiceGroup)) existingService.ServiceGroup = service.ServiceGroup;
            if (!string.IsNullOrEmpty(service.ServiceUnit)) existingService.ServiceUnit = service.ServiceUnit;

            _context.SaveChanges();

            return Ok(existingService);
        }

        // Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var service = _context.Service.FirstOrDefault(p => p.Id == id);
            if (service == null)
            {
                return NotFound($"Service with Id {id} not found.");
            }

            _context.Service.Remove(service);
            _context.SaveChanges();

            return Ok($"Service with Id {id} deleted successfully.");
        }

        // Get Detail by id
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var service = _context.Service.FirstOrDefault(p => p.Id == id);
            if (service == null)
            {
                return NotFound($"Service with Id {id} not found.");
            }

            return Ok(service);
        }
    }
}
