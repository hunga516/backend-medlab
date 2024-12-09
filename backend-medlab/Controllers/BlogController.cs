using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApiContext _context;

        public BlogController(ApiContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Blogs blog, [FromQuery] bool isDraft = false)
        {
            if (blog == null || string.IsNullOrEmpty(blog.Title) || string.IsNullOrEmpty(blog.Content))
            {
                return BadRequest("Invalid blog data.");
            }

            blog.Status = isDraft ? "Bản nháp" : "";

            if (blog.ImageFile != null)
            {
                string directoryPath = Path.Combine("wwwroot", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(blog.ImageFile.FileName)
                                  + "_" + DateTime.Now.Ticks + Path.GetExtension(blog.ImageFile.FileName);
                string filePath = Path.Combine(directoryPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await blog.ImageFile.CopyToAsync(fileStream);
                }

                blog.Img = $"/images/{fileName}";
            }

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = blog.Id }, blog);
        }
        
        [HttpGet]
        public IActionResult Read(int page = 1, int pageSize = 10, string? title = null, string? category = null, string? status = null)
        {
            var query = _context.Blogs.AsQueryable();

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

            var totalBlogs = query.Count();

            if (pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var blogs = query.ToList();

            var response = new
            {
                Blogs = blogs,
                TotalBlogs = totalBlogs,
                TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalBlogs / pageSize) : 1,
                CurrentPage = pageSize > 0 ? page : 1
            };

            return Ok(response);
        }


        //Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Blogs blog, [FromQuery] bool isDraft = false)
        {
            var existingBlog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingBlog == null)
            {
                return NotFound($"Blog with Id {id} not found.");
            }

            existingBlog.Title = !string.IsNullOrEmpty(blog.Title) ? blog.Title : existingBlog.Title;
            existingBlog.Content = !string.IsNullOrEmpty(blog.Content) ? blog.Content : existingBlog.Content;

            if (blog.ImageFile != null)
            {
                string directoryPath = Path.Combine("wwwroot", "images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!string.IsNullOrEmpty(existingBlog.Img))
                {
                    var oldImagePath = Path.Combine("wwwroot", existingBlog.Img.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Path.GetFileNameWithoutExtension(blog.ImageFile.FileName)
                                  + "_" + DateTime.Now.Ticks + Path.GetExtension(blog.ImageFile.FileName);
                string filePath = Path.Combine(directoryPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await blog.ImageFile.CopyToAsync(fileStream);
                }

                existingBlog.Img = $"/images/{fileName}";
            }

            existingBlog.Category = !string.IsNullOrEmpty(blog.Category) ? blog.Category : existingBlog.Category;
            existingBlog.CreatedAt = blog.CreatedAt != default ? blog.CreatedAt : existingBlog.CreatedAt;

            if (isDraft)
            {
                existingBlog.Status = "Bản nháp"; 
            }
            else
            {
                existingBlog.Status = !string.IsNullOrEmpty(blog.Status) ? blog.Status : "";
            }

            await _context.SaveChangesAsync();

            return Ok(existingBlog);
        }




        // Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound($"Blog with Id {id} not found.");
            }

            _context.Blogs.Remove(blog);
            _context.SaveChanges();

            return Ok($"Blog with Id {id} deleted successfully.");
        }
        
        // Get Detail id
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound($"Blog with Id {id} not found.");
            }

            return Ok(blog);
        }
    }
}
