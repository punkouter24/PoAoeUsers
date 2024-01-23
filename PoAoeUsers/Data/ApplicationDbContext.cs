using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PoAoeUsers.Data;

namespace PoAoeUsers.Data
{
    //public class ApplicationDbContext : IdentityDbContext <ApplicationUser>
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }

    //    public DbSet<ListItem> ListItems { get; set; }
    //}


    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ListItem> ListItems { get; set; }
    }


}



