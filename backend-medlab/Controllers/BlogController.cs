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
        public async Task<IActionResult> Create([FromForm] Blogs blog)
        {
            if (blog == null || string.IsNullOrEmpty(blog.Title) || string.IsNullOrEmpty(blog.Content))
            {
                return BadRequest("Invalid blog data.");
            }

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
        public IActionResult Read(int page = 1, int pageSize = 10, string? title = null, string? category = null)
        {
            // Bắt đầu với query ban đầu
            var query = _context.Blogs.AsQueryable();

            // Thêm các bộ lọc nếu có
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));  // Tìm kiếm theo title
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category.Contains(category));  // Tìm kiếm theo category
            }

            // Tính tổng số blog
            var totalBlogs = query.Count();

            // Áp dụng phân trang
            if (pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var blogs = query.ToList();

            // Tạo response
            var response = new
            {
                Blogs = blogs,
                TotalBlogs = totalBlogs,
                TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalBlogs / pageSize) : 1,
                CurrentPage = pageSize > 0 ? page : 1
            };

            return Ok(response);
        }



        // Update
        [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromForm] Blogs blog)
{
    // Tìm blog theo id
    var existingBlog = await _context.Blogs.FirstOrDefaultAsync(p => p.Id == id);
    
    if (existingBlog == null)
    {
        return NotFound($"Blog with Id {id} not found.");
    }

    // Cập nhật các trường nếu có giá trị không null
    existingBlog.Title = !string.IsNullOrEmpty(blog.Title) ? blog.Title : existingBlog.Title;
    existingBlog.Content = !string.IsNullOrEmpty(blog.Content) ? blog.Content : existingBlog.Content;

    // Kiểm tra nếu có file ảnh mới thì lưu lại ảnh mới
    if (blog.ImageFile != null)
    {
        string directoryPath = Path.Combine("wwwroot", "images");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Xóa ảnh cũ nếu có (tuỳ vào yêu cầu bạn có thể giữ lại hoặc xóa)
        if (!string.IsNullOrEmpty(existingBlog.Img))
        {
            var oldImagePath = Path.Combine("wwwroot", existingBlog.Img.TrimStart('/'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath); // Xóa ảnh cũ nếu có
            }
        }

        // Tạo tên file mới và lưu vào thư mục
        string fileName = Path.GetFileNameWithoutExtension(blog.ImageFile.FileName)
                          + "_" + DateTime.Now.Ticks + Path.GetExtension(blog.ImageFile.FileName);
        string filePath = Path.Combine(directoryPath, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await blog.ImageFile.CopyToAsync(fileStream);
        }

        // Cập nhật đường dẫn ảnh vào blog
        existingBlog.Img = $"/images/{fileName}";
    }

    // Cập nhật các trường khác nếu cần
    existingBlog.Category = !string.IsNullOrEmpty(blog.Category) ? blog.Category : existingBlog.Category;
    existingBlog.CreatedAt = blog.CreatedAt != default ? blog.CreatedAt : existingBlog.CreatedAt;

    // Lưu thay đổi vào cơ sở dữ liệu
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
