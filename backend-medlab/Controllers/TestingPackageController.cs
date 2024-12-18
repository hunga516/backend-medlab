using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestingPackageController : ControllerBase
    {
        private readonly ApiContext _context;

        public TestingPackageController(ApiContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] TestingPackage testingPackage, [FromQuery] bool isDraft = false)
        {
            if (testingPackage == null || string.IsNullOrEmpty(testingPackage.Title) || string.IsNullOrEmpty(testingPackage.Content))
            {
                return BadRequest("Invalid testing package data.");
            }

            testingPackage.Status = isDraft ? "Bản nháp" : "";

            if (testingPackage.ImageFile != null)
            {
                string directoryPath = Path.Combine("wwwroot", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(testingPackage.ImageFile.FileName)
                                  + "_" + DateTime.Now.Ticks + Path.GetExtension(testingPackage.ImageFile.FileName);
                string filePath = Path.Combine(directoryPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await testingPackage.ImageFile.CopyToAsync(fileStream);
                }

                testingPackage.Img = $"/images/{fileName}";
            }

            _context.TestingPackage.Add(testingPackage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = testingPackage.Id }, testingPackage);
        }
        
        [HttpGet]
        public IActionResult Read(int page = 1, int pageSize = 10, string? title = null, string? category = null, string? status = null)
        {
            var query = _context.TestingPackage.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));  
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category.Contains(category)); 
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "draft")
                {
                    query = query.Where(b => b.Status == "Bản nháp");
                }
                else if (status == "published")
                {
                    query = query.Where(b => b.Status == "");
                }
            }
            
            query = query.OrderByDescending(b => b.Id);

            var totalTestingPackages = query.Count();

            if (pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var testingPackages = query.ToList();

            var response = new
            {
                TestingPackage = testingPackages,
                TotalTestingPackages = totalTestingPackages,
                TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalTestingPackages / pageSize) : 1,
                CurrentPage = pageSize > 0 ? page : 1
            };

            return Ok(response);
        }


        //Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] TestingPackage testingPackage, [FromQuery] bool isDraft = false)
        {
            var existingTestingPackage = await _context.Blogs.FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingTestingPackage == null)
            {
                return NotFound($"Blog with Id {id} not found.");
            }

            existingTestingPackage.Title = !string.IsNullOrEmpty(testingPackage.Title) ? testingPackage.Title : existingTestingPackage.Title;
            existingTestingPackage.Content = !string.IsNullOrEmpty(testingPackage.Content) ? testingPackage.Content : existingTestingPackage.Content;

            if (testingPackage.ImageFile != null)
            {
                string directoryPath = Path.Combine("wwwroot", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!string.IsNullOrEmpty(existingTestingPackage.Img))
                {
                    var oldImagePath = Path.Combine("wwwroot", existingTestingPackage.Img.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Path.GetFileNameWithoutExtension(testingPackage.ImageFile.FileName)
                                  + "_" + DateTime.Now.Ticks + Path.GetExtension(testingPackage.ImageFile.FileName);
                string filePath = Path.Combine(directoryPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await testingPackage.ImageFile.CopyToAsync(fileStream);
                }

                existingTestingPackage.Img = $"/images/{fileName}";
            }

            existingTestingPackage.Category = !string.IsNullOrEmpty(testingPackage.Category) ? testingPackage.Category : existingTestingPackage.Category;
            existingTestingPackage.CreatedAt = testingPackage.CreatedAt != default ? testingPackage.CreatedAt : existingTestingPackage.CreatedAt;

            if (isDraft)
            {
                existingTestingPackage.Status = "Bản nháp"; 
            }
            else
            {
                existingTestingPackage.Status = !string.IsNullOrEmpty(testingPackage.Status) ? testingPackage.Status : "";
            }

            await _context.SaveChangesAsync();

            return Ok(existingTestingPackage);
        }




        // Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var testingPackage = _context.TestingPackage.FirstOrDefault(p => p.Id == id);
            if (testingPackage == null)
            {
                return NotFound($"Testing Package with Id {id} not found.");
            }

            _context.TestingPackage.Remove(testingPackage);
            _context.SaveChanges();

            return Ok($"Testing Package with Id {id} deleted successfully.");
        }
        
        // Get Detail id
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var testingPackage = _context.TestingPackage.FirstOrDefault(p => p.Id == id);
            if (testingPackage == null)
            {
                return NotFound($"Testing package with Id {id} not found.");
            }

            return Ok(testingPackage);
        }
    }
}
