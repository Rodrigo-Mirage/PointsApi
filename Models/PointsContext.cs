using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PointsApi.Models
{
    public class PointsContext : IdentityDbContext
    {

        public PointsContext (DbContextOptions<PointsContext> options): base (options)
        {

        }

        public DbSet<PointLog> PointLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserAddess> UserAddesses { get; set; }

    }

}