using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;
using System.Linq;

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

        // Create
        [HttpPost]
        public IActionResult Create([FromForm] Blogs blog)
        {
            if (blog == null || string.IsNullOrEmpty(blog.Title) || string.IsNullOrEmpty(blog.Content))
            {
                return BadRequest("Invalid blog data.");
            }

            _context.Blogs.Add(blog);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Read), new { id = blog.Id }, blog);
        }

        // Read
        [HttpGet]
        public IActionResult Read()
        {
            var blogs = _context.Blogs.ToList();
            return Ok(blogs);
        }

        // Update
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Blogs blog)
        {
            var existingBlog = _context.Blogs.FirstOrDefault(p => p.Id == id);
            if (existingBlog == null)
            {
                return NotFound($"Blog with Id {id} not found.");
            }

            existingBlog.Title = blog.Title ?? existingBlog.Title;
            existingBlog.Content = blog.Content ?? existingBlog.Content;
            existingBlog.Img = blog.Img ?? existingBlog.Img;
            existingBlog.CreatedAt = blog.CreatedAt != default ? blog.CreatedAt : existingBlog.CreatedAt;

            _context.SaveChanges();

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
        
        // Get Detail title
        [HttpGet("{title}")]
        public IActionResult GetDetail(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title cannot be empty.");
            }

            var blog = _context.Blogs
                .FirstOrDefault(p => p.Title.ToLower() == title.ToLower());
    
            if (blog == null)
            {
                return NotFound($"Blog with title '{title}' not found.");
            }

            return Ok(blog);
        }



    }
}
