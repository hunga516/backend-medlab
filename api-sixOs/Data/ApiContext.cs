using Microsoft.EntityFrameworkCore;
using api_sixOs.Models;

namespace api_sixOs.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            :base(options)
        { 
             
        }
    }
}
