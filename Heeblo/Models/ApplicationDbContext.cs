using Microsoft.EntityFrameworkCore;

namespace Heeblo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<hbl_tbl_application> hbl_tbl_application { get; set; }
        public DbSet<hbl_tbl_project> hbl_tbl_project { get; set; }
        public DbSet<hbl_tbl_user> hbl_tbl_user { get; set; }
    }
}
