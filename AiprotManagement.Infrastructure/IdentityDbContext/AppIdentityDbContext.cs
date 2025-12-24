using AirportManagement.Application.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApiUser = AirportManagement.Domain.Entities.ApiUser;

namespace AirportManagement.Infrastructure.IdentityDbContext
{
    public class AppIdentityDbContext : IdentityDbContext<ApiUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());
        }

    }
}
